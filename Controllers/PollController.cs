using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class PollController : BaseController
    {
        public PollController(IDataService data) : base(data)
        {
        }

        // View Active Polls (All users)
        public async Task<IActionResult> Index()
        {
            var activePolls = await _data.GetActivePollsAsync();
            var homeownerId = GetCurrentHomeownerId();

            // Check which polls user has voted on
            var votedPolls = new List<int>();
            if (homeownerId > 0)
            {
                foreach (var poll in activePolls)
                {
                    if (await _data.HasHomeownerVotedAsync(poll.PollID, homeownerId))
                    {
                        votedPolls.Add(poll.PollID);
                    }
                }
            }

            ViewBag.VotedPolls = votedPolls;
            return PartialView("Index", activePolls);
        }

        // View Poll Details
        public async Task<IActionResult> Details(int id)
        {
            var poll = await _data.GetPollByIdAsync(id);
            if (poll == null)
            {
                return NotFound();
            }

            var homeownerId = GetCurrentHomeownerId();
            ViewBag.HasVoted = homeownerId > 0 && await _data.HasHomeownerVotedAsync(id, homeownerId);
            ViewBag.CanVote = poll.Status == "Active" && 
                             (poll.StartDate == null || poll.StartDate <= DateTime.UtcNow) &&
                             (poll.EndDate == null || poll.EndDate >= DateTime.UtcNow) &&
                             !ViewBag.HasVoted;

            return PartialView("Details", poll);
        }

        // Homeowner: Vote on Poll
        [Authorize(Roles = "Homeowner")]
        [HttpPost]
        public async Task<IActionResult> Vote(int pollId, int optionId)
        {
            var homeownerId = GetCurrentHomeownerId();
            if (homeownerId == 0)
            {
                return Json(new { success = false, message = "Homeowner not found." });
            }

            var poll = await _data.GetPollByIdAsync(pollId);
            if (poll == null)
            {
                return Json(new { success = false, message = "Poll not found." });
            }

            // Check if poll is active
            if (poll.Status != "Active")
            {
                return Json(new { success = false, message = "This poll is not active." });
            }

            // Check if already voted
            if (await _data.HasHomeownerVotedAsync(pollId, homeownerId))
            {
                return Json(new { success = false, message = "You have already voted on this poll." });
            }

            // Check if option exists
            var option = poll.Options.FirstOrDefault(o => o.OptionID == optionId);
            if (option == null)
            {
                return Json(new { success = false, message = "Invalid option selected." });
            }

            // Add vote
            var vote = new PollVote
            {
                PollID = pollId,
                OptionID = optionId,
                HomeownerID = homeownerId,
                VotedAt = DateTime.UtcNow
            };

            await _data.AddPollVoteAsync(vote);

            return Json(new { success = true, message = "Vote submitted successfully!" });
        }

        // Admin: Create Poll
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePollViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var admin = await _data.GetAdminByEmailAsync(email ?? "");

            var poll = new Poll
            {
                Question = model.Question,
                Description = model.Description,
                CreatedByAdminID = admin?.AdminID ?? 1,
                CreatedAt = DateTime.UtcNow,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                IsAnonymous = model.IsAnonymous,
                AllowMultipleChoices = model.AllowMultipleChoices,
                TotalVotes = 0
            };

            // Add options
            poll.Options = model.Options.Select((opt, index) => new PollOption
            {
                OptionText = opt,
                VoteCount = 0,
                DisplayOrder = index
            }).ToList();

            await _data.AddPollAsync(poll);

            return Json(new { success = true, message = "Poll created successfully!", pollId = poll.PollID });
        }

        // Admin: Manage Polls
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var polls = _data.Polls.OrderByDescending(p => p.CreatedAt).ToList();
            return PartialView("Manage", polls);
        }

        // Admin: View Poll Results
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Results(int id)
        {
            var poll = await _data.GetPollByIdAsync(id);
            if (poll == null)
            {
                return NotFound();
            }

            return PartialView("Results", poll);
        }

        // Admin: Update Poll Status
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var poll = await _data.GetPollByIdAsync(id);
            if (poll == null)
            {
                return Json(new { success = false, message = "Poll not found." });
            }

            poll.Status = status;
            await _data.UpdatePollAsync(poll);

            return Json(new { success = true, message = $"Poll status updated to {status} successfully!" });
        }

        // Admin: Delete Poll
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var poll = await _data.GetPollByIdAsync(id);
            if (poll == null)
            {
                return Json(new { success = false, message = "Poll not found." });
            }

            await _data.DeletePollAsync(id);
            return Json(new { success = true, message = "Poll deleted successfully!" });
        }
    }
}


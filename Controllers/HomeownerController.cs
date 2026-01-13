using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Homeowner")]
    public class HomeownerController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;

        public HomeownerController(IDataService data, IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(data)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        public async Task<IActionResult> Dashboard()
        {
            int homeownerId = GetCurrentHomeownerId();

            var profileImageObj = await _data.GetHomeownerProfileImageAsync(homeownerId);
            var profileImage = profileImageObj?.ImagePath;

            if (string.IsNullOrEmpty(profileImage))
            {
                profileImage = "/uploads/profile_pictures/default-profile.jpg"; // Default profile picture
            }

            ViewData["ProfileImage"] = profileImage;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            int homeownerId = GetCurrentHomeownerId();
            if (homeownerId == 0)
            {
                return Unauthorized("User not logged in.");
            }

            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/profile_pictures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{homeownerId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(profileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            var imagePath = $"/uploads/profile_pictures/{fileName}";

            // Get existing image or create new
            var existingImage = await _data.GetHomeownerProfileImageAsync(homeownerId);
            var today = DateTime.UtcNow.Date;

            if (existingImage != null)
            {
                // Check if user has exceeded daily limit
                if (existingImage.LastUpdatedDate == today && existingImage.ChangeCount >= 3)
                {
                    return BadRequest("You have reached the maximum profile updates for today.");
                }

                // Update existing image
                if (existingImage.LastUpdatedDate != today)
                {
                    existingImage.ChangeCount = 0;
                    existingImage.LastUpdatedDate = today;
                }

                existingImage.ImagePath = imagePath;
                existingImage.UploadedAt = DateTime.UtcNow;
                existingImage.ChangeCount += 1;
            }
            else
            {
                existingImage = new HomeownerProfileImage
                {
                    HomeownerID = homeownerId,
                    ImagePath = imagePath,
                    UploadedAt = DateTime.UtcNow,
                    ChangeCount = 1,
                    LastUpdatedDate = today
                };
            }

            await _data.AddOrUpdateHomeownerProfileImageAsync(existingImage);
            return Ok(new { message = "Profile image updated successfully.", imagePath });
        }

        // Display the Submit Request form
        [HttpGet]
        public async Task<IActionResult> SubmitRequest()
        {
            // Check if user is logged in by verifying the session or claims
            var homeownerId = GetCurrentHomeownerId();

            if (homeownerId == 0)
            {
                return RedirectToAction("Login", "Account");  // Redirect to login if not logged in
            }

            var homeowner = await _data.GetHomeownerByIdAsync(homeownerId);
            var serviceRequests = await _data.GetServiceRequestsByHomeownerIdAsync(homeownerId);
            var staffList = _data.Staff.ToList();

            // Populate the model with request data
            var viewModel = new SubmitRequestViewModel
            {
                NewRequest = new ServiceRequest(),
                ServiceRequests = serviceRequests.OrderByDescending(r => r.CreatedAt).ToList(),
                StaffList = staffList,
                HomeownerId = homeownerId,
                HomeownerName = homeowner?.FullName
            };

            return View("~/Views/Service/SubmitRequest.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRequest(SubmitRequestViewModel model)
        {
            var homeownerId = GetCurrentHomeownerId();

            if (ModelState.IsValid && model.NewRequest != null)
            {
                if (homeownerId > 0)
                {
                    model.NewRequest.HomeownerID = homeownerId;
                    model.NewRequest.CreatedAt = DateTime.UtcNow;

                    if (string.IsNullOrEmpty(model.NewRequest.Status))
                    {
                        model.NewRequest.Status = "Pending";
                    }

                    // Auto-assign staff based on category
                    var staffByPosition = await _data.GetStaffByPositionAsync(model.NewRequest.Category);
                    if (staffByPosition.Any())
                    {
                        model.NewRequest.AssignedStaffID = staffByPosition.First().StaffID;
                    }

                    await _data.AddServiceRequestAsync(model.NewRequest);

                    TempData["Success"] = "Request submitted successfully!";
                    return RedirectToAction("SubmitRequest");
                }
                else
                {
                    ModelState.AddModelError("", "Homeowner ID is missing or invalid.");
                }
            }

            if (homeownerId > 0)
            {
                var serviceRequests = await _data.GetServiceRequestsByHomeownerIdAsync(homeownerId);
                var homeowner = await _data.GetHomeownerByIdAsync(homeownerId);
                
                model.ServiceRequests = serviceRequests
                    .Where(r => r.Status != "Cancelled")
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
                model.StaffList = _data.Staff.ToList();
                model.HomeownerId = homeownerId;
                model.HomeownerName = homeowner?.FullName;
            }

            return View("~/Views/Service/SubmitRequest.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            var request = await _data.GetServiceRequestByIdAsync(requestId);

            if (request == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            if (request.Status != "Pending")
            {
                return Json(new { success = false, message = "Only pending requests can be canceled." });
            }

            // Delete the request from the database
            await _data.DeleteServiceRequestAsync(requestId);

            return Json(new { success = true });
        }







        // View the submitted service requests for the logged-in homeowner
        public async Task<IActionResult> ViewRequest()
        {
            var homeownerId = GetCurrentHomeownerId();
            var serviceRequests = await _data.GetServiceRequestsByHomeownerIdAsync(homeownerId);

            return View(serviceRequests.ToList()); // This will show the requests along with their status
        }

        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var events = await _data.GetEventsAsync();

            // Serialize events and pass to the view
            var serializedEvents = System.Text.Json.JsonSerializer.Serialize(events.Select(e => new
            {
                title = e.Title,
                start = e.EventDate.ToString("yyyy-MM-dd"),
                description = e.Description,
                location = e.Location,
                category = e.Category
            }));

            ViewBag.SerializedEvents = serializedEvents; // Pass serialized events to the view
            return View();
        }








    }
}

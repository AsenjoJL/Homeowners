using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GateAccessLogController : BaseController
    {
        public GateAccessLogController(IDataService data) : base(data)
        {
        }

        // View Access Logs
        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            var logs = _data.GateAccessLogs.OrderByDescending(l => l.AccessTime).ToList();

            if (startDate.HasValue && endDate.HasValue)
            {
                logs = logs.Where(l => l.AccessTime >= startDate.Value && l.AccessTime <= endDate.Value).ToList();
            }
            else if (startDate.HasValue)
            {
                logs = logs.Where(l => l.AccessTime >= startDate.Value).ToList();
            }
            else if (endDate.HasValue)
            {
                logs = logs.Where(l => l.AccessTime <= endDate.Value).ToList();
            }
            else
            {
                // Default to last 7 days
                var defaultStartDate = DateTime.UtcNow.AddDays(-7);
                logs = logs.Where(l => l.AccessTime >= defaultStartDate).ToList();
            }

            // Load homeowner names for logs that have HomeownerID
            var homeownerIds = logs.Where(l => l.HomeownerID.HasValue).Select(l => l.HomeownerID.Value).Distinct().ToList();
            var homeowners = new Dictionary<int, string>();
            foreach (var id in homeownerIds)
            {
                var homeowner = await _data.GetHomeownerByIdAsync(id);
                if (homeowner != null)
                {
                    homeowners[id] = homeowner.FullName;
                }
            }
            ViewBag.Homeowners = homeowners;

            ViewBag.StartDate = startDate ?? DateTime.UtcNow.AddDays(-7);
            ViewBag.EndDate = endDate ?? DateTime.UtcNow;

            return PartialView("Index", logs);
        }

        // View Statistics
        public IActionResult Statistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var logs = _data.GateAccessLogs
                .Where(l => l.AccessTime >= start && l.AccessTime <= end)
                .ToList();

            var stats = new GateAccessStatistics
            {
                TotalEntries = logs.Count(l => l.AccessType == "Entry"),
                TotalExits = logs.Count(l => l.AccessType == "Exit"),
                HomeownerEntries = logs.Count(l => l.UserType == "Homeowner" && l.AccessType == "Entry"),
                VisitorEntries = logs.Count(l => l.UserType == "Visitor" && l.AccessType == "Entry"),
                StaffEntries = logs.Count(l => l.UserType == "Staff" && l.AccessType == "Entry"),
                DeliveryEntries = logs.Count(l => l.UserType == "Delivery" && l.AccessType == "Entry"),
                StartDate = start,
                EndDate = end
            };

            return PartialView("Statistics", stats);
        }
    }
}


using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class VisitorPassController : BaseController
    {
        public VisitorPassController(IDataService data) : base(data)
        {
        }

        // Homeowner: Request Visitor Pass
        [Authorize(Roles = "Homeowner")]
        [HttpGet]
        public IActionResult RequestPass()
        {
            return PartialView("Request");
        }

        [Authorize(Roles = "Homeowner")]
        [HttpPost]
        public async Task<IActionResult> RequestPass(VisitorPassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var homeownerId = GetCurrentHomeownerId();
            if (homeownerId == 0)
            {
                return Json(new { success = false, message = "Homeowner not found." });
            }

            var visitorPass = new VisitorPass
            {
                HomeownerID = homeownerId,
                VisitorName = model.VisitorName,
                VisitorPhone = model.VisitorPhone,
                VisitorIDNumber = model.VisitorIDNumber,
                VehiclePlateNumber = model.VehiclePlateNumber,
                VehicleType = model.VehicleType,
                VisitDate = model.VisitDate,
                ExpectedArrivalTime = model.ExpectedArrivalTime,
                Purpose = model.Purpose,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            await _data.AddVisitorPassAsync(visitorPass);

            return Json(new { success = true, message = "Visitor pass requested successfully! Awaiting admin approval.", passId = visitorPass.VisitorPassID });
        }

        // Homeowner: View My Visitor Passes
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> MyPasses()
        {
            var homeownerId = GetCurrentHomeownerId();
            var passes = await _data.GetVisitorPassesByHomeownerIdAsync(homeownerId);
            return PartialView("MyPasses", passes.OrderByDescending(p => p.RequestedAt));
        }

        // Admin: Manage Visitor Passes
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var passes = _data.VisitorPasses.OrderByDescending(p => p.RequestedAt).ToList();
            return PartialView("Manage", passes);
        }

        // Admin: Approve/Reject Visitor Pass
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? notes = null)
        {
            var pass = await _data.GetVisitorPassByIdAsync(id);
            if (pass == null)
            {
                return Json(new { success = false, message = "Visitor pass not found." });
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var admin = await _data.GetAdminByEmailAsync(email ?? "");

            pass.Status = status;
            pass.AdminNotes = notes;
            
            if (status == "Approved")
            {
                pass.ApprovedAt = DateTime.UtcNow;
                pass.ApprovedByAdminID = admin?.AdminID ?? 1;
            }

            await _data.UpdateVisitorPassAsync(pass);

            return Json(new { success = true, message = $"Visitor pass {status.ToLower()} successfully!" });
        }

        // Admin: Check In/Out Visitor
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CheckIn(int id)
        {
            var pass = await _data.GetVisitorPassByIdAsync(id);
            if (pass == null)
            {
                return Json(new { success = false, message = "Visitor pass not found." });
            }

            pass.CheckedInAt = DateTime.UtcNow;
            await _data.UpdateVisitorPassAsync(pass);

            // Log gate access
            var log = new GateAccessLog
            {
                HomeownerID = pass.HomeownerID,
                VisitorName = pass.VisitorName,
                PlateNumber = pass.VehiclePlateNumber,
                AccessType = "Entry",
                UserType = "Visitor",
                AccessTime = DateTime.UtcNow,
                GateLocation = "Main Gate"
            };
            await _data.AddGateAccessLogAsync(log);

            return Json(new { success = true, message = "Visitor checked in successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CheckOut(int id)
        {
            var pass = await _data.GetVisitorPassByIdAsync(id);
            if (pass == null)
            {
                return Json(new { success = false, message = "Visitor pass not found." });
            }

            pass.CheckedOutAt = DateTime.UtcNow;
            pass.Status = "Completed";
            await _data.UpdateVisitorPassAsync(pass);

            // Log gate access
            var log = new GateAccessLog
            {
                HomeownerID = pass.HomeownerID,
                VisitorName = pass.VisitorName,
                PlateNumber = pass.VehiclePlateNumber,
                AccessType = "Exit",
                UserType = "Visitor",
                AccessTime = DateTime.UtcNow,
                GateLocation = "Main Gate"
            };
            await _data.AddGateAccessLogAsync(log);

            return Json(new { success = true, message = "Visitor checked out successfully!" });
        }
    }
}


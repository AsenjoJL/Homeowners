using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class ComplaintController : BaseController
    {
        public ComplaintController(IDataService data) : base(data)
        {
        }

        // Homeowner: Submit Complaint
        [Authorize(Roles = "Homeowner")]
        [HttpGet]
        public IActionResult Submit()
        {
            return PartialView("Submit");
        }

        [Authorize(Roles = "Homeowner")]
        [HttpPost]
        public async Task<IActionResult> Submit(ComplaintViewModel model)
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

            var complaint = new Complaint
            {
                HomeownerID = homeownerId,
                Subject = model.Subject,
                Description = model.Description,
                Category = model.Category,
                Status = "Submitted",
                SubmittedAt = DateTime.UtcNow,
                Priority = model.Priority,
                IsAnonymous = model.IsAnonymous
            };

            await _data.AddComplaintAsync(complaint);

            return Json(new { success = true, message = "Complaint submitted successfully! Your complaint ID is #" + complaint.ComplaintID, complaintId = complaint.ComplaintID });
        }

        // Homeowner: View My Complaints
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> MyComplaints()
        {
            var homeownerId = GetCurrentHomeownerId();
            var complaints = await _data.GetComplaintsByHomeownerIdAsync(homeownerId);
            return PartialView("MyComplaints", complaints.OrderByDescending(c => c.SubmittedAt));
        }

        // Homeowner: View Complaint Details
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _data.GetComplaintByIdAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            var homeownerId = GetCurrentHomeownerId();
            if (complaint.HomeownerID != homeownerId)
            {
                return Forbid();
            }

            return PartialView("Details", complaint);
        }

        // Admin: Manage Complaints
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var complaints = _data.Complaints.OrderByDescending(c => c.SubmittedAt).ToList();
            return PartialView("Manage", complaints);
        }

        // Admin: View Complaint Details
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var complaint = await _data.GetComplaintByIdAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            // Load homeowner info
            var homeowner = await _data.GetHomeownerByIdAsync(complaint.HomeownerID);
            ViewBag.Homeowner = homeowner;

            return PartialView("AdminDetails", complaint);
        }

        // Admin: Update Complaint Status
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? response = null, string? resolutionNotes = null)
        {
            var complaint = await _data.GetComplaintByIdAsync(id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var admin = await _data.GetAdminByEmailAsync(email ?? "");

            complaint.Status = status;
            complaint.AdminResponse = response;
            complaint.ResolutionNotes = resolutionNotes;

            if (status == "Under Review" && complaint.ReviewedAt == null)
            {
                complaint.ReviewedAt = DateTime.UtcNow;
                complaint.ReviewedByAdminID = admin?.AdminID ?? 1;
            }

            if (status == "Resolved" || status == "Closed")
            {
                complaint.ResolvedAt = DateTime.UtcNow;
            }

            await _data.UpdateComplaintAsync(complaint);

            return Json(new { success = true, message = $"Complaint status updated to {status} successfully!" });
        }

        // Admin: Delete Complaint
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var complaint = await _data.GetComplaintByIdAsync(id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            await _data.DeleteComplaintAsync(id);
            return Json(new { success = true, message = "Complaint deleted successfully!" });
        }
    }
}


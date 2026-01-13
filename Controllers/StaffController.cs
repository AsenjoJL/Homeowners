using HOMEOWNER.Data;
using HOMEOWNER.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : BaseController
    {
        public StaffController(IDataService data) : base(data)
        {
        }

        // GET: Staff/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Staff/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var staff = await _data.GetStaffByEmailAsync(email);

            if (staff == null || string.IsNullOrEmpty(staff.PasswordHash) || !VerifyPassword(password, staff.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password!";
                return View();
            }

            // Set session values
            HttpContext.Session.SetInt32("StaffID", staff.StaffID);
            HttpContext.Session.SetString("StaffName", staff.FullName ?? string.Empty);
            HttpContext.Session.SetString("StaffEmail", staff.Email ?? string.Empty);
            HttpContext.Session.SetString("StaffRole", staff.Position ?? string.Empty);

            return RedirectToAction("Dashboard");
        }



        // GET: Staff/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var staffID = HttpContext.Session.GetInt32("StaffID");

            if (staffID == null)
                return RedirectToAction("Login");

            var position = HttpContext.Session.GetString("StaffRole");

            // Show all service requests that match the staff's department (Maintenance or Security)
            var allRequests = await _data.GetServiceRequestsAsync();
            var requests = allRequests.Where(r => r.Category == position).ToList();

            var pendingRequests = requests.Where(r => r.Status == "Pending").ToList();
            var completedRequests = requests.Where(r => r.Status == "Completed").ToList();

            ViewData["PendingCount"] = pendingRequests.Count;
            ViewData["CompletedCount"] = completedRequests.Count;
            ViewData["PendingRequests"] = pendingRequests;
            ViewData["CompletedRequests"] = completedRequests;

            ViewData["StaffName"] = HttpContext.Session.GetString("StaffName");
            ViewData["Position"] = position;

            return View();
        }





        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status)
        {
            if (status != "Completed")
            {
                TempData["Error"] = "Invalid status update.";
                return RedirectToAction("Dashboard", "Staff");
            }

            var serviceRequest = await _data.GetServiceRequestByIdAsync(requestId);

            if (serviceRequest != null)
            {
                serviceRequest.Status = status;
                serviceRequest.CompletedAt = DateTime.UtcNow; // Set the completion date (UTC for Firestore)

                await _data.UpdateServiceRequestAsync(serviceRequest);

                TempData["Success"] = "Request marked as completed!";
            }

            return RedirectToAction("Dashboard", "Staff");
        }









        public async Task<IActionResult> Management()
        {
            var staffId = HttpContext.Session.GetInt32("StaffID");

            if (staffId == null)
            {
                return RedirectToAction("Login", "Staff");
            }

            var staff = await _data.GetStaffByIdAsync(staffId.Value);
            if (staff == null)
            {
                ViewData["StaffName"] = "Unknown Staff";
                ViewData["Position"] = "Unknown Position";
            }
            else
            {
                ViewData["StaffName"] = staff.FullName ?? "Unknown Staff";
                ViewData["Position"] = staff.Position ?? "Unknown Position";
            }

            var allRequests = await _data.GetServiceRequestsAsync();
            var pendingRequests = allRequests.Where(r => r.Status == "Pending").ToList();
            var completedRequests = allRequests.Where(r => r.Status == "Completed").ToList();

            var viewModel = new Dictionary<string, List<ServiceRequest>>()
            {
                { "Pending", pendingRequests },
                { "Completed", completedRequests }
            };

            return PartialView("_ManagementPartial", viewModel);  // Return the partial view instead of full view
        }




        // GET: Staff/Profile
        public async Task<IActionResult> Profile()
        {
            var staffId = GetCurrentStaffId();
            var staff = await _data.GetStaffByIdAsync(staffId);

            if (staff == null)
                return RedirectToAction("Login");

            return View(staff);
        }

        // GET: Staff/UnauthorizedAccess
        public IActionResult UnauthorizedAccess()
        {
            return View("UnauthorizedAccess");
        }

        // GET: Staff/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Password verification helper (using same method as AccountController)
        private static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                var parts = hash.Split(':');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHash = Convert.FromBase64String(parts[1]);

                var hashOfInput = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32
                );

                return CryptographicOperations.FixedTimeEquals(hashOfInput, storedHash);
            }
            catch
            {
                return false;
            }
        }

        // Role validation helper
        private bool IsLoggedIn(string requiredRole = "")
        {
            var position = HttpContext.Session.GetString("StaffRole");

            if (string.IsNullOrEmpty(position))
                return false;

            return string.IsNullOrEmpty(requiredRole) || position.ToLower() == requiredRole.ToLower();
        }





        // Staff ID retrieval from session (using BaseController method)
        private int GetCurrentStaffId()
        {
            return base.GetCurrentStaffId();
        }
    }
}

using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;




using Microsoft.AspNetCore.Authorization;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IConfiguration _config;


        public AdminController(IDataService data, IConfiguration config) : base(data)
        {
            _config = config;
        }

        public IActionResult Dashboard()
        {
            var homeownerCount = _data.Homeowners.Count(u => u.Role == "Homeowner");
            var staffCount = _data.Staff.Count(u => u.Position == "Maintenance" || u.Position == "Security");
            var reservationCount = _data.Reservations.Count(r => r.Status == "Approved");


            ViewBag.HomeownerCount = homeownerCount;
            ViewBag.StaffCount = staffCount;
            ViewBag.ReservationCount = reservationCount;
            // ✅ Fetch homeowners from database
            var homeowners = _data.Homeowners.ToList();

            if (homeowners == null)
            {
                homeowners = new List<Homeowner>();  // ✅ Ensure it is not null
            }

            return View(homeowners); // ✅ Pass homeowners list to the view

        }





        public IActionResult ManageOwners()
        {
            var homeowners = _data.Homeowners.ToList();

            if (homeowners == null)
            {
                homeowners = new List<Homeowner>(); // Ensure it's not null
            }

            return PartialView("_ManageOwners", homeowners); // ✅ Use PartialView for partials
        }


        public IActionResult AddOwner()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOwner(Homeowner owner)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            // Check for duplicate email
            if (_data.Homeowners.Any(h => h.Email == owner.Email))
            {
                return Json(new { success = false, message = "This email is already registered." });
            }

            // Check for duplicate BlockLotNumber
            if (_data.Homeowners.Any(h => h.BlockLotNumber == owner.BlockLotNumber))
            {
                return Json(new { success = false, message = "This Block & Lot Number is already taken." });
            }

            // Hash the password before storing it
            owner.PasswordHash = HashPassword(owner.PasswordHash);

               // Assign default values
               owner.AdminID = 1; // Set dynamically based on logged-in admin
               owner.Role = "Homeowner";
               owner.CreatedAt = DateTime.UtcNow;

            await _data.AddHomeownerAsync(owner);

            //  Return JSON data instead of HTML
            return Json(new
            {
                success = true,
                homeowner = new
                {
                    id = owner.HomeownerID,
                    fullName = owner.FullName,
                    email = owner.Email,
                    address = owner.Address,
                    contactNumber = owner.ContactNumber,
                    blockLotNumber = owner.BlockLotNumber,
                    role = owner.Role
                }
            });
        }



        public IActionResult ManageStaff()
        {
            var staffList = _data.Staff.ToList();
            return PartialView("ManageStaff", staffList);
        }

        public async Task<IActionResult> LoadStaffList()
        {
            // Use async method to ensure fresh data from Firebase
            var allStaff = await _data.GetStaffAsync();
            return PartialView("_StaffList", allStaff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            // Validate required fields manually since model binding might not work perfectly with AJAX
            if (string.IsNullOrWhiteSpace(staff?.FullName))
            {
                return Json(new { success = false, message = "Full Name is required." });
            }
            if (string.IsNullOrWhiteSpace(staff?.Email))
            {
                return Json(new { success = false, message = "Email is required." });
            }
            if (string.IsNullOrWhiteSpace(staff?.PhoneNumber))
            {
                return Json(new { success = false, message = "Phone Number is required." });
            }
            if (string.IsNullOrWhiteSpace(staff?.Position))
            {
                return Json(new { success = false, message = "Position is required." });
            }
            if (string.IsNullOrWhiteSpace(staff?.PasswordHash))
            {
                return Json(new { success = false, message = "Password is required." });
            }

            try
            {
                // ✅ Ensure default values if any field is missing
                staff.FullName = staff.FullName?.Trim() ?? "Unknown";
                staff.Email = staff.Email?.Trim() ?? "No Email";
                staff.PhoneNumber = staff.PhoneNumber?.Trim() ?? "No Phone";
                staff.Position = staff.Position?.Trim() ?? "No Position";
                staff.AdminID = GetCurrentAdminId();
                if (staff.AdminID == 0)
                {
                    staff.AdminID = 1; // Default admin ID if not found
                }
                staff.IsActive = true;

                // Check for duplicate email
                var existingStaff = await _data.GetStaffByEmailAsync(staff.Email);
                if (existingStaff != null)
                {
                    return Json(new { success = false, message = "This email is already registered." });
                }

                // Hash the password before storing it
                staff.PasswordHash = HashPassword(staff.PasswordHash);

                // StaffID will be generated in AddStaffAsync if it's 0
                staff.StaffID = 0;

                await _data.AddStaffAsync(staff);

                // Get the saved staff by email to verify it was saved
                var savedStaff = await _data.GetStaffByEmailAsync(staff.Email);
                if (savedStaff == null)
                {
                    return Json(new { success = false, message = "Staff was saved but could not be retrieved. Please refresh the page." });
                }

                return Json(new { success = true, message = "Staff added successfully!", staff = savedStaff });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding staff: {ex.Message}" });
            }
        }





        public async Task<IActionResult> EditStaff(int id)
        {
            var staff = await _data.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> EditStaff(Staff model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var staff = await _data.GetStaffByIdAsync(model.StaffID);
            if (staff == null)
            {
                return NotFound();
            }

            staff.FullName = model.FullName;
            staff.Email = model.Email;
            staff.PhoneNumber = model.PhoneNumber;
            staff.Position = model.Position;

            await _data.UpdateStaffAsync(staff);
            return RedirectToAction("ManageStaff");
        }








        private static string HashPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "Password cannot be empty or null.");

            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32
            );

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public IActionResult FacilitiesAndReservations()
        {
            var facilities = _data.Facilities.ToList();
            var reservations = _data.Reservations.ToList();

            var model = new Tuple<IEnumerable<Facility>, IEnumerable<Reservation>>(facilities, reservations);
            return View(model);
        }





        // POST: Delete Facility
        [HttpPost]
        public async Task<IActionResult> DeleteFacility(int facilityId)
        {
            var facility = await _data.GetFacilityByIdAsync(facilityId);
            if (facility == null)
            {
                return NotFound();
            }

            await _data.DeleteFacilityAsync(facilityId);

            return RedirectToAction(nameof(FacilitiesAndReservations));
        }

        public IActionResult ReservationManagement()
        {
            IEnumerable<Facility> facilities = _data.Facilities.ToList();
            IEnumerable<Reservation> reservations = _data.Reservations.ToList();

            var model = Tuple.Create(facilities, reservations);
            return PartialView("ReservationManagement", model);
        }


        private async Task ExpireOldReservations()
        {
            var now = DateTime.Now;

            // Step 1: Pull approved reservations into memory
            var approvedReservations = _data.Reservations
                .Where(r => r.Status == "Approved")
                .ToList();

            // Step 2: Expire those that are already finished
            foreach (var reservation in approvedReservations)
            {
                var endDateTime = reservation.ReservationDate.Date + reservation.EndTime;
                if (endDateTime <= now)
                {
                    reservation.Status = "Expired";
                    reservation.UpdatedAt = now;
                    await _data.UpdateReservationAsync(reservation);
                }
            }
        }










        // Manage service requests (Admin View)
        public IActionResult ManageServiceRequests(string statusFilter = "All")
        {
            var serviceRequests = _data.ServiceRequests.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                serviceRequests = serviceRequests.Where(r => r.Status == statusFilter);
            }

            var viewModel = new SubmitRequestViewModel
            {
                ServiceRequests = serviceRequests.ToList(),
                StaffList = _data.Staff.ToList()
            };

            return PartialView("ManageServiceRequests", viewModel);
        }

        // Helper method to return a list of categories
        private List<string> GetEventCategories()
        {
            return new List<string> { "General", "Meeting", "Conference", "Workshop", "Webinar", "Party", "Training" };
        }

        // Event List View
        [HttpGet]
        public IActionResult EventCalendar()
        {
            var events = _data.Events.OrderBy(e => e.EventDate).ToList();
            return PartialView("EventCalendar", events);
        }

        [HttpGet]
        public IActionResult AddEvent()
        {
            ViewBag.Categories = GetEventCategories();
            return PartialView("_AddEditEventPartial", new EventModel());
        }

        // Handle Add Event Submission (POST)
        [HttpPost]
        public async Task<IActionResult> AddEvent(EventModel model)
        {
            ViewBag.Categories = GetEventCategories();

            if (!ModelState.IsValid || model.EventDate < new DateTime(1753, 1, 1))
            {
                if (model.EventDate < new DateTime(1753, 1, 1))
                {
                    ModelState.AddModelError("EventDate", "Please select a valid event date.");
                }

                return PartialView("_AddEditEventPartial", model);
            }

            try
            {
                // Set created by admin ID
                model.CreatedBy = GetCurrentAdminID();
                
                // Add event to Firebase
                await _data.AddEventAsync(model);

                return Json(new { success = true, message = "Event added successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred while saving.");
                return Json(new { success = false, message = "An error occurred while saving the event." });
            }
        }



        // GET: Edit Event
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventModel = await _data.GetEventByIdAsync(id);
            
            if (eventModel == null)
                return NotFound();

            ViewBag.Categories = GetEventCategories();
            return PartialView("_AddEditEventPartial", eventModel);
        }

        // POST: Edit Event
        [HttpPost]
        public async Task<IActionResult> EditEvent(EventModel model)
        {
            ViewBag.Categories = GetEventCategories();

            if (!ModelState.IsValid || model.EventDate < new DateTime(1753, 1, 1))
            {
                if (model.EventDate < new DateTime(1753, 1, 1))
                {
                    ModelState.AddModelError("EventDate", "Please select a valid event date.");
                }

                return PartialView("_AddEditEventPartial", model);
            }

            try
            {
                await _data.UpdateEventAsync(model);
                return Json(new { success = true, message = "Event updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Error updating the event. Please try again." });
            }
        }


        // POST: Delete Event
        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _data.DeleteEventAsync(id);
                return Json(new { success = true, message = "Event deleted successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting event: " + ex.Message);
                return Json(new { success = false, message = "Error deleting event. Please try again." });
            }
        }


        // Helper: Get Logged-in Admin ID
        private int GetCurrentAdminID()
        {
            // Try to get from claims first
            var adminIdClaim = User.FindFirst("AdminID")?.Value;
            if (!string.IsNullOrEmpty(adminIdClaim) && int.TryParse(adminIdClaim, out int id))
                return id;

            // Fallback to email lookup
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return 0;

            var admin = _data.Admins.FirstOrDefault(a => a.Email == email);
            return admin?.AdminID ?? 0;
        }

        // GET: Show the form for creating an announcement and the list
        public IActionResult AnnouncementList()
        {
            var model = new CombinedAnnouncementViewModel
            {
                Announcements = _data.Announcements.OrderByDescending(a => a.PostedAt).ToList()
            };

            return PartialView("AnnouncementList", model);
        }

        // POST: Create a new announcement (AJAX modal support)
        [HttpPost]
        public async Task<IActionResult> AnnouncementList(AnnouncementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var announcement = new Announcement
                {
                    Title = model.Title,
                    Content = model.Content,
                    PostedAt = DateTime.Now,
                    IsUrgent = model.IsUrgent
                };

                await _data.AddAnnouncementAsync(announcement);


                var partialModel = new CombinedAnnouncementViewModel
                {
                    Announcements = _data.Announcements.OrderByDescending(a => a.PostedAt).ToList()
                };

                // Return only the updated list for AJAX calls
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_AnnouncementListPartial", partialModel);
                }

                TempData["Success"] = "Announcement posted successfully!";
                return RedirectToAction("AnnouncementList");
            }

            // Return validation errors
            var viewModel = new CombinedAnnouncementViewModel
            {
                NewAnnouncement = model,
                Announcements = _data.Announcements.OrderByDescending(a => a.PostedAt).ToList()
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Response.StatusCode = 400;
                return PartialView("_AnnouncementListPartial", viewModel);
            }

            return View(viewModel);
        }


        public IActionResult Analytics()
        {
            var facilities = _data.Facilities.ToList();
            var reservations = _data.Reservations.ToList();

            var model = new Tuple<IEnumerable<Facility>, IEnumerable<Reservation>>(facilities, reservations);
            return PartialView("Analytics", model);
        }

        public IActionResult CreateBilling()
        {
            var billings = _data.Billings.ToList();
            var homeowners = _data.Homeowners.ToList();
            
            // Calculate statistics
            var totalRevenue = billings.Where(b => b.Status == "Paid").Sum(b => (double)b.Amount);
            var paidBills = billings.Count(b => b.Status == "Paid");
            var pendingBills = billings.Count(b => b.Status == "Pending");
            var overdueBills = billings.Count(b => b.Status == "Overdue" || (b.Status == "Pending" && b.DueDate < DateTime.UtcNow));
            
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.PaidBills = paidBills;
            ViewBag.PendingBills = pendingBills;
            ViewBag.OverdueBills = overdueBills;
            ViewBag.Homeowners = homeowners;
            ViewBag.Billings = billings;
            
            return PartialView("CreateBilling", billings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBilling([FromBody] BillingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var billing = new Billing
            {
                HomeownerID = model.HomeownerID,
                Description = model.Description,
                Amount = model.Amount,
                DueDate = model.DueDate,
                BillType = model.BillType,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _data.AddBillingAsync(billing);

            return Json(new { success = true, message = "Bill created successfully!", billingId = billing.BillingID });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBillingStatus(int id, string status, string? paymentMethod = null, string? transactionID = null)
        {
            var billing = await _data.GetBillingByIdAsync(id);
            if (billing == null)
            {
                return Json(new { success = false, message = "Billing record not found." });
            }

            billing.Status = status;
            if (status == "Paid")
            {
                billing.PaidAt = DateTime.UtcNow;
                billing.PaymentMethod = paymentMethod;
                billing.TransactionID = transactionID;
            }

            await _data.UpdateBillingAsync(billing);

            return Json(new { success = true, message = "Billing status updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBilling(int id)
        {
            var billing = await _data.GetBillingByIdAsync(id);
            if (billing == null)
            {
                return Json(new { success = false, message = "Billing record not found." });
            }

            await _data.DeleteBillingAsync(id);

            return Json(new { success = true, message = "Billing record deleted successfully!" });
        }

        [HttpGet]
        public IActionResult GetHomeowners()
        {
            var homeowners = _data.Homeowners.Select(h => new { 
                h.HomeownerID, 
                h.FullName, 
                h.Email 
            }).ToList();
            return Json(homeowners);
        }

    }
}

namespace HOMEOWNER.Models.ViewModels
{
    public class BillingViewModel
    {
        public int HomeownerID { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string BillType { get; set; } = string.Empty;
    }
}

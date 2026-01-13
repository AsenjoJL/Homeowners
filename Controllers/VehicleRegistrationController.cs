using HOMEOWNER.Data;
using HOMEOWNER.Models;
using HOMEOWNER.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class VehicleRegistrationController : BaseController
    {
        public VehicleRegistrationController(IDataService data) : base(data)
        {
        }

        // Homeowner: Register Vehicle
        [Authorize(Roles = "Homeowner")]
        [HttpGet]
        public IActionResult Register()
        {
            return PartialView("Register");
        }

        [Authorize(Roles = "Homeowner")]
        [HttpPost]
        public async Task<IActionResult> Register(VehicleRegistrationViewModel model)
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

            // Check if plate number already exists
            var existingVehicle = await _data.GetVehicleByPlateNumberAsync(model.PlateNumber);
            if (existingVehicle != null)
            {
                return Json(new { success = false, message = "This plate number is already registered." });
            }

            var vehicle = new VehicleRegistration
            {
                HomeownerID = homeownerId,
                PlateNumber = model.PlateNumber,
                VehicleType = model.VehicleType,
                Make = model.Make,
                Model = model.Model,
                Color = model.Color,
                Status = "Pending",
                RegisteredAt = DateTime.UtcNow
            };

            await _data.AddVehicleAsync(vehicle);

            return Json(new { success = true, message = "Vehicle registration submitted successfully! Awaiting admin approval.", vehicleId = vehicle.VehicleID });
        }

        // Homeowner: View My Vehicles
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> MyVehicles()
        {
            var homeownerId = GetCurrentHomeownerId();
            var vehicles = await _data.GetVehiclesByHomeownerIdAsync(homeownerId);
            return PartialView("MyVehicles", vehicles.OrderByDescending(v => v.RegisteredAt));
        }

        // Admin: Manage Vehicle Registrations
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var vehicles = _data.VehicleRegistrations.OrderByDescending(v => v.RegisteredAt).ToList();
            return PartialView("Manage", vehicles);
        }

        // Admin: Approve/Reject Vehicle
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, DateTime? expiryDate = null, string? notes = null)
        {
            var vehicle = await _data.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return Json(new { success = false, message = "Vehicle not found." });
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var admin = await _data.GetAdminByEmailAsync(email ?? "");

            vehicle.Status = status;
            vehicle.AdminNotes = notes;

            if (status == "Approved")
            {
                vehicle.ApprovedAt = DateTime.UtcNow;
                vehicle.ApprovedByAdminID = admin?.AdminID ?? 1;
                vehicle.ExpiryDate = expiryDate ?? DateTime.UtcNow.AddYears(1);
            }

            await _data.UpdateVehicleAsync(vehicle);

            return Json(new { success = true, message = $"Vehicle registration {status.ToLower()} successfully!" });
        }

        // Admin: Delete Vehicle
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _data.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return Json(new { success = false, message = "Vehicle not found." });
            }

            await _data.DeleteVehicleAsync(id);
            return Json(new { success = true, message = "Vehicle registration deleted successfully!" });
        }
    }
}


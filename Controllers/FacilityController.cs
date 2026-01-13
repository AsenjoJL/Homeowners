using HOMEOWNER.Data;
using HOMEOWNER.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacilityController : BaseController
    {
        private readonly ILogger<FacilityController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FacilityController(IDataService data, ILogger<FacilityController> logger, IWebHostEnvironment webHostEnvironment) : base(data)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: Facility
        public IActionResult Index()
        {
            var facilities = _data.Facilities.ToList();
            return View(facilities);
        }

        // GET: Facility/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Facility/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Facility facility, List<IFormFile> ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input. Please check the form." });
            }

            try
            {
                // Check for duplicate facility name
                var existingFacility = _data.Facilities.FirstOrDefault(f => f.FacilityName == facility.FacilityName);
                if (existingFacility != null)
                {
                    return Json(new { success = false, message = "Facility name already exists!" });
                }

                // Process image files
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "facilities");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    List<string> imagePaths = new List<string>();

                    foreach (var imageFile in ImageFiles)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        imagePaths.Add("/images/facilities/" + fileName);
                    }

                    facility.ImageUrl = string.Join(",", imagePaths);
                }

                // Set default availability status
                facility.AvailabilityStatus = "Available";

                // Add facility to Firebase
                await _data.AddFacilityAsync(facility);

                var facilities = _data.Facilities.ToList();
                return Json(new { success = true, message = "Facility added successfully.", facilities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding facility");
                return Json(new { success = false, message = "An error occurred. Please try again." });
            }
        }






        // GET: Facility/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var facility = await _data.GetFacilityByIdAsync(id.Value);
            if (facility == null) return NotFound();

            return View(facility);
        }

        // POST: Facility/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facility facility, IFormFile ImageFile)
        {
            if (id != facility.FacilityID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "facilities");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        facility.ImageUrl = "/images/facilities/" + fileName;
                    }

                    await _data.UpdateFacilityAsync(facility);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating facility");
                    if (await _data.GetFacilityByIdAsync(id) == null) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        // DELETE: Facility/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _data.GetFacilityByIdAsync(id);
            if (facility != null)
            {
                await _data.DeleteFacilityAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
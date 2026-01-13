using HOMEOWNER.Data;
using HOMEOWNER.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DocumentController(IDataService data, IWebHostEnvironment hostingEnvironment) : base(data)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // Admin: Upload Document
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Upload()
        {
            return PartialView("Upload");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string title, string description, string category)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file." });
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new { success = false, message = "Title is required." });
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "documents");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                var relativePath = $"/uploads/documents/{fileName}";

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Get admin ID
                var email = User.FindFirstValue(ClaimTypes.Email);
                var admin = await _data.GetAdminByEmailAsync(email ?? "");
                var adminId = admin?.AdminID ?? 1;

                // Create document record
                var document = new Document
                {
                    Title = title,
                    Description = description,
                    Category = category,
                    FilePath = relativePath,
                    FileType = Path.GetExtension(file.FileName).TrimStart('.'),
                    FileSize = file.Length,
                    UploadedByAdminID = adminId,
                    UploadedAt = DateTime.UtcNow,
                    IsPublic = true,
                    DownloadCount = 0
                };

                await _data.AddDocumentAsync(document);

                return Json(new { success = true, message = "Document uploaded successfully!", documentId = document.DocumentID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error uploading file: {ex.Message}" });
            }
        }

        // View all documents (Homeowner & Admin)
        public IActionResult Index(string category = "All")
        {
            var documents = _data.Documents.Where(d => d.IsPublic).ToList();
            
            if (category != "All")
            {
                documents = documents.Where(d => d.Category == category).ToList();
            }

            ViewBag.Categories = new[] { "All", "Forms", "Guidelines", "Financial Reports", "Meeting Minutes" };
            ViewBag.SelectedCategory = category;

            return PartialView("Index", documents);
        }

        // Download document
        public async Task<IActionResult> Download(int id)
        {
            var document = await _data.GetDocumentByIdAsync(id);
            if (document == null || !document.IsPublic)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Increment download count
            await _data.IncrementDownloadCountAsync(id);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", $"{document.Title}{Path.GetExtension(document.FilePath)}");
        }

        // Admin: Manage Documents
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var documents = _data.Documents.OrderByDescending(d => d.UploadedAt).ToList();
            return PartialView("Manage", documents);
        }

        // Admin: Delete Document
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _data.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            try
            {
                // Delete physical file
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Delete database record
                await _data.DeleteDocumentAsync(id);

                return Json(new { success = true, message = "Document deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting document: {ex.Message}" });
            }
        }
    }
}


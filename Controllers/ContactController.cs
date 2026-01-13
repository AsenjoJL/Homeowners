using HOMEOWNER.Data;
using HOMEOWNER.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public class ContactController : BaseController
    {
        public ContactController(IDataService data) : base(data)
        {
        }

        // View Contact Directory (All users)
        public IActionResult Index()
        {
            var contacts = _data.Contacts.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Category).ToList();
            
            var groupedContacts = contacts.GroupBy(c => c.Category).ToList();
            
            return PartialView("Index", groupedContacts);
        }

        // Admin: Manage Contacts
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var contacts = _data.Contacts.OrderBy(c => c.Category).ThenBy(c => c.DisplayOrder).ToList();
            return PartialView("Manage", contacts);
        }

        // Admin: Add Contact
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("Add");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            await _data.AddContactAsync(contact);
            return Json(new { success = true, message = "Contact added successfully!", contactId = contact.ContactID });
        }

        // Admin: Edit Contact
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _data.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return PartialView("Edit", contact);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            await _data.UpdateContactAsync(contact);
            return Json(new { success = true, message = "Contact updated successfully!" });
        }

        // Admin: Delete Contact
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _data.GetContactByIdAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Contact not found." });
            }

            await _data.DeleteContactAsync(id);
            return Json(new { success = true, message = "Contact deleted successfully!" });
        }
    }
}


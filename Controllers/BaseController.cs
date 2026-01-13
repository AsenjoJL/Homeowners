using HOMEOWNER.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMEOWNER.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected readonly IDataService _data;

        protected BaseController(IDataService data)
        {
            _data = data;
        }

        /// <summary>
        /// Gets the current user's email from claims
        /// </summary>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Gets the current user's role from claims
        /// </summary>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Gets the current user's name from claims
        /// </summary>
        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Gets the current homeowner ID from session or claims
        /// </summary>
        protected int GetCurrentHomeownerId()
        {
            // Try to get from session first
            var sessionId = HttpContext.Session.GetInt32("HomeownerID");
            if (sessionId.HasValue)
                return sessionId.Value;

            // Fallback to claims
            var homeownerIdClaim = User.FindFirst("HomeownerID")?.Value;
            if (!string.IsNullOrEmpty(homeownerIdClaim) && int.TryParse(homeownerIdClaim, out int id))
                return id;

            // Try to get from email lookup
            var email = GetCurrentUserEmail();
            if (!string.IsNullOrEmpty(email))
            {
                var homeowner = _data.GetHomeownerByEmailAsync(email).GetAwaiter().GetResult();
                if (homeowner != null)
                {
                    HttpContext.Session.SetInt32("HomeownerID", homeowner.HomeownerID);
                    return homeowner.HomeownerID;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the current admin ID from claims or email lookup
        /// </summary>
        protected int GetCurrentAdminId()
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email))
                return 0;

            var admin = _data.GetAdminByEmailAsync(email).GetAwaiter().GetResult();
            return admin?.AdminID ?? 0;
        }

        /// <summary>
        /// Gets the current staff ID from session or claims
        /// </summary>
        protected int GetCurrentStaffId()
        {
            // Try to get from session first
            var sessionId = HttpContext.Session.GetInt32("StaffID");
            if (sessionId.HasValue)
                return sessionId.Value;

            // Fallback to email lookup
            var email = GetCurrentUserEmail();
            if (!string.IsNullOrEmpty(email))
            {
                var staff = _data.GetStaffByEmailAsync(email).GetAwaiter().GetResult();
                if (staff != null)
                {
                    HttpContext.Session.SetInt32("StaffID", staff.StaffID);
                    return staff.StaffID;
                }
            }

            return 0;
        }

        /// <summary>
        /// Checks if the current user is an admin
        /// </summary>
        protected bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        /// <summary>
        /// Checks if the current user is a homeowner
        /// </summary>
        protected bool IsHomeowner()
        {
            return GetCurrentUserRole() == "Homeowner";
        }

        /// <summary>
        /// Checks if the current user is staff
        /// </summary>
        protected bool IsStaff()
        {
            return GetCurrentUserRole() == "Staff";
        }
    }
}


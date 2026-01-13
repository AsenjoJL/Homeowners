using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Admin
    {
        [FirestoreProperty]
        [Key]
        public int AdminID { get; set; } // 🔹 Ensure consistency in naming

        [FirestoreProperty]
        [Required]
        public string OfficeLocation { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string Status { get; set; } = "Active";

        [FirestoreProperty]
        [Required]
        public string FullName { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string Role { get; set; } = "Admin";
    }
}

using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Staff
    {
        [FirestoreProperty]
        [Key]
        public int StaffID { get; set; }

        [FirestoreProperty]
        [Required]
        public string? FullName { get; set; }

        [FirestoreProperty]
        [Required, EmailAddress]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required]
        public string? PhoneNumber { get; set; }

        [FirestoreProperty]
        public string? Position { get; set; }  // maintenance, security

        [FirestoreProperty]
        [Required]
        public string? PasswordHash { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public int AdminID { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Contact
    {
        [FirestoreProperty]
        [Key]
        public int ContactID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // HOA Officer, Security, Maintenance, Emergency

        [FirestoreProperty]
        [StringLength(100)]
        public string? Position { get; set; } // e.g., "President", "Security Guard", "Maintenance Head"

        [FirestoreProperty]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [FirestoreProperty]
        [StringLength(20)]
        public string? MobileNumber { get; set; }

        [FirestoreProperty]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [FirestoreProperty]
        [StringLength(200)]
        public string? OfficeLocation { get; set; }

        [FirestoreProperty]
        [StringLength(100)]
        public string? Department { get; set; }

        [FirestoreProperty]
        public bool IsEmergency { get; set; } = false;

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0; // For sorting
    }
}


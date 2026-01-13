using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class VehicleRegistration
    {
        [FirestoreProperty]
        [Key]
        public int VehicleID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty; // Car, Motorcycle, SUV, Truck, etc.

        [FirestoreProperty]
        [StringLength(50)]
        public string? Make { get; set; } // Toyota, Honda, etc.

        [FirestoreProperty]
        [StringLength(50)]
        public string? Model { get; set; }

        [FirestoreProperty]
        [StringLength(20)]
        public string? Color { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Expired

        [FirestoreProperty]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? ApprovedAt { get; set; }

        [FirestoreProperty]
        public int? ApprovedByAdminID { get; set; }

        [FirestoreProperty]
        public DateTime? ExpiryDate { get; set; }

        [FirestoreProperty]
        [StringLength(500)]
        public string? AdminNotes { get; set; }

        // Navigation property
        public Homeowner? Homeowner { get; set; }
    }
}


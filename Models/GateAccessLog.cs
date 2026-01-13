using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class GateAccessLog
    {
        [FirestoreProperty]
        [Key]
        public int LogID { get; set; }

        [FirestoreProperty]
        public int? HomeownerID { get; set; } // Null if visitor

        [FirestoreProperty]
        [StringLength(100)]
        public string? VisitorName { get; set; }

        [FirestoreProperty]
        [StringLength(20)]
        public string? PlateNumber { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string AccessType { get; set; } = string.Empty; // Entry, Exit

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string UserType { get; set; } = string.Empty; // Homeowner, Visitor, Staff, Delivery

        [FirestoreProperty]
        [Required]
        public DateTime AccessTime { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        [StringLength(50)]
        public string? GateLocation { get; set; } // Main Gate, Side Gate, etc.

        [FirestoreProperty]
        [StringLength(100)]
        public string? VerifiedBy { get; set; } // Security guard name

        [FirestoreProperty]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}


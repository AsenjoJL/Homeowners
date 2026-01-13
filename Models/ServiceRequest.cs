using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class ServiceRequest
    {
        [FirestoreProperty]
        [Key]
        public int RequestID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(50)]
        public string? Category { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string? Priority { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(255)]
        public string? Description { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "Pending";

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [FirestoreProperty]
        [StringLength(50)]
        public string? RequestType { get; set; }

        [FirestoreProperty]
        public int? AssignedStaffID { get; set; }

        [FirestoreProperty]
        public DateTime? DeletedAt { get; set; }

        [FirestoreProperty]
        public DateTime? CompletedAt { get; set; }

        // Navigation properties (not stored in Firestore)
        public Homeowner? Homeowner { get; set; }
        public Staff? AssignedStaff { get; set; }
    }
}

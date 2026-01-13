using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Complaint
    {
        [FirestoreProperty]
        [Key]
        public int ComplaintID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        [StringLength(50)]
        public string? Category { get; set; } // Noise, Maintenance, Security, Neighbor Issue, etc.

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Submitted"; // Submitted, Under Review, In Progress, Resolved, Closed

        [FirestoreProperty]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? ReviewedAt { get; set; }

        [FirestoreProperty]
        public int? ReviewedByAdminID { get; set; }

        [FirestoreProperty]
        public DateTime? ResolvedAt { get; set; }

        [FirestoreProperty]
        [StringLength(2000)]
        public string? AdminResponse { get; set; }

        [FirestoreProperty]
        [StringLength(500)]
        public string? ResolutionNotes { get; set; }

        [FirestoreProperty]
        public int? Priority { get; set; } = 1; // 1=Low, 2=Medium, 3=High, 4=Urgent

        [FirestoreProperty]
        public bool IsAnonymous { get; set; } = false;

        // Navigation property
        public Homeowner? Homeowner { get; set; }
    }
}


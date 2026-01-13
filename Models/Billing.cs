using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Billing
    {
        [FirestoreProperty]
        [Key]
        public int BillingID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public decimal Amount { get; set; }

        [FirestoreProperty]
        [Required]
        public DateTime DueDate { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(50)]
        public string BillType { get; set; } = string.Empty; // Association Fee, Maintenance Fee, etc.

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? PaidAt { get; set; }

        [FirestoreProperty]
        public string? PaymentMethod { get; set; }

        [FirestoreProperty]
        public string? TransactionID { get; set; }

        // Navigation property (not stored in Firestore, loaded separately)
        public Homeowner? Homeowner { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Homeowner
    {
        [FirestoreProperty]
        [Key]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        public string? FullName { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        public string ContactNumber { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Address { get; set; }

        [FirestoreProperty]
        public string? Role { get; set; } = "Homeowner";

        [FirestoreProperty]
        public string? BlockLotNumber { get; set; }

        [FirestoreProperty]
        public string? PasswordHash { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public int AdminID { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;
    }
}

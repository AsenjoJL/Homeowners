using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class HomeownerProfileImage
    {
        [FirestoreProperty]
        [Key]
        public int ImageID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(500)]
        public string? ImagePath { get; set; }

        [FirestoreProperty]
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [FirestoreProperty]
        public int ChangeCount { get; set; } = 0;

        [FirestoreProperty]
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow.Date;
    }
}

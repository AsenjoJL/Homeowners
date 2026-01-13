using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Announcement
    {
        [FirestoreProperty]
        [Key]
        public int AnnouncementID { get; set; }

        [FirestoreProperty]
        public string? Title { get; set; }

        [FirestoreProperty]
        public string? Content { get; set; }

        [FirestoreProperty]
        public DateTime PostedAt { get; set; }

        [FirestoreProperty]
        public bool IsUrgent { get; set; }
    }
}

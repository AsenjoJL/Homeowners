using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class CommunitySettings
    {
        [FirestoreProperty]
        [Key]
        public int CommunitySettingsID { get; set; }

        [FirestoreProperty]
        public string BackgroundImageUrl { get; set; } = "/images/default-forum-bg.jpg";

        [FirestoreProperty]
        public string? CustomCSS { get; set; }

        [FirestoreProperty]
        public string? FeaturedMusicUrl { get; set; }

        [FirestoreProperty]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}

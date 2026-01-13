using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class ForumPost
    {
        [FirestoreProperty]
        [Key]
        public int ForumPostID { get; set; }

        [FirestoreProperty]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Content { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [FirestoreProperty]
        public string? MediaUrl { get; set; }

        [FirestoreProperty]
        public string? MediaType { get; set; }

        [FirestoreProperty]
        public string? MusicUrl { get; set; }

        [FirestoreProperty]
        public string? MusicTitle { get; set; }

        // Navigation properties (not stored in Firestore, loaded separately)
        public List<ForumComment> Comments { get; set; } = new();
        public List<Reaction> Reactions { get; set; } = new();
        public Homeowner? Homeowner { get; set; }
    }
}


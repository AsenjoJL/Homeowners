using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class ForumComment
    {
        [FirestoreProperty]
        [Key]
        public int ForumCommentID { get; set; }

        [FirestoreProperty]
        public int ForumPostID { get; set; }

        [FirestoreProperty]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        public string CommentText { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [FirestoreProperty]
        public string? MediaUrl { get; set; }

        // Navigation properties (not stored in Firestore)
        public ForumPost? ForumPost { get; set; }
        public Homeowner? Homeowner { get; set; }
    }
}

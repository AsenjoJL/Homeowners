using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Reaction
    {
        [FirestoreProperty]
        [Key]
        public int ReactionID { get; set; }

        [FirestoreProperty]
        public string ReactionType { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public int ForumPostID { get; set; }

        [FirestoreProperty]
        public int HomeownerID { get; set; }

        // Navigation properties (not stored in Firestore)
        public ForumPost? ForumPost { get; set; }
        public Homeowner? Homeowner { get; set; }
    }
}

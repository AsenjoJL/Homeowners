using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class EventModel
    {
        [FirestoreProperty]
        [Key]
        public int EventID { get; set; }

        [FirestoreProperty]
        public string? Title { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public DateTime EventDate { get; set; }

        [FirestoreProperty]
        public string? Category { get; set; }

        [FirestoreProperty]
        public string? Location { get; set; }

        [FirestoreProperty]
        public int CreatedBy { get; set; }
    }
}

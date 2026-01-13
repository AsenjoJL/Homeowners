using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Reservation
    {
        [FirestoreProperty]
        [Key]
        public int ReservationID { get; set; }

        [FirestoreProperty]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        public int FacilityID { get; set; }

        [FirestoreProperty]
        public DateTime ReservationDate { get; set; }

        [FirestoreProperty]
        public TimeSpan StartTime { get; set; }

        [FirestoreProperty]
        public TimeSpan EndTime { get; set; }

        [FirestoreProperty]
        public string? Status { get; set; }

        [FirestoreProperty]
        public string? Purpose { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; }

        [FirestoreProperty]
        public int? Rating { get; set; }

        // Navigation properties (not stored in Firestore)
        public Homeowner? Homeowner { get; set; }
        public Facility? Facility { get; set; }
    }
}

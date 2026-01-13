using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Facility
    {
        [FirestoreProperty]
        [Key]
        public int FacilityID { get; set; }

        [FirestoreProperty]
        public string? FacilityName { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public int Capacity { get; set; }

        [FirestoreProperty]
        public string? AvailabilityStatus { get; set; }

        [FirestoreProperty]
        public string? ImageUrl { get; set; }

        [FirestoreProperty]
        public int? Rating { get; set; } = 0;
    }
}
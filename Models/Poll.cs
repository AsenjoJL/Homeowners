using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Poll
    {
        [FirestoreProperty]
        [Key]
        public int PollID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(200)]
        public string Question { get; set; } = string.Empty;

        [FirestoreProperty]
        [StringLength(500)]
        public string? Description { get; set; }

        [FirestoreProperty]
        [Required]
        public int CreatedByAdminID { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? StartDate { get; set; }

        [FirestoreProperty]
        public DateTime? EndDate { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Active, Closed

        [FirestoreProperty]
        public bool IsAnonymous { get; set; } = false; // Hide voter identities

        [FirestoreProperty]
        public bool AllowMultipleChoices { get; set; } = false;

        [FirestoreProperty]
        public int TotalVotes { get; set; } = 0;

        // Navigation properties
        public List<PollOption> Options { get; set; } = new();
        public List<PollVote> Votes { get; set; } = new();
    }

    [FirestoreData]
    public class PollOption
    {
        [FirestoreProperty]
        [Key]
        public int OptionID { get; set; }

        [FirestoreProperty]
        [Required]
        public int PollID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(200)]
        public string OptionText { get; set; } = string.Empty;

        [FirestoreProperty]
        public int VoteCount { get; set; } = 0;

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0;
    }

    [FirestoreData]
    public class PollVote
    {
        [FirestoreProperty]
        [Key]
        public int VoteID { get; set; }

        [FirestoreProperty]
        [Required]
        public int PollID { get; set; }

        [FirestoreProperty]
        [Required]
        public int OptionID { get; set; }

        [FirestoreProperty]
        [Required]
        public int HomeownerID { get; set; }

        [FirestoreProperty]
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}


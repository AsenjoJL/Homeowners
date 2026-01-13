using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace HOMEOWNER.Models
{
    [FirestoreData]
    public class Document
    {
        [FirestoreProperty]
        [Key]
        public int DocumentID { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        [StringLength(500)]
        public string? Description { get; set; }

        [FirestoreProperty]
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Forms, Guidelines, Financial Reports, Meeting Minutes

        [FirestoreProperty]
        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [FirestoreProperty]
        [StringLength(50)]
        public string? FileType { get; set; } // pdf, docx, xlsx, etc.

        [FirestoreProperty]
        public long FileSize { get; set; } // in bytes

        [FirestoreProperty]
        [Required]
        public int UploadedByAdminID { get; set; }

        [FirestoreProperty]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public bool IsPublic { get; set; } = true; // Public to all homeowners

        [FirestoreProperty]
        public int DownloadCount { get; set; } = 0;

        // Navigation property
        public Admin? UploadedByAdmin { get; set; }
    }
}


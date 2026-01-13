using System.ComponentModel.DataAnnotations;

namespace HOMEOWNER.Models.ViewModels
{
    public class CreatePollViewModel
    {
        [Required]
        [StringLength(200)]
        public string Question { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";

        public bool IsAnonymous { get; set; } = false;

        public bool AllowMultipleChoices { get; set; } = false;

        [Required]
        [MinLength(2, ErrorMessage = "At least 2 options are required")]
        public List<string> Options { get; set; } = new();
    }
}


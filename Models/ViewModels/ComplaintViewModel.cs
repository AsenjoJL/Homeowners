using System.ComponentModel.DataAnnotations;

namespace HOMEOWNER.Models.ViewModels
{
    public class ComplaintViewModel
    {
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        public int Priority { get; set; } = 1; // 1=Low, 2=Medium, 3=High, 4=Urgent

        public bool IsAnonymous { get; set; } = false;
    }
}


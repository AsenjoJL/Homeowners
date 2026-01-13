using System.ComponentModel.DataAnnotations;

namespace HOMEOWNER.Models.ViewModels
{
    public class VisitorPassViewModel
    {
        [Required]
        public string VisitorName { get; set; } = string.Empty;
        
        public string? VisitorPhone { get; set; }
        
        public string? VisitorIDNumber { get; set; }
        
        public string? VehiclePlateNumber { get; set; }
        
        public string? VehicleType { get; set; }
        
        [Required]
        public DateTime VisitDate { get; set; }
        
        public TimeSpan? ExpectedArrivalTime { get; set; }
        
        public string? Purpose { get; set; }
    }
}


namespace HOMEOWNER.Models.ViewModels
{
    public class GateAccessStatistics
    {
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int HomeownerEntries { get; set; }
        public int VisitorEntries { get; set; }
        public int StaffEntries { get; set; }
        public int DeliveryEntries { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}


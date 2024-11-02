namespace KartverketApplikasjon.Models
{
    public class DashboardViewModel
    {
        public int PendingCount { get; set; }
        public int ApprovedThisWeek { get; set; }
        public int RejectedThisWeek { get; set; }
        public List<MapCorrections> RecentSubmissions { get; set; }

        public DashboardViewModel()
        {
            RecentSubmissions = new List<MapCorrections>();
        }
    }
}
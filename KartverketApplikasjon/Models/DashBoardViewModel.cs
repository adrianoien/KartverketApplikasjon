using KartverketApplikasjon.Data;

namespace KartverketApplikasjon.Models
{
    public class DashboardViewModel
    {
        public int PendingCount { get; set; }
        public int ApprovedThisWeek { get; set; }
        public int RejectedThisWeek { get; set; }
        public List<MapCorrections> RecentMapCorrections { get; set; }
        public List<GeoChange> RecentAreaChanges { get; set; }

        public DashboardViewModel()
        {
            RecentMapCorrections = new List<MapCorrections>();
            RecentAreaChanges = new List<GeoChange>();
        }
    }

}
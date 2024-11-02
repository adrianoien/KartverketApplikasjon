using KartverketApplikasjon.Data;

namespace KartverketApplikasjon.Models
{
    public class UnifiedMapViewModel
    {
        public List<GeoChange> Changes { get; set; }
        public List<MapCorrections> Positions { get; set; }

        public UnifiedMapViewModel()
        {
            Changes = new List<GeoChange>();
            Positions = new List<MapCorrections>();
        }
    }
}
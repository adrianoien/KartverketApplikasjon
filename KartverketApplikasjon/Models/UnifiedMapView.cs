using System.Collections.Generic;

namespace KartverketApplikasjon.Models
{
    public class UnifiedMapViewModel
    {
        public List<MapCorrections> Positions { get; set; }
        public List<AreaChange> Changes { get; set; }

        public UnifiedMapViewModel()
        {
            Positions = new List<MapCorrections>();
            Changes = new List<AreaChange>();
        }
    }
}
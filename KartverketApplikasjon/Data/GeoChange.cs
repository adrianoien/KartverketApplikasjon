using System.ComponentModel.DataAnnotations;
using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Data
{
    public class GeoChange
    {
        [Key]
        public int Id { get; set; }
        public string GeoJson { get; set; }
        public string Description { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public CorrectionStatus Status { get; set; } = CorrectionStatus.Pending;
    }
}
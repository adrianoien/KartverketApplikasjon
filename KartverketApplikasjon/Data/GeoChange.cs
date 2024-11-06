using System.ComponentModel.DataAnnotations;
using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Data
{
    public class GeoChange
    {
        public int Id { get; set; }
        public string GeoJson { get; set; }
        public string Description { get; set; }
        public CorrectionStatus Status { get; set; } = CorrectionStatus.Pending;
        public string? ReviewComment { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedDate { get; set; }
        public string SubmittedBy { get; set; }
    }
}
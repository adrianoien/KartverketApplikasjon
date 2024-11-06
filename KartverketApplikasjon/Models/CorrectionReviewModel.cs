using KartverketApplikasjon.Models;
public class CorrectionReviewViewModel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public CorrectionStatus Status { get; set; }
    public string? ReviewComment { get; set; }
    public string SubmittedBy { get; set; }
    public string? Municipality { get; set; }  // For displaying kommune

    public string Type { get; set; }  // "map" or "area"
    public string GeoJson { get; set; }  // For area changes
    public DateTime SubmittedDate { get; set; }  // Date of submission
}

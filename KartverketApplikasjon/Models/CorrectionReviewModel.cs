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
    public DateTime SubmittedDate { get; set; }
}
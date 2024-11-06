namespace KartverketApplikasjon.Controllers { 
public class ChangeSubmissionModel
{
    public string Type { get; set; } // "area" or "point"
    public string GeoJson { get; set; }
    public string Description { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}
}
namespace KartverketApplikasjon.Models
{
    public class AreaChange
    {
        public string Id { get; set; }
        public string GeoJson { get; set; } //GeoJSON format for points, lines or polygons
        public string Description { get; set; }
    }
}
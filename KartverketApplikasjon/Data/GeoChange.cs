using System.Security.Permissions;

namespace KartverketApplikasjon.Data

{
    public class GeoChange
    {
        public int Id { get; set; }
        public string? GeoJson { get; set; }
        public string? Description { get; set; }
    }
}
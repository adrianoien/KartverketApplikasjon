// Models/Kommune.cs
using System.Text.Json.Serialization;

namespace KartverketApplikasjon.Models
{
    public class Kommune
    {
        [JsonPropertyName("kommunenummer")]
        public string KommuneNummer { get; set; }

        [JsonPropertyName("kommunenavnNorsk")]
        public string KommuneNavn { get; set; }

        [JsonPropertyName("fylkesnavn")]
        public string Fylke { get; set; }

        // Adds more fields if its needed based one the API-respons
    }
}
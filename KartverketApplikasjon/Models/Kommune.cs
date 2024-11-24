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

        // Legg til flere felt hvis nødvendig basert på API-responsen
    }
}
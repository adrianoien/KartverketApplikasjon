// Services/KommuneService.cs
using KartverketApplikasjon.Models;
using System.Net.Http.Json;

namespace KartverketApplikasjon.Services
{
    public interface IKommuneService
    {
        Task<List<Kommune>> HentAlleKommuner();
        Task<Kommune> HentKommune(string kommuneNummer);
    }

    public class KommuneService : IKommuneService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.kartverket.no/kommuneinfo/v1";

        public KommuneService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Kommune>> HentAlleKommuner()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/kommuner");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Kommune>>();

                // Debug logging
                Console.WriteLine($"Hentet {result?.Count} kommuner");

                return result ?? new List<Kommune>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil ved henting av kommuner: {ex.Message}");
                throw new Exception("Kunne ikke hente kommuner", ex);
            }
        }

        public async Task<Kommune> HentKommune(string kommuneNummer)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/kommuner/{kommuneNummer}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Kommune>();
                return result ?? throw new Exception($"Ingen kommune funnet med nummer {kommuneNummer}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil ved henting av kommune {kommuneNummer}: {ex.Message}");
                throw new Exception($"Kunne ikke hente kommune med nummer {kommuneNummer}", ex);
            }
        }
    }
}
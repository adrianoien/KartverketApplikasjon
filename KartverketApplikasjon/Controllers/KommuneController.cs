// Controllers/KommuneController.cs
using KartverketApplikasjon.Models;
using KartverketApplikasjon.Services;
using Microsoft.AspNetCore.Mvc;

namespace KartverketApplikasjon.Controllers
{
    public class KommuneController : Controller
    {
        private readonly IKommuneService _kommuneService;

        public KommuneController(IKommuneService kommuneService)
        {
            _kommuneService = kommuneService;
        }

        [HttpGet]
        [Route("[controller]/HentKommuner")]
        public async Task<IActionResult> HentKommuner()
        {
            try
            {
                var kommuner = await _kommuneService.HentAlleKommuner();
                return Json(kommuner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("[controller]/HentKommune/{kommuneNummer}")]
        public async Task<IActionResult> HentKommune(string kommuneNummer)
        {
            try
            {
                var kommune = await _kommuneService.HentKommune(kommuneNummer);
                return Json(kommune);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
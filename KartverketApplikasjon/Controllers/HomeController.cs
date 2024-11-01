using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KartverketApplikasjon.Models;

using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using KartverketApplikasjon.Data;

namespace KartverketApplikasjon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UnifiedMapView()
        {
            return View();
        }

        public IActionResult Kart()
        {
            return View();
        }

        [Authorize]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                // Insert data using EF
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data.");
                }

                var newGeoChange = new GeoChange
                {
                    GeoJson = geoJson,
                    Description = description
                };

                _context.GeoChanges.Add(newGeoChange);
                _context.SaveChanges();
                // Redirect to the overview of changes
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<IActionResult> AreaChangeOverview()
        {
            var changes = await _context.GeoChanges.ToListAsync();
            return View(changes);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChanges()
        {
            try
            {
                var changes = await _context.GeoChanges.ToListAsync();
                return Json(changes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving changes: {ex.Message}");
                return StatusCode(500, "Error retrieving changes");
            }
        }

        public IActionResult CorrectionsOverview()
        {
            return View();
        }

        public IActionResult CorrectMap()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // API endpoints for map corrections
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveCorrection([FromBody] GeoChange correction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.GeoChanges.Add(correction);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Correction saved successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving correction: {ex.Message}");
                    return Json(new { success = false, message = "Error saving correction" });
                }
            }
            return Json(new { success = false, message = "Invalid model state" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCorrections()
        {
            try
            {
                var corrections = await _context.GeoChanges
                    .Where(c => c.GeoJson != null)
                    .ToListAsync();
                return Json(corrections);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving corrections: {ex.Message}");
                return StatusCode(500, "Error retrieving corrections");
            }
        }
    }
}
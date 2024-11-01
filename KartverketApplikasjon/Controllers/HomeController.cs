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
            // Initialize with a new AreaChange model
            var model = new AreaChange
            {
                Id = "",  // Initialize with empty string since your AreaChange model uses string Id
                GeoJson = "",
                Description = ""
            };
            return View(model);
        }

        public async Task<IActionResult> CorrectionsOverview()
        {
            try
            {
                // Create a sample list for testing (remove this when implementing database)
                var corrections = new List<MapCorrections>
        {
            new MapCorrections
            {
                Latitude = "59.913868",
                Longitude = "10.752245",
                Description = "Test correction - Oslo"
            }
        };

                return View(corrections);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving corrections: {ex.Message}");
                return View(new List<MapCorrections>()); // Return empty list on error
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterAreaChange(string geoJson, string description)
        {
            try
            {
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
                await _context.SaveChangesAsync();
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                // Return to view with error message
                ModelState.AddModelError("", "Failed to save changes. Please try again.");
                return View(new AreaChange { GeoJson = geoJson, Description = description });
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
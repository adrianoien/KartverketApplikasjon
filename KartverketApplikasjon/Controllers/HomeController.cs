using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using KartverketApplikasjon.Models;
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

        [HttpPost]
        public async Task<IActionResult> DeleteCorrection(int id, string type)
        {
            try
            {
                if (type == "point")
                {
                    var correction = await _context.MapCorrections
                        .FirstOrDefaultAsync(c => c.Id == id && c.SubmittedBy == User.Identity.Name);

                    if (correction != null)
                    {
                        _context.MapCorrections.Remove(correction);
                        await _context.SaveChangesAsync();

                        return Json(new
                        {
                            success = true,
                            latitude = correction.Latitude,
                            longitude = correction.Longitude
                        });
                    }
                    return Json(new { success = false, message = "Markør ikke funnet" });
                }

                // Adds to this default return statement
                return Json(new { success = false, message = "Ugyldig type" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting correction: {ex.Message}");
                return Json(new { success = false, message = "En feil oppstod ved sletting av markøren" });
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Kart()
        {
            return View();
        }

        // Redirect to the AreaChange registration page, only accessible to authorized users
        [Authorize]
        public IActionResult RegisterAreaChange()
        {
            return RedirectToAction("Register", "AreaChange");
        }

        // Display an overview of map corrections with a "Pending" status
        public async Task<IActionResult> CorrectionsOverview()
        {
            try
            {
                // Retrieve pending corrections, ordered by submission date (latest first)
                var corrections = await _context.MapCorrections
                    .Where(c => c.Status == CorrectionStatus.Pending)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                return View(corrections);
            }
            catch (Exception ex)
            {
                _logger.LogError($"En feil oppsto ved henting av kartkorreksjoner: {ex.Message}");
                return View(new List<MapCorrections>()); // Return empty list on error
            }
        }
        // Display a view of all submissions by the currently logged-in user, only accessible to authorized users   
        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            try
            {
                // Retrieve map corrections submitted by the current user
                var userSubmissions = await _context.MapCorrections
                    .Where(c => c.SubmittedBy == User.Identity.Name)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                // Retrieve area changes submitted by the current user
                var areaChanges = await _context.GeoChanges
                    .Where(c => c.SubmittedBy == User.Identity.Name)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                // Combine map corrections and area changes into a single view model
                var model = new UnifiedMapViewModel
                {
                    Changes = areaChanges,
                    Positions = userSubmissions
                };

                // Display the submissions view with the unified model
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving submissions: {ex.Message}");
                return View(new UnifiedMapViewModel());
            }
        }
        // Display an error page with no cache settings for immediate refresh
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Pass an ErrorViewModel to the view with the current request ID (for tracking the error)
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}

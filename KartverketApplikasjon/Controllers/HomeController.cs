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

        [Authorize]
        public IActionResult RegisterAreaChange()
        {
            return RedirectToAction("Register", "AreaChange");
        }

        public async Task<IActionResult> CorrectionsOverview()
        {
            try
            {
                var corrections = await _context.MapCorrections
                    .Where(c => c.Status == CorrectionStatus.Pending)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                return View(corrections);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving corrections: {ex.Message}");
                return View(new List<MapCorrections>()); // Return empty list on error
            }
        }

        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            try
            {
                var userSubmissions = await _context.MapCorrections
                    .Where(c => c.SubmittedBy == User.Identity.Name)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                var areaChanges = await _context.GeoChanges
                    .Where(c => c.SubmittedBy == User.Identity.Name)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                var model = new UnifiedMapViewModel
                {
                    Changes = areaChanges,
                    Positions = userSubmissions
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving submissions: {ex.Message}");
                return View(new UnifiedMapViewModel());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
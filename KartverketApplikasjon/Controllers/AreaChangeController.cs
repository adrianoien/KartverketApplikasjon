using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KartverketApplikasjon.Models;
using KartverketApplikasjon.Data;

namespace KartverketApplikasjon.Controllers
{
    [Authorize]
    public class AreaChangeController : Controller
    {
        private readonly ILogger<AreaChangeController> _logger;
        private readonly ApplicationDbContext _context;

        public AreaChangeController(ILogger<AreaChangeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Main view for registering changes
        public IActionResult Register()
        {
            var model = new UnifiedMapViewModel
            {
                Changes = new List<GeoChange>(),
                Positions = new List<MapCorrections>()
            };
            return View(model);
        }

        // Save both area changes and point corrections
        [HttpPost]
        public async Task<IActionResult> SaveChange([FromBody] ChangeSubmissionModel submission)
        {
            try
            {
                if (string.IsNullOrEmpty(submission.Description))
                {
                    return BadRequest("Invalid data.");
                }

                if (submission.Type == "area")
                {
                    var geoChange = new GeoChange
                    {
                        GeoJson = submission.GeoJson,
                        Description = submission.Description,
                        SubmittedBy = User.Identity.Name,
                        SubmittedDate = DateTime.UtcNow,
                        Status = CorrectionStatus.Pending
                    };
                    _context.GeoChanges.Add(geoChange);
                }
                else if (submission.Type == "point")
                {
                    var pointCorrection = new MapCorrections
                    {
                        Description = submission.Description,
                        Latitude = submission.Latitude,
                        Longitude = submission.Longitude,
                        Status = CorrectionStatus.Pending,
                        SubmittedBy = User.Identity.Name,
                        SubmittedDate = DateTime.UtcNow
                    };
                    _context.MapCorrections.Add(pointCorrection);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Change saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving change: {ex.Message}");
                return Json(new { success = false, message = "Error saving change" });
            }
        }

        // Get all changes for the map
        [HttpGet]
        public async Task<IActionResult> GetAllChanges()
        {
            try
            {
                var areaChanges = await _context.GeoChanges.ToListAsync();
                var pointCorrections = await _context.MapCorrections
                    .Where(c => c.SubmittedBy == User.Identity.Name)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                return Json(new
                {
                    areas = areaChanges,
                    points = pointCorrections
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving changes: {ex.Message}");
                return StatusCode(500, "Error retrieving changes");
            }
        }

        // View user's submissions
        public async Task<IActionResult> MySubmissions()
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
    }
}
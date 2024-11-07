using KartverketApplikasjon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // For ToListAsync()
using KartverketApplikasjon.Models;  // For MapCorrections and CorrectionStatus
using KartverketApplikasjon.Controllers;

[Authorize(Roles = "Saksbehandler")]
public class CorrectionManagementController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CorrectionManagementController> _logger;

    public CorrectionManagementController(ApplicationDbContext context,
        ILogger<CorrectionManagementController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // List all corrections with filtering
    public async Task<IActionResult> Index(string status = "Pending")
    {
        var correctionStatus = Enum.TryParse<CorrectionStatus>(status, out var parsedStatus)
            ? parsedStatus
            : CorrectionStatus.Pending;

        // Get map corrections
        var mapCorrectionsQuery = _context.MapCorrections.AsQueryable();
        if (status != "All")
        {
            mapCorrectionsQuery = mapCorrectionsQuery.Where(c => c.Status == correctionStatus);
        }
        IEnumerable<MapCorrections> mapCorrections = await mapCorrectionsQuery
            .OrderByDescending(c => c.SubmittedDate)
            .ToListAsync();

        // Get area changes
        var areaChangesQuery = _context.GeoChanges.AsQueryable();
        if (status != "All")
        {
            areaChangesQuery = areaChangesQuery.Where(c => c.Status == correctionStatus);
        }
        IEnumerable<GeoChange> areaChanges = await areaChangesQuery
            .OrderByDescending(c => c.SubmittedDate)
            .ToListAsync();

        ViewBag.CurrentFilter = status;
        return View((MapCorrections: mapCorrections, AreaChanges: areaChanges));
    }

    // Show detailed view of a correction
    [HttpGet]
    public async Task<IActionResult> Review(int id, string type)
    {
        if (type == "map")
        {
            var correction = await _context.MapCorrections.FindAsync(id);
            if (correction == null)
                return NotFound();


            var viewModel = new CorrectionReviewViewModel
            {
                Id = correction.Id,
                Type = "map",
                Description = correction.Description,
                Latitude = correction.Latitude,
                Longitude = correction.Longitude,
                Status = correction.Status,
                ReviewComment = correction.ReviewComment,
                SubmittedBy = correction.SubmittedBy,
                SubmittedDate = correction.SubmittedDate
            };

            return View(viewModel);
        }
        else if (type == "area")
        {
            var areaChange = await _context.GeoChanges.FindAsync(id);
            if (areaChange == null)
                return NotFound();

            // Ensure GeoJSON is valid
            var geoJson = areaChange.GeoJson;
            if (!string.IsNullOrEmpty(geoJson))
            {
                
                geoJson = geoJson.Trim('"');
                // Ensure it's valid JSON
                try
                {
                    System.Text.Json.JsonDocument.Parse(geoJson);
                }
                catch
                {
                    // If parsing fails, try to clean up the string
                    geoJson = geoJson.Replace("\\\"", "\"").Replace("\\\\", "\\");
                }
            }

            var viewModel = new CorrectionReviewViewModel
            {
                Id = areaChange.Id,
                Type = "area",
                Description = areaChange.Description,
                GeoJson = geoJson, // Use the cleaned up version
                Status = areaChange.Status,
                ReviewComment = areaChange.ReviewComment,
                SubmittedBy = areaChange.SubmittedBy,
                SubmittedDate = areaChange.SubmittedDate
            };

            // Add logging to help debug
            _logger.LogInformation($"Area change ID: {id}");
            _logger.LogInformation($"Original GeoJson: {areaChange.GeoJson}");
            _logger.LogInformation($"Cleaned GeoJson: {geoJson}");

            return View(viewModel);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Review(int id, string type, CorrectionStatus status, string reviewComment)
    {
        if (type == "map")
        {
            var correction = await _context.MapCorrections.FindAsync(id);
            if (correction == null)
                return NotFound();

            correction.Status = status;
            correction.ReviewComment = reviewComment;
            correction.ReviewedBy = User.Identity.Name;
            correction.ReviewedDate = DateTime.UtcNow;
        }
        else if (type == "area")
        {
            var areaChange = await _context.GeoChanges.FindAsync(id);
            if (areaChange == null)
                return NotFound();

            areaChange.Status = status;
            areaChange.ReviewComment = reviewComment;
            areaChange.ReviewedBy = User.Identity.Name;
            areaChange.ReviewedDate = DateTime.UtcNow;
        }

        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Change has been {status.ToString().ToLower()}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating correction status");
            ModelState.AddModelError("", "An error occurred while updating the correction.");
            return View();
        }
    }

    public async Task<IActionResult> Dashboard()
    {
        // Get map corrections counts
        var mapCorrectionsTask = new
        {
            PendingCount = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Pending),

            ApprovedThisWeek = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Approved
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7)),

            RejectedThisWeek = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Rejected
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7))
        };

        // Get area changes counts
        var areaChangesTask = new
        {
            PendingCount = await _context.GeoChanges
                .CountAsync(c => c.Status == CorrectionStatus.Pending),

            ApprovedThisWeek = await _context.GeoChanges
                .CountAsync(c => c.Status == CorrectionStatus.Approved
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7)),

            RejectedThisWeek = await _context.GeoChanges
                .CountAsync(c => c.Status == CorrectionStatus.Rejected
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7))
        };

        // Get recent submissions from both types
        var recentMapCorrections = await _context.MapCorrections
            .OrderByDescending(c => c.SubmittedDate)
            .Take(5)
            .ToListAsync();

        var recentAreaChanges = await _context.GeoChanges
            .OrderByDescending(c => c.SubmittedDate)
            .Take(5)
            .ToListAsync();

        var dashboard = new DashboardViewModel
        {
            PendingCount = mapCorrectionsTask.PendingCount + areaChangesTask.PendingCount,
            ApprovedThisWeek = mapCorrectionsTask.ApprovedThisWeek + areaChangesTask.ApprovedThisWeek,
            RejectedThisWeek = mapCorrectionsTask.RejectedThisWeek + areaChangesTask.RejectedThisWeek,
            RecentMapCorrections = recentMapCorrections,
            RecentAreaChanges = recentAreaChanges
        };

        return View(dashboard);
    }
}
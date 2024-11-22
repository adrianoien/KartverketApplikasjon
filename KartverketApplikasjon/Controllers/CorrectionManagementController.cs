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

    private async Task<List<UserData>> GetAllSaksbehandlereAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Saksbehandler)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    // List all corrections with filtering
    public async Task<IActionResult> Index(string status = "Pending")
    {
        CorrectionStatus? correctionStatus = status switch
        {
            "Approved" => CorrectionStatus.Approved,
            "Rejected" => CorrectionStatus.Rejected,
            "All" => null,
            _ => CorrectionStatus.Pending
        };

        // Get map corrections
        var mapCorrectionsQuery = _context.MapCorrections.AsQueryable();
        if (correctionStatus.HasValue)
        {
            mapCorrectionsQuery = mapCorrectionsQuery.Where(c => c.Status == correctionStatus.Value);
        }
        IEnumerable<MapCorrections> mapCorrections = await mapCorrectionsQuery
            .OrderByDescending(c => c.SubmittedDate)
            .ToListAsync();

        // Get area changes
        var areaChangesQuery = _context.GeoChanges.AsQueryable();
        if (correctionStatus.HasValue)
        {
            areaChangesQuery = areaChangesQuery.Where(c => c.Status == correctionStatus.Value);
        }
        IEnumerable<GeoChange> areaChanges = await areaChangesQuery
            .OrderByDescending(c => c.SubmittedDate)
            .ToListAsync();

        ViewBag.CurrentFilter = status;
        ViewBag.Saksbehandlere = await GetAllSaksbehandlereAsync();
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
        if (User?.Identity?.Name == null)
        {
            return Unauthorized();
        }
        // Check if the review type is "map"
        if (type == "map")
        {
            // Retrieve the map correction by ID from the database
            var correction = await _context.MapCorrections.FindAsync(id);
            if (correction == null)
                return NotFound();

            // Update map correction details: status, review comment, reviewer name, and review date
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

            // Update area change details: status, review comment, reviewer name, and review date
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
            throw;
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

    [HttpGet]
    public async Task<IActionResult> SearchSaksbehandlere(string term)
    {
        try
        {
            // Search for users with the role 'Saksbehandler' whose name or email contains the search term
            var saksbehandlere = await _context.Users
                .Where(u => u.Role == UserRole.Saksbehandler &&
                           (u.Name.Contains(term) || u.Email.Contains(term)))
                  .Select(u => new { name = u.Name, username = u.Name, email = u.Email })
                .ToListAsync();

            // Return the search results as JSON
            return Json(saksbehandlere);
        }
        catch (Exception ex)
        {
            _logger.LogError($"En feil oppsto ved søk av saksbehandlere: {ex.Message}");
            return Json(new List<object>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignCase([FromBody] AssignCaseModel model)
    {
        try
        {
            if (model.CorrectionType == "map")
            {
                // Find the map correction by ID
                var correction = await _context.MapCorrections.FindAsync(model.CorrectionId);
                if (correction != null)
                {
                    // Assign the case to the specified user and update the assignment details
                    correction.AssignedTo = model.AssignTo;
                    correction.AssignmentDate = DateTime.UtcNow;
                    correction.AssignmentStatus = AssignmentStatus.Assigned;
                }
            }
            else if (model.CorrectionType == "area")
            {
                var areaChange = await _context.GeoChanges.FindAsync(model.CorrectionId);
                if (areaChange != null)
                {
                    // Assign the case to the specified user and update the assignment details
                    areaChange.AssignedTo = model.AssignTo;
                    areaChange.AssignmentDate = DateTime.UtcNow;
                    areaChange.AssignmentStatus = AssignmentStatus.Assigned;
                }
            }
            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success message as JSON
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error assigning case: {ex.Message}");
            return Json(new { success = false, message = "Error assigning case" });
        }
    }
}
using KartverketApplikasjon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // For ToListAsync()
using KartverketApplikasjon.Models;  // For MapCorrections and CorrectionStatus


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
        var correctionsQuery = _context.MapCorrections.AsQueryable();

        // Filter by status if provided
        if (Enum.TryParse<CorrectionStatus>(status, out var correctionStatus))
        {
            correctionsQuery = correctionsQuery.Where(c => c.Status == correctionStatus);
        }

        var corrections = await correctionsQuery
            .OrderByDescending(c => c.SubmittedDate)
            .ToListAsync();

        ViewBag.CurrentFilter = status;
        return View(corrections);
    }

    // Show detailed view of a correction
    public async Task<IActionResult> Review(int id)
    {
        var correction = await _context.MapCorrections.FindAsync(id);

        if (correction == null)
            return NotFound();

        var viewModel = new CorrectionReviewViewModel
        {
            Id = correction.Id,
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

    [HttpPost]
    public async Task<IActionResult> Review(int id, CorrectionStatus status, string reviewComment)
    {
        var correction = await _context.MapCorrections.FindAsync(id);

        if (correction == null)
            return NotFound();

        correction.Status = status;
        correction.ReviewComment = reviewComment;
        correction.ReviewedBy = User.Identity.Name;
        correction.ReviewedDate = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Correction has been {status.ToString().ToLower()}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating correction status");
            ModelState.AddModelError("", "An error occurred while updating the correction.");
            return View(correction);
        }
    }
    public async Task<IActionResult> Dashboard()
    {
        var dashboard = new DashboardViewModel
        {
            PendingCount = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Pending),

            ApprovedThisWeek = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Approved
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7)),

            RejectedThisWeek = await _context.MapCorrections
                .CountAsync(c => c.Status == CorrectionStatus.Rejected
                    && c.ReviewedDate >= DateTime.UtcNow.AddDays(-7)),

            RecentSubmissions = await _context.MapCorrections
                .OrderByDescending(c => c.SubmittedDate)
                .Take(10)
                .ToListAsync()
        };

        return View(dashboard);
    }
}
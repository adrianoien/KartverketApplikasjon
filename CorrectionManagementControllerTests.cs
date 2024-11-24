using NSubstitute;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KartverketApplikasjon.Controllers;
using KartverketApplikasjon.Data;
using KartverketApplikasjon.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class CorrectionManagementControllerTests
{
    private readonly ILogger<CorrectionManagementController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly CorrectionManagementController _controller;

    public CorrectionManagementControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _logger = Substitute.For<ILogger<CorrectionManagementController>>();
        _controller = new CorrectionManagementController(_context, _logger);

        // Fake user identity
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.Role, "Saksbehandler"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

    }


    [Fact]
    public async Task Index_ReturnsViewWithCorrectFilteredData()
    {
        // Arrange
        var pending = new MapCorrections
        {
            Status = CorrectionStatus.Pending,
            Description = "Test Pending",
            SubmittedDate = DateTime.UtcNow,
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com"
        };
        var approved = new MapCorrections
        {
            Status = CorrectionStatus.Approved,
            Description = "Test Approved",
            SubmittedDate = DateTime.UtcNow,
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com"
        };
        _context.MapCorrections.AddRange(pending, approved);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index("Pending");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ValueTuple<IEnumerable<MapCorrections>, IEnumerable<GeoChange>>>(
            viewResult.Model);
        Assert.Single(model.Item1);
        Assert.Equal(CorrectionStatus.Pending, model.Item1.First().Status);
    }

    [Fact]
    public async Task Review_ReturnsNotFoundForInvalidId()
    {
        // Act
        var result = await _controller.Review(999, "map");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Review_ReturnsCorrectViewModelForMapCorrection()
    {


        // Arrange
        var correction = new MapCorrections
        {
            Description = "Test Description",
            Latitude = "60.123",
            Longitude = "10.123",
            Status = CorrectionStatus.Pending,
            SubmittedBy = "test@example.com",
            SubmittedDate = DateTime.UtcNow

        };
        await _context.MapCorrections.AddAsync(correction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Review(correction.Id, "map");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrectionReviewViewModel>(viewResult.Model);
        Assert.Equal(correction.Description, model.Description);
        Assert.Equal(correction.Latitude, model.Latitude);
        Assert.Equal(correction.Longitude, model.Longitude);
    }

    [Fact]
    public async Task Dashboard_ReturnsCorrectStatsForLastWeek()
    {
        // Arrange
        var weekOldDate = DateTime.UtcNow.AddDays(-8);
        var recentDate = DateTime.UtcNow.AddDays(-3);

        var corrections = new List<MapCorrections>
    {
        new MapCorrections
        {
            Status = CorrectionStatus.Pending,
            Description = "Test Pending",
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com",
            SubmittedDate = DateTime.UtcNow
        },
        new MapCorrections
        {
            Status = CorrectionStatus.Approved,
            Description = "Test Recent Approved",
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com",
            ReviewedDate = recentDate
        },
        new MapCorrections
        {
            Status = CorrectionStatus.Approved,
            Description = "Test Old Approved",
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com",
            ReviewedDate = weekOldDate
        }
    };

        var areaChanges = new List<GeoChange>
    {
        new GeoChange
        {
            Status = CorrectionStatus.Pending,
            Description = "Test Area Pending",
            GeoJson = "{}",
            SubmittedBy = "test@example.com",
            SubmittedDate = DateTime.UtcNow
        },
        new GeoChange
        {
            Status = CorrectionStatus.Rejected,
            Description = "Test Recent Rejected",
            GeoJson = "{}",
            SubmittedBy = "test@example.com",
            ReviewedDate = recentDate
        }
    };

        await _context.MapCorrections.AddRangeAsync(corrections);
        await _context.GeoChanges.AddRangeAsync(areaChanges);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Dashboard();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DashboardViewModel>(viewResult.Model);

        Assert.Equal(2, model.PendingCount); // 1 map + 1 area pending
        Assert.Equal(1, model.ApprovedThisWeek); // 1 recent approval
        Assert.Equal(1, model.RejectedThisWeek); // 1 recent rejection
        Assert.Equal(3, model.RecentMapCorrections.Count); // Should show latest 5 or all if less
        Assert.Equal(2, model.RecentAreaChanges.Count); // Should show latest 5 or all if less
    }

    [Fact]
    public async Task Dashboard_ReturnsDashboardWithCorrectCounts()
    {
        // Arrange
        var pending = new MapCorrections
        {
            Status = CorrectionStatus.Pending,
            Description = "Test Pending",
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com"
        };
        var approved = new MapCorrections
        {
            Status = CorrectionStatus.Approved,
            Description = "Test Approved",
            Latitude = "60.123",
            Longitude = "10.123",
            SubmittedBy = "test@example.com",
            ReviewedDate = DateTime.UtcNow
        };
        _context.MapCorrections.AddRange(pending, approved);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Dashboard();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DashboardViewModel>(viewResult.Model);
        Assert.Equal(1, model.PendingCount);
        Assert.Equal(1, model.ApprovedThisWeek);
    }

    [Fact]
    public async Task SearchSaksbehandlere_ReturnsMatchingUsers()
    {
        // Arrange
        var users = new List<UserData>
    {
        new UserData
        {
            Name = "John Smith",
            Email = "john@example.com",
            Role = UserRole.Saksbehandler,
            Password = "password"
        },
        new UserData
        {
            Name = "Jane Smith",
            Email = "jane@example.com",
            Role = UserRole.Saksbehandler,
            Password = "password"
        },
        new UserData
        {
            Name = "Bob User",
            Email = "bob@example.com",
            Role = UserRole.User,
            Password = "password"
        }
    };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.SearchSaksbehandlere("Smith");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var searchResults = Assert.IsAssignableFrom<IEnumerable<object>>(jsonResult.Value);
        Assert.Equal(2, searchResults.Count());
    }
}

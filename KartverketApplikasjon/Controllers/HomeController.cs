using System.Diagnostics;
using KartverketApplikasjon.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace KartverketApplikasjon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        // Definerer en liste som en in-memory lagring
        private static List<MapCorrections> positions = new List<MapCorrections>();
        
        private static List<AreaChange> changes = new List<AreaChange>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Action metode som håndterer og vister RegForm view
        [HttpGet]
        public ViewResult RegistrationForm()
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

        [HttpGet]
        public IActionResult CorrectMap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CorrectMap(MapCorrections model)
        {
            if (ModelState.IsValid)
            {
                // Legger til ny posisjon i "positions" listen
                positions.Add(model);
            
                // Viser oppsummering av endring, etter dataen har blitt registrert og lagret i "positions" listen
                return View("CorrectionsOverview", positions);
            }

            return View();
        }

        [HttpGet]
        public IActionResult CorrectionsOverview()
        {
            return View(positions);
        }

        // Handles form submission to register a new change
        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }
        
        // Handles form submission to register a new change
        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            var newChange = new AreaChange
            {
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson,
                Description = description
            };
            
            // Save the change in the static in-memory list
            changes.Add(newChange);
            
            // Redirect to the overview of changes
            return RedirectToAction("AreaChangeOverview");
        }
        
        // Display the overview of registered changes
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            return View(changes);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class MapData
    {
        public List<LatLng> Points { get; set; }
        public List<List<LatLng>> Lines { get; set; }
    }

    public class LatLng
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
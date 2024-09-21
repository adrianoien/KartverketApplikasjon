using System.Diagnostics;
using KartverketApplikasjon.Models;
using Microsoft.AspNetCore.Mvc;

namespace KartverketApplikasjon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
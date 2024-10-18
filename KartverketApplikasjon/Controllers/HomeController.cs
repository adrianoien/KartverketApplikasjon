using System;
using System.Diagnostics;
using System.Collections.Generic;
using KartverketApplikasjon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KartverketApplikasjon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Define lists for in-memory storage
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

        public IActionResult Kart()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CorrectMap()
        {
            return View();
        }

        public IActionResult MapView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetExistingData()
        {
            // For now, we'll return both positions and changes
            var existingData = new { Positions = positions, Changes = changes };
            return Json(existingData);
        }

        [HttpPost]
        public IActionResult SubmitData([FromBody] AreaChange model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid().ToString();
                changes.Add(model);
                return Json(model);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult CorrectMap(MapCorrections model)
        {
            if (ModelState.IsValid)
            {
                positions.Add(model);
                return View("CorrectionsOverview", positions);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult CorrectionsOverview()
        {
            return View(positions);
        }

        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            var newChange = new AreaChange
            {
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson,
                Description = description
            };

            changes.Add(newChange);

            return RedirectToAction("AreaChangeOverview");
        }

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
}
using Microsoft.AspNetCore.Mvc;
using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add logic to save the user to your database
                return RedirectToAction("RegisterSuccess");
            }
            return View(model);
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHealthHub.Models;

namespace PawsitiveHealthHub.Controllers
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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoggedInHome");
            }

            return View(); // shows the default homepage for non-logged-in users
        }

        public IActionResult LoggedInHome()
        {
            return View(); // custom homepage for logged-in users
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

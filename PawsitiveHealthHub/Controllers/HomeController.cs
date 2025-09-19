using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHealthHub.Areas.Identity.Data;
using PawsitiveHealthHub.Models;
using System.Diagnostics;

namespace PawsitiveHealthHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Vet"))
                    {
                        return RedirectToAction("VetHome");
                    }
                    else if (roles.Contains("Owner"))
                    {
                        return RedirectToAction("LoggedInHome");
                    }
                }
            }

            return View(); // shows the default homepage for non-logged-in users
        }


        public IActionResult LoggedInHome()
        {
            return View(); // custom homepage for logged-in users
        }

        [Authorize(Roles = "Vet")]
        public IActionResult VetHome()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutPage()
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

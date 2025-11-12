using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.Models;

namespace MunicipalReporter.Controllers
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

        public IActionResult LearnMore()
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
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
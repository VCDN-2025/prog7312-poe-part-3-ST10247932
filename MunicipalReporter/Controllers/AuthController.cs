using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.Models;
using MunicipalReporter.Repositories;

namespace MunicipalReporter.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepo;

        public AuthController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_userRepo.Register(user))
            {
                TempData["Message"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Username already exists.";
            return View();
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (_userRepo.Login(user))
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Home");

            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
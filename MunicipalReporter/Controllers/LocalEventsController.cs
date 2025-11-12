using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Controllers
{
    public class LocalEventsController : Controller
    {
        private static readonly LocalEventService _service = new LocalEventService();

        // Main listing page
        public IActionResult Index(string category, DateTime? startDate, DateTime? endDate)
        {
            var events = _service.SearchEvents(category, startDate, endDate);

            ViewBag.Categories = _service.GetCategories();
            ViewBag.RecommendedCategory = _service.GetTopCategoryRecommendation();
            ViewBag.Recommended = !string.IsNullOrEmpty(ViewBag.RecommendedCategory)
                ? _service.GetRecommendedEvents()
                : Enumerable.Empty<LocalEvent>();
            ViewBag.Announcements = _service.GetAllAnnouncements();

            // Pass TempData messages to ViewBag for alert rendering
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"];
            if (TempData["MessageType"] != null)
                ViewBag.MessageType = TempData["MessageType"];

            return View(events);
        }

        // ------------------------
        // Announcement Actions
        // ------------------------
        public IActionResult AddAnnouncement()
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Parks", "Other" };
            return View();
        }

        [HttpPost]
        public IActionResult AddAnnouncement(LocalEvent model)
        {
            if (ModelState.IsValid)
            {
                if (_service.TryAddAnnouncement(model, out string message))
                {
                    TempData["Message"] = message;
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = message; // duplicate or error
                    TempData["MessageType"] = "danger";
                }
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Parks", "Other" };
            return View(model);
        }

        // ------------------------
        // Event Actions
        // ------------------------
        public IActionResult AddEvent()
        {
            ViewBag.Categories = _service.GetCategories();
            return View();
        }

        [HttpPost]
        public IActionResult AddEvent(LocalEvent model)
        {
            if (ModelState.IsValid)
            {
                if (_service.TryAddSessionEvent(model, out string message))
                {
                    TempData["Message"] = message;
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = message; // duplicate or error
                    TempData["MessageType"] = "danger";
                }
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _service.GetCategories();
            return View(model);
        }
    }
}
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
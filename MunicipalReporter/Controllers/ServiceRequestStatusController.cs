using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.Managers;
using MunicipalReporter.Models;
using MunicipalReporter.Repositories;

namespace MunicipalReporter.Controllers
{
    public class ServiceRequestStatusController : Controller
    {
        // Declare readonly fields first
        private readonly ServiceRequestManager _mgr;
        private readonly IssueRepository _issueRepository;

        // Constructor injects manager and repository
        public ServiceRequestStatusController(ServiceRequestManager mgr, IssueRepository issueRepository)
        {
            _mgr = mgr;
            _issueRepository = issueRepository;
        }

        // GET: /ServiceRequestStatus
        public IActionResult Index()
        {
            // Seed demo data (optional in production)
            _mgr.SeedSampleData();

            // Load user-submitted issues from repository into manager
            _mgr.LoadFromIssueRepo(_issueRepository.GetAll());

            // Get all requests to display
            var allRequests = _mgr.GetAllOrderedById();

            // Get top priority requests for sidebar or highlights
            var topRequests = _mgr.GetTopPriority(5);
            ViewBag.TopPriority = topRequests;

            return View(allRequests);
        }

        // GET: /ServiceRequestStatus/Details?id=REQ001
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            if (_mgr.TryGetById(id, out var req))
                return View(req);

            return NotFound();
        }

        // POST: Track a request by ID input
        [HttpPost]
        public IActionResult TrackById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index));

            if (_mgr.TryGetById(id, out var req))
            {
                return RedirectToAction(nameof(Details), new { id = req.RequestId });
            }

            TempData["Error"] = $"Request {id} not found";
            return RedirectToAction(nameof(Index));
        }

        // GET: /ServiceRequestStatus/TopPriority (for partial view)
        [HttpGet]
        public IActionResult TopPriority()
        {
            var top = _mgr.GetTopPriority(10);
            return PartialView("_TopPriorityPartial", top);
        }

        // GET: /ServiceRequestStatus/ComputeMst
        [HttpGet]
        public IActionResult ComputeMst()
        {
            var mst = _mgr.ComputeMstForSuburbs();
            return View("MstResult", mst);
        }

        // POST: /ServiceRequestStatus/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(string id, RequestStatus status)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            // Simple admin guard (session must contain admin email)
            var userEmail = HttpContext.Session.GetString("Email") ?? HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userEmail) || !string.Equals(userEmail, "admin@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid(); // or RedirectToAction("Index") with error
            }

            var ok = _mgr.UpdateStatus(id, status);
            if (!ok)
            {
                TempData["Error"] = $"Request {id} not found.";
            }
            else
            {
                TempData["Success"] = $"Request {id} status updated to {status}.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Optional: edit form page (GET). Not required, you can do inline dropdown + post.
        [HttpGet]
        public IActionResult EditStatus(string id)
        {
            var userEmail = HttpContext.Session.GetString("Email") ?? HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userEmail) || !string.Equals(userEmail, "admin@gmail.com", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!_mgr.TryGetById(id, out var req)) return NotFound();
            return View(req); // create Views/ServiceRequestStatus/EditStatus.cshtml
        }

    }
}
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
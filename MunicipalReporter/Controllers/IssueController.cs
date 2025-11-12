using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.Models;
using MunicipalReporter.Repositories;
using MunicipalReporter.Services;

namespace MunicipalReporter.Controllers
{
    public class IssueController : Controller
    {
        private readonly IssueRepository _repo;
        private readonly IWebHostEnvironment _env;
        private readonly IGeocodingService _geocodingService;
        private readonly ChatService _chatService;

        public IssueController(
            IssueRepository repo,
            IWebHostEnvironment env,
            IGeocodingService geocodingService,
            ChatService chatService)
        {
            _repo = repo;
            _env = env;
            _geocodingService = geocodingService;
            _chatService = chatService;
        }
            
        // GET: Issue/Report
        [HttpGet]
        public IActionResult Report()
        {
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Parks", "Other" };
            return View();
        }

        // POST: Issue/Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(Issue formModel, IFormFile[] files, bool lowDataMode = false)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(formModel.Location))
                ModelState.AddModelError(nameof(formModel.Location), "Please enter a location.");
            if (string.IsNullOrWhiteSpace(formModel.Description))
                ModelState.AddModelError(nameof(formModel.Description), "Please enter a description.");

            // Handle "lat,lon" input from Use My Location button
            if (!string.IsNullOrWhiteSpace(formModel.Location) && formModel.Location.Contains(","))
            {
                var parts = formModel.Location.Split(',');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double lat) &&
                    double.TryParse(parts[1], out double lon))
                {
                    // Reverse geocode coordinates to an address
                    var geocodeCoords = await _geocodingService.ValidateAddressAsync($"{lat},{lon}");
                    if (geocodeCoords.IsValid && !string.IsNullOrEmpty(geocodeCoords.FormattedAddress))
                    {
                        formModel.Location = geocodeCoords.FormattedAddress;
                    }
                }
            }

            // Standard geocoding validation for entered/substituted location
            if (!string.IsNullOrWhiteSpace(formModel.Location))
            {
                var geocode = await _geocodingService.ValidateAddressAsync(formModel.Location);
                if (!geocode.IsValid)
                    ModelState.AddModelError(nameof(formModel.Location), "Please enter a valid address or location.");
            }

            ViewBag.Categories = new[] { "Sanitation", "Roads", "Utilities", "Parks", "Other" };

            if (!ModelState.IsValid)
                return View(formModel);

            // Prepare new issue
            var newIssue = new Issue
            {
                Id = Guid.NewGuid(),
                DateReported = DateTime.Now,
                Location = formModel.Location,
                Category = formModel.Category,
                Description = formModel.Description,
                Attachments = new List<string>()
            };

            // File upload handling with 3 MB limit
            long maxFileSize = 3 * 1024 * 1024; // 3 MB

            if (!lowDataMode && files != null && files.Any())
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in files)
                {
                    try
                    {
                        if (file != null && file.Length > 0)
                        {
                            if (file.Length > maxFileSize)
                            {
                                // Convert bytes to MB with 2 decimal places
                                double sizeMB = Math.Round(file.Length / (1024.0 * 1024.0), 2);
                                ModelState.AddModelError("", $"File '{file.FileName}' is {sizeMB} MB and exceeds the 3 MB limit. It was not uploaded.");
                                continue; // Skip this file
                            }

                            var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
                            var savePath = Path.Combine(uploadFolder, safeFileName);

                            using var fs = new FileStream(savePath, FileMode.Create);
                            await file.CopyToAsync(fs);

                            newIssue.Attachments.Add(Path.Combine("uploads", safeFileName));
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Failed to upload file {file.FileName}: {ex.Message}");
                    }
                }

                // If any file errors occurred, show them and don't proceed with redirect
                if (!ModelState.IsValid)
                    return View(formModel);
            }

            // Add issue to repository
            _repo.Add(newIssue);

            // Add a system message to Community Chat
            string chatMessage = $"New Issue Reported: {newIssue.Category} at {newIssue.Location} - {newIssue.Description}";
            _chatService.AddSystemMessage(chatMessage);

            // Export compact JSON
            _repo.ExportCompactJson(_env.WebRootPath);

            // Set success message
            TempData["Success"] = lowDataMode
                ? "Report submitted (Low Data Mode). Attachments are disabled and a compact report was saved."
                : "Report submitted. Attachments were stored and a compact report was saved.";

            return RedirectToAction(nameof(Report));
        }

        // GET: Issue/List
        [HttpGet]
        public IActionResult List()
        {
            var all = _repo.GetAll();
            return View(all);
        }

        // GET: Issue/Compare
        [HttpGet]
        public IActionResult Compare()
        {
            var allIssues = _repo.GetAll().ToList();

            // Compute size per issue
            var fullIssueSizes = allIssues.ToDictionary(issue => issue.Id, issue =>
            {
                long size = 0;
                var json = System.Text.Json.JsonSerializer.Serialize(issue);
                size += (long)System.Text.Encoding.UTF8.GetByteCount(json);

                foreach (var att in issue.Attachments ?? new List<string>())
                {
                    var path = Path.Combine(_env.WebRootPath, att);
                    if (System.IO.File.Exists(path))
                        size += new System.IO.FileInfo(path).Length;
                }

                return size;
            });

            // Only include Low Data Mode submissions (no attachments)
            var compressedIssues = allIssues.Where(i => i.Attachments == null || !i.Attachments.Any()).ToList();
            var compressedIssueSizes = compressedIssues.ToDictionary(issue => issue.Id, issue =>
            {
                var compact = IssueCompactDto.From(issue);
                var json = System.Text.Json.JsonSerializer.Serialize(compact);
                return (long)System.Text.Encoding.UTF8.GetByteCount(json);
            });

            long fullSize = fullIssueSizes.Values.Sum();
            long compressedSize = compressedIssueSizes.Values.Sum();

            var model = new IssueComparisonViewModel
            {
                FullIssues = allIssues,
                CompressedIssues = compressedIssues,
                FullSizeBytes = fullSize,
                CompressedSizeBytes = compressedSize,
                FullIssueSizes = fullIssueSizes,
                CompressedIssueSizes = compressedIssueSizes
            };

            return View(model);
        }
    }
}
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
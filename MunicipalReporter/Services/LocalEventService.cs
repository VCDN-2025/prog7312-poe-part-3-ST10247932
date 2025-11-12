using MunicipalReporter.Models;

namespace MunicipalReporter.Services
{
    public class LocalEventService
    {
        // Primary storage for seeded events
        private readonly SortedDictionary<DateTime, List<LocalEvent>> _eventsByDate = new();

        // Last viewed events stack
        private readonly Stack<LocalEvent> _lastViewedEvents = new();

        // New submissions queue
        private readonly Queue<LocalEvent> _newEventSubmissions = new();

        // Unique categories
        private readonly HashSet<string> _categories = new();

        // Search tracking for recommendations
        private readonly Dictionary<string, int> _categorySearchCount = new();
        private readonly Dictionary<DateTime, int> _dateSearchCount = new();

        // Session-only collections (temporary, lasts while app runs)
        private readonly List<LocalEvent> _sessionEvents = new();           // user-added events
        private readonly List<LocalEvent> _sessionAnnouncements = new();    // user-added announcements

        public LocalEventService()
        {
            SeedEvents();
            SeedAnnouncements();
        }
        // Seeded events
        private void SeedEvents()
        {
            AddEvent(new LocalEvent { Id = 1, Name = "Music Festival", Category = "Entertainment", Date = DateTime.Parse("2025-10-15"), Description = "Annual festival with local bands." });
            AddEvent(new LocalEvent { Id = 2, Name = "Health Camp", Category = "Health", Date = DateTime.Parse("2025-10-18"), Description = "Free health check-ups for all residents." });
            AddEvent(new LocalEvent { Id = 3, Name = "Food Fair", Category = "Community", Date = DateTime.Parse("2025-10-20"), Description = "Taste local cuisine from vendors." });
            AddEvent(new LocalEvent { Id = 4, Name = "Book Drive", Category = "Education", Date = DateTime.Parse("2025-10-22"), Description = "Donate books to local schools." });
            AddEvent(new LocalEvent { Id = 5, Name = "Marathon", Category = "Sports", Date = DateTime.Parse("2025-10-25"), Description = "Join the annual city marathon." });
            AddEvent(new LocalEvent { Id = 6, Name = "Art Exhibition", Category = "Arts", Date = DateTime.Parse("2025-10-28"), Description = "Local artists display their work." });
            AddEvent(new LocalEvent { Id = 7, Name = "Tech Workshop", Category = "Education", Date = DateTime.Parse("2025-11-01"), Description = "Learn coding and robotics." });
            AddEvent(new LocalEvent { Id = 8, Name = "Farmers Market", Category = "Community", Date = DateTime.Parse("2025-11-03"), Description = "Fresh produce from local farmers." });
            AddEvent(new LocalEvent { Id = 9, Name = "Charity Run", Category = "Sports", Date = DateTime.Parse("2025-11-05"), Description = "Support local charities while running." });
            AddEvent(new LocalEvent { Id = 10, Name = "Jazz Night", Category = "Entertainment", Date = DateTime.Parse("2025-11-07"), Description = "Live jazz music in the park." });
            AddEvent(new LocalEvent { Id = 11, Name = "Community Cleanup", Category = "Environment", Date = DateTime.Parse("2025-11-10"), Description = "Help clean local streets and parks." });
            AddEvent(new LocalEvent { Id = 12, Name = "Photography Contest", Category = "Arts", Date = DateTime.Parse("2025-11-12"), Description = "Show off your photography skills." });
            AddEvent(new LocalEvent { Id = 13, Name = "Yoga Workshop", Category = "Health", Date = DateTime.Parse("2025-11-15"), Description = "Outdoor yoga sessions for beginners." });
            AddEvent(new LocalEvent { Id = 14, Name = "Coding Hackathon", Category = "Education", Date = DateTime.Parse("2025-11-18"), Description = "Compete and learn in teams." });
            AddEvent(new LocalEvent { Id = 15, Name = "Holiday Parade", Category = "Community", Date = DateTime.Parse("2025-12-01"), Description = "Celebrate with floats, music, and food." });
        }

        //Seeded Announcements
        private void SeedAnnouncements()
        {
            AddAnnouncement(new LocalEvent
            {
                Id = 101,
                Name = "Water Supply Maintenance",
                Category = "Utilities",
                Date = DateTime.Parse("2025-10-14"),
                Description = "Scheduled maintenance on water lines may cause temporary disruptions in certain neighborhoods."
            });

            AddAnnouncement(new LocalEvent
            {
                Id = 102,
                Name = "Road Closure Notice",
                Category = "Roads",
                Date = DateTime.Parse("2025-10-16"),
                Description = "Main Street will be closed for resurfacing from Oct 16–18. Please use alternative routes."
            });
        }

        public void AddEvent(LocalEvent evt)
        {
            if (!_eventsByDate.ContainsKey(evt.Date))
                _eventsByDate[evt.Date] = new List<LocalEvent>();

            _eventsByDate[evt.Date].Add(evt);
            _categories.Add(evt.Category);
            _newEventSubmissions.Enqueue(evt);
        }

        // Add user-added event (session-only)
        public void AddSessionEvent(LocalEvent evt)
        {
            _sessionEvents.Add(evt);
            _categories.Add(evt.Category);
        }

        // Add user-added announcement (session-only)
        public void AddAnnouncement(LocalEvent announcement)
        {
            _sessionAnnouncements.Add(announcement);
        }

        // Try add (avoid duplicates)
        public bool TryAddSessionEvent(LocalEvent evt, out string message)
        {
            var exists = _eventsByDate.Values.SelectMany(x => x)
                          .Concat(_sessionEvents)
                          .Any(e => e.Name.Equals(evt.Name, StringComparison.OrdinalIgnoreCase)
                                    && e.Date.Date == evt.Date.Date);

            if (exists)
            {
                message = "An event with the same title and date already exists.";
                return false;
            }

            _sessionEvents.Add(evt);
            _categories.Add(evt.Category);
            message = "Event added successfully!";
            return true;
        }

        public bool TryAddAnnouncement(LocalEvent announcement, out string message)
        {
            var exists = _sessionAnnouncements
                          .Any(a => a.Name.Equals(announcement.Name, StringComparison.OrdinalIgnoreCase)
                                    && a.Date.Date == announcement.Date.Date);

            if (exists)
            {
                message = "An announcement with the same title and date already exists.";
                return false;
            }

            _sessionAnnouncements.Add(announcement);
            message = "Announcement added successfully!";
            return true;
        }

        // Getters
        public IEnumerable<LocalEvent> GetAllSessionEvents() => _sessionEvents;
        public IEnumerable<LocalEvent> GetAllAnnouncements() => _sessionAnnouncements;
        public IEnumerable<LocalEvent> GetAllEvents() =>
            _eventsByDate.Values.SelectMany(x => x).Concat(_sessionEvents);
        public IEnumerable<string> GetCategories() => _categories;

        // Search
        public IEnumerable<LocalEvent> SearchEvents(string category, DateTime? startDate, DateTime? endDate)
        {
            if (!string.IsNullOrEmpty(category))
            {
                if (!_categorySearchCount.ContainsKey(category)) _categorySearchCount[category] = 0;
                _categorySearchCount[category]++;
            }

            var allEvents = _eventsByDate.Values.SelectMany(x => x).ToList();
            allEvents.AddRange(_sessionEvents);

            var results = allEvents
                .Where(e =>
                    (string.IsNullOrEmpty(category) || e.Category == category) &&
                    (!startDate.HasValue || e.Date >= startDate.Value) &&
                    (!endDate.HasValue || e.Date <= endDate.Value))
                .ToList();

            foreach (var evt in results)
                _lastViewedEvents.Push(evt);

            return results;
        }

        // Recommendations
        public string GetTopCategoryRecommendation(int minSearches = 2)
        {
            var topCategory = _categorySearchCount
                                .Where(x => x.Value >= minSearches)
                                .OrderByDescending(x => x.Value)
                                .FirstOrDefault().Key;

            return topCategory;
        }

        public IEnumerable<LocalEvent> GetRecommendedEvents(int maxPerCategory = 5, int minSearches = 2)
        {
            var topCategories = _categorySearchCount
                                .Where(x => x.Value >= minSearches)
                                .Select(x => x.Key)
                                .ToList();

            if (!topCategories.Any())
                return Enumerable.Empty<LocalEvent>();

            var recommended = new List<LocalEvent>();
            foreach (var category in topCategories)
            {
                recommended.AddRange(
                    _eventsByDate.Values.SelectMany(x => x)
                        .Concat(_sessionEvents)
                        .Where(e => e.Category == category)
                        .Take(maxPerCategory)
                );
            }

            return recommended;
        }
    }
}
//Reference
//Stackoverflow, 2010, The Purpose of a Service Layer and ASP.NET MVC 2. [online] Available at: https://stackoverflow.com/questions/2762978/the-purpose-of-a-service-layer-and-asp-net-mvc-2 [Accessed 1 September 2025]

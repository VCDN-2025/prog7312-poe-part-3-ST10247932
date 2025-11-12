namespace MunicipalReporter.Models
{
    public class LocalEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public bool IsAnnouncement { get; set; } // true if it's an announcement
    }
}

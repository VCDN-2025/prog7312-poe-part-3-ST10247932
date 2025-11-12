namespace MunicipalReporter.Models
{
    // Custom domain data structure (your "own" Issue class)
    public class Issue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DateReported { get; set; } = DateTime.UtcNow;

        // Essential fields for reporting
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        // List of attachment file names stored in wwwroot/uploads
        public List<string> Attachments { get; set; } = new();
    }
}
//Reference
//TutorialsTeacher, 2024. Model in ASP.NET MVC. [online] Available at:https://www.tutorialsteacher.com/mvc/mvc-model [Accessed 1 September 2025]
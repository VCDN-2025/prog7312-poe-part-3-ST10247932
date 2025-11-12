namespace MunicipalReporter.Models
{
    public class IssueComparisonViewModel
    {
        public List<Issue> FullIssues { get; set; } = new List<Issue>();
        public List<Issue> CompressedIssues { get; set; } = new List<Issue>();
        public long FullSizeBytes { get; set; }
        public long CompressedSizeBytes { get; set; }

        // New properties
        public Dictionary<Guid, long> FullIssueSizes { get; set; }
        public Dictionary<Guid, long> CompressedIssueSizes { get; set; }
    }
}
//Reference
//TutorialsTeacher, 2024. Model in ASP.NET MVC. [online] Available at:https://www.tutorialsteacher.com/mvc/mvc-model [Accessed 1 September 2025]
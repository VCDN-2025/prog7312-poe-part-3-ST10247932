namespace MunicipalReporter.Models
{
    // Compact DTO for low-bandwidth export/sync (short properties)
    public class IssueCompactDto
    {
        public string k { get; set; }   // id
        public long t { get; set; }     // unix timestamp
        public string l { get; set; }   // location
        public string c { get; set; }   // category
        public string d { get; set; }   // description

        public static IssueCompactDto From(Issue issue)
        {
            return new IssueCompactDto
            {
                k = issue.Id.ToString("N"),
                t = new DateTimeOffset(issue.DateReported).ToUnixTimeSeconds(),
                l = issue.Location ?? "",
                c = issue.Category ?? "",
                d = issue.Description ?? ""
            };
        }
    }
}
//Reference
//TutorialsTeacher, 2024. Model in ASP.NET MVC. [online] Available at:https://www.tutorialsteacher.com/mvc/mvc-model [Accessed 1 September 2025]
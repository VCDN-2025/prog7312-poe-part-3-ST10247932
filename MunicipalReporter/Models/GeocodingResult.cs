namespace MunicipalReporter.Models
{
    public class GeocodingResult
    {
        public bool IsValid { get; set; }
        public string? FormattedAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
//Reference
//TutorialsTeacher, 2024. Model in ASP.NET MVC. [online] Available at:https://www.tutorialsteacher.com/mvc/mvc-model [Accessed 1 September 2025]
namespace MunicipalReporter.Models
{
    public class ChatMessage
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public int Id { get; set; }            // unique ID for each message
        public int? ReplyTo { get; set; }      // ID of the parent message (if this is a reply)
    }
}
//Reference
//TutorialsTeacher, 2024. Model in ASP.NET MVC. [online] Available at:https://www.tutorialsteacher.com/mvc/mvc-model [Accessed 1 September 2025]
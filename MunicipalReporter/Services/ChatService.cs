using MunicipalReporter.DataStructures;
using MunicipalReporter.Models;

public class ChatService
{
    private readonly MessageLinkedList _messageList;

    public ChatService(MessageLinkedList messageList)
    {
        _messageList = messageList;
    }

    public void AddSystemMessage(string message)
    {
        _messageList.AddMessage(new ChatMessage
        {
            Username = "System",  // system user for issues
            Message = message,
            Timestamp = DateTime.Now
        });
    }
}
//Reference
//Stackoverflow, 2010, The Purpose of a Service Layer and ASP.NET MVC 2. [online] Available at: https://stackoverflow.com/questions/2762978/the-purpose-of-a-service-layer-and-asp-net-mvc-2 [Accessed 1 September 2025]

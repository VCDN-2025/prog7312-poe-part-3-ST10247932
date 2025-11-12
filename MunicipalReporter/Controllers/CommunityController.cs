using Microsoft.AspNetCore.Mvc;
using MunicipalReporter.DataStructures;
using MunicipalReporter.Models;

namespace MunicipalReporter.Controllers
{
    public class CommunityController : Controller
    {
        private readonly MessageLinkedList _messageList;

        public CommunityController(MessageLinkedList messageList)
        {
            _messageList = messageList;
        }

        public IActionResult Chat()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Auth");

            ViewBag.Username = username;

            // Ensure dummy messages exist only once
            if (!_messageList.GetAllMessages().Any(m => m.Username == "Alice"))
            {
                // Add dummy messages without specifying IDs
                var alice = new ChatMessage { Username = "Alice", Message = "Hello everyone!", Timestamp = DateTime.Now.AddMinutes(-15) };
                _messageList.AddMessage(alice);

                var bob = new ChatMessage { Username = "Bob", Message = "Hi Alice, how are you?", Timestamp = DateTime.Now.AddMinutes(-10), ReplyTo = alice.Id };
                _messageList.AddMessage(bob);

                var charlie = new ChatMessage { Username = "Charlie", Message = "This community chat is working nicely.", Timestamp = DateTime.Now.AddMinutes(-5) };
                _messageList.AddMessage(charlie);
            }

            // Get all messages after adding dummy ones
            var messages = _messageList.GetAllMessages();
            ViewBag.Messages = messages;

            return View();
        }

        [HttpPost]
        public IActionResult Chat(string message, int? replyTo)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Auth");

            if (!string.IsNullOrEmpty(message))
            {
                _messageList.AddMessage(new ChatMessage
                {
                    Username = username,
                    Message = message,
                    Timestamp = DateTime.Now,
                    ReplyTo = replyTo
                });
            }

            return RedirectToAction("Chat");
        }

        public void AddIssueToChat(Issue issue)
        {
            string issueMessage = $"New Issue Reported: {issue.Category} at {issue.Location} - {issue.Description}";

            // Add system message without specifying Id; linked list will assign unique ID automatically
            _messageList.AddMessage(new ChatMessage
            {
                Username = "System",
                Message = issueMessage,
                Timestamp = DateTime.Now
            });
        }
    }
}
// Reference
//Robert, S.,2023.model-view-controller(MVC).[online] Available at:https://www.techtarget.com/whatis/definition/model-view-controller-MVC [Accessed 1 Sepetember 2025]
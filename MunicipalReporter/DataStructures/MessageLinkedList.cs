using MunicipalReporter.Models;

namespace MunicipalReporter.DataStructures
{
    public class MessageNode
    {
        public ChatMessage Message { get; set; }
        public MessageNode Next { get; set; }

        public MessageNode(ChatMessage message)
        {
            Message = message;
            Next = null;
        }
    }

    public class MessageLinkedList
    {
        private MessageNode head;
        private int count;
        private int nextId = 1; // ✅ Track unique IDs

        public void AddMessage(ChatMessage message)
        {
            // Assign a unique ID if not set
            if (message.Id == 0)
                message.Id = nextId++;

            MessageNode newNode = new MessageNode(message);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                MessageNode current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
            count++;
        }

        public List<ChatMessage> GetAllMessages()
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            MessageNode current = head;
            while (current != null)
            {
                messages.Add(current.Message);
                current = current.Next;
            }
            return messages;
        }

        public ChatMessage? GetMessageById(int id)
        {
            MessageNode current = head;
            while (current != null)
            {
                if (current.Message.Id == id)
                    return current.Message;
                current = current.Next;
            }
            return null;
        }

        public int Count => count;

        public void PrependMessage(ChatMessage message)
        {
            if (message.Id == 0)
                message.Id = nextId++;

            var newNode = new MessageNode(message);
            newNode.Next = head;
            head = newNode;
            count++;
        }

    }

}
//Reference
//GeeksForGeeks, 2025. C# LinkedList. [online] Available at:https://www.geeksforgeeks.org/c-sharp/linked-list-implementation-in-c-sharp/  [Accessed 1 September 2025]

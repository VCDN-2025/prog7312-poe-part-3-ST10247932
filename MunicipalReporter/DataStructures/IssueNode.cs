using MunicipalReporter.Models;

namespace MunicipalReporter.DataStructures
{
    public class IssueNode
    {
        public Issue Data { get; set; }
        public IssueNode Next { get; set; }

        public IssueNode(Issue data)
        {
            Data = data;
            Next = null;
        }
    }
}
//Reference
//GeeksForGeeks, 2025. C# LinkedList. [online] Available at:https://www.geeksforgeeks.org/c-sharp/linked-list-implementation-in-c-sharp/  [Accessed 1 September 2025]

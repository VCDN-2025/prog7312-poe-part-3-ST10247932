using MunicipalReporter.Models;

namespace MunicipalReporter.DataStructures
{
    public class IssueLinkedList
    {
        private IssueNode head;

        public IssueLinkedList()
        {
            head = null;
        }

        // Add an issue
        public void Add(Issue issue)
        {
            IssueNode newNode = new IssueNode(issue);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                IssueNode current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
        }

        // Get all issues
        public List<Issue> GetAll()
        {
            List<Issue> issues = new List<Issue>();
            IssueNode current = head;
            while (current != null)
            {
                issues.Add(current.Data);
                current = current.Next;
            }
            return issues;
        }

        // Inside IssueLinkedList
        public List<Issue> GetCompressed()
        {
            List<Issue> compressed = new List<Issue>();
            HashSet<string> seen = new HashSet<string>();

            IssueNode current = head;
            while (current != null)
            {
                // Combine Category + Description as uniqueness criteria
                string uniqueKey = $"{current.Data.Category}_{current.Data.Description}";

                if (!seen.Contains(uniqueKey))
                {
                    compressed.Add(current.Data);
                    seen.Add(uniqueKey);
                }

                current = current.Next;
            }

            return compressed;
        }

    }
}
//Reference
//GeeksForGeeks, 2025. C# LinkedList. [online] Available at:https://www.geeksforgeeks.org/c-sharp/linked-list-implementation-in-c-sharp/  [Accessed 1 September 2025]

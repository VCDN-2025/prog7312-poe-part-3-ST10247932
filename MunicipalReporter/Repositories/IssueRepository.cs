using MunicipalReporter.DataStructures;
using MunicipalReporter.Models;
using System.Text;
using System.Text.Json;

namespace MunicipalReporter.Repositories
{
    public class IssueRepository
    {
        private readonly IssueLinkedList _issues = new IssueLinkedList();

        public void Add(Issue issue) => _issues.Add(issue);

        public IReadOnlyList<Issue> GetAll() => _issues.GetAll().AsReadOnly();

        public void ExportCompactJson(string webRootPath)
        {
            var compactList = _issues.GetAll().Select(IssueCompactDto.From).ToList();
            var sb = new StringBuilder();
            sb.Append("{\"issues\":[");
            for (int i = 0; i < compactList.Count; i++)
            {
                var c = compactList[i];
                string esc(string s) => (s ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
                sb.Append("{");
                sb.AppendFormat("\"k\":\"{0}\",\"t\":{1},\"l\":\"{2}\",\"c\":\"{3}\",\"d\":\"{4}\"",
                    esc(c.k), c.t, esc(c.l), esc(c.c), esc(c.d));
                sb.Append("}");
                if (i < compactList.Count - 1) sb.Append(",");
            }
            sb.Append("]}");
            var outPath = Path.Combine(webRootPath, "issues-compact.json");
            File.WriteAllText(outPath, sb.ToString());
        }

        public List<Issue> GetCompressed()
        {
            return _issues.GetAll().Select(issue => new Issue
            {
                Id = issue.Id,
                DateReported = issue.DateReported,
                Location = issue.Location,
                Category = issue.Category,
                Description = issue.Description,
                Attachments = new List<string>() // always empty
            }).ToList();
        }

        // Get size of full report including attachments (in bytes)
        public long GetFullReportSize(string webRootPath)
        {
            long total = 0;
            foreach (var issue in _issues.GetAll())
            {
                // JSON size
                var json = JsonSerializer.Serialize(issue);
                total += Encoding.UTF8.GetByteCount(json);

                // Attachments size
                foreach (var att in issue.Attachments)
                {
                    var path = Path.Combine(webRootPath, att);
                    if (File.Exists(path))
                        total += new FileInfo(path).Length;
                }
            }
            return total;
        }



        public long GetCompressedReportSize()
        {
            var compactList = _issues.GetAll().Select(IssueCompactDto.From).ToList();
            var json = JsonSerializer.Serialize(compactList);
            return Encoding.UTF8.GetByteCount(json);
        }

    }
}
//Reference
//Stackoverflow, 2012, Why use Repository Pattern or please explain it to me? [online] Available at: https://stackoverflow.com/questions/8749153/why-use-repository-pattern-or-please-explain-it-to-me [Accessed 1 September 2025]

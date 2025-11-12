using MunicipalReporter.DataStructures;
using MunicipalReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalReporter.Managers
{
    public class ServiceRequestManager
    {
        private readonly AvlTree<string, ServiceRequest> avlById = new();
        private readonly MinHeap<ServiceRequestComparable> minHeap = new();
        private readonly Graph<string> relationGraph = new();

        // Wrapper to compare by (Priority asc, CreatedAt asc)
        private class ServiceRequestComparable : IComparable<ServiceRequestComparable>
        {
            public ServiceRequest Req;
            public ServiceRequestComparable(ServiceRequest r) { Req = r; }
            public int CompareTo(ServiceRequestComparable other)
            {
                int c = Req.Priority.CompareTo(other.Req.Priority);
                if (c != 0) return c;
                return Req.CreatedAt.CompareTo(other.Req.CreatedAt);
            }
        }

        // Basic in-memory store (persist separately if needed)
        public void AddOrUpdate(ServiceRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.RequestId))
                throw new ArgumentException("RequestId required");

            avlById.Insert(req.RequestId, req);
            minHeap.Insert(new ServiceRequestComparable(req));
            relationGraph.AddNode(req.Suburb ?? req.RequestId);

            // Add dependency edges (if any)
            foreach (var d in req.Dependencies ?? new List<string>())
            {
                relationGraph.AddNode(d);
                relationGraph.AddEdge(req.RequestId, d, 1.0); // default weight
            }
        }

        public bool TryGetById(string id, out ServiceRequest req) => avlById.TryGet(id, out req);

        public IEnumerable<ServiceRequest> GetAllOrderedById()
        {
            var list = new List<ServiceRequest>();
            avlById.InOrder((k, v) => list.Add(v));
            return list;
        }

        public List<ServiceRequest> GetTopPriority(int n)
        {
            var tmp = new List<ServiceRequestComparable>();
            var outList = new List<ServiceRequest>();
            for (int i = 0; i < n && minHeap.Count > 0; i++)
            {
                var it = minHeap.Pop();
                tmp.Add(it);
                outList.Add(it.Req);
            }
            // Restore heap
            foreach (var x in tmp) minHeap.Insert(x);
            return outList;
        }

        public IEnumerable<string> GetDependencyPathBfs(string startId)
        {
            return relationGraph.BFS(startId);
        }

        public List<(string u, string v, double w)> ComputeMstForSuburbs()
        {
            return relationGraph.KruskalMST();
        }

        // Seed sample data for testing
        public void SeedSampleData()
        {
            var s1 = new ServiceRequest { RequestId = "REQ001", Title = "Pothole Main St", Priority = 1, Suburb = "Central", CreatedAt = DateTime.UtcNow.AddMinutes(-120) };
            var s2 = new ServiceRequest { RequestId = "REQ002", Title = "Streetlight Park Ave", Priority = 3, Suburb = "North", CreatedAt = DateTime.UtcNow.AddMinutes(-60) };
            var s3 = new ServiceRequest { RequestId = "REQ003", Title = "Garbage Missed", Priority = 2, Suburb = "Central", CreatedAt = DateTime.UtcNow.AddMinutes(-90), Dependencies = new List<string> { "REQ001" } };

            AddOrUpdate(s1);
            AddOrUpdate(s2);
            AddOrUpdate(s3);

            // Example edges between suburbs
            relationGraph.AddEdge("Central", "North", 5.2);
            relationGraph.AddEdge("Central", "South", 3.1);
            relationGraph.AddEdge("North", "East", 4.7);
        }

        // Load user-submitted issues into service requests
        public void LoadFromIssueRepo(IEnumerable<Issue> issues)
        {
            foreach (var i in issues)
            {
                if (!avlById.TryGet(i.Id.ToString(), out _)) // avoid duplicates
                {
                    var req = new ServiceRequest
                    {
                        RequestId = i.Id.ToString(),
                        Title = i.Description ?? "No description",
                        Description = i.Description ?? "No description",
                        Suburb = i.Location ?? "Unknown",
                        CreatedAt = i.DateReported,
                        Priority = 5, // default
                        Status = RequestStatus.Submitted
                    };
                    AddOrUpdate(req);
                }
            }
        }

        // Update the status of an existing request (admin functionality)
        public bool UpdateStatus(string requestId, RequestStatus newStatus)
        {
            if (TryGetById(requestId, out var req))
            {
                req.Status = newStatus;
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;

namespace MunicipalReporter.Models
{
    public enum RequestStatus { Submitted, InProgress, Completed, OnHold, Cancelled }

    public class ServiceRequest
    {
        public string RequestId { get; set; }        // unique ID (e.g., "REQ001")
        public string Title { get; set; }
        public string Description { get; set; }
        public string Suburb { get; set; }           // used for graph nodes / routing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Priority { get; set; } = 5;       // lower = more urgent
        public RequestStatus Status { get; set; } = RequestStatus.Submitted;
        public List<string> Dependencies { get; set; } = new();
        public double? Latitude { get; set; }        // optional
        public double? Longitude { get; set; }       // optional

        public override string ToString() =>
            $"{RequestId} | {Title} | {Status} | P:{Priority} | {CreatedAt:yyyy-MM-dd HH:mm}";
    }
}

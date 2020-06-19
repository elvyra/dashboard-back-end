using System;
using System.Text.Json.Serialization;

namespace Dashboard.Models
{
    public class PortalResponse
    {
        public Guid Id { get; set; }
        public DateTime RequestDateTime { get; set; }
        public int Status { get; set; }
        public int ResponseTime { get; set; }
        public Guid StatusPageId { get; set; }

        [JsonIgnore]
        public Portal StatusPage { get; set; }
        public string ErrorMessage { get; set; }
    }
}

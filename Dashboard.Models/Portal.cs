using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dashboard.Models.Enums;

namespace Dashboard.Models
{
    public class Portal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PortalType Type { get; set; }
        public string URL { get; set; }
        public string Parameters { get; set; }
        public PortalStatus Status { get; set; }
        public string Email { get; set; }
        public int CheckInterval { get; set; }
        public int ResponseTimeThreshold { get; set; }
        public RequestMethod Method { get; set; }
        public bool BasicAuth { get; set; }
        public string UserName { get; set; }

        [JsonIgnore]
        public string PasswordHashed { get; set; }
        public DateTime LastNotificationSent { get; set; }
        public DateTime LastRequestDateTime { get; set; }
        public int LastRequestStatus { get; set; }
        public int LastRequestResponseTime { get; set; }
        public string LastRequestErrorMessage { get; set; }
        public IList<PortalResponse> PortalResponses { get; set; } = new List<PortalResponse>();
        public IList<Notification> EmailNotifications { get; set; } = new List<Notification>();
    }
}

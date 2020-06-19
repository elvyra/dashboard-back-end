using System;

namespace Dashboard.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid PortalId { get; set; }
        public Guid PortalResponseId { get; set; }
        public DateTime NotificationSentDateTime { get; set; }
        public string NotificationCauseMessage { get; set; }
        public int EmailProviderResponse { get; set; }
    }
}

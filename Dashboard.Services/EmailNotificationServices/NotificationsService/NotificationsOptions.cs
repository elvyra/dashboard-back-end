namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// Notification service options class
    /// </summary>
    public class NotificationsOptions
    {
        /// <summary>
        /// Hours to ignore same error for email notifications, default - 24 hours
        /// </summary>
        public int HoursToIgnoreContinuousError { get; set; } = 24;
    }
}

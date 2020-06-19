namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// SendGrid service options
    /// </summary>
    public class SendGridOptions
    {
        /// <summary>
        /// SendGrid dynamic template ID for noticications email
        /// </summary>
        public string NotificationsTemplateId { get; set; }

        /// <summary>
        /// SendGrid dynamic template ID for user permissions changed info email
        /// </summary>
        public string UserPermissionsInfoTemplateId { get; set; }

        /// <summary>
        /// From email
        /// </summary>
        public string fromEmail { get; set; }

        /// <summary>
        /// ReplyTo email 
        /// </summary>
        public string replyEmail { get; set; }
    }
}

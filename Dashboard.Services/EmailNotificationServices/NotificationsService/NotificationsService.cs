using Dashboard.Models;
using Dashboard.Services.PortalCrudService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// Email sending logic
    /// </summary>
    public class NotificationsService : INotificationsService
    {
        private readonly int _hoursToIgnoreContinuousError;
        private ISendGridService _sendGridService;
        private IPortalCrudService _portalService;
        private ILogger<NotificationsService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="sendGridService">Email sending service</param>
        /// <param name="portalService">Portal service (for updating last sended email notification datetime)</param>
        /// <param name="options">Options</param>
        public NotificationsService(
            ILogger<NotificationsService> logger, 
            ISendGridService sendGridService, 
            IPortalCrudService portalService, 
            IOptions<NotificationsOptions> options)
        {
            _logger = logger;
            _sendGridService = sendGridService;
            _portalService = portalService;
            _hoursToIgnoreContinuousError = options.Value.HoursToIgnoreContinuousError;
        }

        /// <summary>
        /// Send notification email logic
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="portal">Portal responded</param>
        /// <returns>SendGrid>response code if email notification needed, 204 (NoContent) - if not</returns>
        public async Task<HttpStatusCode> SendNotificationEmailAsync(Portal portal, PortalResponse response)
        {
            // Email Notification setting incorrect, no email notification will be send
            if (_hoursToIgnoreContinuousError < 0)
                return HttpStatusCode.NoContent;

            // Email nofitications on every error response needed
            var allErrorsNotification = _hoursToIgnoreContinuousError == 0;
            
            // It is the first portal error response - notification needed
            var firstPortalErrorNotification = portal.PortalResponses == null || portal.PortalResponses.Count <= 1 || portal.LastNotificationSent == new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

            // First error response after good one
            var firstErrorResponseAfterGoodOne = portal.LastRequestStatus >= 200 && portal.LastRequestStatus < 300;

            // Last portal error response notified is older then setted 
            var lastErrorOlderThenSettedNotification = portal.LastNotificationSent.AddHours(_hoursToIgnoreContinuousError) < response.RequestDateTime;

            if (allErrorsNotification || firstPortalErrorNotification || firstErrorResponseAfterGoodOne || lastErrorOlderThenSettedNotification)
            {
                await _portalService.SetLastNotificationSentAsync(portal.Id, response.RequestDateTime);
                var emailProviderResponse = await _sendGridService.SendEmailAsync(portal, response);
                if (emailProviderResponse == HttpStatusCode.Accepted)
                    _logger.LogInformation($"Email for portal (Id: {portal.Id}) and response (Id: {response.Id}) quened to be send.");
                else
                    _logger.LogWarning($"Email for portal (Id: {portal.Id}) and response (Id: {response.Id}) was not sent.");
                
                var message = "";
                if (lastErrorOlderThenSettedNotification) 
                    message = "Last portal error response notified is older then setted";
                if (firstErrorResponseAfterGoodOne) 
                    message = "First error response after good one";
                if (firstPortalErrorNotification) 
                    message = "First portal error response";
                if (allErrorsNotification) 
                    message = "Email nofitications on every error response needed";
                
                await _portalService.AddNotificationToHistoryAsync(portal, response.Id, response.RequestDateTime, message, (int)emailProviderResponse);
                return emailProviderResponse;
            }

            return HttpStatusCode.NoContent;
        }
    }
}

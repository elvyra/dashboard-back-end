using Dashboard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// SendGrid service (email message formatting and sending)
    /// </summary>
    public class SendGridService : ISendGridService
    {
        private readonly string _apiKey;
        private readonly string _notificationsTemplateId;
        private readonly string _userPermissionsInfoTemplateId;
        private readonly string _fromEmail;
        private readonly string _replyEmail;
        private readonly ILogger<SendGridService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="apiKey">SendGrid ApiKey option</param>
        /// <param name="options">SendGrid options</param>
        public SendGridService(
            ILogger<SendGridService> logger, 
            IOptions<SendGridApiKey> apiKey, 
            IOptions<SendGridOptions> options)
        {
            _logger = logger;
            _apiKey = apiKey.Value.sendGridApiKey;
            _notificationsTemplateId = options.Value.NotificationsTemplateId;
            _userPermissionsInfoTemplateId = options.Value.UserPermissionsInfoTemplateId;
            _fromEmail = options.Value.fromEmail;
            _replyEmail = options.Value.replyEmail;
        }

        /// <summary>
        /// Sends simple email with error response info to the portal.Email provided email address
        /// </summary>
        /// <param name="portal">Portal, responded with error status code</param>
        /// <param name="portalResponse">Portal error response</param>
        /// <returns>Response from SendGrid</returns>
        public async Task<HttpStatusCode> SendEmailSimpleAsync(Portal portal, PortalResponse portalResponse)
        {
            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage();
            msg.From = new EmailAddress(_fromEmail);
            msg.ReplyTo = new EmailAddress(_replyEmail);
            msg.Subject = $"Dashboard Team 1: {portal.Name} error";
            msg.PlainTextContent = $"{portal.Name} (URL: {portal.URL}) responded with status code {portalResponse.Status} and message {portalResponse.ErrorMessage} on {portalResponse.RequestDateTime} (response Id: {portalResponse.Id})";
            msg.HtmlContent = $"<p><strong>Portal name:</strong> {portal.Name}</p>" +
                $"<p><strong>Portal URL:</strong> {portal.URL}</p>" +
                $"<p><strong>Error Id:</strong> {portalResponse.Id}</p>" +
                $"<p><strong>Request DateTime:</strong> {portalResponse.RequestDateTime}</p>" +
                $"<p><strong>Request Status Code:</strong> {portalResponse.Status}</p>" +
                $"<p><strong>Request Message:</strong> {portalResponse.ErrorMessage}</p>";
            msg.AddTo(new EmailAddress(portal.Email));
            var response = await client.SendEmailAsync(msg);
            LogResponse(response, portal, portalResponse);
            return response.StatusCode;
        }

        /// <summary>
        /// Sends tenplated email with error response info to the portal.Email provided email address
        /// </summary>
        /// <param name="portal">Portal, responded with error status code</param>
        /// <param name="portalResponse">Portal error response</param>
        /// <returns>Response from SendGrid</returns>
        public async Task<HttpStatusCode> SendEmailAsync(Portal portal, PortalResponse portalResponse)
        {
            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage();

            msg.From = new EmailAddress(_fromEmail);
            msg.ReplyTo = new EmailAddress(_replyEmail);
            msg.TemplateId = _notificationsTemplateId;
            msg.SetTemplateData(new
            {
                portalName = portal.Name,
                portalUrl = portal.URL,
                portalId = portal.Id,
                errorId = portalResponse.Id,
                errorDateTime = portalResponse.RequestDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                errorStatusCode = portalResponse.Status,
                errorMessage = portalResponse.ErrorMessage,
                errorResponseTime = portalResponse.ResponseTime
            });
            msg.AddTo(new EmailAddress(portal.Email));

            var response = await client.SendEmailAsync(msg);
            LogResponse(response, portal, portalResponse);
            return response.StatusCode;
        }

        /// <summary>
        /// Sends templated email with info aboutt changed permissions and email of changer        
        /// </summary>
        /// <param name="email">Email who changed permissions</param>
        /// <param name="user">User info</param>
        /// <returns>Response from SendGrid</returns>
        public async Task<HttpStatusCode> SendPermissionsChangedEmailAsync(string email, User user)
        {
            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage();

            msg.From = new EmailAddress(_fromEmail);
            msg.ReplyTo = new EmailAddress(_replyEmail);
            msg.TemplateId = _userPermissionsInfoTemplateId;
            msg.SetTemplateData(new
            {
                name = user.Name,
                surname = user.Surname,
                email = user.Email,
                changedBy = email,
                claims = user.Claims != null && user.Claims.Length > 0 ? string.Join(", ", user.Claims) : "none",
            });
            msg.AddTo(new EmailAddress(user.Email));

            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

        /// <summary>
        /// Write Logs 
        /// </summary>
        /// <param name="response">SendGrid response</param>
        /// <param name="portal">Portal information</param>
        /// <param name="portalResponse">Portal response information</param>
        private void LogResponse(Response response, Portal portal, PortalResponse portalResponse) 
        { 
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email notification for portal (Id: {portal.Id}) and response (Id: {portalResponse.Id}) queued to be delivered.");
                return;
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"Email notification for portal (Id: {portal.Id}) and response (Id: {portalResponse.Id}) is valid, but it is not queued to be delivered.");
                return;
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError($"An error occurred on a SendGrid server while trying to send email notification for portal (Id: {portal.Id}) and response (Id: {portalResponse.Id})");
                return;
            }
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogError($"The SendGrid v3 Web API is not available. (Email notification for portal (Id: {portal.Id}) and response (Id: {portalResponse.Id}) not delivered).");
                return;
            }

            _logger.LogError($"Email notification for portal (Id: {portal.Id}) and response (Id: {portalResponse.Id}) not delivered, message is not valid or authorization error occoured.");
        }
    }
}

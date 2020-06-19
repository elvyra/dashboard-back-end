using Dashboard.Models;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// Email sending logic
    /// </summary>
    public interface INotificationsService
    {
        /// <summary>
        /// Send notification email logic
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="portal">Portal responded</param>
        /// <returns>SendGrid>response code if email notification needed, 204 (NoContent) - if not</returns>
        Task<HttpStatusCode> SendNotificationEmailAsync(Portal portal, PortalResponse response);
    }
}
using Dashboard.Models;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// SendGrid service (email message formatting and sending)
    /// </summary>
    public interface ISendGridService
    {
        /// <summary>
        /// Sends simple email with error response info to the portal.Email provided email address
        /// </summary>
        /// <param name="portal">Portal, responded with error status code</param>
        /// <param name="portalResponse">Portal error response</param>
        /// <returns>Response from SendGrid</returns>
        Task<HttpStatusCode> SendEmailSimpleAsync(Portal portal, PortalResponse portalResponse);

        /// <summary>
        /// Sends templated email with error response info to the portal.Email provided email address
        /// </summary>
        /// <param name="portal">Portal, responded with error status code</param>
        /// <param name="portalResponse">Portal error response</param>
        /// <returns>Response from SendGrid</returns>
        Task<HttpStatusCode> SendEmailAsync(Portal portal, PortalResponse portalResponse);

        /// <summary>
        /// Sends templated email with info aboutt changed permissions and email of changer        
        /// </summary>
        /// <param name="email">Email who changed permissions</param>
        /// <param name="user">User info</param>
        /// <returns>Response from SendGrid</returns>
        Task<HttpStatusCode> SendPermissionsChangedEmailAsync(string email, User user);
    }
}
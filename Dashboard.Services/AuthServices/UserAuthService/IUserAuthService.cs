using Dashboard.Models;

namespace Dashboard.Services.AuthServices
{
    /// <summary>
    /// User Auth Service
    /// </summary>
    public interface IUserAuthService
    {
        /// <summary>
        /// Authorizes user
        /// </summary>
        /// <param name="login">Login user credentials</param>
        /// <returns>Authorized user or null</returns>
        User AuthUser(User login);

        /// <summary>
        /// Checks if user is disabled
        /// </summary>
        /// <param name="login">User info</param>
        /// <returns>Boolean</returns>
        bool IsUserDisabled(User login);
    }
}
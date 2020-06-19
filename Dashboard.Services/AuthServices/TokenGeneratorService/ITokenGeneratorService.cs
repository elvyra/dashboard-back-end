using Dashboard.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dashboard.Services.AuthServices
{
    /// <summary>
    /// Token generator
    /// </summary>
    public interface ITokenGeneratorService
    {
        /// <summary>
        /// Generates new JWT Token
        /// </summary>
        /// <param name="userInfo">User info</param>
        /// <returns>JWT Token</returns>
        Task<(string token, string refreshToken)> NewAsync(User userInfo);

        /// <summary>
        /// Validates and refreshes tokens 
        /// </summary>
        /// <param name="principal">Principals</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Tuple: access token and refresh token</returns>
        Task<(string token, string refreshToken)> Refresh(ClaimsPrincipal principal, string refreshToken);

        /// <summary>
        /// Deletes refresh token from Db 
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <param name="refreshToken">refresh token</param>
        /// <returns>True - refresh token removed succeeded, false - failed</returns>
        Task<bool> RemoveRefreshToken(ClaimsPrincipal principal, string refreshToken);

        /// <summary>
        /// Validates if user with given email has given refreah token
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Is valid refresh token</returns>
        Task<bool> ValidRefreshToken(int userId, string refreshToken);
    }
}
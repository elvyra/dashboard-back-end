using Dashboard.Data;
using Dashboard.Hash;
using Dashboard.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Dashboard.Services.AuthServices
{
    /// <summary>
    /// User Auth Service
    /// </summary>
    public class UserAuthService : IUserAuthService
    {
        private readonly DashboardDbContext _context;
        private readonly ILogger<UserAuthService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="logger">ILogger</param>
        public UserAuthService(
            DashboardDbContext context,
            ILogger<UserAuthService> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Authorizes user
        /// </summary>
        /// <param name="login">Login user credentials</param>
        /// <returns>Authorized user or null</returns>
        public User AuthUser(User login)
        {
            User user = null;
            User findUser = _context.Users
                .FirstOrDefault(u => u.Email == login.Email && u.IsActive == true);
            if (findUser != null)
            {
                if (new HashService().Verify(login.Password, findUser.Password))
                {
                    user = findUser;
                }
            }
            return user;
        }

        /// <summary>
        /// Checks if user is disabled
        /// </summary>
        /// <param name="login">User info</param>
        /// <returns>Boolean</returns>
        public bool IsUserDisabled(User login)
        {
            return _context.Users
                .Any(u => u.Email == login.Email && u.IsActive == false);
        }
    }
}

using Dashboard.Data;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Dashboard.Services.UserCrudServices
{
    /// <summary>
    /// Checks if email is already in use
    /// </summary>
    public class CheckEmailService : ICheckEmailService
    {
        private readonly DashboardDbContext _context;
        private readonly ILogger<CheckEmailService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="logger">ILogger</param>
        public CheckEmailService(
            DashboardDbContext context,
            ILogger<CheckEmailService> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// If email is in use (already taken)
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>bool</returns>
        public bool IsAlreadyTaken(string email) =>
            (_context.Users.FirstOrDefault(u => u.Email == email) != null);
    }
}

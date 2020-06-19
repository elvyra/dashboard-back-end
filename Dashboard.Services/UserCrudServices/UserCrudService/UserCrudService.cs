using Dashboard.Data;
using Dashboard.Hash;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Dashboard.Services.EmailNotificationServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Services.UserCrudServices
{
    /// <summary>
    /// User Crud Service
    /// </summary>
    public class UserCrudService : IUserCrudService
    {
        private readonly DashboardDbContext _context;
        private readonly ILogger<UserCrudService> _logger;
        private readonly ICheckEmailService _checkEmailService;
        private readonly ISendGridService _sendGridService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="logger">ILogger</param>
        /// <param name="checkEmailService">Check Email Service</param>
        /// <param name="sendGridService">Sending email notification service</param>
        public UserCrudService(
            DashboardDbContext context,
            ILogger<UserCrudService> logger,
            ICheckEmailService checkEmailService,
            ISendGridService sendGridService
            )
        {
            _context = context;
            _logger = logger;
            _checkEmailService = checkEmailService;
            _sendGridService = sendGridService;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CreateUserAsync(User user)
        {
            user.isPermanent = false;

            user.Password = new HashService()
                    .Hash(user.Password);

            if (!_checkEmailService.IsAlreadyTaken(user.Email))
            {
                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.ToString());
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">User info to update</param>
        /// <returns>Boolean</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user.UserId < 0)
            {
                _logger.LogDebug("Tried to update Main User");
                return false;
            }

            User userForUpdate = await _context.Users.FindAsync(user.UserId);

            if (userForUpdate != null)
                if (userForUpdate.isPermanent)
                {
                    _logger.LogDebug("Tried to update Permanent User");
                    return false;
                }

                if (!_checkEmailService.IsAlreadyTaken(user.Email) || user.Email == userForUpdate.Email)
                {
                    try
                    {
                        userForUpdate.Email = user.Email;

                        if (user.Password != null)
                            userForUpdate.Password = new HashService().Hash(user.Password);
                        else
                            _context.Entry(userForUpdate).Property(u => u.Password).IsModified = false;

                        userForUpdate.Name = user.Name;
                        userForUpdate.Surname = user.Surname;
                        userForUpdate.IsActive = user.IsActive;

                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"User {user.Email} information updated");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex.ToString());
                        return false;
                    }
                }
            return false;
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Boolean</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogDebug("Tried to remove Main User");
                return false;
            }

            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user.isPermanent)
                {
                    _logger.LogDebug("Tried to remove Permanent User");
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Get List of users in DB
        /// </summary>
        /// <returns>List of users</returns>
        public IEnumerable<User> GetAll()
        {
            var users = _context.Users
                .Where(c => c.UserId > 0)
                .Where(u => !u.isPermanent)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    IsActive = u.IsActive
                })
                .ToList();

            return users;
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>USer in Db</returns>
        public async Task<User> GetOneAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogDebug("Tried to get Main User");
                return null;
            }

            var user = await _context.Users.FindAsync(id);

            if (user.isPermanent)
            {
                _logger.LogDebug("Tried to get Permanent User");
                return null;
            }

            user.Password = null;
            return user;
        }

        /// <summary>
        /// Toggles user isActive prop
        /// </summary>
        /// <param name="id">USer Id</param>
        /// <returns>Void</returns>
        public async Task ToggleActiveAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogDebug("Tried to set as inActive Main User");
                return;
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return;

            if (user.isPermanent)
            {
                _logger.LogDebug("Tried to set as inActive permanent User");
                return;
            }

            user.IsActive = !user.IsActive;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sets as admin
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="email">Email who initiated</param>
        /// <returns>Void</returns>
        public async Task<bool?> ToggleIsAdmin(int id, string email)
        {
            if (id < 0)
            {
                _logger.LogDebug("Tried to update Main User");
                return false;
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogDebug($"User with Id = {id} not found");
                return false;
            }

            if (user.isPermanent)
            {
                _logger.LogDebug("Tried to update permanent User");
                return false;
            }

            if (string.Equals(user.Email, email))
            {
                _logger.LogDebug("Can not toggle own isAdmin claim");
                return null;
            }

            List<string> list = new List<string>();

            if (user.Claims == null)
            {
                list.Add(ClaimType.isAdmin.ToString());
                user.Claims = list.ToArray();
                try
                {
                    await _context.SaveChangesAsync();
                    await _sendGridService.SendPermissionsChangedEmailAsync(email, user);
                    return true;
                }
                catch (Exception)
                {
                    _logger.LogDebug("Error on saving when setting as Admin");
                    return false;
                }
            }

            if (user.Claims.ToList().Contains(ClaimType.isAdmin.ToString()))
            {
                list = user.Claims.ToList();
                list.Remove(ClaimType.isAdmin.ToString());
                user.Claims = list.ToArray();

                try
                {
                    await _context.SaveChangesAsync();
                    await _sendGridService.SendPermissionsChangedEmailAsync(email, user);
                    return true;
                }
                catch (Exception)
                {
                    _logger.LogDebug("Error on saving when removing isAdmin");
                    return false;
                }
            }
            
            list = user.Claims.ToList();
            list.Add(ClaimType.isAdmin.ToString());
            user.Claims = list.ToArray();

            try
            {
                await _context.SaveChangesAsync();
                await _sendGridService.SendPermissionsChangedEmailAsync(email, user);
                return true;
            }
            catch (Exception)
            {
                _logger.LogDebug("Error on saving when setting as Admin");
                return false;
            }
        }
    }
}

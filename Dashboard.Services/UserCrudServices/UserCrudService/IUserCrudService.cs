using Dashboard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard.Services.UserCrudServices
{
    /// <summary>
    /// User Crud Service
    /// </summary>
    public interface IUserCrudService
    {
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>Boolean</returns>
        Task<bool> CreateUserAsync(User user);

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">User info to update</param>
        /// <returns>Boolean</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Boolean</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Get List of users in DB
        /// </summary>
        /// <returns>List of users</returns>
        IEnumerable<User> GetAll();

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User in Db</returns>
        Task<User> GetOneAsync(int id);

        /// <summary>
        /// Toggles user isActive prop
        /// </summary>
        /// <param name="id">USer Id</param>
        /// <returns>Void</returns>
        Task ToggleActiveAsync(int id);

        /// <summary>
        /// Sets as admin
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="email">Email who initiated</param>
        /// <returns>Void</returns>
        Task<bool?> ToggleIsAdmin(int id, string email);
    }
}
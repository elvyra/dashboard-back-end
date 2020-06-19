using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard.Services.PortalCrudService
{
    /// <summary>
    /// Portals CRUD service
    /// </summary>
    public interface IPortalCrudService
    {
        /// <summary>
        /// Returns all portals list including deleted 
        /// </summary>
        /// <returns>All portals in Db</returns>
        Task<IEnumerable<Portal>> GetAllAsync();

        /// <summary>
        /// Returns all portals list with status Active
        /// </summary>
        /// <returns>All active portals in Db</returns>
        Task<IEnumerable<Portal>> GetAllActiveAsync();

        /// <summary>
        /// Returns all portals list excluding status Deletes
        /// </summary>
        /// <returns>Portals list</returns>
        Task<IEnumerable<Portal>> GetAllToDisplayAsync();

        /// <summary>
        /// Returns all portals list with status Deleted
        /// </summary>
        /// <returns>All deleted portals in Db</returns>
        Task<IEnumerable<Portal>> GetAllDeletedAsync();

        /// <summary>
        /// Clears Db from portals with status Deleted
        /// </summary>
        /// <returns>All deleted portals list</returns>
        Task<IEnumerable<Portal>> ClearAllDeletedAsync();

        /// <summary>
        /// Sets all Deleted portal status as Not Active
        /// </summary>
        /// <returns>Portal edited (or null if not found)</returns>
        Task<IEnumerable<Portal>> SetDeletedPortalsAsNotActiveAsync();

        /// <summary>
        /// Gets Portal from Db by Id
        /// </summary>
        /// <param name="Id">Portal Id to search</param>
        /// <returns>Portal (or null if not found)</returns>
        Task<Portal> GetPortalByIdAsync(Guid Id);

        /// <summary>
        /// Creates new portal in Db
        /// </summary>
        /// <param name="portal">Portal to create</param>
        /// <returns>Portal Created (or null if error)</returns>
        Task<Portal> CreateNewPortalAsync(Portal portal);

        /// <summary>
        /// Edites portal info
        /// </summary>
        /// <param name="portal">Portal to edit</param>
        /// <returns>Edited portal (or null if not found or error)</returns>
        Task<Portal> EditPortalAsync(Portal portal);

        /// <summary>
        /// Sets portal status to Active
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        Task<Portal> SetAsActiveAsync(Guid Id);

        /// <summary>
        /// Sets portal status to NotActive
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        Task<Portal> SetAsNotActiveAsync(Guid Id);

        /// <summary>
        /// Invert portal status (Active -> NotActive)
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        Task<Portal> InvertStatusAsync(Guid Id);

        /// <summary>
        /// Sets portal status to Deleted
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal marked as deleted (or null if not found)</returns>
        Task<Portal> SetAsDeletedAsync(Guid Id);

        /// <summary>
        /// Sets LastNotificationSent property to given DateTime
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <param name="dateTime">DateTime to set</param>
        /// <returns>Portal updated</returns>
        Task<Portal> SetLastNotificationSentAsync(Guid Id, DateTime dateTime);

        /// <summary>
        /// Clears portal by Id from Db
        /// </summary>
        /// <returns>Deleted portal</returns>
        Task<Portal> ClearDeletedByIdAsync(Guid Id);

        /// <summary>
        /// Notification add to history 
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <param name="responseId">Portal error response Id</param>
        /// <param name="requestDateTime">Notification initializing date time</param>
        /// <param name="message">Notification cause message</param>
        /// <param name="emailProviderResponse">Email provider response code</param>
        /// <returns>Portal updated</returns>
        Task<Notification> AddNotificationToHistoryAsync(Portal portal, Guid responseId, DateTime requestDateTime, string message, int emailProviderResponse);
    }
}

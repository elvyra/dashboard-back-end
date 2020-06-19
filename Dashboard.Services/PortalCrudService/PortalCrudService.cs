using Dashboard.Data;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Services.PortalCrudService
{
    /// <summary>
    /// Portals CRUD service
    /// </summary>
    public class PortalCrudService : IPortalCrudService
    {
        private ILogger<PortalCrudService> _logger;
        private DashboardDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="context">DbContext</param>
        public PortalCrudService(
            ILogger<PortalCrudService> logger, 
            DashboardDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Creates new portal in Db
        /// </summary>
        /// <param name="portal">Portal to create</param>
        /// <returns>Portal Created (or null if error)</returns>
        public async Task<Portal> CreateNewPortalAsync(Portal portal)
        {
            try
            {
                await _context.Portals.AddAsync(portal);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

            _logger.LogInformation("Portal (Id: {0}) created.", portal.Id);
            return portal;
        }

        /// <summary>
        /// Edites portal info
        /// </summary>
        /// <param name="portal">Portal to edit</param>
        /// <returns>Edited portal (or null if not found or error)</returns>
        public async Task<Portal> EditPortalAsync(Portal portal)
        {
            try
            {
                var portalInDb = await _context.Portals.FindAsync(portal.Id);

                if (portalInDb != null)
                {
                    portalInDb.Name = portal.Name;
                    portalInDb.Type = portal.Type;
                    portalInDb.URL = portal.URL;
                    portalInDb.Parameters = portal.Parameters;
                    portalInDb.Status = portal.Status;
                    portalInDb.Email = portal.Email;
                    portalInDb.CheckInterval = portal.CheckInterval;
                    portalInDb.ResponseTimeThreshold = portal.ResponseTimeThreshold;
                    portalInDb.Method = portal.Method;
                    portalInDb.BasicAuth = portal.BasicAuth;
                    portalInDb.UserName = portal.UserName;

                    if (portal.BasicAuth && !string.IsNullOrEmpty(portal.PasswordHashed))
                        portalInDb.PasswordHashed = portal.PasswordHashed;
                    else
                        _context.Entry(portalInDb)
                            .Property(u => u.PasswordHashed).IsModified = false;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Portal (Id: {0}) edited.", portal.Id);
                    return portalInDb;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for editing.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns all portals list including deleted 
        /// </summary>
        /// <returns>All portals in Db</returns>
        public async Task<IEnumerable<Portal>> GetAllAsync()
        {
            _logger.LogInformation("All portals in Db requested");
            return await _context.Portals.ToListAsync();
        }

        /// <summary>
        /// Returns all portals list with status Active
        /// </summary>
        /// <returns>All active portals in Db</returns>
        public async Task<IEnumerable<Portal>> GetAllActiveAsync()
        {
            _logger.LogInformation("All active portals in Db requested");
            return await _context.Portals.Where(p => p.Status == PortalStatus.Active).ToListAsync();
        }

        /// <summary>
        /// Returns all portals list with status Deleted
        /// </summary>
        /// <returns>All deleted portals in Db</returns>
        public async Task<IEnumerable<Portal>> GetAllDeletedAsync()
        {
            _logger.LogInformation("All portals with status Deleted in Db requested");
            return await _context.Portals.Where(p => p.Status == PortalStatus.Deleted).ToListAsync();
        }

        /// <summary>
        /// Clears Db from portals with status Deleted
        /// </summary>
        /// <returns>All deleted portals list</returns>
        public async Task<IEnumerable<Portal>> ClearAllDeletedAsync()
        {
            var List = await _context.Portals
                .Where(p => p.Status == PortalStatus.Deleted).ToListAsync();

            try
            {
                _context.RemoveRange(List);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

            _logger.LogInformation("Portal list in Db cleared from portals with status Deleted.");
            return List;
        }

        /// <summary>
        /// Gets Portal from Db by Id
        /// </summary>
        /// <param name="Id">Portal Id to search</param>
        /// <returns>Portal (or null if not found)</returns>
        public async Task<Portal> GetPortalByIdAsync(Guid Id)
        {
            _logger.LogInformation("All portals in Db requested");
            return await _context.Portals.FindAsync(Id);
        }

        /// <summary>
        /// Returns all portals list excluding status Deletes
        /// </summary>
        /// <returns>Portals list</returns>
        public async Task<IEnumerable<Portal>> GetAllToDisplayAsync()
        {
            _logger.LogInformation("All portals excluding with status Deleted in Db requested");
            return await _context.Portals.Where(p => p.Status != PortalStatus.Deleted).ToListAsync();
        }

        /// <summary>
        /// Sets portal status to Active
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        public async Task<Portal> SetAsActiveAsync(Guid Id)
        {
            try
            {
                var portal = await _context.Portals.FindAsync(Id);

                if (portal != null)
                {
                    portal.Status = PortalStatus.Active;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Portal (Id: {0}) status changed to Active.", portal.Id);
                    return portal;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for setting as Active.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Invert portal status (Active -> NotActive)
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        public async Task<Portal> InvertStatusAsync(Guid Id)
        {
            try
            {
                var portal = await _context.Portals.FindAsync(Id);

                if (portal != null)
                {
                    if (portal.Status == PortalStatus.Active)
                        portal.Status = PortalStatus.NotActive;
                    else if (portal.Status == PortalStatus.NotActive)
                        portal.Status = PortalStatus.Active;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Portal (Id: {0}) status inverted.", portal.Id);
                    return portal;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for inverting status.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets portal status to Deleted
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal marked as deleted (or null if not found)</returns>
        public async Task<Portal> SetAsDeletedAsync(Guid Id)
        {
            try
            {
                var portal = await _context.Portals.FindAsync(Id);

                if (portal != null)
                {
                    portal.Status = PortalStatus.Deleted;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Portal (Id: {0}) status changed to Deleted.", portal.Id);
                    return portal;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for setting as Deleted.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets portal status to NotActive
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <returns>Portal edited (or null if not found)</returns>
        public async Task<Portal> SetAsNotActiveAsync(Guid Id)
        {
            try
            {
                var portal = await _context.Portals.FindAsync(Id);

                if (portal != null)
                {
                    portal.Status = PortalStatus.NotActive;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Portal (Id: {0}) status changed to NotActive.", portal.Id);
                    return portal;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for setting as not active.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets all Deleted portal status as Not Active
        /// </summary>
        /// <returns>Portal edited (or null if not found)</returns>
        public async Task<IEnumerable<Portal>> SetDeletedPortalsAsNotActiveAsync()
        {
            _logger.LogInformation("All portals with status Deleted requested to set as Not Active");

            try
            {
                var portals = await _context.Portals
                    .Where(p => p.Status == PortalStatus.Deleted).ToListAsync();

                if (portals != null)
                {
                    foreach (var portal in portals)
                        portal.Status = PortalStatus.NotActive;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("All Deleted Portals status changed to NotActive.");
                    return portals;
                }
                _logger.LogWarning("No Portals found.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets LastNotificationSent property to given DateTime
        /// </summary>
        /// <param name="Id">Portal Id</param>
        /// <param name="dateTime">DateTime to set</param>
        /// <returns>Portal updated</returns>
        public async Task<Portal> SetLastNotificationSentAsync(Guid Id, DateTime dateTime)
        {
            try
            {
                var portal = await _context.Portals.FindAsync(Id);

                if (portal != null)
                {
                    portal.LastNotificationSent = dateTime;
                    _context.SaveChanges();
                    _logger.LogInformation("Portal (Id: {0}) last notification sent datetime changed.", portal.Id);
                    return portal;
                }

                _logger.LogWarning("Portal (Id: {0}) not found for changing last notification sent datetime.", portal.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Clears portal by Id from Db
        /// </summary>
        /// <returns>Deleted portal</returns>
        public async Task<Portal> ClearDeletedByIdAsync(Guid Id)
        {
            _logger.LogInformation("Requested to clear portal by Id from Db");

            var portal = await _context.Portals.FindAsync(Id);
            if (portal != null && portal.Status == PortalStatus.Deleted)
            {
                try
                {
                    _context.Remove(portal);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
                _logger.LogInformation("Portal by Id in Db cleared from portals with status Deleted.");
                return portal;
            }
            _logger.LogInformation("Portal founded by Id, but status was not Deleted. Portal was not cleared from Db");
            return null;
        }

        /// <summary>
        /// Notification add to history 
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <param name="responseId">Portal error response Id</param>
        /// <param name="requestDateTime">Notification initializing date time</param>
        /// <param name="message">Notification cause message</param>
        /// <param name="emailProviderResponse">Email provider response code</param>
        /// <returns>Portal updated</returns>
        public async Task<Notification> AddNotificationToHistoryAsync(Portal portal, Guid responseId, DateTime requestDateTime, string message, int emailProviderResponse)
        {
            var notification = new Notification();
            notification.PortalId = portal.Id;
            notification.PortalResponseId = responseId;
            notification.NotificationSentDateTime = requestDateTime;
            notification.NotificationCauseMessage = message;
            notification.EmailProviderResponse = emailProviderResponse;

            try
            {
                portal.EmailNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return notification;
        }
    }
}

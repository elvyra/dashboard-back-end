using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard.Services.QueryPortalService
{
    /// <summary>
    /// QueryPortal Service
    /// </summary>
    public interface IQueryPortalService
    {
        /// <summary>
        /// Get all portals list with last failure
        /// </summary>
        /// <returns>Portals array</returns>
        Task<IEnumerable<Portal>> GetAllAsync();

        /// <summary>
        /// Queries portal by Id
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns>Portal response and last portal failure DateTime</returns>
        Task<(PortalResponse portalResponse, DateTime? lastPortalFailureDateTime)> QueryByIdAsync(Guid id);

        /// <summary>
        /// Queries portal by Id
        /// </summary>
        /// <param name="portal">Portal Id</param>
        /// <returns>Portal response</returns>
        Task<PortalResponse> QueryByPortalAsync(Portal portal);
    }
}
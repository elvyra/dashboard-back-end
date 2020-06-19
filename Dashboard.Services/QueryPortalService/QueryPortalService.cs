using Dashboard.Data;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Dashboard.Services.EmailNotificationServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dashboard.Services.QueryPortalService
{
    /// <summary>
    /// QueryPortal Service
    /// </summary>
    public class QueryPortalService : IQueryPortalService
    {
        private readonly DashboardDbContext _context;
        private readonly HttpClient _http;
        private readonly INotificationsService _notificationsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="http">HttpClient</param>
        /// <param name="notificationsService">Email Notification service</param>
        public QueryPortalService(
            DashboardDbContext context,
            HttpClient http,
            INotificationsService notificationsService
            )
        {
            _context = context;
            _http = http;
            _notificationsService = notificationsService;
        }

        /// <summary>
        /// Write bad response to DB and logic
        /// </summary>
        /// <param name="response">Portal response</param>
        /// <param name="id">Portal Id</param>
        /// <returns></returns>
        private async Task WriteBadResponseAsync(PortalResponse response, Guid id)
        {
            var portal = await _context.Portals.FindAsync(id);
            portal.PortalResponses.Add(response);
            _context.SaveChanges();

            await WriteLastResponseData(portal, response);

            // Portal with responses history and email notification sending
            var portalWithResponses = _context.Portals.Include(r => r.PortalResponses).First(p => p.Id == id);
            await _notificationsService.SendNotificationEmailAsync(portalWithResponses, response);
        }

        /// <summary>
        /// Queries portal by Id
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns>Portal response and last portal failure DateTime</returns>
        public async Task<(PortalResponse portalResponse, DateTime? lastPortalFailureDateTime)> QueryByIdAsync(Guid id)
        {

            Portal portal = await _context.Portals
                .Include(p => p.PortalResponses)
                .Where(p => p.Status == PortalStatus.Active)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portal == null)
                return (null, null);

            var portalResponse = await QueryByPortalAsync(portal);

            DateTime? lastPortalFailureDateTime = null;
            if (portal.PortalResponses != null && portal.PortalResponses.Count() > 0)
            {
                lastPortalFailureDateTime = portal.PortalResponses
                    .Reverse()
                    .FirstOrDefault()
                    .RequestDateTime;
            }

            return (portalResponse, lastPortalFailureDateTime);
        }

        /// <summary>
        /// Queries portal by Id
        /// </summary>
        /// <param name="portal">Portal Id</param>
        /// <returns>Portal response</returns>
        public async Task<PortalResponse> QueryByPortalAsync(Portal portal)
        {
            if (portal.Type == PortalType.WebApp)
            {
                return await QueryPortalAsync(portal);
            }
            else
            {
                if (portal.Method == RequestMethod.POST)
                    return await QueryServicePostAsync(portal);
                return await QueryServiceGetAsync(portal);
            }
        }

        /// <summary>
        /// Portal type WebApp query
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <returns>Portal response</returns>
        private async Task<PortalResponse> QueryPortalAsync(Portal portal)
        {
            Stopwatch timer = new Stopwatch();
            PortalResponse portalResponse = new PortalResponse();
            portalResponse.RequestDateTime = DateTime.UtcNow;

            try
            {
                timer.Start();
                var get = await _http.GetAsync(portal.URL);
                timer.Stop();

                var response = get.StatusCode;
                int responseTime = (int)timer.ElapsedMilliseconds;

                portalResponse.Status = (int)response;
                portalResponse.ResponseTime = responseTime;
                portalResponse.StatusPageId = portal.Id;
                portalResponse.StatusPage = portal;

                await WriteLastResponseData(portal, portalResponse);

                if (portalResponse.Status < 200 || portalResponse.Status > 299)
                {
                    portalResponse.ErrorMessage = get.ToString();
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                }

                return portalResponse;
            }
            catch (HttpRequestException e)
            {
                portalResponse.ErrorMessage = e.Message;
                await WriteBadResponseAsync(portalResponse, portal.Id);
                return portalResponse;
            }
            catch (Exception ex)
            {
                portalResponse.ErrorMessage = ex.Message;
                await WriteBadResponseAsync(portalResponse, portal.Id);
                return portalResponse;
            }
        }

        /// <summary>
        /// Query service POST
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <returns>Portal response</returns>
        private async Task<PortalResponse> QueryServicePostAsync(Portal portal)
        {
            Stopwatch timer = new Stopwatch();
            PortalResponse portalResponse = new PortalResponse();
            portalResponse.RequestDateTime = DateTime.UtcNow;

            if (portal.Type == PortalType.ServiceSOAP)
            {
                var xml = XElement.Parse(portal.Parameters).ToString();
                var content = new StringContent(xml, Encoding.UTF8, "application/xml");

                try
                {
                    timer.Start();
                    var post = await _http.PostAsync(portal.URL, content);
                    timer.Stop();

                    var response = post.StatusCode;
                    int responseTime = (int)timer.ElapsedMilliseconds;

                    portalResponse.Status = (int)response;
                    portalResponse.ResponseTime = responseTime;
                    portalResponse.StatusPageId = portal.Id;
                    portalResponse.StatusPage = portal;

                    await WriteLastResponseData(portal, portalResponse);

                    if (portalResponse.Status < 200 || portalResponse.Status > 299)
                    {
                        portalResponse.ErrorMessage = post.ToString();
                        await WriteBadResponseAsync(portalResponse, portal.Id);
                    }

                    return portalResponse;
                }
                catch (HttpRequestException e)
                {
                    portalResponse.ErrorMessage = e.Message;
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                    return portalResponse;
                }
                catch (Exception ex)
                {
                    portalResponse.ErrorMessage = ex.Message;
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                    return portalResponse;
                }
            }
            else
            {
                var content = new StringContent(portal.Parameters, Encoding.UTF8, "application/json");

                try
                {
                    timer.Start();
                    var post = await _http.PostAsync(portal.URL, content);
                    timer.Stop();

                    var response = post.StatusCode;
                    int responseTime = (int)timer.ElapsedMilliseconds;

                    portalResponse.Status = (int)response;
                    portalResponse.ResponseTime = responseTime;
                    portalResponse.StatusPageId = portal.Id;
                    portalResponse.StatusPage = portal;

                    await WriteLastResponseData(portal, portalResponse);

                    if (portalResponse.Status < 200 || portalResponse.Status > 299)
                    {
                        portalResponse.ErrorMessage = post.ToString();
                        await WriteBadResponseAsync(portalResponse, portal.Id);
                    }

                    return portalResponse;
                }
                catch (HttpRequestException e)
                {
                    portalResponse.ErrorMessage = e.Message;
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                    return portalResponse;
                }
                catch (Exception ex)
                {
                    portalResponse.ErrorMessage = ex.Message;
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                    return portalResponse;
                }
            }
        }

        /// <summary>
        /// Query service GET
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <returns>Portal response</returns>
        private async Task<PortalResponse> QueryServiceGetAsync(Portal portal)
        {
            Stopwatch timer = new Stopwatch();
            PortalResponse portalResponse = new PortalResponse();
            portalResponse.RequestDateTime = DateTime.UtcNow;

            try
            {
                timer.Start();
                var get = await _http.GetAsync(portal.URL);
                timer.Stop();

                var response = get.StatusCode;
                int responseTime = (int)timer.ElapsedMilliseconds;

                portalResponse.Status = (int)response;
                portalResponse.ResponseTime = responseTime;
                portalResponse.StatusPageId = portal.Id;
                portalResponse.StatusPage = portal;

                await WriteLastResponseData(portal, portalResponse);

                if (portalResponse.Status < 200 || portalResponse.Status > 299)
                {
                    portalResponse.ErrorMessage = get.ToString();
                    await WriteBadResponseAsync(portalResponse, portal.Id);
                }

                return portalResponse;
            }
            catch (HttpRequestException e)
            {
                portalResponse.ErrorMessage = e.Message;
                await WriteBadResponseAsync(portalResponse, portal.Id);
                return portalResponse;
            }
            catch (Exception ex)
            {
                portalResponse.ErrorMessage = ex.Message;
                await WriteBadResponseAsync(portalResponse, portal.Id);
                return portalResponse;
            }
        }

        /// <summary>
        /// Get all portals list with last failure
        /// </summary>
        /// <returns>Portals array</returns>
        public async Task<IEnumerable<Portal>> GetAllAsync()
        {
            var portals = await _context.Portals
                 .Include(p => p.PortalResponses)
                 .Where(p => p.Status == PortalStatus.Active)
                 .ToListAsync();

            foreach (var portal in portals)
            {
                portal.PortalResponses = portal.PortalResponses
                    .Reverse()
                    .Take(1)
                    .ToList();
            }

            return portals;
        }

        /// <summary>
        /// Writes Last response data to portal entity
        /// </summary>
        /// <param name="portal">Portal</param>
        /// <param name="response">Last response</param>
        /// <returns></returns>
        private async Task WriteLastResponseData(Portal portal, PortalResponse response)
        {
            portal.LastRequestDateTime = response.RequestDateTime;
            portal.LastRequestErrorMessage = response.ErrorMessage;
            portal.LastRequestResponseTime = response.ResponseTime;
            portal.LastRequestStatus = response.Status;
            await _context.SaveChangesAsync();
        }
    }
}

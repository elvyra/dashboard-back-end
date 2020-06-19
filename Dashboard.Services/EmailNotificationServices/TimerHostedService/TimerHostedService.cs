using Dashboard.Data;
using Dashboard.Services.QueryPortalService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services.EmailNotificationServices
{
    /// <summary>
    /// Timer Hosted Service
    /// </summary>
    public class TimerHostedService : BackgroundService
    {
        /// <summary>
        /// Services in app
        /// </summary>
        public IServiceProvider Services { get; }

        // Frequecy to check active not queries portals list, timer setting
        private readonly int _frequency;

        private readonly ILogger<TimerHostedService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public TimerHostedService(
            IServiceProvider services, 
            IOptions<TimerHostedServiceOptions> options, 
            ILogger<TimerHostedService> logger)
        {
            Services = services;
            var frequency = int.TryParse(options.Value.FrequencyInMinutes, out _frequency);
            if (!frequency) _frequency = 60;
            _frequency *= 60000;
            _logger = logger;
        }

        /// <summary>
        /// Actions to implement on timer count
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Timed Hosted Service is working. DateTime:{0}", DateTime.UtcNow);
                await DoWork(stoppingToken);
                await Task.Delay(_frequency, stoppingToken);
            }
        }

        /// <summary>
        /// Queries active portal list (the ones, not requested by other services)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var DbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

                var http = scope.ServiceProvider.GetRequiredService<HttpClient>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationsService>();

                IQueryPortalService queryPortalService = new Dashboard.Services.QueryPortalService.QueryPortalService(DbContext, http, notificationService);

                var time = DateTime.UtcNow;

                var activePortalsListFull = await DbContext.Portals.Where(p => p.Status == Models.Enums.PortalStatus.Active).ToListAsync();

                var checkPortalsList = activePortalsListFull
                    .Where(p =>
                    {
                        var lastRequestDateTime = p.LastRequestDateTime;
                        var nextRequestDateTime = lastRequestDateTime.AddSeconds(p.CheckInterval);
                        _logger.LogInformation("Portal {0} => {1}", p.Name, nextRequestDateTime);
                        return nextRequestDateTime < time;
                    })
                    .ToList();

                foreach (var portal in checkPortalsList)
                {
                    var response = await queryPortalService.QueryByIdAsync(portal.Id);
                    _logger.LogInformation("Portal (Id: {0}, Name: \"{1}\", responded with status code \"{2}\", message \"{3}\"", portal.Id, portal.Name, response.portalResponse.Status, response.portalResponse.ErrorMessage);
                }
            }
        }
    }
}

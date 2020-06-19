using Dashboard.Models;
using Dashboard.Models.Enums;
using Dashboard.Services.EmailNotificationServices;
using Dashboard.Services.PortalCrudService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using Xunit;

namespace Dashboard.xUnitTest
{
    public class NotificationServiceTests
    {
        private Mock<ISendGridService> _mockSendGridService;
        private Mock<IPortalCrudService> _mockPortalService;
        private Mock<ILogger<NotificationsService>> _mockLogger;

        public NotificationServiceTests()
        {
            _mockSendGridService = new Mock<ISendGridService>();
            _mockPortalService = new Mock<IPortalCrudService>();
            _mockLogger = new Mock<ILogger<NotificationsService>>();
        }

        [Fact]
        public async void Incorrect_Options_Negative_hoursToIgnoreSameError()
        {
            var options = Options.Create(new NotificationsOptions() 
            { 
                HoursToIgnoreContinuousError = -3 
            });
            var mockOptions = new Mock<IOptions<NotificationsOptions>>();
            mockOptions.Setup(o => o.Value).Returns(options.Value);

            var portal = new Portal();
            var portalResponse = new PortalResponse();

            var notificationsService = new NotificationsService(_mockLogger.Object, _mockSendGridService.Object, _mockPortalService.Object, mockOptions.Object);

            var response = await notificationsService.SendNotificationEmailAsync(portal, portalResponse);

            Assert.Equal(HttpStatusCode.NoContent, response);
            _mockPortalService.Verify(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
            _mockSendGridService.Verify(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>()), Times.Never);
        }


        [Fact]
        public async void Email_Notification_On_Every_Bad_Response()
        {
            var options = Options.Create(new NotificationsOptions() 
            { 
                HoursToIgnoreContinuousError = 0 
            });
            var mockOptions = new Mock<IOptions<NotificationsOptions>>();
            mockOptions.Setup(o => o.Value).Returns(options.Value);

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false,
                LastNotificationSent = DateTime.Now.AddHours(-2)
            };

            var portalResponse = new PortalResponse()
            {
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.Now.AddHours(-1),
                Status = 404,
                ResponseTime = 230,
                StatusPageId = portal.Id,
                StatusPage = portal,
                ErrorMessage = "Test error message"
            };

            var portalUpdated = portal;
            portalUpdated.LastNotificationSent = portalResponse.RequestDateTime;

            _mockPortalService.Setup(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(portalUpdated);

            _mockSendGridService.Setup(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>())).ReturnsAsync(HttpStatusCode.Accepted);

            var notificationsService = new NotificationsService(_mockLogger.Object, _mockSendGridService.Object, _mockPortalService.Object, mockOptions.Object);

            var responseFromService = await notificationsService.SendNotificationEmailAsync(portal, portalResponse);

            Assert.Equal(HttpStatusCode.Accepted, responseFromService);
            _mockPortalService.Verify(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            _mockSendGridService.Verify(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>()), Times.Once);
        }

        [Fact]
        public async void Email_Notification_Needed()
        {
            var options = Options.Create(new NotificationsOptions() 
            { 
                HoursToIgnoreContinuousError = 8 
            });
            var mockOptions = new Mock<IOptions<NotificationsOptions>>();
            mockOptions.Setup(o => o.Value).Returns(options.Value);

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false,
                LastNotificationSent = DateTime.Now.AddHours(-9)
            };

            var portalResponse = new PortalResponse()
            {
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                Status = 404,
                ResponseTime = 230,
                StatusPageId = portal.Id,
                StatusPage = portal,
                ErrorMessage = "Test error message"
            };

            var portalUpdated = portal;
            portalUpdated.LastNotificationSent = portalResponse.RequestDateTime;

            _mockPortalService.Setup(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(portalUpdated);

            _mockSendGridService.Setup(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>())).ReturnsAsync(HttpStatusCode.Accepted);

            var notificationsService = new NotificationsService(_mockLogger.Object, _mockSendGridService.Object, _mockPortalService.Object, mockOptions.Object);

            var responseFromService = await notificationsService.SendNotificationEmailAsync(portal, portalResponse);

            Assert.Equal(HttpStatusCode.Accepted, responseFromService);
            _mockPortalService.Verify(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            _mockSendGridService.Verify(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>()), Times.Once);
        }

        [Fact]
        public async void Email_Notification_Not_Needed()
        {
            var options = Options.Create(new NotificationsOptions() 
            { 
                HoursToIgnoreContinuousError = 8 
            });
            var mockOptions = new Mock<IOptions<NotificationsOptions>>();
            mockOptions.Setup(o => o.Value).Returns(options.Value);

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false,
                LastNotificationSent = DateTime.Now.AddHours(-7),
                LastRequestStatus = 404
            };

            portal.PortalResponses.Add(new PortalResponse()
            {
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.Now.AddHours(-9),
                Status = 404,
                ResponseTime = 230,
                StatusPageId = portal.Id,
                StatusPage = portal,
                ErrorMessage = "Test error message"
            });

            portal.PortalResponses.Add(new PortalResponse() 
            {
                Id = Guid.NewGuid(),
                RequestDateTime = portal.LastNotificationSent,
                Status = 404,
                ResponseTime = 230,
                StatusPageId = portal.Id,
                StatusPage = portal,
                ErrorMessage = "Test error message"
            });

            var portalResponse = new PortalResponse()
            {
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                Status = 404,
                ResponseTime = 230,
                StatusPageId = portal.Id,
                StatusPage = portal,
                ErrorMessage = "Test error message"
            };

            var portalUpdated = portal;
            portalUpdated.LastNotificationSent = portalResponse.RequestDateTime;
            portalUpdated.PortalResponses.Add(portalResponse);
            portalUpdated.LastRequestDateTime = portalResponse.RequestDateTime;
            portalUpdated.LastRequestStatus = portalResponse.Status;
            portalUpdated.LastRequestResponseTime = portalResponse.ResponseTime;
            portalUpdated.LastRequestErrorMessage = portalResponse.ErrorMessage;

            _mockPortalService.Setup(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(portalUpdated);

            _mockSendGridService.Setup(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>())).ReturnsAsync(HttpStatusCode.Accepted);

            var notificationsService = new NotificationsService(_mockLogger.Object, _mockSendGridService.Object, _mockPortalService.Object, mockOptions.Object);

            var responseFromService = await notificationsService.SendNotificationEmailAsync(portal, portalResponse);

            Assert.Equal(HttpStatusCode.NoContent, responseFromService);
            _mockPortalService.Verify(x => x.SetLastNotificationSentAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
            _mockSendGridService.Verify(x => x.SendEmailAsync(It.IsAny<Portal>(), It.IsAny<PortalResponse>()), Times.Never);
        }
    }
}

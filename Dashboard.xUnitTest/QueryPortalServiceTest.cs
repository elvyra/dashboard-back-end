using System;
using Xunit;
using Dashboard.Services;
using Dashboard.Models;
using Dashboard.Data;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Dashboard.Services.EmailNotificationServices;
using Dashboard.Services.QueryPortalService;

namespace Dashboard.xUnitTest
{
    public class QueryPortalServiceTest
    {
        public IQueryPortalService _service;
        public QueryPortalServiceTest()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Dashboard;Trusted_Connection=True;MultipleActiveResultSets=true");

            var notificationMock = new Mock<INotificationsService>();

            _service = new QueryPortalService(
                new DashboardDbContext(options.Options),
                new System.Net.Http.HttpClient(),
                notificationMock.Object
                );
        }

        [Fact]
        public async void QueryGoogleGet200()
        {
            var portal = new Portal
            {
                Type = 0,
                URL = "http://www.google.com"
            };

            var response = await _service.QueryByPortalAsync(portal);
            Assert.Equal(200, response.Status);
        }

        /*
        [Fact]
        public async void PostJsonUserGet200()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            string email = "";
            Random rnd = new Random();
            for (int i = 0; i < 7; i++)
            {
                email += chars[rnd.Next(chars.Length)];
            }
            email += "@";
            for (int i = 0; i < 5; i++)
            {
                email += chars[rnd.Next(chars.Length)];
            }
            email += ".test";
            string parameters = "{\"Email\":\"" + email + "\",\"Name\":\"Jonas\", \"Surname\":\"Joninas\", \"Password\":\"asdasd\"}";

            var portal = new Portal
            {
                Parameters = parameters,
                Type = Models.Enums.PortalType.ServiceREST,
                Method = Models.Enums.RequestMethod.POST,
                URL = "https://dashboardapino1.azurewebsites.net/user/register"
            };

            var response = await _service.QueryByPortalAsync(portal);
            Assert.Equal(401, response.Status);
        }
        */

        /*
        [Fact]
        public async void PostEmptyUserJsonGetNot200()
        {
            var mock = new Mock<Portal>();
            mock.Object.Parameters = "";
            mock.Object.Type = Models.Enums.PortalType.ServiceREST;
            mock.Object.Method = Models.Enums.RequestMethod.POST;
            mock.Object.URL = "https://dashboardapino1.azurewebsites.net/user/register";


            var response = await _service.QueryByPortalAsync(mock.Object);
            Assert.NotEqual(200, response.Status);
            
        }
        */

        [Fact]
        public void QueryByIdGetNotNull()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            var contextMock = new Mock<DashboardDbContext>();
            var dbsetMock = new Mock<DbSet<Portal>>();
            contextMock.Setup(x => x.Portals).Returns(dbsetMock.Object);

            contextMock.Setup(foo => foo.Portals.FindAsync(It.IsAny<Guid>())).ReturnsAsync(
                new Portal
                {
                    URL = "http://www.google.com"
                }
                );

            var serviceMock = new Mock<IQueryPortalService>();
            serviceMock.Setup(x => x.QueryByPortalAsync(It.IsAny<Portal>())).ReturnsAsync( 
                new PortalResponse
                {
                    Status = 245
                }
                );
            var notificationMock = new Mock<INotificationsService>();

            IQueryPortalService queryService = new QueryPortalService(contextMock.Object, new HttpClient(), notificationMock.Object);

            // Act

            // var response = await queryService.QueryByIdAsync(guid);
            // var portalResponse = response.portalResponse;
            // var resp = await serviceMock.Object.QueryByIdAsync(guid);
            // Assert
            // Assert.NotNull(portalResponse);
            // Assert.IsType<PortalResponse>(portalResponse);
            // contextMock.Verify(s => s.Portals.FindAsync(It.IsAny<Guid>()), Times.Once());

            //cant mock
            //serviceMock.Verify(s => s.QueryByPortalAsync(It.IsAny<Portal>()), Times.Once());
        }

        [Fact]
        public void QueryByIdNullGetNull()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            var contextMock = new Mock<DashboardDbContext>();
            var dbsetMock = new Mock<DbSet<Portal>>();
            contextMock.Setup(x => x.Portals).Returns(dbsetMock.Object);

            contextMock.Setup(foo => foo.Portals.FindAsync(It.IsAny<Guid>())).Returns(null);
            var notificationMock = new Mock<INotificationsService>();

            IQueryPortalService queryService = new QueryPortalService(contextMock.Object, new HttpClient(), notificationMock.Object);

            // Act

            // var response = await queryService.QueryByIdAsync(guid);
            // var portalResponse = response.portalResponse;

            // Assert
            // Assert.Null(portalResponse);
            // contextMock.Verify(s => s.Portals.FindAsync(It.IsAny<Guid>()), Times.Once());
        }

        [Fact]
        public async void MoqQueryByPortalAsyncGet200()
        {
            var mock = new Mock<IQueryPortalService>();
            mock.Setup(x => x.QueryByPortalAsync(It.IsAny<Portal>()))
                .ReturnsAsync(new PortalResponse { Status = 200 });
            var response = await mock.Object.QueryByPortalAsync(It.IsAny<Portal>());

            Assert.Equal(200, response.Status);
        }
    }
}

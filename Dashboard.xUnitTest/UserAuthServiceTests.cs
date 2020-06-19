using Dashboard.Data;
using Dashboard.Models;
using Dashboard.Services.AuthServices;
using Dashboard.Services.EmailNotificationServices;
using Dashboard.Services.UserCrudServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Dashboard.xUnitTest
{
    public class UserAuthServiceTests
    {
        private Mock<ILogger<UserAuthService>> _mockLogger;
        private Mock<ILogger<CheckEmailService>> _mockLoggerCheckEmail;
        private Mock<ILogger<UserCrudService>> _mockLoggerUserCrud;
        private Mock<ISendGridService> _mockSendGridService;

        public UserAuthServiceTests()
        {
            _mockLogger = new Mock<ILogger<UserAuthService>>();
            _mockLoggerCheckEmail = new Mock<ILogger<CheckEmailService>>();
            _mockLoggerUserCrud = new Mock<ILogger<UserCrudService>>();
            _mockSendGridService = new Mock<ISendGridService>();
        }

        [Fact]
        public async void Authorize_Valid_User()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Authorize_Valid_User")
                .Options;

            var user = new User()
            {
                Email = "test@test.com",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var serviceCheckEmail = new CheckEmailService(context, _mockLoggerCheckEmail.Object);
                var serviceCrud = new UserCrudService(context, _mockLoggerUserCrud.Object, serviceCheckEmail, _mockSendGridService.Object);
                
                var userCreate = new User()
                {
                    Email = user.Email,
                    Password = user.Password,
                    IsActive = user.IsActive
                };

                await serviceCrud.CreateUserAsync(userCreate);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var serviceAuth = new UserAuthService(context, _mockLogger.Object);
                var result = serviceAuth.AuthUser(user);

                Assert.Equal(1, await context.Users.CountAsync());
                Assert.NotNull(result);
                Assert.Equal(user.Email, result.Email);
            }
        }

        [Fact]
        public async void Authorize_InValid_User()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Authorize_InValid_User")
                .Options;

            var user = new User()
            {
                Email = "test@test.com",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var serviceCheckEmail = new CheckEmailService(context, _mockLoggerCheckEmail.Object);
                var serviceCrud = new UserCrudService(context, _mockLoggerUserCrud.Object, serviceCheckEmail, _mockSendGridService.Object);

                var userCreate = new User()
                {
                    Email = user.Email,
                    Password = "IncorrectP@ssw0rd",
                    IsActive = user.IsActive
                };

                await serviceCrud.CreateUserAsync(userCreate);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var serviceAuth = new UserAuthService(context, _mockLogger.Object);
                var result = serviceAuth.AuthUser(user);

                Assert.Equal(1, await context.Users.CountAsync());
                Assert.Null(result);
            }
        }

        [Fact]
        public async void Is_User_Disabled_True()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Is_User_Disabled_True")
                .Options;

            var user = new User()
            {
                Email = "test@test.com",
                Password = "P@ssw0rd",
                IsActive = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var serviceCheckEmail = new CheckEmailService(context, _mockLoggerCheckEmail.Object);
                var serviceCrud = new UserCrudService(context, _mockLoggerUserCrud.Object, serviceCheckEmail, _mockSendGridService.Object);

                var userCreate = new User()
                {
                    Email = user.Email,
                    Password = user.Password,
                    IsActive = user.IsActive
                };

                await serviceCrud.CreateUserAsync(userCreate);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var serviceAuth = new UserAuthService(context, _mockLogger.Object);
                var result = serviceAuth.IsUserDisabled(user);

                Assert.Equal(1, await context.Users.CountAsync());
                Assert.True(result);
            }
        }

        [Fact]
        public async void Is_User_Disabled_False()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Is_User_Disabled_False")
                .Options;

            var user = new User()
            {
                Email = "test@test.com",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var serviceCheckEmail = new CheckEmailService(context, _mockLoggerCheckEmail.Object);
                var serviceCrud = new UserCrudService(context, _mockLoggerUserCrud.Object, serviceCheckEmail, _mockSendGridService.Object);

                var userCreate = new User()
                {
                    Email = user.Email,
                    Password = user.Password,
                    IsActive = user.IsActive
                };

                await serviceCrud.CreateUserAsync(userCreate);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var serviceAuth = new UserAuthService(context, _mockLogger.Object);
                var result = serviceAuth.IsUserDisabled(user);

                Assert.Equal(1, await context.Users.CountAsync());
                Assert.False(result);
            }
        }
    }
}

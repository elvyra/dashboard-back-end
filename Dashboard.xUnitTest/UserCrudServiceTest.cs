using Dashboard.Data;
using Dashboard.Hash;
using Dashboard.Models;
using Dashboard.Services.EmailNotificationServices;
using Dashboard.Services.UserCrudServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Xunit;

namespace Dashboard.xUnitTest
{
    public class UserCrudServiceTest
    {
        private Mock<ILogger<UserCrudService>> _mockLogger;
        private Mock<ISendGridService> _mockSendGridService;

        public UserCrudServiceTest()
        {
            _mockLogger = new Mock<ILogger<UserCrudService>>();
            _mockSendGridService = new Mock<ISendGridService>();
        }

        [Fact]
        public async void Create_User()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Create_New_User")
                .Options;

            var user = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Users.CountAsync().Result);
                Assert.Equal(user.Name, context.Users.SingleAsync().Result.Name);
            }
        }

        [Fact]
        public async void Update_User_Changes_Property()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Update_User_Changes_Property")
                .Options;

            var user = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var userUpdated = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "Leon",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user);
                await service.UpdateUserAsync(userUpdated);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Users.CountAsync().Result);
                Assert.Equal(userUpdated.Name, context.Users.SingleAsync().Result.Name);
            }
        }

        [Fact]
        public async void Update_User_Changes_Password()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Update_User_Changes_Password")
                .Options;

            var user = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var userUpdated = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd222",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user);
                await context.SaveChangesAsync();
                await service.UpdateUserAsync(userUpdated);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Users.CountAsync().Result);
                Assert.True(new HashService().Verify(userUpdated.Password, context.Users.SingleAsync().Result.Password));
            }
        }

        [Fact]
        public async void Update_User_Changes_Password_Not_Needed()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Update_User_Changes_Password_Not_Needed")
                .Options;

            var user = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var userUpdated = new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "Leon",
                Surname = "Smith",
                IsActive = true
            };

            var userHashedPassword = "";
            var userUpdatedHashedPassword = "";
            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user);
                userHashedPassword = context.Users.SingleOrDefaultAsync().Result.Password;
                await context.SaveChangesAsync();
            }  
            
            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.UpdateUserAsync(userUpdated);
                userUpdatedHashedPassword = context.Users.SingleOrDefaultAsync().Result.Password;
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Users.CountAsync().Result);
                Assert.Equal(userHashedPassword, userUpdatedHashedPassword);
            }
        }

        [Fact]
        public async void Delete_User()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Delete_User")
                .Options;

            var user1 = new User()
            {
                UserId = 1,
                Email = "test1@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var user2 = new User()
            {
                UserId = 2,
                Email = "test2@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd222",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user1);
                await service.CreateUserAsync(user2);
                await context.SaveChangesAsync();
                await service.DeleteUserAsync(user1.UserId);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Users.CountAsync().Result);
                Assert.Equal(user2.UserId, context.Users.SingleAsync().Result.UserId);
            }
        }

        [Fact]
        public async void Get_All()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_All")
                .Options;

            var user1 = new User()
            {
                UserId = 1,
                Email = "test1@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var user2 = new User()
            {
                UserId = 2,
                Email = "test2@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd222",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user1);
                await service.CreateUserAsync(user2);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                var list = service.GetAll();

                Assert.Equal(2, list.Count());
            }
        }

        [Fact]
        public async void Get_One()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_One")
                .Options;

            var user1 = new User()
            {
                UserId = 1,
                Email = "test1@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            var user2 = new User()
            {
                UserId = 2,
                Email = "test2@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd222",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user1);
                await service.CreateUserAsync(user2);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                var result = await service.GetOneAsync(user1.UserId);

                Assert.Equal(user1.Email, result.Email);
            }
        }

        [Fact]
        public async void Toggle_Active()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Toggle_Active")
                .Options;

            var user = new User()
            {
                UserId = 1,
                Email = "test1@email.com",
                Name = "John",
                Surname = "Smith",
                Password = "P@ssw0rd",
                IsActive = true
            };

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.CreateUserAsync(user);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var mockCheckEmailService = new CheckEmailService(new DashboardDbContext(options), new Mock<ILogger<CheckEmailService>>().Object);

                var service = new UserCrudService(context, _mockLogger.Object, mockCheckEmailService, _mockSendGridService.Object);
                await service.ToggleActiveAsync(user.UserId);

                Assert.Equal(!user.IsActive, context.Users.SingleOrDefaultAsync().Result.IsActive);
            }
        }
    }
}

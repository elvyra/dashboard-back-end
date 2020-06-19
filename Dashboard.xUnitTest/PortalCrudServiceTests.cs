using Dashboard.Data;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Dashboard.Services.PortalCrudService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Dashboard.xUnitTest
{
    public class PortalCrudServiceTests
    {
        private Mock<ILogger<PortalCrudService>> _mockLogger;

        public PortalCrudServiceTests()
        {
            _mockLogger = new Mock<ILogger<PortalCrudService>>();
        }

        [Fact]
        public async void Create_New_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

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
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                Assert.Equal(1, context.Portals.Count());
                Assert.Equal(portal.Id, context.Portals.Single().Id);
            }

        }

        [Fact]
        public async void Edit_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Edit_Portal_in_database")
                .Options;

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
                BasicAuth = false
            };

            var portalEdited = new Portal()
            {
                Id = portal.Id,
                Name = "Test Portal Edited",
                Type = PortalType.ServiceREST,
                URL = "http://www.delfi.lt/test",
                Parameters = "{}",
                Status = PortalStatus.NotActive,
                Email = "test@email.com",
                CheckInterval = 200,
                ResponseTimeThreshold = 200,
                Method = RequestMethod.POST,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.EditPortalAsync(portalEdited);
                Assert.Equal(portalEdited.Id, result.Id);
                Assert.Equal(portalEdited.Name, result.Name);
                Assert.Equal(portalEdited.Type, result.Type);
                Assert.Equal(portalEdited.URL, result.URL);
                Assert.Equal(portalEdited.Parameters, result.Parameters);
                Assert.Equal(portalEdited.Status, result.Status);
                Assert.Equal(portalEdited.Email, result.Email);
                Assert.Equal(portalEdited.CheckInterval, result.CheckInterval);
                Assert.Equal(portalEdited.ResponseTimeThreshold, result.ResponseTimeThreshold);
                Assert.Equal(portalEdited.Method, result.Method);
            }          
        }

        [Fact]
        public async void Edit_Portal_With_Password()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Edit_Portal_in_database_With_Password")
                .Options;

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
                BasicAuth = true,
                UserName = "testUser",
                PasswordHashed = "P@ssw0rd"
            };

            var portalEdited = new Portal()
            {
                Id = portal.Id,
                Name = "Test Portal Edited",
                Type = PortalType.ServiceREST,
                URL = "http://www.delfi.lt/test",
                Parameters = "{}",
                Status = PortalStatus.NotActive,
                Email = "test@email.com",
                CheckInterval = 200,
                ResponseTimeThreshold = 200,
                Method = RequestMethod.POST,
                BasicAuth = true,
                UserName = "testUser",
                PasswordHashed = "TestPAss123"
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.EditPortalAsync(portalEdited);
                Assert.Equal(portalEdited.Id, result.Id);
                Assert.Equal(portalEdited.Name, result.Name);
                Assert.Equal(portalEdited.Type, result.Type);
                Assert.Equal(portalEdited.URL, result.URL);
                Assert.Equal(portalEdited.Parameters, result.Parameters);
                Assert.Equal(portalEdited.Status, result.Status);
                Assert.Equal(portalEdited.Email, result.Email);
                Assert.Equal(portalEdited.CheckInterval, result.CheckInterval);
                Assert.Equal(portalEdited.ResponseTimeThreshold, result.ResponseTimeThreshold);
                Assert.Equal(portalEdited.Method, result.Method);
                Assert.Equal(portalEdited.BasicAuth, result.BasicAuth);
                Assert.Equal(portalEdited.UserName, result.UserName);
                Assert.Equal(portalEdited.PasswordHashed, result.PasswordHashed);
            }
        }

        [Fact]
        public async void Edit_Portal_No_Password()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Edit_Portal_in_database_No_Password")
                .Options;

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
                BasicAuth = true,
                UserName = "testUser",
                PasswordHashed = "P@ssw0rd"
            };

            var portalEdited = new Portal()
            {
                Id = portal.Id,
                Name = "Test Portal Edited",
                Type = PortalType.ServiceREST,
                URL = "http://www.delfi.lt/test",
                Parameters = "{}",
                Status = PortalStatus.NotActive,
                Email = "test@email.com",
                CheckInterval = 200,
                ResponseTimeThreshold = 200,
                Method = RequestMethod.POST,
                BasicAuth = true,
                UserName = "testUser"
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.EditPortalAsync(portalEdited);
                Assert.Equal(portalEdited.Id, result.Id);
                Assert.Equal(portalEdited.Name, result.Name);
                Assert.Equal(portalEdited.Type, result.Type);
                Assert.Equal(portalEdited.URL, result.URL);
                Assert.Equal(portalEdited.Parameters, result.Parameters);
                Assert.Equal(portalEdited.Status, result.Status);
                Assert.Equal(portalEdited.Email, result.Email);
                Assert.Equal(portalEdited.CheckInterval, result.CheckInterval);
                Assert.Equal(portalEdited.ResponseTimeThreshold, result.ResponseTimeThreshold);
                Assert.Equal(portalEdited.Method, result.Method);
                Assert.Equal(portalEdited.BasicAuth, result.BasicAuth);
                Assert.Equal(portalEdited.UserName, result.UserName);
                Assert.Equal(portal.PasswordHashed, result.PasswordHashed);
            }
        }

        [Fact]
        public async void Get_All_Portals()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_All")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.GetAllAsync();
                Assert.Equal(3, result.Count());
            }
        }

        [Fact]
        public async void Get_All_Active_Portals()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_All_Active")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.GetAllActiveAsync();
                Assert.Single(result);
            }
        }


        [Fact]
        public async void Get_All_Deleted()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_All_Deleted")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.GetAllDeletedAsync();
                Assert.Single(result);
            }
        }

        [Fact]
        public async void Get_All_To_Display_Portals()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_All_To_Display")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.GetAllToDisplayAsync();
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async void Clear_All_Deleted_Portals()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Clear_All_Deleted")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.ClearAllDeletedAsync();
                var resultList = await service.GetAllAsync();
                Assert.Single(result);
                Assert.Equal(2, resultList.Count());
            }
        }


        [Fact]
        public async void Get_Portal_By_Id()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Get_Portal_By_Id")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.GetPortalByIdAsync(portal2.Id);
                Assert.Equal(portal2.Id, result.Id);
                Assert.Equal(portal2.Name, result.Name);
                Assert.Equal(portal2.URL, result.URL);
            }
        }

        [Fact]
        public async void Set_As_Active_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Set_As_Active")
                .Options;

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };         

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.SetAsActiveAsync(portal.Id);
                Assert.Equal(PortalStatus.Active, result.Status);
            }
        }

        [Fact]
        public async void Set_As_NotActive_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Set_As_Not_Active")
                .Options;

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.SetAsNotActiveAsync(portal.Id);
                Assert.Equal(PortalStatus.NotActive, result.Status);
            }
        }

        [Fact]
        public async void Set_As_Deleted_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Set_As_Deleted")
                .Options;

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.SetAsDeletedAsync(portal.Id);
                Assert.Equal(PortalStatus.Deleted, result.Status);
            }
        }

        [Fact]
        public async void Invert_Status_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Invert_Status")
                .Options;

            var portal1 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.Active,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal2 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 2",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/2",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portal3 = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 3",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/3",
                Parameters = "",
                Status = PortalStatus.Deleted,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal1);
                await service.CreateNewPortalAsync(portal2);
                await service.CreateNewPortalAsync(portal3);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result1 = await service.InvertStatusAsync(portal1.Id);
                var result2 = await service.InvertStatusAsync(portal2.Id);
                var result3 = await service.InvertStatusAsync(portal3.Id);
                Assert.Equal(PortalStatus.NotActive, result1.Status);
                Assert.Equal(PortalStatus.Active, result2.Status);
                Assert.Equal(PortalStatus.Deleted, result3.Status);
            }
        }

        [Fact]
        public async void Set_Last_Notification_Sent_Portal()
        {
            var options = new DbContextOptionsBuilder<DashboardDbContext>()
                .UseInMemoryDatabase(databaseName: "Set_Last_Notification_Sent")
                .Options;

            var portal = new Portal()
            {
                Id = Guid.NewGuid(),
                Name = "Test Portal 1",
                Type = PortalType.WebApp,
                URL = "http://www.delfi.lt/1",
                Parameters = "",
                Status = PortalStatus.NotActive,
                Email = "test@test.com",
                CheckInterval = 20,
                ResponseTimeThreshold = 20,
                Method = RequestMethod.GET,
                BasicAuth = false,
                LastNotificationSent = DateTime.Now.AddDays(-30)
            };

            using (var context = new DashboardDbContext(options))
            {
                var service = new PortalCrudService(_mockLogger.Object, context);
                await service.CreateNewPortalAsync(portal);
                await context.SaveChangesAsync();
            }

            using (var context = new DashboardDbContext(options))
            {
                var newDateTime = DateTime.Now;
                var service = new PortalCrudService(_mockLogger.Object, context);
                var result = await service.SetLastNotificationSentAsync(portal.Id, newDateTime);
                Assert.Equal(result.LastNotificationSent, newDateTime);
            }
        }
    }
}

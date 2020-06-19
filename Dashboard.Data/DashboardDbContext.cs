using Dashboard.Hash;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dashboard.Data
{
    public class DashboardDbContext : DbContext
    {
        public DbSet<PortalResponse> PortalResponses { get; set; }
        public virtual DbSet<Portal> Portals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DashboardDbContext()
        {

        }
        public DashboardDbContext(DbContextOptions<DashboardDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
               .Property(p => p.Claims)
               .HasConversion(
                   c => string.Join(",", c),
                   c => c.Split(',', StringSplitOptions.RemoveEmptyEntries)
               );

            builder.Entity<User>().HasData(
                new User
                {
                    UserId = -1,
                    Email = "admin@admin.com",
                    Name = "Main",
                    Surname = "User",
                    Password = new HashService().Hash("Baklazanas123@"),
                    IsActive = true,
                    isPermanent = true,
                    Claims = new string[] { ClaimType.isAdmin.ToString() }
                }
            );
        }
    }
}

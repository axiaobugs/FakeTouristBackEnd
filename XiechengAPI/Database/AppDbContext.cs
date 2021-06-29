using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using XiechengAPI.Moldes;


namespace XiechengAPI.Database
{

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(jsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var jsonPicData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutesPic = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(jsonPicData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutesPic);



            // Initial User and role seed data
            // 1: update User and roles FK
            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles)
                    .WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            // 2: Add admin role
            var adminRoleId = "63d5b08c-aed5-48d3-a44c-3404f0e87afe";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            );

            // 3: add user
            var adminUserId = new Guid().ToString();
            ApplicationUser admiUser = new ApplicationUser()
            {
                Id = adminUserId,
                UserName = "admin@axiaobug.com",
                NormalizedEmail = "admin@axiaobug.com".ToUpper(),
                Email = "admin@axiaobug.com",
                NormalizedUserName = "admin@axiaobug.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "12345",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            admiUser.PasswordHash = ph.HashPassword(admiUser, "Fake123$");

            modelBuilder.Entity<ApplicationUser>().HasData(admiUser);

            // 4: set admin role to this user
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );


            base.OnModelCreating(modelBuilder);
        }
    }
}

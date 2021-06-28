using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using XiechengAPI.Moldes;


namespace XiechengAPI.Database
{

    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(jsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var jsonPicData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutesPic = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(jsonPicData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutesPic);
           
            base.OnModelCreating(modelBuilder);
        }
    }
}

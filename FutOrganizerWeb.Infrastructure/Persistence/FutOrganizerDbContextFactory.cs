using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace FutOrganizerWeb.Infrastructure.Persistence
{
    public class FutOrganizerDbContextFactory : IDesignTimeDbContextFactory<FutOrganizerDbContext>
    {
        public FutOrganizerDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile("FutOrganizerWeb/appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FutOrganizerDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new FutOrganizerDbContext(optionsBuilder.Options);
        }
    }
}

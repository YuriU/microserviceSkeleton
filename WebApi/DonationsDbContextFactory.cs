using System.IO;
using Data.Repository.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class DonationsDbContextFactory : IDesignTimeDbContextFactory<DonationsDbContext>
    {
        public DonationsDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .Build();


            return new DonationsDbContext(options => options.UseNpgsql(configuration["Database:ConnectionString"],
                builder => builder.MigrationsAssembly(this.GetType().Assembly.FullName)));
        }
    }
}
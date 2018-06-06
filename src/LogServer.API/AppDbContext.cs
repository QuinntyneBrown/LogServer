using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public interface IAppDbContext
    {
        DbSet<Log> Logs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Log> Logs { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(configuration["Data:DefaultConnection:ConnectionString"])
                .Options);
        }
    }
}

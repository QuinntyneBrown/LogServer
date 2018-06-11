using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
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
        private readonly IMediator _mediator;
        public AppDbContext(DbContextOptions options, IMediator mediator = default(IMediator))
            : base(options) {
            _mediator = mediator;
        }

        public DbSet<Log> Logs { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int result = default(int);
            var domainEventEntities = ChangeTracker.Entries<IEntity>()
                .Select(po => po.Entity)
                .Where(po => po.DomainEvents.Any())
                .ToArray();
            
            result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entity in domainEventEntities)
            {
                var events = entity.DomainEvents.ToArray();
                entity.ClearEvents();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent);
                }
            }

            return result;
        }
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

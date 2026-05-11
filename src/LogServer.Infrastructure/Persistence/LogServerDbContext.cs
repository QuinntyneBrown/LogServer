using LogServer.Application.Common.Interfaces;
using LogServer.Core.Logs;
using Microsoft.EntityFrameworkCore;

namespace LogServer.Infrastructure.Persistence;

public class LogServerDbContext : DbContext, ILogServerDbContext
{
    public LogServerDbContext(DbContextOptions<LogServerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogServerDbContext).Assembly);
    }
}

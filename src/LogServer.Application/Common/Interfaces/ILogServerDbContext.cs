using LogServer.Core.Logs;
using Microsoft.EntityFrameworkCore;

namespace LogServer.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the EF Core <c>DbContext</c> that command and query
/// handlers depend on. The concrete <c>LogServerDbContext</c> in the
/// Infrastructure layer implements this interface.
/// </summary>
public interface ILogServerDbContext
{
    DbSet<Log> Logs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using LogServer.Application.Common.Interfaces;
using LogServer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

/// <summary>
/// Boots the API in-process for integration tests, replacing SQL Server with
/// an EF Core InMemory provider so each test gets an isolated database.
/// </summary>
public sealed class LogServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the production DbContext registrations.
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<LogServerDbContext>) ||
                    d.ServiceType == typeof(LogServerDbContext) ||
                    d.ServiceType == typeof(ILogServerDbContext))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<LogServerDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddScoped<ILogServerDbContext>(sp =>
                sp.GetRequiredService<LogServerDbContext>());
        });
    }
}

using System.Threading;
using System.Threading.Tasks;
using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LogServer.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions options)
            :base(options) { }

        public DbSet<StoredEvent> StoredEvents { get; set; }
    }
}

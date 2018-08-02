using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace LogServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder()
                .ConfigureLogging(logging => logging.ClearProviders())
                .Build();

            ProcessDbCommands(args, host);

            host.Run();
        }
        
        public static IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

        private static void ProcessDbCommands(string[] args, IWebHost host)
        {
            if (args.Contains("ci"))
                args = new string[4] { "dropdb", "migratedb", "seeddb", "stop" };

            if (args.Contains("seeddb"))
                AppInitializer.Seed();

            if (args.Contains("stop"))
                Environment.Exit(0);
        }        
    }
}

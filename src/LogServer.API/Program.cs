using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Linq;

namespace LogServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder().Build();
            ProcessDbCommands(args, host);
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

        private static void ProcessDbCommands(string[] args, IWebHost host) {
            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            using (var scope = services.CreateScope())
            {
                if (args.Contains("ci"))
                    args = new string[3] { "dropdb", "migratedb", "stop" };

                if (args.Contains("dropdb"))
                    GetDbContext(scope).Database.EnsureDeleted();

                if (args.Contains("migratedb"))
                    GetDbContext(scope).Database.Migrate();
                
                if (args.Contains("stop"))
                    Environment.Exit(0);
            }
        }

        private static AppDbContext GetDbContext(IServiceScope services)
            => services.ServiceProvider.GetRequiredService<AppDbContext>();

        private static IConfiguration GetConfiguration(IServiceScope services)
            => services.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));
            
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new SignalRContractResolver()
            });

            services.Add(new ServiceDescriptor(typeof(JsonSerializer),
                                               provider => serializer,
                                               ServiceLifetime.Transient));
            services.AddSignalR();
            services.AddMvc();
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCreatedChangedBehavior<,>));
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "LogServer.API",
                    Version = "v1",
                    Description = "LogServer.API REST API",
                });
                options.CustomSchemaIds(x => x.FullName);
            });
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options
                .UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
            });

            services.AddMediatR(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseSignalR(routes => routes.MapHub<AppHub>("/hub"));
            app.UseSwagger();
            app.UseSwaggerUI(options
                => options.SwaggerEndpoint("/swagger/v1/swagger.json", "LogServer API"));
        }
    }
}

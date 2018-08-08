using FluentValidation.AspNetCore;
using LogServer.Core;
using LogServer.Core.Behaviours;
using LogServer.Core.Extensions;
using LogServer.Core.Interfaces;
using LogServer.Core.Middleware;
using LogServer.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;
        
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomSecurity(Configuration)
                .AddCustomSignalR()
                .AddCustomSwagger()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient<IEventStore, EventStore>()
                .AddHttpContextAccessor()
                .AddHostedService<QueuedHostedService>()
                .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>()
                .AddCustomMvc<Startup>();

            services.AddMediatR(typeof(Startup).Assembly);

        }

        public void Configure(IApplicationBuilder app)
            => app.UseCors(CorsDefaults.Policy)
                .UseMiddleware<RequestLoggerMiddleware>()
                .UseMvc()
                .UseSignalR(routes => routes.MapHub<IntegrationEventsHub>("/hub"))
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "LogServer API");
                    options.RoutePrefix = string.Empty;
                });
    }
}

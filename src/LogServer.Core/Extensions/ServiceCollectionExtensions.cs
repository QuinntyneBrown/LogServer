using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace LogServer.Core.Extensions
{
    public static class CorsDefaults
    {
        public static readonly string Policy = "CorsPolicy";
    }

    public static class ServiceCollectionExtensions
    {        
        public static IServiceCollection AddCustomMvc<T>(this IServiceCollection services)
        {
            services.AddMvcCore()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddApiExplorer()
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<T>());

            return services;
        }

        public static IServiceCollection AddCustomSignalR(this IServiceCollection services)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SignalRContractResolver()
            };

            var serializer = JsonSerializer.Create(settings);

            services.Add(new ServiceDescriptor(typeof(JsonSerializer),
                                               provider => serializer,
                                               ServiceLifetime.Transient));
            services.AddSignalR();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "LogServer",
                    Version = "v1",
                    Description = "LogServer REST API",
                });
                options.CustomSchemaIds(x => x.FullName);
            });

            services.ConfigureSwaggerGen(options =>
            {  });

            return services;
        }
        
        public static IServiceCollection AddCustomSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy(CorsDefaults.Policy,
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));
            
            return services;
        }
    }
}

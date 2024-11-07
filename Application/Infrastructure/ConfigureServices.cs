using Application.Common.Behaviours;
using Application.Common.HealthChecks;
using Application.Common.Interfaces;
using Application.Infrastructure.Repositories;
using Application.Infrastructure.Services;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Application.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            //https://weblogs.asp.net/ricardoperes/checking-the-heath-of-an-asp-net-core-application
            // Add basic health checks
            services.AddHealthChecks();

            // Add health checks for specific services like databases, cache, etc.
            services.AddHealthChecks()
                    .AddCheck("SQL Server check", new SqlHealthCheck(configuration.GetConnectionString("DefaultConnection")), tags: new[] { "db", "sql" })
                    .AddCheck("Web Check", new WebHealthCheck("https://google.com"), tags: new[] { "tcp", "http" })
                    .AddCheck("Ping Check", new PingHealthCheck("8.8.8.8"), tags: new[] { "tcp" })
                    .AddCheck<CustomHealthCheck>("customer check");
            services.AddHealthChecksUI().AddInMemoryStorage();
            return services;
        }
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                options.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                options.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            });

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<,>),typeof(GenericRepository<,>));
            // Service
            services.AddScoped<IProductService, ProductService>();

            // Repository
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
        /// <summary>
        /// The Assembly parameter should reference the assembly containing your IEndpoint implementations.
        /// If your endpoints are spread across multiple assemblies (or projects), 
        /// you can easily modify this method to accept a collection of assemblies for broader coverage
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            ServiceDescriptor[] serviceDescriptors = assembly
                                                    .DefinedTypes
                                                    .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                                                                   type.IsAssignableTo(typeof(IEndpoint)))
                                                    .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                                                    .ToArray();
            services.TryAddEnumerable(serviceDescriptors);
            return services;
        }
        /// <summary>
        /// This method will search for all registered services that implement the IEndpoint interface.
        /// These endpoint classes can then be registered with the application by invoking MapEndpoint.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="routeGroupBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
        {
            IEnumerable<IEndpoint> endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder =
                routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }

            return app;
        }

        public static IApplicationBuilder MapHealthChecks(this WebApplication app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI(setup =>
               {
                   setup.UIPath = "/health-ui";  // Endpoint for the health check UI
               });
            });
            return app;
        }
    }
}

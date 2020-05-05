using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using RegistrationService.API.Infrastructure.AutofacModules;
using RegistrationService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using RegistrationService.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using RegistrationService.API.Infrastructure.Filters;
using iPas.Infrastructure.EventBus;
using iPas.Infrastructure.EventBus.Abstractions;
using iPas.Infrastructure.EventBusServiceBus;
using iPas.Infrastructure.IntegrationEventLogEF;
using iPas.Infrastructure.IntegrationEventLogEF.Services;
using RegistrationService.API.IntegrationEvents;
using System.Data.Common;
using Microsoft.Azure.ServiceBus;
using System.Reflection;
using RegistrationService.API.Grpc;
using RegistrationService.Data.Repositories;

namespace RegistrationService.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var dbSettings = new DocumentDBSettings();
            Configuration.GetSection("DocumentDatabase").Bind(dbSettings);

            services
                .AddGrpc().Services
                .AddCustomMvc()
                .AddCustomDbContext(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddEventBus(Configuration)
                .AddDocumentRepository(Configuration);
            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);



            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration.GetConnectionString("PatientConnex"), 
                Configuration.GetValue<string>("CliendServiceEndPoint:ClientServiceURL"),
                Configuration.GetValue<string>("Module:Name")));

            return new AutofacServiceProvider(container.Build());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                
            endpoints.MapGrpcService<RegistrationService.API.Grpc.RegistrationService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
                });

            });
            
        }
    }

    static class CustomExtensionsMethods
    {

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
                // Added for functional tests
                .AddApplicationPart(typeof(RegistrationController).Assembly)
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            ;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

 

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<RegistrationContext>(options =>
                   {
                       options.UseSqlServer(configuration.GetConnectionString("PatientConnex"),
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           })
                       .EnableSensitiveDataLogging();
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PatientConnex"),
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }


        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IRegistrationIntegrationEventService, RegistrationIntegrationEventService>();

                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = configuration["EventBusConnection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });           

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });


            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddDocumentRepository(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddSingleton<IDocumentRepository, DocumentRepository>(dR =>
            {

                var logger = dR.GetRequiredService<ILogger<DocumentRepository>>();
                return new DocumentRepository(configuration.GetValue<string>("DocumentDatabase:EndpointUri"),
                    configuration.GetValue<string>("DocumentDatabase:PrimaryKey"),
                    configuration.GetValue<string>("DocumentDatabase:ApplicationName"),
                    configuration.GetValue<string>("DocumentDatabase:DatabaseName"),
                    configuration.GetValue<string>("DocumentDatabase:ContainerName"),
                    configuration.GetValue<string>("DocumentDatabase:PartitionKey"), logger);

            });
            return services;
        }


        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<RegistrationSettings>(configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }
    }
}

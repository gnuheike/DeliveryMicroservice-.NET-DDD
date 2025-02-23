using System.Reflection;
using DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;
using DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Commands.AssignCouriers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.UpdateCourierLocations;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Primitives;

namespace DeliveryApp.Api;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Health Checks
        services.AddHealthChecks();

        // Cors
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy => { policy.AllowAnyOrigin(); });
        });

        // Configuration
        services.ConfigureOptions<SettingsSetup>();

        // Domain Services
        services.AddSingleton<ICourierScoringService, CourierScoringService>();

        // Database
        var connectionString = configuration["CONNECTION_STRING"];
        services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });

                // Since it's a study application, we can enable sensitive data logging.
                options.EnableSensitiveDataLogging();
            }
        );
        services.AddSingleton(_ => new NpgsqlConnection(connectionString));


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICourierRepository, PostgresCourierRepository>();
        services.AddScoped<IOrderRepository, PostgresOrderRepository>();


        // MediatR
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        );

        // Commands
        services.AddTransient<
            IRequestHandler<CreateOrderCommand, bool>,
            CreateOrderCommandHandler
        >();
        services.AddTransient<
            IRequestHandler<UpdateCourierLocationsCommand, bool>,
            UpdateCourierLocationsCommandHandler
        >();
        services.AddTransient<
            IRequestHandler<AssignCouriersCommand, bool>,
            AssignCouriersCommandHandler
        >();

        // Queries
        services.AddTransient<
            IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>,
            GetBusyCouriersQueryHandler
        >();
        services.AddTransient<
            IRequestHandler<GetAllNonCompletedOrdersQuery, GetAllNonCompletedOrdersResponse>,
            PostgresGetAllNonCompletedOrdersQueryHandler
        >();
    }
}
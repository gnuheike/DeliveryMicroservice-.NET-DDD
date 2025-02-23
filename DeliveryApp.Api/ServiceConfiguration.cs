using System.Reflection;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;
using DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Primitives;
using Quartz;

namespace DeliveryApp.Api;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        Settings(services);
        HealthChecks(services);
        Cors(services);

        MediatR(services);
        Quartz(services);

        Handlers(services);
        DomainServices(services);
        Database(services, configuration);
        Repositories(services);
        Handlers(services);

        Swagger(services);
    }

    private static void Swagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("1.0.0", new OpenApiInfo
            {
                Title = "Delivery Service",
                Version = "v1"
            });
        });
        services.AddMvcCore();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGenNewtonsoftSupport();
    }

    private static void Quartz(IServiceCollection services)
    {
        // CRON Jobs
        services.AddQuartz(configure =>
        {
            var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
            var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
            configure
                .AddJob<AssignOrdersJob>(assignOrdersJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(assignOrdersJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(1)
                                .RepeatForever()))
                .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(moveCouriersJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(2)
                                .RepeatForever()));
        });
        services.AddQuartzHostedService();
    }

    private static void HealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    private static void Cors(IServiceCollection services)
    {
        services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin(); }); });
    }

    private static void Settings(IServiceCollection services)
    {
        services.ConfigureOptions<SettingsSetup>();
    }

    private static void DomainServices(IServiceCollection services)
    {
        services.AddSingleton<ICourierScoringService, CourierScoringService>();
    }

    private static void Database(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["CONNECTION_STRING"];
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
            options.EnableSensitiveDataLogging();
        });
        services.AddSingleton(_ => new NpgsqlConnection(connectionString));
    }

    private static void Repositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICourierRepository, PostgresCourierRepository>();
        services.AddScoped<IOrderRepository, PostgresOrderRepository>();
    }

    private static void MediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    private static void Handlers(IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<CreateOrderCommand, bool>, CreateOrderCommandHandler>();
        services
            .AddTransient<IRequestHandler<MoveCouriersCommand, bool>, MoveCouriersCommandHandler>();
        services.AddTransient<IRequestHandler<AssignOrdersCommand, bool>, AssignOrdersCommandHandler>();
        services
            .AddTransient<IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>,
                GetBusyCouriersQueryHandler>();
        services
            .AddTransient<IRequestHandler<GetAllNonCompletedOrdersQuery, GetAllNonCompletedOrdersResponse>,
                PostgresGetAllNonCompletedOrdersQueryHandler>();
    }
}
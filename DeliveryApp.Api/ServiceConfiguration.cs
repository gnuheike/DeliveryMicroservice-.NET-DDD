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
using Npgsql;
using Primitives;
using Quartz;

namespace DeliveryApp.Api;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddHealthChecks(services);
        ConfigureCors(services);
        ConfigureSettings(services);
        RegisterDomainServices(services);
        ConfigureDatabase(services, configuration);
        RegisterRepositories(services);
        ConfigureMediatR(services);
        RegisterHandlers(services);
        RegisterQuartz(services);
    }

    private static void RegisterQuartz(IServiceCollection services)
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
            configure.UseMicrosoftDependencyInjectionJobFactory();
        });
        services.AddQuartzHostedService();
    }

    private static void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    private static void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin(); }); });
    }

    private static void ConfigureSettings(IServiceCollection services)
    {
        services.ConfigureOptions<SettingsSetup>();
    }

    private static void RegisterDomainServices(IServiceCollection services)
    {
        services.AddSingleton<ICourierScoringService, CourierScoringService>();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
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

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICourierRepository, PostgresCourierRepository>();
        services.AddScoped<IOrderRepository, PostgresOrderRepository>();
    }

    private static void ConfigureMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    private static void RegisterHandlers(IServiceCollection services)
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
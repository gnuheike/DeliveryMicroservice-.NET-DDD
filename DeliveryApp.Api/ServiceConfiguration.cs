using System.Reflection;
using Api.Filters;
using Api.OpenApi;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Api.Adapters.Http.Mapper;
using DeliveryApp.Api.Adapters.Kafka;
using DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;
using DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.DomainEventHandlers;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Grps.GeoService;
using DeliveryApp.Infrastructure.Adapters.Kafka.OrderCompleted;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.BackgroundJobs;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        DomainEvents(services);
        Database(services, configuration);
        Repositories(services);
        HttpMappers(services);
        Kafka(services);

        Swagger(services);
    }

    private static void DomainEvents(IServiceCollection services)
    {
        services.AddTransient<OutboxDomainEventsSaver>();
        services.AddTransient<INotificationHandler<OrderCompletedDomainEvent>, OrderCompletedDomainEventHandler>();
    }

    private static void Kafka(IServiceCollection services)
    {
        services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });

        services.AddHostedService(BasketConfirmedConsumerHostedServiceFactory.Create);

        services.AddTransient<IOrderCompletedMessageBusProducer, KafkaOrderCompletedMessageBusProducer>(
            KafkaOrderCompletedMessageBusProducerFactory.Create
        );
    }

    private static void HttpMappers(IServiceCollection services)
    {
        services.AddTransient<OrderMapper>();
        services.AddTransient<CouriersMapper>();
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
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments(
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
            options.DocumentFilter<BasePathFilter>("");
            options.OperationFilter<GeneratePathParamsValidationFilter>();
        });
        services.AddMvcCore();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddControllers();
    }

    private static void Quartz(IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
            configure
                .AddJob<AssignOrdersJob>(assignOrdersJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(assignOrdersJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(1)
                                .RepeatForever()));
        });
        services.AddQuartz(configure =>
        {
            var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
            configure
                .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(moveCouriersJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(2)
                                .RepeatForever()));
        });
        services.AddQuartz(configure =>
        {
            var outboxMessagesProcessorJobKey = new JobKey(nameof(OutboxMessagesProcessorBackgroundJob));
            configure
                .AddJob<OutboxMessagesProcessorBackgroundJob>(outboxMessagesProcessorJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(outboxMessagesProcessorJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(3)
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

        services.AddSingleton(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<Settings>>();
            return GeoClientFactory.Create(settings);
        });
        services.AddSingleton<ILocationProvider, GrpsLocationProvider>();
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
        services.AddTransient(_ => new NpgsqlConnection(connectionString));
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
        services.AddTransient<IRequestHandler<MoveCouriersCommand, bool>, MoveCouriersCommandHandler>();
        services.AddTransient<IRequestHandler<AssignOrdersCommand, bool>, AssignOrdersCommandHandler>();
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
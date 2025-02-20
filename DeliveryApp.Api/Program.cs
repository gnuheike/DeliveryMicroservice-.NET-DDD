using System.Reflection;
using DeliveryApp.Api;
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
using Primitives;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.AllowAnyOrigin(); });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();

// Domain Services
builder.Services.AddSingleton<ICourierScoringService, CourierScoringService>();

// Database
var connectionString = builder.Configuration["CONNECTION_STRING"];
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString,
            sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });

        // Since it's a study application, we can enable sensitive data logging.
        options.EnableSensitiveDataLogging();
    }
);


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICourierRepository, PostgresCourierRepository>();
builder.Services.AddScoped<IOrderRepository, PostgresOrderRepository>();


// MediatR
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
);

// Commands
builder.Services.AddTransient<
    IRequestHandler<CreateOrderCommand, bool>,
    CreateOrderCommandHandler
>();
builder.Services.AddTransient<
    IRequestHandler<UpdateCourierLocationsCommand, bool>,
    UpdateCourierLocationsCommandHandler
>();
builder.Services.AddTransient<
    IRequestHandler<AssignCouriersCommand, bool>,
    AssignCouriersCommandHandler
>();

// Queries
builder.Services.AddTransient<
    IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>,
    GetBusyCouriersQueryHandler
>();
builder.Services.AddTransient<
    IRequestHandler<GetAllNonCompletedOrdersQuery, GetAllNonCompletedOrdersResponse>,
    PostgresGetAllNonCompletedOrdersQueryHandler
>();

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


app.Run();
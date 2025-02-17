using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepositoryShould : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("delivery")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private readonly Location _location = Location.MinimalLocation;
    private readonly Transport _transport = Transport.Bicycle;

    private ApplicationDbContext _context;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
            _container.GetConnectionString(),
            sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }
        ).Options;
        _context = new ApplicationDbContext(contextOptions);

        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CanAddCourier()
    {
        // Arrange
        var courier = Courier.Create("name", _transport, _location).Value;
        var courierRepository = new PostgresCourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        // Act
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var addedCourier = await courierRepository.GetAsync(courier.Id);
        Assert.Equal(courier, addedCourier);
    }

    [Fact]
    public async Task CanUpdateCourier()
    {
        // Arrange
        var courier = Courier.Create("name", _transport, _location).Value;
        var courierRepository = new PostgresCourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        // Act
        courier.SetBusy();
        await courierRepository.UpdateAsync(courier);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var updatedCourier = await courierRepository.GetAsync(courier.Id);
        Assert.Equal(courier, updatedCourier);
    }

    [Fact]
    public async Task CanGetAllFreeAsync()
    {
        // Arrange
        var courier1 = Courier.Create("name1", _transport, _location).Value;
        var courier2 = Courier.Create("name2", _transport, _location).Value;
        courier2.SetBusy();

        var courierRepository = new PostgresCourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        // Act
        await courierRepository.AddAsync(courier1);
        await courierRepository.AddAsync(courier2);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var freeCouriers = await courierRepository.GetAllFreeAsync();
        Assert.Contains(courier1, freeCouriers);
        Assert.DoesNotContain(courier2, freeCouriers);
    }
}
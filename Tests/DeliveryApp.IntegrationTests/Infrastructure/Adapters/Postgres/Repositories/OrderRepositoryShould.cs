using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure.Adapters.Postgres.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("delivery")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private readonly IMediator _mediator = Substitute.For<IMediator>();

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
    public async Task AddOrder()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        var orderRepository = new PostgresOrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context, _mediator);

        // Act
        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var addedOrder = await orderRepository.GetAsync(order.Id);
        Assert.Equal(order, addedOrder);
    }


    [Fact]
    public async Task UpdateOrder()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        var orderRepository = new PostgresOrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context, _mediator);

        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        // Act
        order.Complete();
        await orderRepository.UpdateAsync(order);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var updatedOrder = await orderRepository.GetAsync(order.Id);
        Assert.Equal(order, updatedOrder);
    }


    private Courier CreateCourier()
    {
        var order1Courier = Courier.Create(
            "name",
            Transport.Bicycle,
            Location.MaximumLocation()
        );
        if (order1Courier.IsFailure) throw new Exception();

        return order1Courier.Value;
    }

    [Fact]
    public async Task GetAllAssignedOrders()
    {
        // Arrange
        var order1 = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        order1.Assign(CreateCourier());

        var order2 = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        order2.Assign(CreateCourier());

        var orderRepository = new PostgresOrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context, _mediator);

        // Act
        await orderRepository.AddAsync(order1);
        await orderRepository.AddAsync(order2);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var assignedOrders = await orderRepository.GetAllAssignedAsync();
        Assert.Contains(order1, assignedOrders);
        Assert.Contains(order2, assignedOrders);
    }

    [Fact]
    public async Task GetAllCreatedOrders()
    {
        // Arrange
        var order1 = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        var order2 = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        var order3 = Order.Create(Guid.NewGuid(), Location.MinimumLocation()).Value;
        order3.Assign(CreateCourier());


        var orderRepository = new PostgresOrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context, _mediator);

        // Act
        await orderRepository.AddAsync(order1);
        await orderRepository.AddAsync(order2);
        await orderRepository.AddAsync(order3);
        await unitOfWork.SaveChangesAsync();

        //perform raw query just to check what's in the table
        var orders = await _context.Orders.ToListAsync();
        foreach (var order in orders) Console.WriteLine(order);

        // Assert
        var createdOrders = await orderRepository.GetAllCreatedAsync();
        Assert.Contains(order1, createdOrders);
        Assert.Contains(order2, createdOrders);
        Assert.DoesNotContain(order3, createdOrders);
    }
}
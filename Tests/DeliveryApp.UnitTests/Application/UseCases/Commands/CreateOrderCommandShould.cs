using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.SharedKernel;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.Commands;

public class CreateOrderCommandShould
{
    private readonly ILocationProvider _locationProvider = Substitute.For<ILocationProvider>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CreateOrder()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(), "street");

        // Act
        _unitOfWork.SaveChangesAsync().Returns(true);
        _locationProvider.Execute(command.Street, CancellationToken.None).Returns(Location.Create(1, 1));

        var handler = new CreateOrderCommandHandler(_orderRepository, _locationProvider, _unitOfWork);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        await _orderRepository.Received().AddAsync(Arg.Any<Order>());
    }
}
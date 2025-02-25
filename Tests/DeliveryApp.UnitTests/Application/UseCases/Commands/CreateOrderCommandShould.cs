using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.Commands;

public class CreateOrderCommandShould
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CreateOrder()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(), "street");

        // Act
        _unitOfWork.SaveChangesAsync().Returns(true);
        var handler = new CreateOrderCommandHandler(_orderRepository, _unitOfWork);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        await _orderRepository.Received().AddAsync(Arg.Any<Order>());
    }
}
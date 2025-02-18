using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate.VO;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.OrderAggregate;

public class OrderShould
{
    public static IEnumerable<object[]> GetIncorrectOrderParams()
    {
        yield return [Guid.Empty, Location.Create(1, 1).Value];
        yield return [Guid.NewGuid(), null];
    }

    private static Location CreateLocation(int x, int y)
    {
        return Location.Create(x, y).Value;
    }

    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var location = CreateLocation(5, 5);

        //Act
        var result = Order.Create(orderId, location);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Location.Should().Be(location);
    }

    [Theory]
    [MemberData(nameof(GetIncorrectOrderParams))]
    public void ReturnValueIsRequiredErrorWhenOrderIdIsEmpty(Guid orderId, Location location)
    {
        //Arrange

        var result = Order.Create(orderId, location);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void CanAssignToCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), CreateLocation(5, 5)).Value;
        var courier = Courier.Create("Ваня", Transport.Pedestrian, CreateLocation(1, 1)).Value;

        //Act
        var result = order.Assign(courier);

        //Assert
        result.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }

    [Fact]
    public void CanComplete()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), CreateLocation(5, 5)).Value;
        var courier = Courier.Create("Ваня", Transport.Pedestrian, CreateLocation(1, 1)).Value;
        order.Assign(courier);

        //Act
        var result = order.Complete();

        //Assert
        result.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Completed);
    }
}
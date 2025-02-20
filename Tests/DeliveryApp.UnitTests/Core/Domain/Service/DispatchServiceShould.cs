using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Service;

public class DispatchServiceShould
{
    [Fact]
    public void DispatchOrderToClosestCourier()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimalLocation).Value;
        var closestCourier = Courier.Create("Ivan", Transport.Pedestrian, Location.MinimalLocation).Value;
        var fartherCourier = Courier.Create("Oleg", Transport.Pedestrian, Location.MaximumLocation).Value;

        var dispatchService = new CourierScoringService();

        // Act
        var result = dispatchService.Execute(order, [closestCourier, fartherCourier]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(closestCourier.Id);
    }


    [Fact]
    public void NotDispatchOrderToBusyCourier()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimalLocation).Value;
        var busyCourier = Courier.Create("Ivan", Transport.Pedestrian, Location.MinimalLocation).Value;
        busyCourier.SetBusy();
        var freeCourier = Courier.Create("Oleg", Transport.Pedestrian, Location.MaximumLocation).Value;

        var dispatchService = new CourierScoringService();

        // Act
        var result = dispatchService.Execute(order, [busyCourier, freeCourier]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(freeCourier.Id);
    }

    [Fact]
    public void ReturnError_WhenNoCourierFound()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimalLocation).Value;
        var busyCourier = Courier.Create("Ivan", Transport.Pedestrian, Location.MinimalLocation).Value;
        busyCourier.SetBusy();

        var dispatchService = new CourierScoringService();

        // Act
        var result = dispatchService.Execute(order, [busyCourier]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierScoringService.Errors.NoCourierFound());
    }

    [Fact]
    public void ReturnError_WhenEmptyCourierList()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinimalLocation).Value;
        var dispatchService = new CourierScoringService();

        // Act
        var result = dispatchService.Execute(order, []);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierScoringService.Errors.AtLeastOneCourierIsRequired());
    }

    [Fact]
    public void ReturnError_WhenOrderIsNull()
    {
        // Arrange
        var dispatchService = new CourierScoringService();

        // Act
        var result = dispatchService.Execute(null, []);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierScoringService.Errors.OrderIsRequired());
    }
}
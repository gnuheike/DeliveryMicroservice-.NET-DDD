using System;
using System.Reflection;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.CourierAggregate;

public class TransportShould
{
    [Fact]
    public void HaveCorrectProperties()
    {
        var pedestrian = Transport.Pedestrian;
        var bicycle = Transport.Bicycle;
        var car = Transport.Car;

        pedestrian.Id.Should().Be(1);
        pedestrian.Name.Should().Be("pedestrian");
        pedestrian.Speed.Should().Be(1);

        bicycle.Id.Should().Be(2);
        bicycle.Name.Should().Be("bicycle");
        bicycle.Speed.Should().Be(2);

        car.Id.Should().Be(3);
        car.Name.Should().Be("car");
        car.Speed.Should().Be(3);
    }

    [Fact]
    public void ReturnCorrectTransport_WhenGetByIdIsCalledWithValidId()
    {
        var expectedPedestrian = Transport.Pedestrian;
        var expectedBicycle = Transport.Bicycle;
        var expectedCar = Transport.Car;

        var resultPedestrian = Transport.GetById(1);
        var resultBicycle = Transport.GetById(2);
        var resultCar = Transport.GetById(3);

        resultPedestrian.IsSuccess.Should().BeTrue();
        resultPedestrian.Value.Should().Be(expectedPedestrian);

        resultBicycle.IsSuccess.Should().BeTrue();
        resultBicycle.Value.Should().Be(expectedBicycle);

        resultCar.IsSuccess.Should().BeTrue();
        resultCar.Value.Should().Be(expectedCar);
    }

    [Fact]
    public void ReturnCorrectTransport_WhenGetByNameIsCalledWithValidName()
    {
        var expectedPedestrian = Transport.Pedestrian;
        var expectedBicycle = Transport.Bicycle;
        var expectedCar = Transport.Car;

        var resultPedestrian = Transport.GetByName("pedestrian");
        var resultBicycle = Transport.GetByName("bicycle");
        var resultCar = Transport.GetByName("car");

        resultPedestrian.IsSuccess.Should().BeTrue();
        resultPedestrian.Value.Should().Be(expectedPedestrian);

        resultBicycle.IsSuccess.Should().BeTrue();
        resultBicycle.Value.Should().Be(expectedBicycle);

        resultCar.IsSuccess.Should().BeTrue();
        resultCar.Value.Should().Be(expectedCar);
    }

    [Fact]
    public void BeEqual_WhenTwoTransportsHaveTheSameId()
    {
        var pedestrian1 = Transport.Pedestrian;
        var pedestrian2 = CreateTransport(1, "pedestrian", 1);

        pedestrian1.Should().Be(pedestrian2);
    }

    private static Transport CreateTransport(int id, string name, int speed)
    {
        var constructor = typeof(Transport)
            .GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                [typeof(int), typeof(string), typeof(int)],
                null);

        if (constructor == null) throw new InvalidOperationException("Private constructor not found.");

        return (Transport)constructor.Invoke([id, name, speed]);
    }
}
using System;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.SharedKernel;

public class LocationShould
{
    [Fact]
    public void CreateWithValidValues()
    {
        var location = new Location(5, 5);
        location.Width.Should().Be(5);
        location.Height.Should().Be(5);
    }

    [Fact]
    public void ThrowExceptionForInvalidWidth()
    {
        Action act1 = () => new Location(0, 5);
        Action act2 = () => new Location(11, 5);
        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowExceptionForInvalidHeight()
    {
        Action act1 = () => new Location(5, 0);
        Action act2 = () => new Location(5, 11);
        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateRandomLocationWithinValidRange()
    {
        var location = Location.CreateRandom();
        location.Width.Should().BeInRange(1, 10);
        location.Height.Should().BeInRange(1, 10);
    }

    [Fact]
    public void ReturnNewLocationWithUpdatedWidth()
    {
        var location = new Location(5, 5);
        var newLocation = location.SetWidth(7);
        newLocation.Width.Should().Be(7);
        newLocation.Height.Should().Be(5);
    }

    [Fact]
    public void ReturnNewLocationWithUpdatedHeight()
    {
        var location = new Location(5, 5);
        var newLocation = location.SetHeight(7);
        newLocation.Width.Should().Be(5);
        newLocation.Height.Should().Be(7);
    }

    [Fact]
    public void CalculateCorrectDistanceToAnotherLocation()
    {
        var location1 = new Location(2, 3);
        var location2 = new Location(5, 6);
        location1.GetDistanceTo(location2).Should().Be(6);
    }

    [Fact]
    public void BeEqualWhenCoordinatesAreSame()
    {
        var location1 = new Location(5, 5);
        var location2 = new Location(5, 5);
        location1.Should().Be(location2);
    }

    [Fact]
    public void NotBeEqualWhenCoordinatesDiffer()
    {
        var location1 = new Location(5, 5);
        var location2 = new Location(6, 5);
        location1.Should().NotBe(location2);
    }

    [Fact]
    public void ShouldBeImmutable()
    {
        var location = new Location(5, 5);

        var newLocation1 = location.SetWidth(7);
        var newLocation2 = location.SetHeight(7);

        newLocation1.Should().NotBeSameAs(location);
        newLocation2.Should().NotBeSameAs(location);
    }
}
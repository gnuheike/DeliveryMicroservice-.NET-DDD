using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.SharedKernel;

public class LocationShould
{
    [Fact]
    public void CreateWithValidValues()
    {
        var location = Location.Create(5, 5).Value;
        location.X.Should().Be(5);
        location.Y.Should().Be(5);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(11, 5)]
    [InlineData(5, 0)]
    [InlineData(5, 11)]
    public void NotCreateWithInvalidValues(int x, int y)
    {
        var result = Location.Create(x, y);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReturnNewLocationWithUpdatedX()
    {
        var location = Location.Create(5, 5).Value;
        var newLocation = location.SetX(7).Value;
        newLocation.X.Should().Be(7);
        newLocation.Y.Should().Be(5);
    }

    [Fact]
    public void ReturnNewLocationWithUpdatedY()
    {
        var location = Location.Create(5, 5).Value;
        var newLocation = location.SetY(7).Value;
        newLocation.X.Should().Be(5);
        newLocation.Y.Should().Be(7);
    }

    [Fact]
    public void CalculateCorrectDistanceToAnotherLocation()
    {
        var location1 = Location.Create(2, 3).Value;
        var location2 = Location.Create(5, 6).Value;
        location1.GetDistanceTo(location2).Should().Be(6);
    }

    [Fact]
    public void BeEqualWhenCoordinatesAreSame()
    {
        var location1 = Location.Create(5, 5).Value;
        var location2 = Location.Create(5, 5).Value;
        location1.Should().Be(location2);
    }

    [Fact]
    public void NotBeEqualWhenCoordinatesDiffer()
    {
        var location1 = Location.Create(5, 5).Value;
        var location2 = Location.Create(6, 5).Value;
        location1.Should().NotBe(location2);
    }

    [Fact]
    public void ShouldBeImmutable()
    {
        var location = Location.Create(5, 5).Value;

        var newLocation1 = location.SetX(7);
        var newLocation2 = location.SetY(7);

        newLocation1.Should().NotBeSameAs(location);
        newLocation2.Should().NotBeSameAs(location);
    }

    [Fact]
    public void ReturnSameInstanceWhenMovingToSameLocation()
    {
        var location = Location.Create(5, 5).Value;
        var targetLocation = Location.Create(5, 5).Value;

        var result = location.MoveTo(targetLocation).Value;

        result.Should().BeSameAs(location);
    }

    [Fact]
    public void MoveHorizontallyWhenHorizontalDistanceIsGreater()
    {
        var location = Location.Create(3, 5).Value;
        var targetLocation = Location.Create(5, 5).Value;
        var expectedLocation = Location.Create(4, 5).Value;

        var result = location.MoveTo(targetLocation).Value;

        result.Should().BeEquivalentTo(expectedLocation);
    }

    [Fact]
    public void MoveVerticallyWhenVerticalDistanceIsGreater()
    {
        var location = Location.Create(5, 3).Value;
        var targetLocation = Location.Create(5, 5).Value;
        var expectedLocation = Location.Create(5, 4).Value;

        var result = location.MoveTo(targetLocation).Value;

        result.Should().BeEquivalentTo(expectedLocation);
    }

    [Fact]
    public void PrioritizeHorizontalMoveWhenDistancesAreEqual()
    {
        var location = Location.Create(3, 5).Value;
        var targetLocation = Location.Create(5, 5).Value;
        var expectedLocation = Location.Create(4, 5).Value;

        var result = location.MoveTo(targetLocation).Value;

        result.Should().BeEquivalentTo(expectedLocation);
    }
}
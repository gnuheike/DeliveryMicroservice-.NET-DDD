using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.CourierAggregate;

public class CourierShould
{
    private static Location CreateLocation()
    {
        return Location.MinimalLocation;
    }

    [Fact]
    public void SetStatusToBusy_WhenFree()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;

        var result = courier.SetBusy();

        Assert.True(result.IsSuccess);
        Assert.Equal(CourierStatus.Busy, courier.Status);
    }

    [Fact]
    public void ReturnError_WhenSettingBusy_AndNotFree()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;
        courier.SetBusy();

        var result = courier.SetBusy();

        Assert.False(result.IsSuccess);
        Assert.Equal("courier.is.not.free", result.Error.Code);
    }

    [Fact]
    public void SetStatusToFree_WhenBusy()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;
        courier.SetBusy();

        var result = courier.SetFree();

        Assert.True(result.IsSuccess);
        Assert.Equal(CourierStatus.Free, courier.Status);
    }

    [Fact]
    public void ReturnError_WhenSettingFree_AndNotBusy()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;

        var result = courier.SetFree();

        Assert.False(result.IsSuccess);
        Assert.Equal("courier.is.not.busy", result.Error.Code);
    }

    [Fact]
    public void CalculateCorrectStepsToTargetLocation()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;
        var targetLocation = Location.Create(5, 5).Value;

        var steps = courier.GetDistanceTo(targetLocation);

        Assert.Equal(4, steps); // 8 cells distance / 2 speed = 4 steps
    }

    [Fact]
    public void UpdateLocation_WhenMovedToTarget()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;
        var targetLocation = Location.Create(5, 5).Value;

        courier.MoveTo(targetLocation);

        Assert.Equal(Location.Create(2, 2), courier.Location);
    }

    [Fact]
    public void MoveCorrectly_WhenTargetIsWithinStepRange()
    {
        var courier = Courier.Create("John Doe", Transport.Car, CreateLocation()).Value;
        var targetLocation = Location.Create(2, 1).Value;

        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }

    [Fact]
    public void MoveCorrectly_4Steps()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, CreateLocation()).Value;
        var targetLocation = Location.Create(5, 5).Value;

        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }

    [Fact]
    public void MoveCorrectly_4StepsBackwards()
    {
        var courier = Courier.Create("John Doe", Transport.Bicycle, Location.MaximumLocation).Value;
        var targetLocation = Location.Create(6, 6).Value;

        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }
}
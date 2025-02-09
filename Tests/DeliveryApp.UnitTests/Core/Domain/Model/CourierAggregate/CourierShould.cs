using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.CourierAggregate;

public class CourierShould
{
    [Fact]
    public void SetStatusToBusy_WhenFree()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));

        var result = courier.SetBusy();

        Assert.True(result.IsSuccess);
        Assert.Equal(CourierStatus.Busy, courier.Status);
    }

    [Fact]
    public void ReturnError_WhenSettingBusy_AndNotFree()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));
        courier.SetBusy();

        var result = courier.SetBusy();

        Assert.False(result.IsSuccess);
        Assert.Equal("courier.is.not.free", result.Error.Code);
    }

    [Fact]
    public void SetStatusToFree_WhenBusy()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));
        courier.SetBusy();

        var result = courier.SetFree();

        Assert.True(result.IsSuccess);
        Assert.Equal(CourierStatus.Free, courier.Status);
    }

    [Fact]
    public void ReturnError_WhenSettingFree_AndNotBusy()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));

        var result = courier.SetFree();

        Assert.False(result.IsSuccess);
        Assert.Equal("courier.is.not.busy", result.Error.Code);
    }

    [Fact]
    public void CalculateCorrectStepsToTargetLocation()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));
        var targetLocation = new Location(5, 5);

        var steps = courier.GetDistanceTo(targetLocation);

        Assert.Equal(4, steps); // 8 cells distance / 2 speed = 4 steps
    }

    [Fact]
    public void UpdateLocation_WhenMovedToTarget()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));
        var targetLocation = new Location(5, 5);

        courier.MoveTo(targetLocation);

        Assert.Equal(new Location(2, 2), courier.Location);
    }

    [Fact]
    public void MoveCorrectly_WhenTargetIsWithinStepRange()
    {
        var courier = new Courier("John Doe", Transport.Car, new Location(1, 1));
        var targetLocation = new Location(2, 1);

        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }

    [Fact]
    public void MoveCorrectly_4Steps()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(1, 1));
        var targetLocation = new Location(5, 5);

        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }

    [Fact]
    public void MoveCorrectly_4StepsBackwards()
    {
        var courier = new Courier("John Doe", Transport.Bicycle, new Location(10, 10));
        var targetLocation = new Location(6, 6);

        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);
        courier.MoveTo(targetLocation);

        Assert.Equal(targetLocation, courier.Location);
    }
}
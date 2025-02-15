using DeliveryApp.Core.Domain.Models.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.CourierAggregate;

public class CourierStatusShould
{
    [Fact]
    public void ShouldHaveCorrectNameForFreeStatus()
    {
        var status = CourierStatus.Free;
        status.Name.Should().Be("free");
    }

    [Fact]
    public void ShouldHaveCorrectNameForBusyStatus()
    {
        var status = CourierStatus.Busy;
        status.Name.Should().Be("busy");
    }

    [Fact]
    public void FreeAndBusyStatusesShouldBeDifferent()
    {
        CourierStatus.Free.Should().NotBe(CourierStatus.Busy);
    }
}
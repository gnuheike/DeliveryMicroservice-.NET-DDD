using DeliveryApp.Core.Domain.Model.OrderAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.OrderAggregate;

public class OrderStatusShould
{
    [Fact]
    public void ShouldHaveCorrectNameForCreatedStatus()
    {
        var status = OrderStatus.Created;
        status.Name.Should().Be("created");
    }

    [Fact]
    public void ShouldHaveCorrectNameForAssignedStatus()
    {
        var status = OrderStatus.Assigned;
        status.Name.Should().Be("assigned");
    }

    [Fact]
    public void ShouldHaveCorrectNameForCompletedStatus()
    {
        var status = OrderStatus.Completed;
        status.Name.Should().Be("completed");
    }

    [Fact]
    public void CreatedAndAssignedStatusesShouldBeDifferent()
    {
        OrderStatus.Created.Should().NotBe(OrderStatus.Assigned);
    }

    [Fact]
    public void AssignedAndCompletedStatusesShouldBeDifferent()
    {
        OrderStatus.Assigned.Should().NotBe(OrderStatus.Completed);
    }
}
using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

public class OrderStatus : ValueObject
{
    [ExcludeFromCodeCoverage]
    private OrderStatus()
    {
    }

    private OrderStatus(string name)
    {
        Name = name;
    }

    public string Name { get; }

    // We have to keep the reference to the courier for Entity Framework
    public Guid OrderId { get; private set; }

    public static OrderStatus Created()
    {
        return new OrderStatus(nameof(Created).ToLowerInvariant());
    }

    public static OrderStatus Assigned()
    {
        return new OrderStatus(nameof(Assigned).ToLowerInvariant());
    }

    public static OrderStatus Completed()
    {
        return new OrderStatus(nameof(Completed).ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
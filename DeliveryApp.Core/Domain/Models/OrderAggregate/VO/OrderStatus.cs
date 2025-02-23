using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate.VO;

public class OrderStatus : ValueObject
{
    public static readonly OrderStatus Created = new(nameof(Created).ToLowerInvariant());
    public static readonly OrderStatus Assigned = new(nameof(Assigned).ToLowerInvariant());
    public static readonly OrderStatus Completed = new(nameof(Completed).ToLowerInvariant());

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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
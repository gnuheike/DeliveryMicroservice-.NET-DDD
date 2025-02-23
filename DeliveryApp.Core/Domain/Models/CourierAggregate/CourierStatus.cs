using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public class CourierStatus : ValueObject
{
    [ExcludeFromCodeCoverage]
    private CourierStatus()
    {
    }

    private CourierStatus(string name) : this()
    {
        Name = name;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public string Name { get; private set; }
    public Guid CourierId { get; private set; }

    public static CourierStatus Free()
    {
        return new CourierStatus(nameof(Free).ToLowerInvariant());
    }

    public static CourierStatus Busy()
    {
        return new CourierStatus(nameof(Busy).ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

/**
 * This class represents a type of transport, such as a car, bicycle, or pedestrian.
 * Transport is used to move a courier from one location to another in the learning app.
 */
public class Transport : Entity<int>
{
    public static Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1);
    public static Transport Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(), 2);
    public static Transport Car = new(3, nameof(Car).ToLowerInvariant(), 3);
    private static readonly Transport[] All;

    static Transport()
    {
        All = [Pedestrian, Bicycle, Car];
    }

    [ExcludeFromCodeCoverage]
    private Transport()
    {
    }

    private Transport(int id, string name, int speed) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(speed);

        Id = id;
        Name = name;
        Speed = speed;
    }

    public string Name { get; }

    /**
     * Default speed in cells per tick.
     * For example, a car can move 3 cells per tick.
     */
    public int Speed { get; }

    public static Result<Transport, Error> GetById(int id)
    {
        var transport = All.FirstOrDefault(t => t.Id == id);
        return transport == null
            ? Result.Failure<Transport, Error>(Errors.TransportNotFound(id))
            : Result.Success<Transport, Error>(transport);
    }

    public static Result<Transport, Error> GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Transport, Error>(Errors.TransportNotFound(name));

        var lowerName = name.ToLowerInvariant();
        var transport = All.FirstOrDefault(t => t.Name == lowerName);
        return transport == null
            ? Result.Failure<Transport, Error>(Errors.TransportNotFound(lowerName))
            : Result.Success<Transport, Error>(transport);
    }

    public override string ToString()
    {
        return $"{Name} ({Speed})";
    }

    public static IEnumerable<Transport> List()
    {
        yield return Pedestrian;
        yield return Bicycle;
        yield return Car;
    }

    private static class Errors
    {
        public static Error TransportNotFound(int id)
        {
            return new Error(
                $"{nameof(Transport)}.NotFound",
                $"Transport with ID '{id}' does not exist."
            );
        }

        public static Error TransportNotFound(string name)
        {
            return new Error(
                $"{nameof(Transport)}.NotFound",
                $"Transport with name '{name}' does not exist."
            );
        }
    }
}
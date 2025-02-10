using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel;

public class Location : ValueObject
{
    public static readonly Location MinimalLocation = new(1, 1);
    public static readonly Location MaximumLocation = new(10, 10);

    [ExcludeFromCodeCoverage]
    private Location()
    {
    }

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }


    public int X { get; }
    public int Y { get; }

    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < MinimalLocation.X || x > MaximumLocation.X) return GeneralErrors.ValueIsInvalid(nameof(x));
        if (y < MinimalLocation.Y || y > MaximumLocation.Y) return GeneralErrors.ValueIsInvalid(nameof(y));

        return new Location(x, y);
    }

    public static Location CreateRandom()
    {
        var random = new Random();
        return new Location(
            random.Next(MinimalLocation.X, MaximumLocation.X + 1),
            random.Next(MinimalLocation.Y, MaximumLocation.Y + 1)
        );
    }

    public Result<Location, Error> SetX(int newWidth)
    {
        return Create(newWidth, Y);
    }

    public Result<Location, Error> SetY(int newHeight)
    {
        return Create(X, newHeight);
    }

    public int GetDistanceTo(Location location)
    {
        if (location == null) return 0;
        if (Equals(location)) return 0;

        return Math.Abs(X - location.X) + Math.Abs(Y - location.Y);
    }

    public Result<Location, Error> MoveTo(Location targetLocation)
    {
        if (Equals(targetLocation)) return this;

        var distanceX = targetLocation.X - X;
        var distanceY = targetLocation.Y - Y;

        if (Math.Abs(distanceX) >= Math.Abs(distanceY)) return Create(X + Math.Sign(distanceX), Y);
        if (distanceY != 0) return Create(X, Y + Math.Sign(distanceY));


        return Create(X + Math.Sign(distanceX), Y);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}
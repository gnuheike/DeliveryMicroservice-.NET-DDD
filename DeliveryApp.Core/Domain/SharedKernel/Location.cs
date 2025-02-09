using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel;

public class Location(int width, int height) : ValueObject
{
    private const int MinimumWidth = 1;
    private const int MaximumWidth = 10;
    private const int MinimumHeight = 1;
    private const int MaximumHeight = 10;

    public int Width { get; } = width switch
    {
        < MinimumWidth => throw new ArgumentException("Width cannot be negative"),
        > MaximumWidth => throw new ArgumentException("Width cannot be greater than 10"),
        _ => width
    };

    public int Height { get; } = height switch
    {
        < MinimumHeight => throw new ArgumentException("Height cannot be negative"),
        > MaximumHeight => throw new ArgumentException("Height cannot be greater than 10"),
        _ => height
    };

    public static Location CreateRandom()
    {
        var random = new Random();
        return new Location(
            random.Next(MinimumWidth, MaximumWidth + 1),
            random.Next(MinimumHeight, MaximumHeight + 1)
        );
    }

    public Location SetWidth(int newWidth)
    {
        return new Location(newWidth, Height);
    }

    public Location SetHeight(int newHeight)
    {
        return new Location(Width, newHeight);
    }

    public int GetDistanceTo(Location location)
    {
        return Math.Abs(Width - location.Width) + Math.Abs(Height - location.Height);
    }

    public Location MoveTo(Location targetLocation)
    {
        if (this == targetLocation) return this;

        var distanceX = targetLocation.Width - Width;
        var distanceY = targetLocation.Height - Height;

        if (Math.Abs(distanceX) >= Math.Abs(distanceY)) return new Location(Width + Math.Sign(distanceX), Height);

        if (distanceY != 0) return new Location(Width, Height + Math.Sign(distanceY));

        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
    }

    public static class Errors
    {
        public static Error CannotMoveWithSpeedBelowOne()
        {
            return new Error(
                $"{nameof(Location).ToLowerInvariant()}.cannot.move.with.speed.below.one",
                "Cannot move with speed below 1"
            );
        }
    }
}
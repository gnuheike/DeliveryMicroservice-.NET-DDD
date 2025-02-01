using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

public class Location(int width, int height) : ValueObject
{
    public int Width { get; private set; } = width switch
    {
        < 1 => throw new ArgumentException("Width cannot be negative"),
        > 10 => throw new ArgumentException("Width cannot be greater than 10"),
        _ => width
    };

    public int Height { get; private set; } = height switch
    {
        < 1 => throw new ArgumentException("Height cannot be negative"),
        > 10 => throw new ArgumentException("Height cannot be greater than 10"),
        _ => height
    };

    public static Location CreateRandom()
    {
        var random = new Random();
        return new Location(random.Next(1, 11), random.Next(1, 11));
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
    }
}
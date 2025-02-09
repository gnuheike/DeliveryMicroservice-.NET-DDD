using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    private Courier()
    {
    }

    public Courier(string name, Transport transport, Location location)
    {
        Id = Guid.NewGuid();
        Name = name;
        Transport = transport;
        Location = location;
        Status = CourierStatus.Free;
    }

    public string Name { get; }
    public Transport Transport { get; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }

    public UnitResult<Error> SetBusy()
    {
        if (!Status.Equals(CourierStatus.Free)) return Errors.CourierIsNotFree();

        Status = CourierStatus.Busy;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetFree()
    {
        if (!Status.Equals(CourierStatus.Busy)) return Errors.CourierIsNotBusy();

        Status = CourierStatus.Free;
        return UnitResult.Success<Error>();
    }

    public int GetDistanceTo(Location location)
    {
        return Location.GetDistanceTo(location) / Transport.Speed;
    }

    public void MoveTo(Location location)
    {
        var currentLocation = Location;
        var remainingSteps = Transport.Speed;

        while (remainingSteps > 0)
        {
            var locationAfterMove = currentLocation.MoveTo(location);
            Console.WriteLine($"width: ${locationAfterMove.Width}, height: ${locationAfterMove.Height}");
            if (locationAfterMove == currentLocation)
            {
                currentLocation = locationAfterMove;
                break;
            }

            currentLocation = locationAfterMove;
            remainingSteps--;
        }

        Location = currentLocation;
    }

    public static class Errors
    {
        public static Error CourierIsNotFree()
        {
            return new Error(
                $"{nameof(Courier).ToLowerInvariant()}.is.not.free",
                "Courier is not free"
            );
        }

        public static UnitResult<Error> CourierIsNotBusy()
        {
            return new Error(
                $"{nameof(Courier).ToLowerInvariant()}.is.not.busy",
                "Courier is not busy"
            );
        }
    }
}
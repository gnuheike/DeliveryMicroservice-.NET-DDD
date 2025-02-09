using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    private Courier(Guid id, string name, Transport transport, Location location, CourierStatus status)
    {
        Id = id;
        Name = name;
        Transport = transport;
        Location = location;
        Status = status;
    }

    public string Name { get; }
    public Transport Transport { get; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }

    public static Result<Courier, Error> Create(string name, Transport transport, Location location)
    {
        if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

        return new Courier(
            Guid.NewGuid(),
            name,
            transport,
            location,
            CourierStatus.Free
        );
    }

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

    /**
     * Moves the courier towards the target location.
     * The courier moves in the direction of the target location, one step at a time.
     * If the target location is not reachable in one step, the courier moves as far as possible towards it.
     * The courier moves horizontally or vertically, not diagonally.
     * The courier can move up to N cells per step, where N is the speed of the transport.
     */
    public void MoveTo(Location location)
    {
        var currentLocation = Location;
        // In this test project, the steps is the same as the speed
        var availableSteps = Transport.Speed;

        while (availableSteps > 0)
        {
            var locationAfterMove = currentLocation.MoveTo(location);
            if (locationAfterMove == currentLocation) break;

            currentLocation = locationAfterMove;
            availableSteps--;
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

        public static Error CourierIsNotBusy()
        {
            return new Error(
                $"{nameof(Courier).ToLowerInvariant()}.is.not.busy",
                "Courier is not busy"
            );
        }
    }
}
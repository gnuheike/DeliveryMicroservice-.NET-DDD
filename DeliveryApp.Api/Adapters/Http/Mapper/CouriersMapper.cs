using Api.Models;
using DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;

namespace DeliveryApp.Api.Adapters.Http.Mapper;

public class CouriersMapper
{
    public Courier Execute(CourierDto courier)
    {
        return new Courier
        {
            Id = courier.Id,
            Name = courier.Name,
            Location = MapLocation(courier.LocationDto)
        };
    }

    private Location MapLocation(LocationDto location)
    {
        return new Location
        {
            X = location.X,
            Y = location.Y
        };
    }
}
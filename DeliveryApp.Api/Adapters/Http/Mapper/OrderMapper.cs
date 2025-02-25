using Api.Models;
using DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;

namespace DeliveryApp.Api.Adapters.Http.Mapper;

public class OrderMapper
{
    public Order Execute(OrderDto order)
    {
        return new Order
        {
            Location = MapLocation(order.LocationDto),
            Id = order.Id
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
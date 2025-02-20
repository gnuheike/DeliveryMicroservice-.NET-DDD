using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;

public class GetBusyCouriersResponse(List<CourierDto> couriers)
{
    public List<CourierDto> Couriers { get; private set; } = couriers;
}

public class CourierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LocationDto LocationDto { get; set; }
    public int TransportId { get; set; }

    public static CourierDto FromDomain(Courier courier)
    {
        return new CourierDto
        {
            Id = courier.Id,
            Name = courier.Name,
            LocationDto = LocationDto.FromDomain(courier.Location),
            TransportId = courier.Transport.Id
        };
    }
}

public class LocationDto
{
    public int X { get; set; }
    public int Y { get; set; }

    public static LocationDto FromDomain(Location location)
    {
        return new LocationDto { X = location.X, Y = location.Y };
    }
}
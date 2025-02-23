namespace DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;

public class GetAllNonCompletedOrdersResponse(List<OrderDto> orders)
{
    public List<OrderDto> Orders { get; } = orders;
}

public class OrderDto
{
    public Guid Id { get; set; }
    public LocationDto LocationDto { get; set; }
}

public class LocationDto
{
    public int X { get; set; }
    public int Y { get; set; }
}
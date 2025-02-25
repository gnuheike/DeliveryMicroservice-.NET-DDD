using DeliveryApp.Core.Domain.Ports;
using MediatR;

namespace DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;

public class GetBusyCouriersQueryHandler(
    ICourierRepository courierRepository
) : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
{
    public async Task<GetBusyCouriersResponse> Handle(
        GetBusyCouriersQuery request,
        CancellationToken cancellationToken
    )
    {
        var couriers = await courierRepository.GetAllBusyAsync();
        var courierDtos = couriers.Select(CourierDto.FromDomain).ToList();

        return new GetBusyCouriersResponse(courierDtos);
    }
}
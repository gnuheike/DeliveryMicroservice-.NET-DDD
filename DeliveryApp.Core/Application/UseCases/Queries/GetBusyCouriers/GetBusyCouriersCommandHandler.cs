using DeliveryApp.Core.Domain.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public class GetBusyCouriersCommandHandler(
    ICourierRepository courierRepository
) : IRequestHandler<GetBusyCouriersCommand, GetBusyCouriersResponse>
{
    public async Task<GetBusyCouriersResponse> Handle(
        GetBusyCouriersCommand request,
        CancellationToken cancellationToken
    )
    {
        var couriers = await courierRepository.GetAllBusyAsync();
        var courierDtos = couriers.Select(CourierDto.FromDomain).ToList();

        return new GetBusyCouriersResponse(courierDtos);
    }
}
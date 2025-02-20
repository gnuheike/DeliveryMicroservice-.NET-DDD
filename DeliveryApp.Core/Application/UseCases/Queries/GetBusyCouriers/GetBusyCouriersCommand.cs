using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public class GetBusyCouriersCommand : IRequest<GetBusyCouriersResponse>;
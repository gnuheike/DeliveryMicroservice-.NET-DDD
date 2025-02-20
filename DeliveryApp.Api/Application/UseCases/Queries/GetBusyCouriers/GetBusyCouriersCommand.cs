using MediatR;

namespace DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;

public class GetBusyCouriersCommand : IRequest<GetBusyCouriersResponse>;
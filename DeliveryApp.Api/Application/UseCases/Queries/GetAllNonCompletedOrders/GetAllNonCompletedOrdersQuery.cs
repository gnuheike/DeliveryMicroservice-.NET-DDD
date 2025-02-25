using MediatR;

namespace DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;

public class GetAllNonCompletedOrdersQuery : IRequest<GetAllNonCompletedOrdersResponse>;
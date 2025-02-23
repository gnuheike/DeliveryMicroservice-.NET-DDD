using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;

public class AssignOrdersCommand : IRequest<bool>;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCouriers;

public class AssignCouriersCommand : IRequest<bool>;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.Services;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;

public class AssignOrdersCommandHandler(
    IOrderRepository orderRepository,
    ICourierRepository courierRepository,
    ICourierScoringService courierScoringService,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignOrdersCommand, bool>
{
    public async Task<bool> Handle(AssignOrdersCommand request, CancellationToken cancellationToken)
    {
        var createdOrders = await orderRepository.GetAllCreatedAsync();
        var freeCouriers = await courierRepository.GetAllFreeAsync();

        if (createdOrders.Count == 0 || freeCouriers.Count == 0)
            return false;

        foreach (var order in createdOrders)
        {
            var (_, isFailure, closestCourier) = courierScoringService.FindClosestAvailableCourier(order, freeCouriers);
            if (isFailure) continue;

            order.Assign(closestCourier);
            closestCourier.SetBusy();
            freeCouriers.Remove(closestCourier);

            await orderRepository.UpdateAsync(order);
            await courierRepository.UpdateAsync(closestCourier);
        }

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
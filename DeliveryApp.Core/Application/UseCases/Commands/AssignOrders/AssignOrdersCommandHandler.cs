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
            var bestCourier = courierScoringService.FindClosestAvailableCourier(order, freeCouriers);

            if (bestCourier.IsFailure)
            {
                if (bestCourier.Error == CourierScoringService.Errors.NoCourierFound()) return false;
                throw new Exception(bestCourier.Error.Message);
            }

            order.Assign(bestCourier.Value);
            freeCouriers.Remove(bestCourier.Value);

            await orderRepository.UpdateAsync(order);
            await courierRepository.UpdateAsync(bestCourier.Value);
        }

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
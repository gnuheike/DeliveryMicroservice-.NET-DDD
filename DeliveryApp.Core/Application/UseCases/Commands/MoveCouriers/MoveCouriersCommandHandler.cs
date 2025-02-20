using DeliveryApp.Core.Domain.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public class MoveCouriersCommandHandler(
    ICourierRepository courierRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<MoveCouriersCommand, bool>
{
    public async Task<bool> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        var assignedOrders = await orderRepository.GetAllAssignedAsync();
        var busyCouriers = await courierRepository.GetAllBusyAsync();

        foreach (var order in assignedOrders)
        {
            var courier = busyCouriers.Find(x => x.Id == order.CourierId);
            if (courier == null)
                throw new Exception("Courier not found");

            courier.MoveTo(order.Location);

            if (courier.GetDistanceTo(order.Location) == 0)
            {
                order.Complete();
                courier.SetFree();
                await orderRepository.UpdateAsync(order);
            }

            await courierRepository.UpdateAsync(courier);
        }

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
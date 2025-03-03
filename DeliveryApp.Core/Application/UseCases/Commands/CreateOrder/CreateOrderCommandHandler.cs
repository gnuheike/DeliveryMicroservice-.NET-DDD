using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ILocationProvider locationProvider,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var location = await locationProvider.Execute(request.Street, cancellationToken);
        if (location.IsFailure) return false;

        var (_, isFailure, order) = Order.Create(Guid.NewGuid(), location.Value);
        if (isFailure)
            return false;

        await orderRepository.AddAsync(order);

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
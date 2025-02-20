using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.SharedKernel;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var location = Location.CreateRandom();
        var (_, isFailure, order) = Order.Create(Guid.NewGuid(), location);
        if (isFailure)
            return false;

        await orderRepository.AddAsync(order);

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
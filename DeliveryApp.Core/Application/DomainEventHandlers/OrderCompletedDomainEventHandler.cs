using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Services;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public class OrderCompletedDomainEventHandler(IOrderCompletedMessageBusProducer orderCompletedMessageBus)
    : INotificationHandler<OrderCompletedDomainEvent>
{
    public Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        return orderCompletedMessageBus.ProduceAsync(notification, cancellationToken);
    }
}
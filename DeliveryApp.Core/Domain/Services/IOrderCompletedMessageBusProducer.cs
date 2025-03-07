using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Domain.Services;

public interface IOrderCompletedMessageBusProducer
{
    Task ProduceAsync(OrderCompletedDomainEvent orderCompletedDomainEvent, CancellationToken cancellationToken);
}
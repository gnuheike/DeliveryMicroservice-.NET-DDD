using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;

public sealed record OrderCompletedDomainEvent(Order order) : DomainEvent;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

public class Order : Aggregate<Guid>
{
    private Order()
    {
    }

    private Order(Guid orderId, Location location)
    {
        Id = orderId;
        Location = location;
        Status = OrderStatus.Created();
    }

    private Order(Guid orderId, Location location, Guid? courierId, OrderStatus status) : this(orderId, location)
    {
        CourierId = courierId;
        Status = status;
    }

    public Guid? CourierId { get; private set; }
    public Location Location { get; private set; }
    public OrderStatus Status { get; private set; }

    public static Result<Order, Error> Create(Guid orderId, Location location)
    {
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
        return new Order(orderId, location);
    }

    public UnitResult<Error> Assign(Courier courier)
    {
        if (courier == null) return Errors.CourierIsRequired();
        if (Status == OrderStatus.Completed()) return Errors.CantAssignCompletedOrder();
        if (Status == OrderStatus.Assigned()) return Errors.OrderAlreadyAssigned();
        if (courier.Status == CourierStatus.Busy()) return Errors.CantAssignOrderToBusyCourier(courier.Id);


        CourierId = courier.Id;
        Status = OrderStatus.Assigned();
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned()) return Errors.CantCompletedNotAssignedOrder();

        Status = OrderStatus.Completed();
        RaiseDomainEvent(new OrderCompletedDomainEvent(this));
        return UnitResult.Success<Error>();
    }

    public static class Errors
    {
        public static Error OrderAlreadyAssigned()
        {
            return new Error(
                $"{nameof(Order).ToLowerInvariant()}.already.assigned",
                "Order already assigned"
            );
        }

        public static Error CantAssignCompletedOrder()
        {
            return new Error(
                $"{nameof(Order).ToLowerInvariant()}.cannot.assign.completed.order",
                "Cannot assign a completed order"
            );
        }

        public static Error CantCompletedNotAssignedOrder()
        {
            return new Error(
                $"{nameof(Order).ToLowerInvariant()}.cannot.complete.unassigned.order",
                "Cannot complete an unassigned order"
            );
        }

        public static Error CantAssignOrderToBusyCourier(Guid courierId)
        {
            return new Error(
                $"{nameof(Order).ToLowerInvariant()}.cant.assign.order.to.busy.courier",
                $"Cannot assign order to busy courier with id {courierId}"
            );
        }

        public static Error CourierIsRequired()
        {
            return new Error(
                $"{nameof(Order).ToLowerInvariant()}.courier.is.required",
                "Courier is required"
            );
        }
    }
}
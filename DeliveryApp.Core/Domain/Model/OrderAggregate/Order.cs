using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    private Order()
    {
    }

    private Order(Guid orderId, Location location) : this()
    {
        Id = orderId;
        Location = location;
        Status = OrderStatus.Created;
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
        if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));

        CourierId = courier.Id;
        Status = OrderStatus.Assigned;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned) return Errors.CantCompletedNotAssignedOrder();

        Status = OrderStatus.Completed;
        return UnitResult.Success<Error>();
    }

    public static class Errors
    {
        public static Error CantCompletedNotAssignedOrder()
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.completed.not.assigned.order",
                "Нельзя завершить заказ, который не был назначен");
        }

        public static Error CantAssignOrderToBusyCourier(Guid courierId)
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.assign.order.to.busy.courier",
                $"Нельзя назначить заказ на курьера, который занят. Id курьера = {courierId}");
        }
    }
}
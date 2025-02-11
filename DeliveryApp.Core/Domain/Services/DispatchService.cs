using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
    {
        if (order == null) return Errors.OrderIsRequired();
        if (couriers == null || couriers.Count == 0) return Errors.AtLeastOneCourierIsRequired();

        var closestCourier = couriers
            .Where(courier => courier.Status == CourierStatus.Free)
            .OrderBy(courier => courier.GetDistanceTo(order.Location))
            .FirstOrDefault();

        if (closestCourier == null) return Errors.NoCourierFound();

        return closestCourier;
    }

    public static class Errors
    {
        public static Error AtLeastOneCourierIsRequired()
        {
            return new Error(
                $"{nameof(DispatchService).ToLowerInvariant()}.at.least.one.courier.is.required",
                "At least one courier is required"
            );
        }

        public static Error OrderIsRequired()
        {
            return new Error(
                $"{nameof(DispatchService).ToLowerInvariant()}.order.is.required",
                "Order is required"
            );
        }

        public static Error NoCourierFound()
        {
            return new Error(
                $"{nameof(DispatchService).ToLowerInvariant()}.no.courier.found",
                "No courier found"
            );
        }
    }
}
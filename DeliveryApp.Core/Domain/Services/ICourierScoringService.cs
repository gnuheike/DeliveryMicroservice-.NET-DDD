using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public interface ICourierScoringService
{
    public Result<Courier, Error> FindClosestAvailableCourier(Order order, List<Courier> couriers);
}
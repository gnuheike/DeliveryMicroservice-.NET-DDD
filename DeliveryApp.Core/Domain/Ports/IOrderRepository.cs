using DeliveryApp.Core.Domain.Models.OrderAggregate;

namespace DeliveryApp.Core.Domain.Ports;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order> GetAsync(Guid orderId);
    Task<List<Order>> GetAllCreatedAsync();
    Task<List<Order>> GetAllAssignedAsync();
}
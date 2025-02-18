using DeliveryApp.Core.Domain.Models.CourierAggregate;

namespace DeliveryApp.Core.Domain.Ports;

public interface ICourierRepository
{
    Task AddAsync(Courier courier);
    Task UpdateAsync(Courier courier);
    Task<Courier> GetAsync(Guid id);
    Task<List<Courier>> GetAllFreeAsync();
}
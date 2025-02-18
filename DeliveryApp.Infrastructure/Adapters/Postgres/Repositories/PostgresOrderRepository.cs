using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate.VO;
using DeliveryApp.Core.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class PostgresOrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <remarks>
    ///     The changes are saved in the UnitOfWork.
    /// </remarks>
    /// <seealso cref="UnitOfWork" />
    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    /// <remarks>
    ///     The changes are saved in the UnitOfWork.
    /// </remarks>
    /// <seealso cref="UnitOfWork" />
    public Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task<Order> GetAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<List<Order>> GetAllCreatedAsync()
    {
        return await _dbContext.Orders
            .Include(x => x.Status)
            .Where(x => x.Status == OrderStatus.Created)
            .ToListAsync();
    }

    public async Task<List<Order>> GetAllAssignedAsync()
    {
        return await _dbContext.Orders
            .Include(x => x.Status)
            .Where(x => x.Status == OrderStatus.Assigned)
            .ToListAsync();
    }
}
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class PostgresCourierRepository(ApplicationDbContext dbContext) : ICourierRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <remarks>
    ///     The changes are saved in the UnitOfWork.
    /// </remarks>
    /// <seealso cref="UnitOfWork" />
    public async Task AddAsync(Courier courier)
    {
        if (courier.Transport != null) _dbContext.Attach(courier.Transport);
        await _dbContext.Couriers.AddAsync(courier);
    }

    /// <remarks>
    ///     The changes are saved in the UnitOfWork.
    /// </remarks>
    /// <seealso cref="UnitOfWork" />
    public Task UpdateAsync(Courier courier)
    {
        if (courier.Transport != null) _dbContext.Attach(courier.Transport);
        _dbContext.Couriers.Update(courier);

        return Task.CompletedTask;
    }

    public async Task<Courier> GetAsync(Guid id)
    {
        return await _dbContext
            .Couriers
            .Include(x => x.Transport)
            .SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Courier>> GetAllFreeAsync()
    {
        return await _dbContext
            .Couriers
            .Include(x => x.Transport)
            .Where(x => x.Status == CourierStatus.Free)
            .ToListAsync();
    }
}
using DeliveryApp.Infrastructure.Adapters.Postgres.Outbox;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public sealed class UnitOfWork(
    ApplicationDbContext dbContext,
    IOutboxDomainEventsSaver outboxDomainEventsSaver
) : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await outboxDomainEventsSaver.Execute(cancellationToken);
        var affectedRecords = await _dbContext.SaveChangesAsync(cancellationToken);

        return affectedRecords != 0;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _dbContext.Dispose();
        _disposed = true;
    }
}
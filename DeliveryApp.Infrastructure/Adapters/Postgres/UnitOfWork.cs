using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public sealed class UnitOfWork(ApplicationDbContext dbContext, IMediator mediator) : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var affectedRecords = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affectedRecords == 0) return false;

        await PublishDomainEventsAsync();
        return true;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents) await mediator.Publish(domainEvent);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _dbContext.Dispose();
        _disposed = true;
    }
}
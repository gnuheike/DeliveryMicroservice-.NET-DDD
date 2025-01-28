using CSharpFunctionalExtensions;

namespace Primitives;

public abstract class Aggregate<TId> : Entity<TId>, IAggregateRoot where TId : IComparable<TId>
{
    private readonly List<DomainEvent> _domainEvents = new();

    protected Aggregate(TId id) : base(id)
    {
    }

    protected Aggregate()
    {
    }

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

public interface IAggregateRoot
{
    public IReadOnlyCollection<DomainEvent> GetDomainEvents();

    public void ClearDomainEvents();
}
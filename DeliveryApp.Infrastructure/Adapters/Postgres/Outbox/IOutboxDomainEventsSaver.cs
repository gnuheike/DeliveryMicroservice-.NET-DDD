namespace DeliveryApp.Infrastructure.Adapters.Postgres.Outbox;

public interface IOutboxDomainEventsSaver
{
    public Task Execute(CancellationToken cancellationToken);
}
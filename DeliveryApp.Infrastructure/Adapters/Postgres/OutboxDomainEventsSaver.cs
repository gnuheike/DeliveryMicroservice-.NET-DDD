using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class OutboxDomainEventsSaver(ApplicationDbContext dbContext)
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new() { TypeNameHandling = TypeNameHandling.All };

    public async Task Execute(CancellationToken cancellationToken)
    {
        var outboxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.GetDomainEvents();
                aggregate.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutBoxMessage
            {
                Id = domainEvent.EventId,
                Type = domainEvent.GetType().Name,

                CreatedAtUtc = DateTime.UtcNow,
                Content = JsonConvert.SerializeObject(domainEvent, _jsonSerializerSettings)
            }).ToList();

        await dbContext.OutBoxMessages.AddRangeAsync(outboxMessages, cancellationToken);
    }
}
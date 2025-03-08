using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Primitives;
using Quartz;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.BackgroundJobs;

public class OutboxMessagesProcessorBackgroundJob(
    ApplicationDbContext dbContext,
    IMediator mediator
) : IJob
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await GetOutboxMessages();
        foreach (var message in messages) await ProcessOutboxMessage(message);

        await dbContext.SaveChangesAsync();
    }

    private async Task ProcessOutboxMessage(OutBoxMessage message)
    {
        var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(message.Content, _jsonSerializerSettings);
        await mediator.Publish(domainEvent);

        message.ProcessedAtUtc = DateTime.UtcNow;
    }

    private async Task<List<OutBoxMessage>> GetOutboxMessages()
    {
        return await dbContext.OutBoxMessages
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(10)
            .ToListAsync();
    }
}
using BasketConfirmed;
using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using MediatR;
using Newtonsoft.Json;

namespace DeliveryApp.Api.Adapters.Kafka;

public class BasketConfirmedConsumerHostedService(
    IServiceProvider serviceProvider,
    IConsumer<Ignore, string> consumer,
    string topic
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        consumer.Subscribe(topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

                var consumeResult = consumer.Consume(cancellationToken);
                if (consumeResult.IsPartitionEOF) continue;

                Console.WriteLine(
                    $"Received message at {consumeResult.TopicPartitionOffset}\n " +
                    $"Key:{consumeResult.Message.Key}\n " +
                    $"Value:{consumeResult.Message.Value}"
                );
                var basketConfirmedIntegrationEvent = JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(
                    consumeResult.Message.Value
                );

                await CreateOrder(
                    basketConfirmedIntegrationEvent.BasketId,
                    basketConfirmedIntegrationEvent.Address.Street,
                    cancellationToken
                );
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }

    private async Task CreateOrder(string basketId, string street, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var orderCreateCommand = new CreateOrderCommand(
                Guid.Parse(basketId),
                street
            );

            var response = await mediator.Send(orderCreateCommand, cancellationToken);
            if (response) Console.WriteLine("Order created");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
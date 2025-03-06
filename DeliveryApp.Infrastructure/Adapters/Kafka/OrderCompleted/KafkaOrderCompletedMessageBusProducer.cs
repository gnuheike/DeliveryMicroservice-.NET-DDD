using Confluent.Kafka;
using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderStatusChanged;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderCompleted;

public class KafkaOrderCompletedMessageBusProducer(
    IServiceProvider serviceProvider,
    IProducer<string, string> producer,
    string topicName
) : IOrderCompletedMessageBusProducer
{
    public async Task ProduceAsync(OrderCompletedDomainEvent orderCompletedDomainEvent,
        CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent
        {
            OrderId = orderCompletedDomainEvent.order.Id.ToString(),
            OrderStatus = OrderStatus.Completed
        };

        var message = new Message<string, string>
        {
            Value = JsonConvert.SerializeObject(orderStatusChangedIntegrationEvent)
        };

        try
        {
            using var scope = serviceProvider.CreateScope();
            var response = await producer.ProduceAsync(topicName, message, cancellationToken);
            Console.WriteLine(
                $"Message produced to topic {topicName} with {response.Value} at {response.TopicPartitionOffset}"
            );
        }
        catch (ProduceException<Ignore, string> e)
        {
            Console.WriteLine($"Failed to produce message: {e.Message} - {e.Error.Reason}");
        }
    }
}
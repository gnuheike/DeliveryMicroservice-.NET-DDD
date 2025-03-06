using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderCompleted;

public static class KafkaOrderCompletedMessageBusProducerFactory
{
    public static KafkaOrderCompletedMessageBusProducer Create(
        IServiceProvider serviceProvider
    )
    {
        var settings = serviceProvider.GetRequiredService<IOptions<Settings>>();

        ArgumentNullException.ThrowIfNull(settings.Value.MessageBrokerHost);
        ArgumentNullException.ThrowIfNull(settings.Value.OrderStatusChangedTopic);

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost
        };

        var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        return new KafkaOrderCompletedMessageBusProducer(
            serviceProvider,
            producer,
            settings.Value.OrderStatusChangedTopic
        );
    }
}
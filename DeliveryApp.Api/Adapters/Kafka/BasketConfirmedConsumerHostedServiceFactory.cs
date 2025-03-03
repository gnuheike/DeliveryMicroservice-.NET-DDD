using Confluent.Kafka;
using DeliveryApp.Infrastructure;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Api.Adapters.Kafka;

public static class BasketConfirmedConsumerHostedServiceFactory
{
    public static BasketConfirmedConsumerHostedService Create(
        IServiceProvider serviceProvider
    )
    {
        var settings = serviceProvider.GetRequiredService<IOptions<Settings>>();

        ArgumentNullException.ThrowIfNull(settings.Value.MessageBrokerHost);
        ArgumentNullException.ThrowIfNull(settings.Value.BasketConfirmedTopic);

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost,
            GroupId = "DeliveryConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };

        var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        return new BasketConfirmedConsumerHostedService(
            serviceProvider,
            consumer,
            settings.Value.BasketConfirmedTopic
        );
    }
}
using DeliveryApp.Infrastructure;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Api;

public class SettingsSetup(IConfiguration configuration) : IConfigureOptions<Settings>
{
    public void Configure(Settings options)
    {
        options.ConnectionString = configuration["CONNECTION_STRING"];
        options.GeoServiceGrpcHost = configuration["GEO_SERVICE_GRPC_HOST"];
        options.MessageBrokerHost = configuration["MESSAGE_BROKER_HOST"];
        options.OrderStatusChangedTopic = configuration["ORDER_STATUS_CHANGED_TOPIC"];
        options.BasketConfirmedTopic = configuration["BASKET_CONFIRMED_TOPIC"];
    }
}
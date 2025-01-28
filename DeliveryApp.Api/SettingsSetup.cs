using DeliveryApp.Infrastructure;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Api;

public class SettingsSetup : IConfigureOptions<Settings>
{
    private readonly IConfiguration _configuration;

    public SettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(Settings options)
    {
        options.ConnectionString = _configuration["CONNECTION_STRING"];
        options.GeoServiceGrpcHost = _configuration["GEO_SERVICE_GRPC_HOST"];
        options.MessageBrokerHost = _configuration["MESSAGE_BROKER_HOST"];
        options.OrderStatusChangedTopic = _configuration["ORDER_STATUS_CHANGED_TOPIC"];
        options.BasketConfirmedTopic = _configuration["BASKET_CONFIRMED_TOPIC"];
    }
}
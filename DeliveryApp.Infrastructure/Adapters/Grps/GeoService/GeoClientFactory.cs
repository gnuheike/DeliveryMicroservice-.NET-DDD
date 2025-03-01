using GeoApp.Api;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Infrastructure.Adapters.Grps.GeoService;

public static class GeoClientFactory
{
    private static GrpcChannel _channel;


    public static Geo.GeoClient Create(IOptions<Settings> options)
    {
        var url = options.Value.GeoServiceGrpcHost;
        ArgumentNullException.ThrowIfNull(url);

        var socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        var methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };


        _channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler,
            ServiceConfig = new ServiceConfig { MethodConfigs = { methodConfig } }
        });


        return new Geo.GeoClient(_channel);
    }
}
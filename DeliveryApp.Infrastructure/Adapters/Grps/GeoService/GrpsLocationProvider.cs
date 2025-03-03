using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Ports;
using GeoApp.Api;
using Grpc.Core;
using Primitives;
using Location = DeliveryApp.Core.Domain.SharedKernel.Location;

namespace DeliveryApp.Infrastructure.Adapters.Grps.GeoService;

public class GrpsLocationProvider(Geo.GeoClient geoClient) : ILocationProvider
{
    public async Task<Result<Location, Error>> Execute(string street, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(geoClient);

        var request = new GetGeolocationRequest { Street = street };

        try
        {
            var response = await geoClient.GetGeolocationAsync(
                request,
                null,
                DateTime.UtcNow.AddSeconds(5),
                cancellationToken
            );

            if (response.Location == null) return LocationProviderErrors.NotFound(street);
            return Location.Create(response.Location.X, response.Location.Y);
        }
        catch (RpcException e)
        {
            return LocationProviderErrors.ClientError(e.Message);
        }
    }
}
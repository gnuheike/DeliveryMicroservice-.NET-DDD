using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Ports;

public interface ILocationProvider
{
    Task<Result<Location, Error>> Execute(string street, CancellationToken cancellationToken);
}

public static class LocationProviderErrors
{
    public static Error NotFound(string street)
    {
        return new Error(
            $"{nameof(LocationProviderErrors).ToLowerInvariant()}.not.found",
            $"Location for street '{street}' not found"
        );
    }

    public static Error ClientError(string message)
    {
        return new Error(
            $"{nameof(LocationProviderErrors).ToLowerInvariant()}.client.error",
            message
        );
    }
}
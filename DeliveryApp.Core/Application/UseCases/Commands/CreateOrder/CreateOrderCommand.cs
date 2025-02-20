using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

/**
 * This is sample class for DDD course, no authentication or authorization implemented
 */
public class CreateOrderCommand : IRequest<bool>
{
    public CreateOrderCommand(Guid basketId, string street)
    {
        if (basketId == Guid.Empty)
            throw new ArgumentException("Basket ID cannot be empty", nameof(basketId));

        if (string.IsNullOrEmpty(street))
            throw new ArgumentNullException(nameof(street));

        BasketId = basketId;
        Street = street;
    }

    public Guid BasketId { get; private set; }
    public string Street { get; private set; }
}
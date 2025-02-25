using Api.Controllers;
using DeliveryApp.Api.Adapters.Http.Mapper;
using DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;
using DeliveryApp.Api.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController(
    IMediator mediator,
    OrderMapper orderMapper,
    CouriersMapper couriersMapper
) : DefaultApiController
{
    public override async Task<IActionResult> CreateOrder()
    {
        var createOrderCommand = new CreateOrderCommand(Guid.NewGuid(), "street");
        var response = await mediator.Send(createOrderCommand);

        if (response) return Ok();

        return Conflict();
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var query = new GetBusyCouriersQuery();
        var response = await mediator.Send(query);

        if (response is null) return NotFound();

        var models = response.Couriers.Select(couriersMapper.Execute);

        return Ok(models);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var query = new GetAllNonCompletedOrdersQuery();
        var response = await mediator.Send(query);

        if (response is null) return NotFound();

        var models = response.Orders.Select(orderMapper.Execute);

        return Ok(models);
    }
}
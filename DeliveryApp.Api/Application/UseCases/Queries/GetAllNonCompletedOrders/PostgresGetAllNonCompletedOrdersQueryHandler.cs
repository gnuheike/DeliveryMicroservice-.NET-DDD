using System.Data;
using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate.VO;
using MediatR;
using Npgsql;

namespace DeliveryApp.Api.Application.UseCases.Queries.GetAllNonCompletedOrders;

public class PostgresGetAllNonCompletedOrdersQueryHandler(
    NpgsqlConnection connection
) : IRequestHandler<GetAllNonCompletedOrdersQuery, GetAllNonCompletedOrdersResponse>
{
    private const string GetNonCompletedOrdersQuery = @"
            SELECT 
                id AS Id,
                location_x AS LocationX, 
                location_y AS LocationY
            FROM 
                public.orders 
            WHERE 
                status != @status
        ";

    public async Task<GetAllNonCompletedOrdersResponse> Handle(
        GetAllNonCompletedOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        if (connection == null) throw new Exception("No hay database, señor.");
        if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

        var orders = await connection.QueryAsync<OrderDto, LocationDto, OrderDto>(
            GetNonCompletedOrdersQuery,
            (order, location) =>
            {
                order.LocationDto = location;
                return order;
            },
            new { status = OrderStatus.Completed.Name },
            splitOn: "LocationX"
        );

        return new GetAllNonCompletedOrdersResponse(orders.AsList());
    }
}
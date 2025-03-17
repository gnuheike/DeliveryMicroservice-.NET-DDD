# Delivery App Microservice

Delivery App is a full‐stack delivery management microservice designed using domain–driven design principles and CQRS. It manages orders, assigns available couriers, tracks courier movements and uses background jobs and messaging to ensure eventual consistency. The application exposes a RESTful API for clients and integrates with external systems (e.g. Geo service via gRPC and Kafka message brokers).

---

## Modules and Components

- **DeliveryApp.Api**  
  – Contains the ASP.NET Core API endpoints, controllers and background jobs.  
  – Implements HTTP adapters (controllers, formatters, attributes, mappers) and Kafka consumers (e.g. BasketConfirmedConsumerHostedService).  
  – Configures Swagger and defines global API behaviors.

- **DeliveryApp.Core**  
  – Holds the domain models (Order, Courier, Transport, Value Objects) and domain services.  
  – Implements use cases (commands and queries) via MediatR.  
  – Manages domain events and aggregates following DDD principles.

- **DeliveryApp.Infrastructure**  
  – Provides persistence using EF Core with PostgreSQL; repositories are implemented here.  
  – Implements the outbox pattern to reliably dispatch domain events.  
  – Includes adapters for external services such as a GRPC-based Geo service and Kafka message bus producers.

- **Tests**  
  – Contains integration and unit tests to validate API endpoints, domain models, services, and repository behavior.
  
- **Utils/Primitives**  
  – Implements common abstractions such as aggregates, domain events, error handling and unit-of-work patterns used across the solution.

---

## Patterns, Techniques, and Technologies

- **Domain–Driven Design (DDD)**  
  Encapsulates business logic in aggregates, entities, value objects and domain events.

- **CQRS with MediatR**  
  Commands and queries are implemented in a request–handler pattern for separation of concerns.

- **Outbox Pattern**  
  Uses an outbox table to reliably persist and publish domain events.

- **Dependency Injection (DI)**  
  Services, repositories and adapters are injected into components via .NET Core’s DI container.

- **Background Processing (Quartz)**  
  Quartz.NET is used for scheduling background jobs such as order assignment and courier movement.

- **RESTful API with ASP.NET Core**  
  Controllers expose CRUD endpoints with integrated Swagger support for OpenAPI documentation.

- **Messaging and gRPC**  
  Confluent.Kafka library is used for Kafka integration and a gRPC client connects to an external Geo service.

- **Persistence**  
  EF Core with Npgsql is used for data access; Dapper is used for optimized read queries.

---

## Key Features

- **Order Management**  
  Create, assign, dispatch and complete orders using domain–driven logic.

- **Courier Assignment and Movement**  
  A scoring service selects the closest available courier for a given order. Background jobs simulate courier movement toward the order location.

- **Integration with External Systems**  
  Integration with a Geo service (gRPC) to determine location coordinates and with Kafka for real–time event propagation.

- **Outbox Messaging**  
  Reliable domain event processing through the outbox pattern ensures eventual consistency between database state and external message bus.

- **API Documentation**  
  Self–documenting API using Swagger UI and OpenAPI annotations.

---

## Project Structure

Below is a high-level directory breakdown:

```
DeliveryApp.Api/
├── Adapters/
│   ├── BackgroundJobs/
│   │   ├── AssignOrdersJob.cs
│   │   └── MoveCouriersJob.cs
│   ├── Http/
│   │   ├── Contract/
│   │   │   ├── Api/
│   │   │   │   ├── Controllers/
│   │   │   │   │   └── DefaultApi.cs
│   │   │   │   ├── Models/
│   │   │   │   │   ├── Courier.cs
│   │   │   │   │   ├── Error.cs
│   │   │   │   │   ├── Location.cs
│   │   │   │   │   └── Order.cs
│   │   │   │   ├── Filters/
│   │   │   │   │   └── GeneratePathParamsValidationFilter.cs
│   │   │   │   ├── Formatters/
│   │   │   │   │   └── InputFormatterStream.cs
│   │   │   │   └── Startup.cs, Program.cs, etc.
│   │   └── Mapper/
│   │       ├── CouriersMapper.cs
│   │       └── OrderMapper.cs
│   └── Kafka/
│       ├── BasketConfirmedConsumerHostedService.cs
│       └── BasketConfirmedConsumerHostedServiceFactory.cs
├── Application/
│   └── UseCases/
│       ├── Commands/
│       │   ├── CreateOrder/
│       │   │   ├── CreateOrderCommand.cs
│       │   │   └── CreateOrderCommandHandler.cs
│       │   ├── AssignOrders/
│       │   │   ├── AssignOrdersCommand.cs
│       │   │   └── AssignOrdersCommandHandler.cs
│       │   └── MoveCouriers/
│       │       ├── MoveCouriersCommand.cs
│       │       └── MoveCouriersCommandHandler.cs
│       └── Queries/
│           ├── GetAllNonCompletedOrders/
│           │   ├── GetAllNonCompletedOrdersQuery.cs
│           │   ├── GetAllNonCompletedOrdersResponse.cs
│           │   └── PostgresGetAllNonCompletedOrdersQueryHandler.cs
│           └── GetBusyCouriers/
│               ├── GetBusyCouriersQuery.cs
│               ├── GetBusyCouriersQueryHandler.cs
│               └── GetBusyCouriersResponse.cs
├── ServiceConfiguration.cs
└── SettingsSetup.cs

DeliveryApp.Core/
├── Application/
│   └── DomainEventHandlers/
│       └── OrderCompletedDomainEventHandler.cs
├── Domain/
│   ├── Models/
│   │   ├── CourierAggregate/
│   │   │   ├── Courier.cs
│   │   │   ├── CourierStatus.cs
│   │   │   └── Transport.cs
│   │   └── OrderAggregate/
│   │       ├── Order.cs
│   │       ├── OrderStatus.cs
│   │       └── DomainEvents/
│   │           └── OrderCompletedDomainEvent.cs
│   ├── Ports/
│   │   ├── ICourierRepository.cs
│   │   ├── IOrderRepository.cs
│   │   └── ILocationProvider.cs
│   └── Services/
│       ├── CourierScoringService.cs
│       └── IOrderCompletedMessageBusProducer.cs
└── SharedKernel/
    └── Location.cs

DeliveryApp.Infrastructure/
├── Adapters/
│   ├── Grps/
│   │   ├── GeoService/
│   │   │   ├── GeoClientFactory.cs
│   │   │   └── GrpsLocationProvider.cs
│   │   └── Kafka/
│   │       └── OrderCompleted/
│   │           ├── KafkaOrderCompletedMessageBusProducer.cs
│   │           └── KafkaOrderCompletedMessageBusProducerFactory.cs
│   └── Postgres/
│       ├── ApplicationDbContext.cs
│       ├── BackgroundJobs/
│       │   └── OutboxMessagesProcessorBackgroundJob.cs
│       ├── Entities/
│       │   └── OutBoxMessage.cs
│       ├── EntityConfigurations/
│       │   ├── CourierAggregate/
│       │   │   ├── CourierEntityTypeConfiguration.cs
│       │   │   └── TransportEntityTypeConfiguration.cs
│       │   ├── OrderAggregate/
│       │   │   └── OrderEntityTypeConfiguration.cs
│       │   └── Outbox/
│       │       └── OutboxEntityTypeConfiguration.cs
│       ├── Migrations/
│       ├── Outbox/
│       │   ├── IOutboxDomainEventsSaver.cs
│       │   └── PostgresOutboxDomainEventsSaver.cs
│       └── Repositories/
│           ├── PostgresCourierRepository.cs
│           └── PostgresOrderRepository.cs
├── Settings.cs
└── UnitOfWork.cs

Tests/
├── DeliveryApp.IntegrationTests/
│   └── Infrastructure/Adapters/Postgres/Repositories/
│       ├── CourierRepositoryShould.cs
│       └── OrderRepositoryShould.cs
└── DeliveryApp.UnitTests/
    ├── Application/UseCases/Commands/CreateOrderCommandShould.cs
    ├── Core/Domain/Model/
    │   ├── CourierAggregate/
    │   │   ├── CourierShould.cs
    │   │   └── CourierStatusShould.cs
    │   ├── OrderAggregate/
    │   │   ├── OrderShould.cs
    │   │   └── OrderStatusShould.cs
    │   └── SharedKernel/
    │       └── LocationShould.cs
    └── Core/Domain/Service/
        └── DispatchServiceShould.cs

Utils/Primitives/
├── Aggregate.cs
├── DomainEvent.cs
├── Error.cs
├── GenaralErrors.cs
├── IRepository.cs
├── IUnitOfWork.cs
└── Extensions/
    └── StringExtension.cs
```

---

## Notable Code Snippets

### 1. Courier Assignment in a Background Job

The `AssignOrdersJob` uses Quartz.NET to trigger order assignment. It sends an `AssignOrdersCommand` via MediatR:

```csharp
[DisallowConcurrentExecution]
public class AssignOrdersJob(IMediator mediator) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return mediator.Send(new AssignOrdersCommand());
    }
}
```

*Explanation:*  
This job ensures that newly created orders are periodically evaluated and assigned to the closest available couriers.

---

### 2. Domain–Driven Design in Order Aggregate

The `Order` entity encapsulates business rules for order creation and completion:

```csharp
public class Order : Aggregate<Guid>
{
    private Order(Guid orderId, Location location)
    {
        Id = orderId;
        Location = location;
        Status = OrderStatus.Created();
    }

    public static Result<Order, Error> Create(Guid orderId, Location location)
    {
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
        return new Order(orderId, location);
    }

    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned()) return Errors.CantCompletedNotAssignedOrder();

        Status = OrderStatus.Completed();
        RaiseDomainEvent(new OrderCompletedDomainEvent(this));
        return UnitResult.Success<Error>();
    }
    // ...
}
```

*Explanation:*  
This approach encapsulates creation logic, validation and domain events to keep the entity self–contained and consistent.

---

## External Libraries and Resources

- **ASP.NET Core** – [Microsoft Documentation](https://docs.microsoft.com/aspnet/core/)
- **Entity Framework Core** – [EF Core Docs](https://docs.microsoft.com/ef/core/)
- **Npgsql** – [Npgsql Documentation](https://www.npgsql.org/)
- **Quartz.NET** – [Quartz.NET Docs](https://www.quartz-scheduler.net/)
- **MediatR** – [MediatR GitHub](https://github.com/jbogard/MediatR)
- **Confluent.Kafka** – [Confluent Kafka Documentation](https://docs.confluent.io/)
- **gRPC for .NET** – [gRPC Documentation](https://docs.microsoft.com/aspnet/core/grpc/)
- **Swagger / Swashbuckle** – [Swashbuckle GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- **CSharpFunctionalExtensions** – [GitHub Repository](https://github.com/vkostadinov/CSharpFunctionalExtensions)

---

## Setup and Usage Instructions

1. **Prerequisites**  
   – .NET SDK (8.0 recommended)  
   – PostgreSQL server  
   – Kafka broker (if running messaging features)  
   – gRPC–enabled Geo service endpoint

2. **Configuration**  
   Configure environment variables or `appsettings.json` with:  
   • CONNECTION_STRING  
   • GEO_SERVICE_GRPC_HOST  
   • MESSAGE_BROKER_HOST  
   • ORDER_STATUS_CHANGED_TOPIC  
   • BASKET_CONFIRMED_TOPIC

3. **Database Migration**  
   From the project root, run:
   ```bash
   dotnet ef database update --project DeliveryApp.Infrastructure
   ```

4. **Running the Application**  
   Build and run the API project:
   ```bash
   dotnet run --project DeliveryApp.Api
   ```
   The API will be hosted (e.g., at http://0.0.0.0:8080).

5. **Swagger UI**  
   Once running, navigate to `/openapi` to view the API documentation and interact with endpoints.

6. **Running Tests**  
   Execute unit and integration tests using your preferred test runner:
   ```bash
   dotnet test
   ```

---

## Contributing

Contributions are welcome. To contribute:

1. Fork the repository.
2. Create a feature branch:  
   ```bash
   git checkout -b feature/YourFeature
   ```
3. Commit your changes with clear commit messages.
4. Push to your branch and create a pull request.
5. Follow the style guidelines used in the project.

---

## License

This project is published with no specific license (“NoLicense”) as specified in the API documentation. For further use or redistribution, please contact the maintainers.

---

This README provides a concise technical overview of the project, its architecture, key features, and setup steps. For additional details refer to the in–code comments and documentation in each module.

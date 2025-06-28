[![.NET](https://github.com/gnuheike/DeliveryMicroservice-.NET-DDD/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gnuheike/DeliveryMicroservice-.NET-DDD/actions/workflows/dotnet.yml)

[![Delivery App Microservice](https://github.com/gnuheike/DeliveryMicroservice-.NET-DDD/blob/main/cover.jpg?raw=true)](https://github.com/gnuheike/DeliveryMicroservice-.NET-DDD)


# Delivery App Microservice

**Delivery App Microservice** is a robust, full-stack solution for managing delivery operations, designed with scalability and maintainability in mind. Built using Domain-Driven Design (DDD) and CQRS, it orchestrates the entire delivery process—from order creation and courier assignment to dispatch and completion. The microservice exposes a RESTful API and integrates with external systems, such as a Geo service via gRPC and Kafka for real-time event streaming, ensuring a responsive and reliable system.

---

## Architecture and Design

The project adheres to modern architectural principles:
* Domain-Driven Design (DDD): Encapsulates business logic within aggregates, entities, and value objects.
* CQRS with MediatR: Separates command and query responsibilities for optimized performance and clarity.
* Outbox Pattern: Guarantees reliable event publishing to external systems.

---

## Technologies Used
* Backend: ASP.NET Core, Entity Framework Core, PostgreSQL
* Messaging: Kafka (Confluent.Kafka)
* Background Jobs: Quartz.NET
* External Services: gRPC
* Testing: xUnit, Moq
* Documentation: Swagger (Swashbuckle)

---

## Patterns and Techniques

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

The solution is organized following clean architecture principles:
* DeliveryApp.Api: RESTful API layer with controllers, background jobs, and Kafka consumers.
* DeliveryApp.Core: Domain layer containing models, services, and use cases.
* DeliveryApp.Infrastructure: Persistence with EF Core, external service adapters, and the outbox pattern.
* Tests: Unit and integration tests for comprehensive validation.
* Utils/Primitives: Shared utilities and abstractions (e.g., aggregates, domain events).

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
   ``

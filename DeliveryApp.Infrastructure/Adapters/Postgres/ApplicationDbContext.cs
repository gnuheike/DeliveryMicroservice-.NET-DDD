using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Courier> Couriers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply Configuration
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TransportEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());

        modelBuilder.Entity<Transport>(b =>
        {
            var allTransports = Transport.List().ToList();
            b.HasData(allTransports.Select(c => new { c.Id, c.Name, c.Speed }));
        });
    }
}
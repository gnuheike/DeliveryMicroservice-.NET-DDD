using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

internal class CourierEntityTypeConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> entityTypeBuilder)
    {
        entityTypeBuilder.ToTable("couriers");

        entityTypeBuilder.HasKey(entity => entity.Id);

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired();

        entityTypeBuilder
            .HasOne(entity => entity.Transport)
            .WithMany()
            .IsRequired()
            .HasForeignKey("transport_id");

        entityTypeBuilder
            .OwnsOne(entity => entity.Status, s =>
            {
                s.Property(status => status.Name)
                    .HasColumnName("status_name")
                    .IsRequired();
            });


        entityTypeBuilder
            .OwnsOne(entity => entity.Location, l =>
            {
                l.Property(x => x.X).HasColumnName("location_x").IsRequired();
                l.Property(y => y.Y).HasColumnName("location_y").IsRequired();
                l.WithOwner();
            });
    }
}
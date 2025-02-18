﻿// <auto-generated />
using System;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DeliveryApp.Core.Domain.Models.CourierAggregate.Courier", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("transport_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("transport_id");

                    b.ToTable("couriers", (string)null);
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.Models.CourierAggregate.Transport", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("Speed")
                        .HasColumnType("integer")
                        .HasColumnName("speed");

                    b.HasKey("Id");

                    b.ToTable("transports", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "pedestrian",
                            Speed = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "bicycle",
                            Speed = 2
                        },
                        new
                        {
                            Id = 3,
                            Name = "car",
                            Speed = 3
                        });
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.Models.OrderAggregate.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("CourierId")
                        .HasColumnType("uuid")
                        .HasColumnName("courier_id");

                    b.HasKey("Id");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.Models.CourierAggregate.Courier", b =>
                {
                    b.HasOne("DeliveryApp.Core.Domain.Models.CourierAggregate.Transport", "Transport")
                        .WithMany()
                        .HasForeignKey("transport_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("CourierId")
                                .HasColumnType("uuid");

                            b1.Property<int>("X")
                                .HasColumnType("integer")
                                .HasColumnName("location_x");

                            b1.Property<int>("Y")
                                .HasColumnType("integer")
                                .HasColumnName("location_y");

                            b1.HasKey("CourierId");

                            b1.ToTable("couriers");

                            b1.WithOwner()
                                .HasForeignKey("CourierId");
                        });

                    b.OwnsOne("DeliveryApp.Core.Domain.Models.CourierAggregate.CourierStatus", "Status", b1 =>
                        {
                            b1.Property<Guid>("CourierId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("status_name");

                            b1.HasKey("CourierId");

                            b1.ToTable("couriers");

                            b1.WithOwner()
                                .HasForeignKey("CourierId");
                        });

                    b.Navigation("Location");

                    b.Navigation("Status");

                    b.Navigation("Transport");
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.Models.OrderAggregate.Order", b =>
                {
                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<int>("X")
                                .HasColumnType("integer")
                                .HasColumnName("location_x");

                            b1.Property<int>("Y")
                                .HasColumnType("integer")
                                .HasColumnName("location_y");

                            b1.HasKey("OrderId");

                            b1.ToTable("orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("DeliveryApp.Core.Domain.Models.OrderAggregate.VO.OrderStatus", "Status", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("status_name");

                            b1.HasKey("OrderId");

                            b1.ToTable("orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Location");

                    b.Navigation("Status");
                });
#pragma warning restore 612, 618
        }
    }
}

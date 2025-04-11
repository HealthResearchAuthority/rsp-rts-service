﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rsp.RtsService.Infrastructure;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    [DbContext(typeof(RtsDbContext))]
    [Migration("20250409133532_RemoveColumnsFromOrganisationRole")]
    partial class RemoveColumnsFromOrganisationRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Rsp.RtsService.Domain.Entities.Organisation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Imported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SystemUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(150)");

                    b.Property<string>("TypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organisation");
                });

            modelBuilder.Entity("Rsp.RtsService.Domain.Entities.OrganisationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("OrganisationId")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Scoper")
                        .HasColumnType("varchar(150)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Imported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SystemUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id", "OrganisationId", "Scoper", "CreatedDate");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationRole");
                });

            modelBuilder.Entity("Rsp.RtsService.Domain.Entities.OrganisationRole", b =>
                {
                    b.HasOne("Rsp.RtsService.Domain.Entities.Organisation", null)
                        .WithMany("Roles")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Rsp.RtsService.Domain.Entities.Organisation", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}

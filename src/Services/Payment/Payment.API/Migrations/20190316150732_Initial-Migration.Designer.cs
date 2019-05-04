﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Payment.API.Infrastructure;

namespace Payment.API.Migrations
{
    [DbContext(typeof(PaymentContext))]
    [Migration("20190316150732_Initial-Migration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Payment.API.Model.PaymentAccount", b =>
                {
                    b.Property<Guid>("BuyerId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountAvailable");

                    b.HasKey("BuyerId");

                    b.ToTable("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}

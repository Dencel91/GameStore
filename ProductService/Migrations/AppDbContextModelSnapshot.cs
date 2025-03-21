﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductService.Data;

#nullable disable

namespace ProductService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductService.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("ThumbnailUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ProductService.Models.ProductImage", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(1);

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "ImageUrl");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("ProductService.Models.ProductReview", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsRecommended")
                        .HasColumnType("bit");

                    b.Property<string>("Review")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId", "UserId");

                    b.ToTable("ProductReviews");
                });

            modelBuilder.Entity("ProductService.Models.ProductToUser", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("ProductId", "UserId");

                    b.ToTable("ProductToUsers");
                });

            modelBuilder.Entity("ProductService.Models.ProductImage", b =>
                {
                    b.HasOne("ProductService.Models.Product", null)
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductService.Models.ProductReview", b =>
                {
                    b.HasOne("ProductService.Models.Product", null)
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductService.Models.Product", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}

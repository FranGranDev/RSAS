﻿using Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Application.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        //Stock And Products
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockProducts> StockProducts { get; set; }

        //Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        //Sales
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleProduct> SaleProducts { get; set; }

        //Clients
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Stock and Products
            builder.Entity<StockProducts>()
                .HasKey(x => new { x.StockId, x.ProductId });

            builder.Entity<StockProducts>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.StockProducts)
                .HasForeignKey(ps => ps.ProductId);

            builder.Entity<StockProducts>()
                .HasOne(ps => ps.Stock)
                .WithMany(s => s.StockProducts)
                .HasForeignKey(ps => ps.StockId);


            //Clients
            builder.Entity<Client>()
                .HasOne(u => u.User)
                .WithOne(x => x.Client)
                .HasForeignKey<Client>(x => x.UserId);

            builder.Entity<Employee>()
                .HasOne(u => u.User)
                .WithOne(x => x.Employee)
                .HasForeignKey<Employee>(x => x.UserId);

            //Orders
            builder.Entity<OrderProduct>()
                .HasKey(x => new { x.OrderId, x.ProductId });

            builder.Entity<OrderProduct>()
                .HasOne(x => x.Order)
                .WithMany(p => p.Products)
                .HasForeignKey(k => k.OrderId);

            builder.Entity<OrderProduct>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(k => k.ProductId);

            builder.Entity<Order>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(k => k.UserId);

            builder.Entity<Order>()
                .HasOne(x => x.Stock)
                .WithMany()
                .HasForeignKey(k => k.StockId)
                .IsRequired(false);

            builder.Entity<Order>()
                .HasOne(o => o.Delivery)
                .WithOne(d => d.Order)
                .HasForeignKey<Delivery>(d => d.OrderId);

            builder.Entity<Delivery>()
                .HasKey(k => k.OrderId);

            //Sales
            builder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne<Order>(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Products)
                    .WithOne(e => e.Sale)
                    .HasForeignKey(e => e.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SaleProduct>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne<Sale>(e => e.Sale)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Product>(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
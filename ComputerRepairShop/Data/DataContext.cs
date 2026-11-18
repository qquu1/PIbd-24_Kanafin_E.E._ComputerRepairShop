using ComputerRepairShop.Entities;
using ComputerRepairShop.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Data
{
    /// <summary>
    /// Контекст базы данных (Code First) для работы через Entity Framework Core.
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<SparePart> SpareParts { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<RepairOrder> RepairOrders { get; set; } = null!;
        public DbSet<OrderSparePart> OrderSpareParts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            optionsBuilder.UseNpgsql(
                "Host=127.0.0.1;Port=5432;Database=ComputerRepairShopDb;Username=postgres;Password=1234aS;");

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("clients");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Phone).HasDatabaseName("ix_clients_phone");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("devices");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SerialNumber).IsUnique().HasDatabaseName("ux_devices_serial_number");
                entity.Property(e => e.DeviceType).HasConversion<int>();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeePost).HasConversion<int>();
            });

            modelBuilder.Entity<SparePart>(entity =>
            {
                entity.ToTable("spare_parts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.PartCategory).HasConversion<int>();
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("warehouses");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<RepairOrder>(entity =>
            {
                entity.ToTable("repair_orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalCost).HasPrecision(10, 2);
                entity.Property(e => e.RepairStatus).HasConversion<int>();
            });

            modelBuilder.Entity<OrderSparePart>(entity =>
            {
                entity.ToTable("order_spare_parts");
                entity.HasKey(e => e.Id);
            });


            modelBuilder.Entity<Device>()
                .HasOne(d => d.Client)
                .WithMany(c => c.Devices)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairOrder>()
                .HasOne(r => r.Device)
                .WithMany(d => d.RepairOrders)
                .HasForeignKey(r => r.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairOrder>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.RepairOrders)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairOrder>()
                .HasOne(r => r.Warehouse)
                .WithMany(w => w.RepairOrders)
                .HasForeignKey(r => r.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderSparePart>()
                .HasOne(o => o.RepairOrder)
                .WithMany(r => r.OrderSpareParts)
                .HasForeignKey(o => o.RepairOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderSparePart>()
                .HasOne(o => o.SparePart)
                .WithMany(s => s.OrderSpareParts)
                .HasForeignKey(o => o.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

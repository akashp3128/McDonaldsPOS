using Microsoft.EntityFrameworkCore;
using McDonaldsPOS.Core.Models;

namespace McDonaldsPOS.Data;

/// <summary>
/// Entity Framework Core database context for the POS system
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<MenuItemModifier> MenuItemModifiers => Set<MenuItemModifier>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemModifier> OrderItemModifiers => Set<OrderItemModifier>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Pin).HasMaxLength(10).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // MenuItem configuration
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ShortName).HasMaxLength(30);
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);
            entity.Property(e => e.SmallPrice).HasPrecision(10, 2);
            entity.Property(e => e.MediumPrice).HasPrecision(10, 2);
            entity.Property(e => e.LargePrice).HasPrecision(10, 2);
            entity.Property(e => e.ComboUpcharge).HasPrecision(10, 2);
            entity.HasIndex(e => e.Category);
        });

        // Modifier configuration
        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ShortName).HasMaxLength(20);
            entity.Property(e => e.PriceAdjustment).HasPrecision(10, 2);
        });

        // MenuItemModifier - many-to-many
        modelBuilder.Entity<MenuItemModifier>(entity =>
        {
            entity.HasKey(e => new { e.MenuItemId, e.ModifierId });
            entity.HasOne(e => e.MenuItem)
                .WithMany(m => m.AvailableModifiers)
                .HasForeignKey(e => e.MenuItemId);
            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifierId);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subtotal).HasPrecision(10, 2);
            entity.Property(e => e.TaxRate).HasPrecision(5, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(10, 2);
            entity.Property(e => e.Total).HasPrecision(10, 2);
            entity.Property(e => e.AmountTendered).HasPrecision(10, 2);
            entity.Property(e => e.ChangeGiven).HasPrecision(10, 2);
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItemModifier configuration
        modelBuilder.Entity<OrderItemModifier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModifierName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PriceAdjustment).HasPrecision(10, 2);
            entity.HasOne(e => e.OrderItem)
                .WithMany(oi => oi.Modifiers)
                .HasForeignKey(e => e.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifierId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

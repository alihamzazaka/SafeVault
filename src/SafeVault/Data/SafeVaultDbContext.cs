using Microsoft.EntityFrameworkCore;
using SafeVault.Models;

namespace SafeVault.Data;

public class SafeVaultDbContext(DbContextOptions<SafeVaultDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
        modelBuilder.Entity<Product>().HasMany(x => x.OrderItems).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
    }
}

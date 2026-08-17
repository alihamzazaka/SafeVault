using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models;

public class User
{
    public int Id { get; set; }
    [Required, StringLength(50, MinimumLength = 3)] public string Username { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required] public string Role { get; set; } = "User";
    public ICollection<Order> Orders { get; set; } = [];
}

public class Product
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Range(0.01, 1000000)] public decimal Price { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; }
}

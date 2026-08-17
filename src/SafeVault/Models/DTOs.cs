using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models;

public record LoginRequest([Required, StringLength(50, MinimumLength = 3)] string Username, [Required, StringLength(100, MinimumLength = 8)] string Password);
public record CreateProductRequest([Required, StringLength(100, MinimumLength = 2)] string Name, [Range(0.01, 1000000)] decimal Price);
public record CreateOrderRequest([Required, MinLength(1), MaxLength(50)] List<OrderLineRequest> Items);
public record OrderLineRequest([Range(1, int.MaxValue)] int ProductId, [Range(1, 1000)] int Quantity);

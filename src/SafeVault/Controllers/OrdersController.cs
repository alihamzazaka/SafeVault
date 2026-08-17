using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(SafeVaultDbContext db) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var orders = await db.Orders.AsNoTracking().Where(x => x.UserId == CurrentUserId).Select(x => new
        {
            x.Id,
            x.CreatedAt,
            Items = x.Items.Select(i => new { i.ProductId, Product = i.Product!.Name, i.Quantity })
        }).ToListAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var ids = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var validProducts = await db.Products.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();
        if (validProducts.Count != ids.Count) return BadRequest(new { message = "One or more products are invalid." });
        var order = new Order { UserId = CurrentUserId, Items = request.Items.Select(x => new OrderItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList() };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMine), new { id = order.Id }, order.Id);
    }
}

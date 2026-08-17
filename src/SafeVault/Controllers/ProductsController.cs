using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SafeVault.Data;
using SafeVault.Models;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(SafeVaultDbContext db, IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts()
    {
        var products = await cache.GetOrCreateAsync("products", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await db.Products.AsNoTracking().Select(x => new { x.Id, x.Name, x.Price }).ToListAsync();
        });
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var product = new Product { Name = request.Name.Trim(), Price = request.Price };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        cache.Remove("products");
        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }
}

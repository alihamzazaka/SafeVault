using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(SafeVaultDbContext db, IPasswordHasher hasher, ITokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username == request.Username);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash)) return Unauthorized(new { message = "Invalid credentials." });
        return Ok(new { accessToken = tokens.CreateToken(user.Id, user.Username, user.Role) });
    }
}

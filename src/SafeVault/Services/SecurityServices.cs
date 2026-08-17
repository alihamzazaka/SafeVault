using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SafeVault.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string encodedHash);
}

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string encodedHash)
    {
        var parts = encodedHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch { return false; }
    }
}

public interface ITokenService
{
    string CreateToken(int userId, string username, string role);
}

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(int userId, string username, string role)
    {
        var key = configuration["Jwt:Key"] ?? "SafeVault-development-key-change-in-production-2026-very-long";
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
        var token = new JwtSecurityToken("SafeVault", "SafeVault.Api", claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

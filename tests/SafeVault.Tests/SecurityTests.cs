using SafeVault.Services;

namespace SafeVault.Tests;

public class SecurityTests
{
    [Fact]
    public void PasswordHash_IsNotPlainText_AndVerifies()
    {
        var hasher = new PasswordHasher();
        var password = "StrongPassword@123";
        var hash = hasher.Hash(password);
        Assert.NotEqual(password, hash);
        Assert.True(hasher.Verify(password, hash));
        Assert.False(hasher.Verify("WrongPassword", hash));
    }

    [Fact]
    public void PasswordHash_UsesDifferentSaltEachTime()
    {
        var hasher = new PasswordHasher();
        Assert.NotEqual(hasher.Hash("Password@123"), hasher.Hash("Password@123"));
    }
}

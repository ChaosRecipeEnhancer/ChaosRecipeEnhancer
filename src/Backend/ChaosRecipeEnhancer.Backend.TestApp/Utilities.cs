using System.Security.Cryptography;

namespace ChaosRecipeEnhancer.Backend.TestApp;

internal static class Utilities
{
    public static string GenerateRandomString(int length)
    {
        var randomBytes = new byte[length];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes)[..length];
    }

    public static string Base64UrlEncode(string input)
    {
        return input.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}

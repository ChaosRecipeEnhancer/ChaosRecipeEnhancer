using System;
using System.Security.Cryptography;
using System.Text;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for authentication-related tasks.
/// </summary>
public static class AuthUtilities
{
    /// <summary>
    /// Generates a random code verifier.
    /// </summary>
    /// <returns>The generated code verifier.</returns>
    public static string GenerateCodeVerifier()
    {
        return StringUtilities.GenerateRandomString(128);
    }

    /// <summary>
    /// Generates a code challenge from the provided code verifier.
    /// </summary>
    /// <param name="codeVerifier">The code verifier.</param>
    /// <returns>The generated code challenge.</returns>
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return UrlUtilities.Base64UrlEncode(Convert.ToBase64String(hash));
    }

    /// <summary>
    /// Generates a random state.
    /// </summary>
    /// <returns>The generated state.</returns>
    public static string GenerateState()
    {
        return StringUtilities.GenerateRandomString(32);
    }
}

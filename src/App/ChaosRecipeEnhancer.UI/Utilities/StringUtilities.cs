using Serilog;
using System;
using System.Security.Cryptography;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for string operations, including generating random strings.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Generates a random string of a specified length.
    /// </summary>
    /// <param name="length">The desired length of the random string.</param>
    /// <returns>A random string of the specified length.</returns>
    /// <remarks>
    /// The method generates a byte array of the specified length, fills it with random values,
    /// and then converts it to a Base64 string. The result is trimmed to the requested length
    /// to ensure it matches exactly the specified length requirement.
    /// </remarks>
    public static string GenerateRandomString(int length)
    {
        try
        {
            var randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes);
            var result = Convert.ToBase64String(randomBytes)[..length];

            Log.Debug("Generated a random string of length {Length}: {Result}", length, result);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to generate a random string of length {Length}", length);
            throw; // Re-throwing the exception to ensure that calling code is aware of the failure.
        }
    }
}

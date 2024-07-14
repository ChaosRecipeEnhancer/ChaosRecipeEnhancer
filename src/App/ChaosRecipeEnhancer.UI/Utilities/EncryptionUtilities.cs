using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for encrypting and decrypting sensitive data.
/// </summary>
public static class EncryptionUtilities
{
    /// <summary>
    /// A lazy-loaded byte array that stores the entropy used for encryption and decryption.
    /// The entropy is generated only once when first accessed and then reused.
    /// </summary>
    private static readonly Lazy<byte[]> LazyEntropy = new(() => GetEntropy());

    /// <summary>
    /// Gets the entropy byte array used for encryption and decryption.
    /// </summary>
    private static byte[] Entropy => LazyEntropy.Value;

    /// <summary>
    /// Encrypts a string using the Windows Data Protection API (DPAPI) with additional entropy.
    /// </summary>
    /// <param name="input">The string to encrypt.</param>
    /// <returns>The encrypted string as a Base64-encoded string.</returns>
    /// <remarks>
    /// The encryption is tied to the current user account and cannot be decrypted on a different account or machine.
    /// </remarks>
    public static string EncryptString(string input)
    {
        byte[] encryptedData = ProtectedData.Protect(
            Encoding.Unicode.GetBytes(input),
            Entropy,
            DataProtectionScope.CurrentUser
        );

        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Decrypts a previously encrypted string using the Windows Data Protection API (DPAPI) with additional entropy.
    /// </summary>
    /// <param name="encryptedData">The Base64-encoded encrypted string to decrypt.</param>
    /// <returns>The decrypted string, or an empty string if decryption fails.</returns>
    /// <remarks>
    /// This method will only successfully decrypt data that was encrypted on the same user account and machine.
    /// </remarks>
    public static string DecryptString(string encryptedData)
    {
        try
        {
            byte[] decryptedData = ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedData),
                Entropy,
                DataProtectionScope.CurrentUser
            );

            return Encoding.Unicode.GetString(decryptedData);
        }
        catch
        {
            // Return an empty string if decryption fails for any reason
            return string.Empty;
        }
    }

    /// <summary>
    /// Generates a unique entropy based on the current machine, user, and process.
    /// </summary>
    /// <returns>A byte array containing the generated entropy.</returns>
    /// <remarks>
    /// The entropy is created by combining the machine name, user name, and process name,
    /// then hashing the result using SHA256. This ensures that the entropy is unique
    /// for each user on each machine, but remains consistent across multiple runs of the application.
    /// </remarks>
    private static byte[] GetEntropy()
    {
        string machineName = Environment.MachineName;
        string userName = Environment.UserName;
        string processName = Process.GetCurrentProcess().ProcessName;

        return SHA256.HashData(Encoding.UTF8.GetBytes(machineName + userName + processName));
    }
}
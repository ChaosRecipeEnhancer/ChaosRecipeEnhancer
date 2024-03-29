using Serilog;
using System;
using System.Diagnostics;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for URL operations, including opening URLs in a browser and encoding strings for use in URLs.
/// </summary>
public static class UrlUtilities
{
    /// <summary>
    /// Opens a given URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to be opened.</param>
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            Log.Information("Opened URL: {Url}", url);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error opening URL: {Url}", url);
        }
    }

    /// <summary>
    /// Encodes a string into a Base64 URL compliant format.
    /// </summary>
    /// <param name="input">The string to be encoded.</param>
    /// <returns>A Base64 URL encoded string.</returns>
    /// <remarks>
    /// Replaces '+' with '-', '/' with '_', and removes trailing '=' characters to ensure URL compliance.
    /// </remarks>
    public static string Base64UrlEncode(string input)
    {
        var encoded = input.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        Log.Debug("Encoded string to Base64 URL: Original {Input}, Encoded {Encoded}", input, encoded);
        return encoded;
    }
}

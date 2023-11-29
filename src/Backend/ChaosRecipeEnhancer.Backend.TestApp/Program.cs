using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ChaosRecipeEnhancer.Backend.TestApp;

internal static class Program
{
    private static void Main()
    {
        // Generate a random string
        var codeVerifier = Utilities.GenerateRandomString(128);

        // Create a SHA256 hash

        var bytes = Encoding.UTF8.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);

        // Convert to Base64
        var base64Digest = Convert.ToBase64String(hash);

        // Convert Base64 to Base64URL
        var codeChallenge = Utilities.Base64UrlEncode(base64Digest);

        Console.WriteLine("Code Verifier: " + codeVerifier);
        Console.WriteLine("Code Challenge: " + codeChallenge);

        // Generate a random string for state
        var state = Utilities.GenerateRandomString(32);
        Console.WriteLine("State: " + state);

        var url =
            "https://www.pathofexile.com/oauth/authorize?" +
            "client_id=chaosrecipeenhancer" +
            "&response_type=code" +
            "&scope=account:leagues account:stashes account:characters account:item_filter service:leagues service:psapi" +
            $"&state=${state}" +
            "&redirect_uri=https://sandbox.chaos-recipe.com/auth/success" +
            $"&code_challenge=${codeChallenge}" +
            "&code_challenge_method=S256";

        // Open the URL in the default browser
        OpenUrl(url);
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while trying to open the URL: " + ex.Message);
        }
    }
}

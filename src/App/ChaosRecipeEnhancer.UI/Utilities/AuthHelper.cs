using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Utilities;

public static class AuthHelper
{
    public static string CodeVerifier { get; set; } = string.Empty;

    public static void Login()
    {
        // Generate a random string
        var codeVerifier = GenerateRandomString(128);

        // hack
        CodeVerifier = codeVerifier;

        // Create a SHA256 hash
        var bytes = Encoding.UTF8.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);

        // Convert to Base64
        var base64Digest = Convert.ToBase64String(hash);

        // Convert Base64 to Base64URL
        var codeChallenge = Base64UrlEncode(base64Digest);

        Console.WriteLine("Code Verifier: " + codeVerifier);
        Console.WriteLine("Code Challenge: " + codeChallenge);

        // Generate a random string for state
        var state = GenerateRandomString(32);
        Console.WriteLine("State: " + state);

        // You can only request account:* scopes - NO service:* scopes
        const string scopes = "account:leagues account:stashes account:characters account:item_filter";
        var encodedScopes = Uri.EscapeDataString(scopes); // This will encode the scopes correctly

        var url =
            "https://www.pathofexile.com/oauth/authorize?" +
            "client_id=chaosrecipeenhancer" +
            "&response_type=code" +
            "&scope=" + encodedScopes +
            $"&state=${state}" +
            "&redirect_uri=https://sandbox.chaos-recipe.com/auth/success" +
            $"&code_challenge=${codeChallenge}" +
            "&code_challenge_method=S256";

        // Open the URL in the default browser
        OpenUrl(url);
    }


    private static string GenerateRandomString(int length)
    {
        var randomBytes = new byte[length];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes)[..length];
    }

    private static string Base64UrlEncode(string input)
    {
        return input.Replace('+', '-').Replace('/', '_').TrimEnd('=');
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

    public static async Task RetrieveAuthToken(string authCode, string state)
    {
        var httpClient = new HttpClient();
        var requestUri = "https://sandbox.chaos-recipe.com/auth/token";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("code_verifier", CodeVerifier)
        });

        try
        {
            var response = await httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Process the response content (token information) here
                Trace.WriteLine("RetrieveAuthToken - Token Response: " + responseContent);
            }
            else
            {
                // Handle error response
                Trace.WriteLine("RetrieveAuthToken - Error retrieving token: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Trace.WriteLine("RetrieveAuthToken - Exception occurred: " + ex.Message);
        }
    }

}

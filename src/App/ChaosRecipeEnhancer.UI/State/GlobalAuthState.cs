using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.State;

public class GlobalAuthState
{
    #region Fields

    private static readonly Lazy<GlobalAuthState> _instance = new(() => new GlobalAuthState());
    private string _username;
    private string _authToken;
    private string _codeVerifier;
    private DateTime? _tokenExpiration;

    #endregion

    #region Properties

    public static GlobalAuthState Instance => _instance.Value;

    public string AuthToken => _authToken;

    #endregion

    #region Domain Methods

    public void InitializeAuthFlow()
    {
        Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.AttemptingLogin;

        // Resetting global auth state
        _authToken = string.Empty;
        _codeVerifier = string.Empty;
        _tokenExpiration = null;

        // Generate a random string
        var codeVerifier = GenerateRandomString(128);

        // hack
        _codeVerifier = codeVerifier;

        // Create a SHA256 hash
        var bytes = Encoding.UTF8.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);

        // Convert to Base64
        var base64Digest = Convert.ToBase64String(hash);

        // Convert Base64 to Base64URL
        var codeChallenge = Base64UrlEncode(base64Digest);

        Trace.WriteLine("Code Verifier: " + codeVerifier);
        Trace.WriteLine("Code Challenge: " + codeChallenge);

        // Generate a random string for state
        var state = GenerateRandomString(32);
        Trace.WriteLine("State: " + state);

        // You can only request account:* scopes - NO service:* scopes
        const string scopes = "account:leagues account:stashes account:characters account:item_filter";
        var encodedScopes = Uri.EscapeDataString(scopes); // This will encode the scopes correctly

        var url =
            "https://www.pathofexile.com/oauth/authorize?" +
            "client_id=chaosrecipeenhancer" +
            "&response_type=code" +
            "&scope=" + encodedScopes +
            $"&state=${state}" +
            "&redirect_uri=https://chaos-recipe.com/auth/success" +
            $"&code_challenge=${codeChallenge}" +
            "&code_challenge_method=S256";

        // Open the URL in the default browser
        OpenUrl(url);
    }

    public bool ValidateLocalAuthToken()
    {
        // if we don't have an auth token, try to load it from the settings
        if (string.IsNullOrEmpty(_authToken)) _authToken = Settings.Default.PathOfExileApiAuthToken;

        // if we don't have a token expiration, try to load it from the settings
        if (_tokenExpiration is null) _tokenExpiration = Settings.Default.PathOfExileApiAuthTokenExpiration;

        var isValid = !string.IsNullOrEmpty(_authToken) && DateTime.UtcNow < _tokenExpiration
          // we should also return false if the locally stored auth token doesn't match what's in the global state
          || _authToken != Settings.Default.PathOfExileApiAuthToken;

        // if the token is invalid, we should purge it from the global state
        if (!isValid)
        {
            PurgeLocalAuthToken();
            return false;
        }

        // if the token is valid, we should update the global state
        Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ValidatedConnection;
        Settings.Default.Save();
        return true;
    }

    public void PurgeLocalAuthToken()
    {
        _authToken = string.Empty;
        _tokenExpiration = null;

        // Also update App Settings
        Settings.Default.PathOfExileAccountName = string.Empty;
        Settings.Default.PathOfExileApiAuthToken = string.Empty;

        // only reset this specific setting if we're not in the middle of an auth flow
        if (Settings.Default.PoEAccountConnectionStatus != (int)ConnectionStatusTypes.AttemptingLogin)
        {
            Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;
        }

        // for one, we can't set this to null because it's a value type
        // in this case, we'll simply ignore it (it's not used anywhere else)
        // Settings.Default.PathOfExileApiAuthTokenExpiration = null;

        Settings.Default.Save();
    }

    public async Task<string> GenerateAuthToken(string authCode)
    {
        const string requestUri = "https://chaos-recipe.com/auth/token";
        var httpClient = new HttpClient();

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("code_verifier", _codeVerifier),
        });

        try
        {
            var response = await httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Process the response content (token information) here
                Trace.WriteLine("RetrieveAuthToken - Token Response: " + responseContent);

                // Updating global auth state
                var authTokenResponse = JsonSerializer.Deserialize<AuthTokenResponse>(responseContent);

                // This is an important step we cannot miss as it will also update the auth settings that persist
                // across application restarts (and saved to the settings file on disk)
                UpdateAuthUserSettings(authTokenResponse);

                return responseContent;
            }

            // Handle error response
            Trace.WriteLine("RetrieveAuthToken - Error retrieving token: " + response.StatusCode);
            Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionError;
            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Trace.WriteLine("RetrieveAuthToken - Exception occurred: " + ex.Message);
            Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionError;
            Settings.Default.Save();
        }

        return string.Empty;
    }

    #endregion

    #region Utilities

    private void UpdateAuthUserSettings(AuthTokenResponse authTokenResponse)
    {
        // Updating Global State
        _username = authTokenResponse.Username;
        _authToken = authTokenResponse.AccessToken;

        // the included `ExpiresIn` value is likely inaccurate...
        // i'm going to go off the docs and refresh this every 10 hours
        // _tokenExpiration = DateTime.UtcNow.AddSeconds(authTokenResponse.ExpiresIn);
        _tokenExpiration = DateTime.UtcNow.AddHours(10);

        // Also update App Settings
        Settings.Default.PathOfExileAccountName = _username;
        Settings.Default.PathOfExileApiAuthToken = _authToken;
        Settings.Default.PathOfExileApiAuthTokenExpiration = (DateTime)_tokenExpiration;
        Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ValidatedConnection;
        Settings.Default.Save();
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
            Trace.WriteLine("An error occurred while trying to open the URL: " + ex.Message);
        }
    }

    #endregion

    #region POCOs
    public class AuthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    #endregion
}

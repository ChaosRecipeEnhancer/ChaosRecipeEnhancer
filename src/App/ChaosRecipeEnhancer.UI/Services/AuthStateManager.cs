using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IAuthStateManager
{
    string AuthToken { get; }
    event EventHandler AuthStateChanged;
    void Login(bool autoRedirect = true);
    void Logout();
    Task<string> GenerateAuthToken(string authCode);
    bool ValidateAuthToken();
}

/// <summary>
/// Manages the authentication state of the application.
/// </summary>
public class AuthStateManager : IAuthStateManager
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<AuthStateManager>();
    private readonly HttpClient _httpClient;
    private readonly IUserSettings _userSettings;
    private string _codeVerifier;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthStateManager"/> class.
    /// </summary>
    /// <param name="userSettings">The user settings.</param>
    /// <param name="httpClient">The client used to make requests against the auth service.</param>
    public AuthStateManager(IUserSettings userSettings, HttpClient httpClient)
    {
        _userSettings = userSettings;
        _httpClient = httpClient;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authentication token.
    /// </summary>
    public string AuthToken => _userSettings.PathOfExileApiAuthToken;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the authentication state changes.
    /// </summary>
    public event EventHandler AuthStateChanged;

    /// <summary>
    /// Raises the <see cref="AuthStateChanged"/> event.
    /// </summary>
    protected virtual void OnAuthStateChanged()
    {
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Initiates the login process.
    /// </summary>
    public void Login(bool autoRedirect = true)
    {
        try
        {
            _userSettings.PoEAccountConnectionStatus = ConnectionStatusTypes.AttemptingLogin;
            OnAuthStateChanged();

            // Generate code verifier and state
            var codeVerifier = GenerateCodeVerifier();
            var state = GenerateState();

            // Needs to be persisted for the token request that will be made by the browser
            _codeVerifier = codeVerifier;

            // Generate code challenge from the code verifier
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            _log.Information($"Code Verifier: {codeVerifier}");
            _log.Information($"Code Challenge: {codeChallenge}");
            _log.Information($"State: {state}");

            // Open the URL in the default browser (disabled for unit tests)
            if (autoRedirect)
            {
                UrlUtilities.OpenUrl(AuthConfig.RedirectUri(state, codeChallenge));
            }
        }
        catch (Exception ex)
        {
            _log.Error($"Error initializing OAuth flow: {ex.Message}");
        }
    }

    /// <summary>
    /// Logs out the user and resets the authentication state.
    /// </summary>
    public void Logout()
    {
        // Reset user settings
        _userSettings.PathOfExileAccountName = string.Empty;
        _userSettings.PathOfExileApiAuthToken = string.Empty;

        // Reset connection status if not in the middle of an auth flow
        if (_userSettings.PoEAccountConnectionStatus != ConnectionStatusTypes.AttemptingLogin)
        {
            _userSettings.PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionNotValidated;
        }

        // Navigate the user back to the account tab in the settings nav
        _userSettings.SettingsWindowNavIndex = 0;

        // Notify subscribers about the auth state change
        OnAuthStateChanged();
    }

    /// <summary>
    /// Generates the authentication token using the provided authorization code.
    /// </summary>
    /// <param name="authCode">The authorization code.</param>
    /// <returns>The generated authentication token.</returns>
    public async Task<string> GenerateAuthToken(string authCode)
    {
        try
        {
            HttpResponseMessage response = await SendTokenRequest(authCode);
            string responseContent = await ProcessTokenResponse(response);

            if (!string.IsNullOrEmpty(responseContent))
            {
                AuthTokenResponse authTokenResponse = JsonSerializer.Deserialize<AuthTokenResponse>(responseContent);
                UpdateSettingsAndNotify(authTokenResponse);

                return responseContent;
            }
        }
        catch (Exception ex)
        {
            _log.Error("RetrieveAuthToken - Exception occurred: {ExceptionMessage}", ex.Message);
            HandleAuthTokenError();
        }

        return string.Empty;
    }

    /// <summary>
    /// Validates the authentication token.
    /// </summary>
    /// <returns><c>true</c> if the token is valid; otherwise, <c>false</c>.</returns>
    public bool ValidateAuthToken()
    {
        _log.Debug("Validating authentication token.");

        var isValid = !string.IsNullOrEmpty(_userSettings.PathOfExileApiAuthToken) && DateTime.UtcNow < _userSettings.PathOfExileApiAuthTokenExpiration;

        if (!isValid)
        {
            _log.Warning("Authentication token is invalid or expired. Initiating logout.");
            Logout();
            return false;
        }

        _log.Information("Authentication token validated successfully. Global state updated.");
        return true;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Sends the token request to retrieve the authentication token.
    /// </summary>
    /// <param name="authCode">The authorization code.</param>
    /// <returns>The HTTP response message.</returns>
    private async Task<HttpResponseMessage> SendTokenRequest(string authCode)
    {
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("code_verifier", _codeVerifier),
        ]);

        return await _httpClient.PostAsync(AuthConfig.OAuthTokenEndpoint, content);
    }

    /// <summary>
    /// Processes the token response and returns the response content.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The response content, or <c>null</c> if an error occurred.</returns>
    private async Task<string> ProcessTokenResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            _log.Information("RetrieveAuthToken - Token Response: {ResponseContent}", responseContent);
            return responseContent;
        }
        else
        {
            _log.Information("RetrieveAuthToken - Error retrieving token: {StatusCode}", response.StatusCode);
            HandleAuthTokenError();
            return null;
        }
    }

    /// <summary>
    /// Updates the user settings with the authentication token response and notifies subscribers.
    /// </summary>
    /// <param name="authTokenResponse">The authentication token response.</param>
    private void UpdateSettingsAndNotify(AuthTokenResponse authTokenResponse)
    {
        _log.Information("Updating user auth settings");

        _userSettings.PathOfExileAccountName = authTokenResponse.Username;
        _userSettings.PathOfExileApiAuthToken = authTokenResponse.AccessToken;
        _userSettings.PathOfExileApiAuthTokenExpiration = DateTime.UtcNow.AddHours(AuthConfig.DefaultTokenExpirationHours);
        _userSettings.PoEAccountConnectionStatus = ConnectionStatusTypes.ValidatedConnection;
        OnAuthStateChanged();

        _log.Information("Auth state changed, notified subscribers");
        _log.Information("Updated app global user settings: PathOfExileAccountName={PathOfExileAccountName}, PathOfExileApiAuthToken={PathOfExileApiAuthToken}, PathOfExileApiAuthTokenExpiration={PathOfExileApiAuthTokenExpiration}, PoEAccountConnectionStatus={PoEAccountConnectionStatus}",
            _userSettings.PathOfExileAccountName,
            _userSettings.PathOfExileApiAuthToken,
            _userSettings.PathOfExileApiAuthTokenExpiration,
            _userSettings.PoEAccountConnectionStatus
        );
    }

    /// <summary>
    /// Handles authentication token retrieval errors.
    /// </summary>
    private void HandleAuthTokenError()
    {
        _userSettings.PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionError;
        OnAuthStateChanged();
    }

    /// <summary>
    /// Generates a random code verifier.
    /// </summary>
    /// <returns>The generated code verifier.</returns>
    private static string GenerateCodeVerifier()
    {
        return StringUtilities.GenerateRandomString(128);
    }

    /// <summary>
    /// Generates a code challenge from the provided code verifier.
    /// </summary>
    /// <param name="codeVerifier">The code verifier.</param>
    /// <returns>The generated code challenge.</returns>
    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return UrlUtilities.Base64UrlEncode(Convert.ToBase64String(hash));
    }

    /// <summary>
    /// Generates a random state.
    /// </summary>
    /// <returns>The generated state.</returns>
    private static string GenerateState()
    {
        return StringUtilities.GenerateRandomString(32);
    }

    #endregion
}
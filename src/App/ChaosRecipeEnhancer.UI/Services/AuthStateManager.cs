using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Constants;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IAuthStateManager
{
    string AuthToken { get; }
    event EventHandler AuthStateChanged;
    void Login(bool autoRedirect = true);
    void Logout();
    Task<string> GenerateAuthToken(string authCode);
    bool ValidateAuthToken();
    public Task HandleAuthRedirection(string data);
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
            var codeVerifier = AuthUtilities.GenerateCodeVerifier();

            // generates random string of 16 characters to be our state
            var state = AuthUtilities.GenerateState();

            // Needs to be persisted for the token request that will be made by the browser
            _codeVerifier = codeVerifier;

            // Generate code challenge from the code verifier
            var codeChallenge = AuthUtilities.GenerateCodeChallenge(codeVerifier);
            var encodedCreClientVersion = UrlUtilities.Base64UrlEncode(AuthConfig.CreClientVersionAuthParam);

            _log.Information($"Code Verifier: {codeVerifier}");
            _log.Information($"Code Challenge: {codeChallenge}");
            _log.Information($"State: {state}");
            _log.Information($"Encoded Client Version: {encodedCreClientVersion}");

            // Open the URL in the default browser (disabled for unit tests)
            if (autoRedirect)
            {
                // note: the code challenge is based on the code verifier; the code verifier is a random string
                UrlUtilities.OpenUrl(AuthConfig.RedirectUri(state, codeChallenge, encodedCreClientVersion));
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
        _userSettings.PathOfExileApiAuthTokenExpiration = DateTime.MinValue;

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
            _log.Information("GenerateAuthToken - Token Request Response: {Response}", response);

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
            _log.Error("GenerateAuthToken - Exception occurred: {ExceptionMessage}", ex.Message);
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

        // I really REALLY need to implement a proper token validation here
        // I have sent out a request to the PoE API team to see if they can provide
        // my app with the `oath:introspect` scope so I can validate the token

        var isValid =
            !string.IsNullOrEmpty(_userSettings.PathOfExileApiAuthToken) &&
            DateTime.UtcNow < _userSettings.PathOfExileApiAuthTokenExpiration;

        if (!isValid)
        {
            _log.Warning("Authentication token is invalid or expired. Initiating logout.");
            Logout();
            return false;
        }

        _log.Information("Authentication token validated successfully. Global state updated.");
        return true;
    }

    /// <summary>
    /// Handles the redirection from the OAuth flow.
    /// </summary>
    /// <param name="data">The data received from the redirection.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAuthRedirection(string data)
    {
        _log.Information($"Pinged by other processes - Received data: {data}");

        if (!string.IsNullOrEmpty(data) && data.StartsWith(CreAppConstants.ProtocolPrefix))
        {
            var uri = new Uri(data);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            var authCode = queryParams["code"];
            var state = queryParams["state"];

            _log.Information("Auth Code: " + authCode);
            _log.Information("State: " + state);

            await GenerateAuthToken(authCode);
        }
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
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("code_verifier", _codeVerifier),
        });

            _log.Information($"Sending token request to: {AuthConfig.OAuthTokenEndpoint}");
            _log.Information($"Request content: {await content.ReadAsStringAsync()}");

            var result = await _httpClient.PostAsync(AuthConfig.OAuthTokenEndpoint, content);

            _log.Information($"Token request response status: {result.StatusCode}");
            _log.Information($"Token request response content: {await result.Content.ReadAsStringAsync()}");

            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error occurred while sending token request: {ex.Message}");
            _log.Error($"Exception details: {ex}");
            throw; // Re-throwing the exception to ensure that calling code is aware of the failure.
        }
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
            _log.Information("ProcessTokenResponse - Token Response: {ResponseContent}", responseContent);
            return responseContent;
        }
        else
        {
            _log.Information("ProcessTokenResponse - Error retrieving token: {StatusCode}", response.StatusCode);
            HandleAuthTokenError();
            return null;
        }
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
    /// Updates the user settings with the authentication token response and notifies subscribers.
    /// </summary>
    /// <param name="authTokenResponse">The authentication token response.</param>
    private void UpdateSettingsAndNotify(AuthTokenResponse authTokenResponse)
    {
        _log.Information("Updating user auth settings");

        // Calculate the expiration DateTime from now + expires_in seconds
        var tokenExpiration = DateTime.UtcNow.AddSeconds(authTokenResponse.ExpiresIn);

        _userSettings.PathOfExileAccountName = authTokenResponse.Username;
        _userSettings.PathOfExileApiAuthToken = authTokenResponse.AccessToken;
        _userSettings.PathOfExileApiAuthTokenExpiration = tokenExpiration;
        _userSettings.PoEAccountConnectionStatus = ConnectionStatusTypes.ValidatedConnection;
        OnAuthStateChanged();

        _log.Information("Auth state changed, notified subscribers");
        _log.Information("Updated app global user settings: PathOfExileAccountName={PathOfExileAccountName}, PathOfExileApiAuthToken={PathOfExileApiAuthToken}, PathOfExileApiAuthTokenExpiration={PathOfExileApiAuthTokenExpiration}, PoEAccountConnectionStatus={PoEAccountConnectionStatus}",
            _userSettings.PathOfExileAccountName,
            _userSettings.PathOfExileApiAuthToken,
            _userSettings.PathOfExileApiAuthTokenExpiration,
            _userSettings.PoEAccountConnectionStatus
        );

        // log tokenExpiration and PathofExileApiAuthTokenExpiration
        _log.Information("Token expiration: {TokenExpiration}", tokenExpiration);
        _log.Information("PathOfExileApiAuthTokenExpiration: {PathOfExileApiAuthTokenExpiration}", _userSettings.PathOfExileApiAuthTokenExpiration);

    }

    #endregion
}
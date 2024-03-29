using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using Moq;
using Moq.Protected;
using System.Net;

namespace ChaosRecipeEnhancer.UI.Tests.State;

public class AuthStateManagerTests
{
    private readonly Mock<IUserSettings> _userSettingsMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly AuthStateManager _authStateManager;

    public AuthStateManagerTests()
    {
        _userSettingsMock = new Mock<IUserSettings>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _authStateManager = new AuthStateManager(_userSettingsMock.Object, _httpClient);
    }

    [Fact]
    public void Login_ShouldSetConnectionStatusToAttemptingLogin()
    {
        // Act
        _authStateManager.Login(autoRedirect: false);

        // Assert
        _userSettingsMock.VerifySet(x => x.PoEAccountConnectionStatus = ConnectionStatusTypes.AttemptingLogin, Times.Once);
    }

    [Fact]
    public void Logout_ShouldResetUserSettings()
    {
        // Act
        _authStateManager.Logout();

        // Assert
        _userSettingsMock.VerifySet(x => x.PathOfExileAccountName = string.Empty, Times.Once);
        _userSettingsMock.VerifySet(x => x.PathOfExileApiAuthToken = string.Empty, Times.Once);
        _userSettingsMock.VerifySet(x => x.PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionNotValidated, Times.Once);
        _userSettingsMock.VerifySet(x => x.SettingsWindowNavIndex = 0, Times.Once);
    }

    [Fact]
    public async Task GenerateAuthToken_ShouldUpdateSettingsAndNotifyOnSuccess()
    {
        // Arrange
        var authCode = "sample_auth_code";
        var authTokenResponse = new AuthTokenResponse
        {
            Username = "sample_username",
            AccessToken = "sample_access_token",
            ExpiresIn = 3600
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(authTokenResponse))
            });

        // Act
        await _authStateManager.GenerateAuthToken(authCode);

        // Assert
        _userSettingsMock.VerifySet(x => x.PathOfExileAccountName = authTokenResponse.Username, Times.Once);
        _userSettingsMock.VerifySet(x => x.PathOfExileApiAuthToken = authTokenResponse.AccessToken, Times.Once);
        _userSettingsMock.VerifySet(x => x.PoEAccountConnectionStatus = ConnectionStatusTypes.ValidatedConnection, Times.Once);
    }

    [Fact]
    public void ValidateAuthToken_ShouldReturnTrueWhenTokenIsValid()
    {
        // Arrange
        _userSettingsMock.SetupGet(x => x.PathOfExileApiAuthToken).Returns("sample_token");
        _userSettingsMock.SetupGet(x => x.PathOfExileApiAuthTokenExpiration).Returns(DateTime.UtcNow.AddHours(1));

        // Act
        var result = _authStateManager.ValidateAuthToken();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateAuthToken_ShouldReturnFalseAndInitiateLogoutWhenTokenIsInvalid()
    {
        // Arrange
        _userSettingsMock.SetupGet(x => x.PathOfExileApiAuthToken).Returns(string.Empty);

        // Act
        var result = _authStateManager.ValidateAuthToken();

        // Assert
        Assert.False(result);
        _userSettingsMock.VerifySet(x => x.PathOfExileAccountName = string.Empty, Times.Once);
        _userSettingsMock.VerifySet(x => x.PathOfExileApiAuthToken = string.Empty, Times.Once);
        _userSettingsMock.VerifySet(x => x.PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionNotValidated, Times.Once);
    }
}
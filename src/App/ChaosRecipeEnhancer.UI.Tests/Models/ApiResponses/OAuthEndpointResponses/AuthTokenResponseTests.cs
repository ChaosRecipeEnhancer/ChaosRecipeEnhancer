using ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses.OAuthEndpointResponses;

public class AuthTokenResponseTests
{
    [Fact]
    public void Deserialization_FromJson_PopulatesPropertiesCorrectly()
    {
        // Arrange
        var json = "{\"access_token\":\"abc123\",\"expires_in\":3600,\"token_type\":\"Bearer\",\"scope\":\"user\",\"sub\":\"123456\",\"username\":\"testuser\",\"refresh_token\":\"xyz789\"}";

        // Act
        var authTokenResponse = JsonSerializer.Deserialize<AuthTokenResponse>(json);

        // Assert
        authTokenResponse.Should().NotBeNull();
        authTokenResponse!.AccessToken.Should().Be("abc123");
        authTokenResponse.ExpiresIn.Should().Be(3600);
        authTokenResponse.TokenType.Should().Be("Bearer");
        authTokenResponse.Scope.Should().Be("user");
        authTokenResponse.Sub.Should().Be("123456");
        authTokenResponse.Username.Should().Be("testuser");
        authTokenResponse.RefreshToken.Should().Be("xyz789");
    }

    [Fact]
    public void Serialization_ToJson_GeneratesExpectedJson()
    {
        // Arrange
        var authTokenResponse = new AuthTokenResponse
        {
            AccessToken = "abc123",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Scope = "user",
            Sub = "123456",
            Username = "testuser",
            RefreshToken = "xyz789"
        };
        var expectedJsonPart = "\"access_token\":\"abc123\",\"expires_in\":3600,\"token_type\":\"Bearer\",\"scope\":\"user\",\"sub\":\"123456\",\"username\":\"testuser\",\"refresh_token\":\"xyz789\"";

        // Act
        var json = JsonSerializer.Serialize(authTokenResponse);

        // Assert
        json.Should().Contain(expectedJsonPart);
    }

    [Fact]
    public void RoundTrip_SerializationAndDeserialization_RetainsAllPropertyValues()
    {
        // Arrange: Create and set up the original object
        var originalAuthTokenResponse = new AuthTokenResponse
        {
            AccessToken = "abc123",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Scope = "user",
            Sub = "123456",
            Username = "testuser",
            RefreshToken = "xyz789"
        };

        // Act: Serialize the object to JSON, then deserialize that JSON back to a new object
        var json = JsonSerializer.Serialize(originalAuthTokenResponse);
        var deserializedAuthTokenResponse = JsonSerializer.Deserialize<AuthTokenResponse>(json);

        // Assert: The deserialized object should match the original object in all property values
        deserializedAuthTokenResponse.Should().BeEquivalentTo(originalAuthTokenResponse);
    }
}
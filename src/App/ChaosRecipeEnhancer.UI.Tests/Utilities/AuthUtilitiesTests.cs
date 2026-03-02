using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class AuthUtilitiesTests
{
    [Fact]
    public void GenerateCodeVerifier_WhenCalled_Returns128CharString()
    {
        // Arrange & Act
        var result = AuthUtilities.GenerateCodeVerifier();

        // Assert
        result.Should().HaveLength(128);
    }

    [Fact]
    public void GenerateCodeVerifier_WhenCalledTwice_ReturnsDifferentValues()
    {
        // Arrange & Act
        var first = AuthUtilities.GenerateCodeVerifier();
        var second = AuthUtilities.GenerateCodeVerifier();

        // Assert
        first.Should().NotBe(second);
    }

    [Fact]
    public void GenerateCodeChallenge_GivenCodeVerifier_ReturnsBase64UrlEncodedSha256()
    {
        // Arrange
        var codeVerifier = AuthUtilities.GenerateCodeVerifier();

        // Act
        var result = AuthUtilities.GenerateCodeChallenge(codeVerifier);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain("+");
        result.Should().NotContain("/");
        result.Should().NotContain("=");
    }

    [Fact]
    public void GenerateState_WhenCalled_Returns32CharString()
    {
        // Arrange & Act
        var result = AuthUtilities.GenerateState();

        // Assert
        result.Should().HaveLength(32);
    }
}

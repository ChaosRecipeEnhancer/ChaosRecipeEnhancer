using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class UrlUtilitiesTests
{
    [Fact]
    public void Base64UrlEncode_GivenStringWithPlus_ReplacesPlusWithDash()
    {
        // Arrange
        var input = "abc+def";

        // Act
        var result = UrlUtilities.Base64UrlEncode(input);

        // Assert
        result.Should().NotContain("+");
        result.Should().Contain("-");
        result.Should().Be("abc-def");
    }

    [Fact]
    public void Base64UrlEncode_GivenStringWithSlash_ReplacesSlashWithUnderscore()
    {
        // Arrange
        var input = "abc/def";

        // Act
        var result = UrlUtilities.Base64UrlEncode(input);

        // Assert
        result.Should().NotContain("/");
        result.Should().Contain("_");
        result.Should().Be("abc_def");
    }

    [Fact]
    public void Base64UrlEncode_GivenStringWithTrailingEquals_TrimsEquals()
    {
        // Arrange
        var input = "abc===";

        // Act
        var result = UrlUtilities.Base64UrlEncode(input);

        // Assert
        result.Should().NotContain("=");
        result.Should().Be("abc");
    }

    [Fact]
    public void Base64UrlEncode_GivenComplexString_EncodesCorrectly()
    {
        // Arrange
        var input = "abc+def/ghi==";

        // Act
        var result = UrlUtilities.Base64UrlEncode(input);

        // Assert
        result.Should().NotContain("+");
        result.Should().NotContain("/");
        result.Should().NotContain("=");
        result.Should().Be("abc-def_ghi");
    }
}

using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class StringUtilitiesTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(128)]
    public void GenerateRandomString_GivenLength_ReturnsStringOfCorrectLength(int length)
    {
        // Arrange & Act
        var result = StringUtilities.GenerateRandomString(length);

        // Assert
        result.Should().HaveLength(length);
    }

    [Fact]
    public void GenerateRandomString_WhenCalledTwice_ReturnsDifferentValues()
    {
        // Arrange & Act
        var first = StringUtilities.GenerateRandomString(32);
        var second = StringUtilities.GenerateRandomString(32);

        // Assert
        first.Should().NotBe(second);
    }
}

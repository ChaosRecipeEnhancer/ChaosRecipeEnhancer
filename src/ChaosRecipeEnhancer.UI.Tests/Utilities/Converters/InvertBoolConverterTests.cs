using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class InvertBoolConverterTests
{
    [Fact]
    public void Convert_GivenTrue_ReturnsFalse()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(false);
    }
    
    [Fact]
    public void Convert_GivenFalse_ReturnsTrue()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        var result = converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void Convert_GivenNonBool_ThrowsArgumentException()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        Action act = () => converter.Convert(1, null, null, null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void ConvertBack_GivenTrue_ReturnsFalse()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        var result = converter.ConvertBack(true, null, null, null);

        // Assert
        result.Should().Be(false);
    }
    
    [Fact]
    public void ConvertBack_GivenFalse_ReturnsTrue()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        var result = converter.ConvertBack(false, null, null, null);

        // Assert
        result.Should().Be(true);
    }
    
    [Fact]
    public void ConvertBack_GivenNonBool_ThrowsArgumentException()
    {
        // Arrange
        var converter = new InvertBoolConverter();

        // Act
        Action act = () => converter.ConvertBack(1, null, null, null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
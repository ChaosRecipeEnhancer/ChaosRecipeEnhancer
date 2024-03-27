using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class BoolToObjectConverterTests
{
    [Fact]
    public void Convert_True_ReturnsTrueValue()
    {
        // Arrange
        var converter = new BoolToObjectConverter { TrueValue = "Yes", FalseValue = "No" };

        // Act
        var result = converter.Convert(true, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Yes", result);
    }

    [Fact]
    public void Convert_False_ReturnsFalseValue()
    {
        // Arrange
        var converter = new BoolToObjectConverter { TrueValue = "Yes", FalseValue = "No" };

        // Act
        var result = converter.Convert(false, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("No", result);
    }

    [Fact]
    public void ConvertBack_TrueValue_ReturnsTrue()
    {
        // Arrange
        var converter = new BoolToObjectConverter { TrueValue = "Yes", FalseValue = "No" };

        // Act
        var result = converter.ConvertBack("Yes", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void ConvertBack_FalseValue_ReturnsFalse()
    {
        // Arrange
        var converter = new BoolToObjectConverter { TrueValue = "Yes", FalseValue = "No" };

        // Act
        var result = converter.ConvertBack("No", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    // Old Tests; Still valid
    private readonly BoolToObjectConverter _converter = new() { TrueValue = true, FalseValue = false };

    [Fact]
    public void Convert_GivenTrue_ReturnsTrueValue()
    {
        // Arrange & Act
        var result = _converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(_converter.TrueValue);
    }

    [Fact]
    public void Convert_GivenFalse_ReturnsFalseValue()
    {
        // Arrange & Act
        var result = _converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(_converter.FalseValue);
    }

    [Fact]
    public void Convert_GivenNonBoolValue_ReturnsFalseValue()
    {
        // Arrange & Act
        var result = _converter.Convert("NotABool", null, null, null);

        // Assert
        result.Should().Be(_converter.FalseValue);
    }

    [Fact]
    public void ConvertBack_GivenValue_ReturnsTrueValue()
    {
        // Arrange & Act
        var result = _converter.ConvertBack(true, null, null, null);

        // Assert
        result.Should().Be(_converter.TrueValue);
    }
}

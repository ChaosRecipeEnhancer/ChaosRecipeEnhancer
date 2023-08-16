using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class BoolToObjectConverterTests
{
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
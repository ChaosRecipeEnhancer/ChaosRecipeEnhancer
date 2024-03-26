using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class MultiBoolToBoolAndConverterTests
{
    [Fact]
    public void Convert_AllTrue_ReturnsTrue()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();
        var values = new object[] { true, true, true };

        // Act
        var result = converter.Convert(values, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Convert_OneFalse_ReturnsFalse()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();
        var values = new object[] { true, false, true };

        // Act
        var result = converter.Convert(values, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_GivenTrueAndTrue_ReturnsTrue()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();

        // Act
        var result = converter.Convert(new object[] { true, true }, null, null, null);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void Convert_GivenTrueAndFalse_ReturnsFalse()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();

        // Act
        var result = converter.Convert(new object[] { true, false }, null, null, null);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_GivenFalseAndTrue_ReturnsFalse()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();

        // Act
        var result = converter.Convert(new object[] { false, true }, null, null, null);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_GivenFalseAndFalse_ReturnsFalse()
    {
        // Arrange
        var converter = new MultiBoolToBoolAndConverter();

        // Act
        var result = converter.Convert(new object[] { false, false }, null, null, null);

        // Assert
        result.Should().Be(false);
    }
}

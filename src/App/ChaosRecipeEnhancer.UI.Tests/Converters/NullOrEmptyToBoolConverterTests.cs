using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;

namespace ChaosRecipeEnhancer.Tests.Converters;

public class NullOrEmptyToBoolConverterTests
{
    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        // Arrange
        var converter = new NullOrEmptyToBoolConverter();

        // Act
        var result = converter.Convert(null, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsFalse()
    {
        // Arrange
        var converter = new NullOrEmptyToBoolConverter();

        // Act
        var result = converter.Convert(string.Empty, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_NonEmptyString_ReturnsTrue()
    {
        // Arrange
        var converter = new NullOrEmptyToBoolConverter();

        // Act
        var result = converter.Convert("Hello", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Convert_NonNullObject_ReturnsTrue()
    {
        // Arrange
        var converter = new NullOrEmptyToBoolConverter();
        var obj = new object();

        // Act
        var result = converter.Convert(obj, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new NullOrEmptyToBoolConverter();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(true, null, null, CultureInfo.InvariantCulture));
    }
}
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class EncryptionUtilitiesTests
{
    [Fact]
    public void EncryptString_ThenDecryptString_ReturnsOriginalInput()
    {
        // Arrange
        var originalInput = "This is a secret message";

        // Act
        var encrypted = EncryptionUtilities.EncryptString(originalInput);
        var decrypted = EncryptionUtilities.DecryptString(encrypted);

        // Assert
        decrypted.Should().Be(originalInput);
    }

    [Fact]
    public void DecryptString_GivenInvalidInput_ReturnsEmptyString()
    {
        // Arrange
        var invalidInput = "this-is-not-valid-encrypted-data";

        // Act
        var result = EncryptionUtilities.DecryptString(invalidInput);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EncryptString_GivenDifferentInputs_ReturnsDifferentOutputs()
    {
        // Arrange
        var input1 = "Secret message 1";
        var input2 = "Secret message 2";

        // Act
        var encrypted1 = EncryptionUtilities.EncryptString(input1);
        var encrypted2 = EncryptionUtilities.EncryptString(input2);

        // Assert
        encrypted1.Should().NotBe(encrypted2);
    }
}

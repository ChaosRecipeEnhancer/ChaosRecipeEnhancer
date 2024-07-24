using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses.Shared;

public class BaseItemInfluencesTests
{
    [Fact]
    public void Serialization_WithAllFalse_ShouldMatchExpectedJson()
    {
        // Arrange
        var influences = new BaseItemInfluences();
        var expectedJson = "{\"shaper\":false,\"elder\":false,\"crusader\":false,\"redeemer\":false,\"hunter\":false,\"warlord\":false}";

        // Act
        var json = JsonSerializer.Serialize(influences);

        // Assert
        json.Should().Be(expectedJson);
    }

    [Fact]
    public void Deserialization_FromJson_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var json = "{\"shaper\":true,\"elder\":true,\"crusader\":false,\"redeemer\":true,\"hunter\":false,\"warlord\":true}";

        // Act
        var influences = JsonSerializer.Deserialize<BaseItemInfluences>(json);

        // Assert
        influences.Should().NotBeNull();
        influences!.Shaper.Should().BeTrue();
        influences.Elder.Should().BeTrue();
        influences.Crusader.Should().BeFalse();
        influences.Redeemer.Should().BeTrue();
        influences.Hunter.Should().BeFalse();
        influences.Warlord.Should().BeTrue();
    }
}
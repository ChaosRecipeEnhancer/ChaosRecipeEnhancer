using ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses.OAuthEndpointResponses;

public class BaseStashTabMetadataTests
{
    [Fact]
    public void Deserialization_ShouldCorrectlySetProperties()
    {
        // Arrange
        var json = "{\"id\":\"85d828223b\",\"name\":\"Chaos Recipe\",\"type\":\"PremiumStash\",\"index\":0,\"children\":[],\"items\":[]}";

        // Act
        var stashTab = JsonSerializer.Deserialize<BaseStashTabMetadata>(json);

        // Assert
        stashTab.Should().NotBeNull();
        stashTab!.Id.Should().Be("85d828223b");
        stashTab.Name.Should().Be("Chaos Recipe");
        stashTab.Type.Should().Be("PremiumStash");
        stashTab.Index.Should().Be(0);
        stashTab.Children.Should().BeEmpty();
        stashTab.Items.Should().BeEmpty();
    }

    [Fact]
    public void Serialization_IncludesAllProperties()
    {
        // Arrange
        var stashTab = new BaseStashTabMetadata
        {
            Id = "85d828223b",
            Name = "Chaos Recipe",
            Type = "PremiumStash",
            Index = 0,
            Children = [],
            Items = []
        };
        var expectedJsonPart = "\"id\":\"85d828223b\",\"name\":\"Chaos Recipe\",\"type\":\"PremiumStash\",\"index\":0";

        // Act
        var json = JsonSerializer.Serialize(stashTab);

        // Assert
        json.Should().Contain(expectedJsonPart);
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var stashTab = new BaseStashTabMetadata
        {
            Index = 0,
            Name = "Chaos Recipe"
        };
        var expectedString = "Chaos Recipe";

        // Act
        var result = stashTab.ToString();

        // Assert
        result.Should().Be(expectedString);
    }
}

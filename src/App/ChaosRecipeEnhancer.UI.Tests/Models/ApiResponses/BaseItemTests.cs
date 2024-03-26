using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses;

public class BaseItemTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var width = 2u;
        var height = 3u;
        var identified = true;
        int? itemLevel = 84;
        var frameType = ItemFrameType.Gem;
        var x = 11;
        var y = 22;
        var baseItemInfluences = new BaseItemInfluences(); // Assume we have a default constructor or a mock
        var icon = "http://example.com/icon.png";

        // Act
        var item = new BaseItem(width, height, identified, itemLevel, frameType, x, y, baseItemInfluences, icon);

        // Assert
        item.Width.Should().Be(width);
        item.Height.Should().Be(height);
        item.Identified.Should().Be(identified);
        item.ItemLevel.Should().Be(itemLevel);
        item.FrameType.Should().Be(frameType);
        item.X.Should().Be(x);
        item.Y.Should().Be(y);
        item.BaseItemInfluences.Should().Be(baseItemInfluences);
        item.Icon.Should().Be(icon);
    }

    [Fact]
    public void CopyConstructor_CreatesExactCopy()
    {
        // Arrange
        var original = new BaseItem
        {
            Width = 2u,
            Height = 3u,
            Identified = true,
            ItemLevel = 84,
            FrameType = ItemFrameType.Gem,
            X = 11,
            Y = 22,
            BaseItemInfluences = new BaseItemInfluences(), // Assume we have a mock or a default
            Icon = "http://example.com/icon.png"
        };

        // Act
        var copy = new BaseItem(original);

        // Assert
        copy.Should().BeEquivalentTo(original, options => options.ComparingByMembers<BaseItem>());
    }

    [Fact]
    public void Serialization_ProducesCorrectJson()
    {
        // Arrange
        var item = new BaseItem
        {
            Width = 2,
            Height = 3,
            Identified = true,
            ItemLevel = 84,
            FrameType = ItemFrameType.Magic,
            X = 11,
            Y = 22,
            Icon = "http://example.com/icon.png"
        };
        var expectedJson = "{\"w\":2,\"h\":3,\"identified\":true,\"ilvl\":84,\"frameType\":1,\"x\":11,\"y\":22,\"influences\":null,\"icon\":\"http://example.com/icon.png\"}";

        // Act
        var json = JsonSerializer.Serialize(item);

        // Assert
        json.Should().Be(expectedJson);
    }

    [Fact]
    public void Deserialization_ProducesCorrectObject()
    {
        // Arrange
        var json = "{\"w\":2,\"h\":3,\"identified\":true,\"ilvl\":84,\"frameType\":1,\"x\":11,\"y\":22,\"influences\":{},\"icon\":\"http://example.com/icon.png\"}";

        // Act
        var item = JsonSerializer.Deserialize<BaseItem>(json);

        // Assert
        item.Should().NotBeNull();
        item!.Width.Should().Be(2);
        item.Height.Should().Be(3);
        item.Identified.Should().BeTrue();
        item.ItemLevel.Should().Be(84);
        item.FrameType.Should().Be(ItemFrameType.Magic);
        item.X.Should().Be(11);
        item.Y.Should().Be(22);
        item.Icon.Should().Be("http://example.com/icon.png");
    }
}

using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses;

public class GetStashResponseTests
{
    [Fact]
    public void Deserialization_FromJson_PopulatesStashPropertyCorrectly()
    {
        // Arrange
        var json = "{\"stash\":{\"id\":\"123\",\"name\":\"Test Stash\",\"type\":\"PremiumStash\",\"index\":0,\"children\":[],\"items\":[]}}";

        // Act
        var response = JsonSerializer.Deserialize<GetStashResponse>(json);

        // Assert
        response.Should().NotBeNull();
        response!.Stash.Should().NotBeNull();
        response.Stash.Id.Should().Be("123");
        response.Stash.Name.Should().Be("Test Stash");
        response.Stash.Type.Should().Be("PremiumStash");
        response.Stash.Index.Should().Be(0);
        response.Stash.Children.Should().BeEmpty();
        response.Stash.Items.Should().BeEmpty();
    }

    [Fact]
    public void Serialization_ToJson_RepresentsStashPropertyCorrectly()
    {
        // Arrange
        var response = new GetStashResponse
        {
            Stash = new BaseStashTabMetadata
            {
                Id = "123",
                Name = "Test Stash",
                Type = "PremiumStash",
                Index = 0,
                Children = [],
                Items = []
            }
        };
        var expectedJsonSubstring = "\"stash\":{\"id\":\"123\",\"name\":\"Test Stash\",\"type\":\"PremiumStash\",\"index\":0";

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        json.Should().Contain(expectedJsonSubstring);
    }

    [Fact]
    public void RoundTrip_SerializationAndDeserialization_RetainsAllPropertyValues()
    {
        // Arrange
        var originalResponse = new GetStashResponse
        {
            Stash = new BaseStashTabMetadata
            {
                Id = "123",
                Name = "Test Stash",
                Type = "PremiumStash",
                Index = 0,
                Children = [],
                Items = []
            }
        };

        // Act
        var json = JsonSerializer.Serialize(originalResponse);
        var deserializedResponse = JsonSerializer.Deserialize<GetStashResponse>(json);

        // Assert
        deserializedResponse.Should().BeEquivalentTo(originalResponse);
    }
}
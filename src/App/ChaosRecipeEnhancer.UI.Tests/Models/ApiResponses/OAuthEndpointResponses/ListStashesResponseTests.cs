using ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses.OAuthEndpointResponses;

public class ListStashesResponseTests
{
    [Fact]
    public void Deserialization_WithEmptyList_SetsStashTabsToEmpty()
    {
        var json = @"{ ""stashes"": [] }";

        var response = JsonSerializer.Deserialize<ListStashesResponse>(json);

        response.Should().NotBeNull();
        response!.StashTabs.Should().BeEmpty();
    }

    [Fact]
    public void Deserialization_WithNonEmptyList_PopulatesStashTabsCorrectly()
    {
        // Simplified JSON for a single stash tab for brevity. Extend as needed.
        var json = @"{
            ""stashes"": [
                { ""id"": ""1"", ""name"": ""Tab 1"", ""type"": ""PremiumStash"", ""index"": 0, ""items"": [] },
                { ""id"": ""2"", ""name"": ""Tab 2"", ""type"": ""QuadStash"", ""index"": 1, ""items"": [] }
            ]
        }";

        var response = JsonSerializer.Deserialize<ListStashesResponse>(json);

        response.Should().NotBeNull();
        response!.StashTabs.Should().HaveCount(2);
        response.StashTabs[0].Id.Should().Be("1");
        response.StashTabs[1].Id.Should().Be("2");
        response.StashTabs[0].Name.Should().Be("Tab 1");
        response.StashTabs[1].Name.Should().Be("Tab 2");
        response.StashTabs[0].Type.Should().Be("PremiumStash");
        response.StashTabs[1].Type.Should().Be("QuadStash");
        response.StashTabs[0].Index.Should().Be(0);
        response.StashTabs[1].Index.Should().Be(1);
    }

    [Fact]
    public void Deserialization_WithNullStashes_SetsStashTabsToNull()
    {
        var json = @"{ ""stashes"": null }";

        var response = JsonSerializer.Deserialize<ListStashesResponse>(json);

        response!.StashTabs.Should().BeNull();
    }

    [Fact]
    public void RoundTripSerializationAndDeserialization_RetainsAllValues()
    {
        var originalResponse = new ListStashesResponse
        {
            StashTabs =
            [
                new BaseStashTabMetadata { Id = "1", Name = "Tab 1", Type = "PremiumStash", Index = 0 },
                new BaseStashTabMetadata { Id = "2", Name = "Tab 2", Type = "QuadStash", Index = 1 }
            ]
        };

        var json = JsonSerializer.Serialize(originalResponse);
        var deserializedResponse = JsonSerializer.Deserialize<ListStashesResponse>(json);

        deserializedResponse.Should().BeEquivalentTo(originalResponse);
    }

}

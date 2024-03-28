using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using System.Text.Json;

namespace ChaosRecipeEnhancer.UI.Tests.Models.ApiResponses;

public class LeagueResponseTests
{
    [Fact]
    public void Deserialization_ShouldCorrectlyPopulateProperties()
    {
        // JSON string shortened for brevity, use full JSON in actual test
        var json = @"{
            ""leagues"": [
                {
                    ""id"": ""Standard"",
                    ""realm"": ""pc"",
                    ""category"": { ""id"": ""Standard"" },
                    ""rules"": []
                }
            ]
        }";

        // Act
        var response = JsonSerializer.Deserialize<LeagueResponse>(json);

        // Assert
        response.Should().NotBeNull();
        response!.Leagues.Should().HaveCountGreaterThan(0);
        var firstLeague = response.Leagues[0];
        firstLeague.Should().NotBeNull();
        firstLeague.Id.Should().Be("Standard");
        firstLeague.Realm.Should().Be("pc");
        firstLeague.Category.Id.Should().Be("Standard");
        firstLeague.Rules.Should().BeEmpty();
    }

    [Fact]
    public void Deserialization_WithOptionalAndNullValues_HandlesThemCorrectly()
    {
        // Simplified JSON to highlight optional and null properties
        var json = @"{
        ""leagues"": [
                {
                    ""id"": ""TestLeague"",
                    ""endAt"": null,
                    ""privateLeagueUrl"": null,
                    ""rules"": []
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<LeagueResponse>(json);

        response.Should().NotBeNull();
        var league = response!.Leagues[0];
        league.EndAt.Should().BeNull();
        league.PrivateLeagueUrl.Should().BeNull();
        league.Rules.Should().BeEmpty();
    }

    [Fact]
    public void Deserialization_ParsesDateTimePropertiesCorrectly()
    {
        var json = @"{
            ""leagues"": [
                {
                    ""id"": ""DateTimeLeague"",
                    ""startAt"": ""2023-12-08T19:00:00Z"",
                    ""registerAt"": ""2023-12-08T16:00:00Z""
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<LeagueResponse>(json);

        var league = response!.Leagues[0];
        league.StartAt.Should().Be(new DateTime(2023, 12, 8, 19, 0, 0, DateTimeKind.Utc));
        league.RegisterAt.Should().Be(new DateTime(2023, 12, 8, 16, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Deserialization_WithRulesAndCategories_PopulatesThemCorrectly()
    {
        var json = @"{
        ""leagues"": [
                {
                    ""id"": ""RulesTestLeague"",
                    ""rules"": [
                        { ""id"": ""Hardcore"", ""name"": ""Hardcore"", ""description"": ""A character..."" }
                    ],
                    ""category"": { ""id"": ""Standard"", ""current"": true }
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<LeagueResponse>(json);

        var league = response!.Leagues[0];
        league.Rules.Should().ContainSingle();
        league.Rules[0].Id.Should().Be("Hardcore");
        league.Category.Should().NotBeNull();
        league.Category.Current.Should().BeTrue();
    }

    [Fact]
    public void RoundTripSerializationAndDeserialization_RetainsAllValues()
    {
        var originalLeague = new LeagueResponse
        {
            Leagues =
            [
                new League
                {
                    Id = "RoundTripLeague",
                    Realm = "pc",
                    StartAt = new DateTime(2023, 12, 8, 19, 0, 0, DateTimeKind.Utc),
                    Rules =
                    [
                        new Rule { Id = "Solo", Name = "Solo", Description = "No parties allowed." }
                    ],
                    Category = new Category { Id = "Challenge", Current = false }
                }
            ]
        };

        var json = JsonSerializer.Serialize(originalLeague);
        var deserializedLeague = JsonSerializer.Deserialize<LeagueResponse>(json);

        deserializedLeague.Should().BeEquivalentTo(originalLeague);
    }

}
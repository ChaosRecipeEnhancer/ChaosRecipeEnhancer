using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses;

public class LeagueResponse
{
    [JsonPropertyName("leagues")]
    public List<League> Leagues { get; set; }
}

public class League
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("realm")]
    public string Realm { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("startAt")]
    public DateTime? StartAt { get; set; }

    [JsonPropertyName("endAt")]
    public DateTime? EndAt { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("category")]
    public Category Category { get; set; }

    [JsonPropertyName("registerAt")]
    public DateTime? RegisterAt { get; set; }

    [JsonPropertyName("delveEvent")]
    public bool DelveEvent { get; set; }

    [JsonPropertyName("rules")]
    public List<Rule> Rules { get; set; }

    // This property is only present in some league entries.
    [JsonPropertyName("privateLeagueUrl")]
    public string PrivateLeagueUrl { get; set; }
}

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    // Optional, as it's not present in all categories.
    [JsonPropertyName("current")]
    public bool? Current { get; set; }
}

public class Rule
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

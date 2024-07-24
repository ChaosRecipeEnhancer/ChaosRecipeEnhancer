namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.SessionIdEndpointResponses;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AccountAvatarResponse
{
    [JsonPropertyName("collection")]
    public List<AccountAvatarItem> Collection { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class AccountAvatarItem
{
    [JsonPropertyName("custom")]
    public bool Custom { get; set; }

    [JsonPropertyName("avatar_id")]
    public int AvatarId { get; set; }

    [JsonPropertyName("image")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("current")]
    public bool Current { get; set; }
}
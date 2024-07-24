using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public ErrorDetails Error { get; set; }
}

public class ErrorDetails
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
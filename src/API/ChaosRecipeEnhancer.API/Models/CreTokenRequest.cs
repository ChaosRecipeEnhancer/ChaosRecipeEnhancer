namespace ChaosRecipeEnhancer.API.Models;

public class CreTokenRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ClientId { get; set; }
    public string AccessToken { get; set; }
    public string Version { get; set; }
}
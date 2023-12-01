namespace ChaosRecipeEnhancer.UI.ServiceClients;

public interface IServiceClient : ICanAuthenticate
{
    string BaseUrl { get; set; }
}

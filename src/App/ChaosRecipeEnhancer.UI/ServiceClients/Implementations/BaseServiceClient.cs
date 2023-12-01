using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.ServiceClients;

public class BaseServiceClient : ICanAuthenticate
{
    public string BearerToken { get; set; }

    public Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
    {
        var msg = new HttpRequestMessage();

        // set the bearer token
        msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        return Task.FromResult(msg);
    }
}

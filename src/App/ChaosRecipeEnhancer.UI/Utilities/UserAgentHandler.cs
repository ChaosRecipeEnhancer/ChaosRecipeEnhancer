using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Utilities;

public class UserAgentHandler : DelegatingHandler
{
    private readonly string _userAgent;

    public UserAgentHandler(string userAgent)
    {
        _userAgent = userAgent;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.UserAgent.ParseAdd(_userAgent);
        return await base.SendAsync(request, cancellationToken);
    }
}
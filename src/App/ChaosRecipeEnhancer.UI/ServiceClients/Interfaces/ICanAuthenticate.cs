using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.ServiceClients;

public interface ICanAuthenticate
{
    string BearerToken { get; set; }
    Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken);
}

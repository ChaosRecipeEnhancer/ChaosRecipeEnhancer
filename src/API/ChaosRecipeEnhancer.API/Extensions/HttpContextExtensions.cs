using Amazon.Lambda.APIGatewayEvents;

namespace ChaosRecipeEnhancer.API.Extensions;

/// <summary>
/// Provides extension methods for the <s cref="HttpContext"/> class.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the API Gateway Proxy Request from the HttpContext.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <returns>The APIGatewayProxyRequest or null if not found.</returns>
    public static APIGatewayProxyRequest? GetAPIGatewayProxyRequest(this HttpContext context)
    {
        var apiGatewayRequest =
            context
                ?.Items?.FirstOrDefault(
                    x =>
                        x.Key.Equals(
                            Amazon // Use Equals instead of == for string comparison
                                .Lambda
                                .AspNetCoreServer
                                .AbstractAspNetCoreFunction
                                .LAMBDA_REQUEST_OBJECT
                        )
                )
                .Value as APIGatewayProxyRequest;
        return apiGatewayRequest;
    }
}

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Tracing;

namespace ChaosRecipeEnhancer.API;

/// <summary>
/// Provides logging functionality for the ChaosRecipeEnhancer API using AWS Lambda Powertools.
/// </summary>
public sealed class PowerToolsLogger
{
    /// <summary>
    /// Logs the context of the APIGatewayProxyRequest and ILambdaContext using the provided transactionId.
    /// </summary>
    /// <param name="lambdaEvent">The APIGatewayProxyRequest object.</param>
    /// <param name="context">The ILambdaContext object.</param>
    /// <param name="transactionId">The transaction ID to log.</param>
    [Logging(CorrelationIdPath = CorrelationIdPaths.ApiGatewayRest)]
    public static void LogContext(
        APIGatewayProxyRequest? lambdaEvent,
        ILambdaContext context,
        string transactionId
    )
    {
        Logger.AppendKey("transactionId", transactionId);
        Tracing.AddAnnotation("transactionId", transactionId);
    }
}

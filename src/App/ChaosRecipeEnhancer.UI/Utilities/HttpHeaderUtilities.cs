using ChaosRecipeEnhancer.UI.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for handling HTTP headers, specifically for PoE API responses.
/// </summary>
public static class HttpHeaderUtilities
{
    /// <summary>
    /// Extracts rate limit headers from an HTTP response and updates the global rate limit state.
    /// </summary>
    /// <param name="response">The HttpResponseMessage containing the headers.</param>
    /// <param name="log">The logger instance for logging debug messages.</param>
    /// <param name="requestUri">The original request URI for context in logging.</param>
    public static void ProcessRateLimitHeaders(HttpResponseMessage response, ILogger log, Uri requestUri)
    {
        // Basic null checks for robustness
        if (response == null)
        {
            log?.Warning("ProcessRateLimitHeaders called with null response for {RequestUri}", requestUri);
            return;
        }

        string rateLimit = null;
        if (response.Headers.TryGetValues("X-Rate-Limit-Account", out IEnumerable<string> rateLimitValues))
        {
            rateLimit = rateLimitValues.FirstOrDefault();
        }

        string rateLimitState = null;
        if (response.Headers.TryGetValues("X-Rate-Limit-Account-State", out IEnumerable<string> rateLimitStateValues))
        {
            rateLimitState = rateLimitStateValues.FirstOrDefault();
        }

        string resultTime = null;
        if (response.Headers.TryGetValues("Date", out IEnumerable<string> resultTimeValues))
        {
            resultTime = resultTimeValues.FirstOrDefault();
        }

        // Update GlobalRateLimitState only if headers were found
        if (!string.IsNullOrEmpty(rateLimitState))
        {
            // Pass rateLimit even if null, as DeserializeRateLimits should handle it
            GlobalRateLimitState.DeserializeRateLimits(rateLimit, rateLimitState);
        }
        else
        {
            log?.Debug("Rate limit state header not found for {RequestUri}", requestUri);
        }

        if (!string.IsNullOrEmpty(resultTime))
        {
            GlobalRateLimitState.DeserializeResponseSeconds(resultTime);
        }
        else
        {
            log?.Debug("Date header not found for {RequestUri}", requestUri);
        }
    }
}

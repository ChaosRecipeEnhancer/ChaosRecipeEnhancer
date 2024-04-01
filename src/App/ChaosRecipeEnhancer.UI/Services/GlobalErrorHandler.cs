using ChaosRecipeEnhancer.UI.Windows;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;

namespace ChaosRecipeEnhancer.UI.Services;

public static class GlobalErrorHandler
{
    private static readonly ILogger _log = Log.ForContext(typeof(GlobalErrorHandler));
    private static bool _errorAlreadyShown;

    public static void Spawn(string content, string title, string preamble = null)
    {
        var errorDialog = new ErrorWindow(title, content, preamble);
        errorDialog.ShowDialog();
    }

    #region API Error Handling

    private static void HandleApiError(string responseString, string errorMessage, string preamble)
    {
        _log.Error(errorMessage);
        if (!_errorAlreadyShown)
        {
            var truncatedResponse = TruncateResponseString(responseString);
            Spawn(truncatedResponse, $"Error: API Service - {errorMessage}", preamble: preamble);
        }
        _errorAlreadyShown = true;
    }

    public static void HandleUnspecifiedErrorFromApi(string responseString)
    {
        HandleApiError(responseString, "Unspecified Error", "An unspecified error occurred while communicating with the Path of Exile API.");
    }

    #region Handle 4XX Errors

    public static void HandleError401FromApi(string responseString)
    {
        HandleApiError(responseString, "401 Unauthorized", "It looks like your auth token is invalid. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.");
    }

    public static void HandleError403FromApi(string responseString)
    {
        HandleApiError(responseString, "403 Forbidden", "It looks like your auth token has expired. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.");
    }

    public static int HandleError429FromApi(HttpResponseMessage response, string responseString)
    {
        var retryAfterSeconds = GetRetryAfterSeconds(response);
        var timeoutString = GenerateTimeoutString(retryAfterSeconds);

        GlobalRateLimitState.SetRateLimitExpiresOn(retryAfterSeconds);

        HandleApiError(responseString, "429 Too Many Requests (Rate Limit)", $"You are making too many requests in a short period of time - You are rate limited. Wait at least {timeoutString} and try again.");

        return retryAfterSeconds;
    }

    #endregion

    #region Handle 5XX Errors

    public static void HandleError500FromApi(string responseString)
    {
        HandleApiError(responseString, "500 Internal Server Error", "The Path of Exile API is currently experiencing an internal server error. Please try again later.");
    }

    public static void HandleError503FromApi(string responseString)
    {
        HandleApiError(responseString, "503 Service Unavailable", "The Path of Exile API is currently down. This is usually for maintenance, or DDoS, or league launch shenanigans - maybe all three!");
    }

    #endregion

    #endregion

    #region Utility Methods

    private static int GetRetryAfterSeconds(HttpResponseMessage response)
    {
        const string retryHeader = "Retry-After";

        var retryAfter = response.Headers.GetValues(retryHeader).Select(int.Parse).OrderByDescending(x => x).FirstOrDefault();

        return retryAfter;
    }

    private static string GenerateTimeoutString(int retryAfterSeconds)
    {
        var timeSpan = TimeSpan.FromSeconds(retryAfterSeconds);

        var timeComponents = new[]
        {
            (timeSpan.Hours, "hour"),
            (timeSpan.Minutes, "minute"),
            (timeSpan.Seconds, "second")
        };

        var formattedTimeoutString = string.Join(", ", timeComponents
            .Where(c => c.Item1 > 0)
            .Select(c => $"{c.Item1} {c.Item2}{(c.Item1 > 1 ? "s" : "")}"));

        return formattedTimeoutString.Contains(',')
            ? formattedTimeoutString.Insert(formattedTimeoutString.LastIndexOf(','), " and").Replace(", and", " and")
            : formattedTimeoutString;
    }

    private static string TruncateResponseString(string responseString)
    {
        const int maxLength = 500;
        return responseString.Length > maxLength
            ? responseString[..maxLength] + "...\n\n(Truncated for brevity)"
            : responseString;
    }

    #endregion
}
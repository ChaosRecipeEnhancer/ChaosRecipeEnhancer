using Serilog;
using System;

namespace ChaosRecipeEnhancer.UI.Services;

public static class GlobalRateLimitState
{
    public static bool RateLimitExceeded { get; set; }

    public static int RequestCounter { get; set; }

    /// <summary>
    /// Represents the state of the rate limit, typically includes current hits, period tested, and time restricted.
    /// </summary>
    public static int[] RateLimitState { get; set; } = new int[3];

    public static int MaximumRequests { get; set; } = 45;

    /// <summary>
    /// Represents the duration in seconds for which the client is banned from making requests.
    /// </summary>
    public static int BanTime { get; set; }

    public static DateTime RateLimitExpiresOn
    {
        get => Properties.Settings.Default.RateLimitExpiresOn;
        set
        {
            Log.Information("Rate limit expires on: {RateLimitExpiresOn}", value);

            if (Properties.Settings.Default.RateLimitExpiresOn != value)
            {
                // Update the user setting whenever the value is set
                Properties.Settings.Default.RateLimitExpiresOn = value;
                Properties.Settings.Default.Save();
            }
        }
    }

    /// <summary>
    /// Stores the number of seconds since the last successful request was made.
    /// </summary>
    public static int ResponseSeconds { get; set; }

    /// <summary>
    /// Extracts and stores the number of seconds since the last request from a formatted response time string.
    /// </summary>
    /// <param name="responseTime">A string containing the response time in a formatted string.</param>
    public static void DeserializeResponseSeconds(string responseTime)
    {
        // Split the string to isolate seconds.
        var split = responseTime.Split(':');
        var split2 = split[2].Split(' ');

        // Attempt to parse and store the seconds value.
        if (int.TryParse(split2[0], out int seconds))
        {
            ResponseSeconds = seconds;
        }
    }

    /// <summary>
    /// Parses and updates the rate limit state from the provided string.
    /// </summary>
    /// <param name="_">Unused parameter, present for interface compatibility.</param>
    /// <param name="rateLimitStateString">The string representing the current rate limit state.</param>
    public static void DeserializeRateLimits(string _, string rateLimitStateString)
    {
        // Split the state string to isolate individual state components.
        var maxSplitsState = rateLimitStateString.Split(',');
        var maxSplitsLowState = maxSplitsState[0].Split(':');

        // Parse each component and update the RateLimitState array.
        for (var i = 0; i < maxSplitsLowState.Length; i++)
        {
            if (int.TryParse(maxSplitsLowState[i], out int splitInt))
            {
                RateLimitState[i] = splitInt;
            }
        }
    }

    /// <summary>
    /// Determines if the client is currently banned based on the rate limit state.
    /// </summary>
    /// <returns>True if the client is currently banned, otherwise false.</returns>
    public static bool CheckForBan()
    {
        // Check if there's a rate limit from a previous session
        if (DateTime.Now < RateLimitExpiresOn)
        {
            return true;
        }

        // Check the current rate limit state
        if (RateLimitState[2] > 0)
        {
            // If the client is banned, update the rate limit expiry time
            RateLimitExpiresOn = DateTime.Now.AddSeconds(RateLimitState[2]);
            BanTime = RateLimitState[2];
            return true;
        }

        // If the client is not banned, reset the rate limit state
        RateLimitExceeded = false;
        return false;
    }

    /// <summary>
    /// Calculates the wait time based on the rate limit state and the response time.
    /// </summary>
    /// <returns>The number of seconds to wait before making another request.</returns>
    public static int GetSecondsToWait()
    {
        // Calculate the remaining time until the rate limit expires
        TimeSpan remainingTime = RateLimitExpiresOn - DateTime.Now;
        return (int)remainingTime.TotalSeconds;
    }

    public static void SetRateLimitExpiresOn(int retryAfterSeconds)
    {
        RateLimitExpiresOn = DateTime.Now.AddSeconds(retryAfterSeconds);
        BanTime = retryAfterSeconds;
    }
}

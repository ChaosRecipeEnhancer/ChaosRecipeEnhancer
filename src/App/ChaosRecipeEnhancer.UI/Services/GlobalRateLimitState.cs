namespace ChaosRecipeEnhancer.UI.Services;

public static class GlobalRateLimitState
{
    public static bool RateLimitExceeded;

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
        // The third element indicates the ban time; a value greater than 0 means the client is banned.
        if (RateLimitState[2] > 0)
        {
            BanTime = RateLimitState[2];
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates the wait time based on the rate limit state and the response time.
    /// </summary>
    /// <returns>The number of seconds to wait before making another request.</returns>
    public static int GetSecondsToWait() => 60 - ResponseSeconds;
}

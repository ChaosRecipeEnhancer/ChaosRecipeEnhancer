namespace ChaosRecipeEnhancer.UI.State;

public static class GlobalRateLimitState
{
    public static bool RateLimitExceeded;
    public static int RequestCounter { get; set; }
    public static int[] RateLimitState { get; set; } = new int[3];
    public static int MaximumRequests { get; set; } = 45;
    public static int BanTime { get; set; }
    public static int ResponseSeconds { get; set; }

    public static void DeserializeResponseSeconds(string responseTime)
    {
        var split = responseTime.Split(':');
        var split2 = split[2].Split(' ');
        if (int.TryParse(split2[0], out int s))
        {
            ResponseSeconds = s;
        }
    }

    public static void DeserializeRateLimits(string _, string rateLimitStateString)
    {
        var maxSplitsState = rateLimitStateString.Split(',');
        var maxSplitsLowState = maxSplitsState[0].Split(':');

        for (var i = 0; i < maxSplitsLowState.Length; i++)
        {
            if (int.TryParse(maxSplitsLowState[i], out int splitInt))
            {
                RateLimitState[i] = splitInt;
            }
        }
    }

    public static bool CheckForBan()
    {
        if (RateLimitState[2] > 0)
        {
            BanTime = RateLimitState[2];
            return true;
        }
        return false;
    }

    public static int GetSecondsToWait() => 60 - ResponseSeconds;
}

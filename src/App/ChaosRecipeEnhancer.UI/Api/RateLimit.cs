namespace ChaosRecipeEnhancer.UI.Api;

internal static class RateLimit
{
	public static int[] RateLimitState { get; set; } = new int[3];
	public static int RequestCounter
	{
		get; set;
	}
	public static int MaximumRequests { get; set; } = 45;
	public static bool RateLimitExceeded;
	public static int BanTime
	{
		get; set;
	}
	public static int ResponseSeconds
	{
		get; set;
	}

	public static void DeserializeResponseSeconds(string responseTime)
	{
		string[] split = responseTime.Split(':');
		string[] split2 = split[2].Split(' ');
		if (int.TryParse(split2[0], out int s))
		{
			ResponseSeconds = s;
		}
	}

	public static void DeserializeRateLimits(string _, string rateLimitStateString)
	{
		string[] maxSplitsState = rateLimitStateString.Split(',');
		string[] maxSplitsLowState = maxSplitsState[0].Split(':');

		for (int i = 0; i < maxSplitsLowState.Length; i++)
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

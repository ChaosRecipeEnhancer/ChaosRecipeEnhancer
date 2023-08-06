namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.DataFetching;

internal static class RateLimit
{
	private static RuleDefinition _currentRateLimit;

	// initialize to max, this will be updated on our first fetch
	private static RuleDefinition _maxRateLimit = new RuleDefinition(int.MaxValue, 0, 0);
	private static bool HeaderParsingError
	{
		get; set;
	}
	private static int ResponseSeconds
	{
		get; set;
	}

	public static bool rateLimitExceeded;
	public static int CurrentRequests => _currentRateLimit.HitCount;
	public static int MaximumRequests => _maxRateLimit.HitCount;
	public static int BanTime
	{
		get; set;
	}

	public static void DeserializeResponseSeconds(string responseTime)
	{
		var split = responseTime.Split(':');
		var split2 = split[2].Split(' ');

		if (int.TryParse(split2[0], out var s))
			ResponseSeconds = s;
		else
			HeaderParsingError = true;
	}

	public static void DeserializeRateLimits(string rateLimitMaxString, string rateLimitStateString)
	{
		// defined by the PoE API
		const int expectedRuleCount = 3;

		HeaderParsingError = false;

		// Store the dynamic maximum limit rules
		var maxSplits = rateLimitMaxString.Split(',');
		var maxSplitsLow = maxSplits[0].Split(':');
		_maxRateLimit = new RuleDefinition(
			int.Parse(maxSplitsLow[0]),
			int.Parse(maxSplitsLow[1]),
			int.Parse(maxSplitsLow[2]));

		// Store the current limit rules
		var maxSplitsState = rateLimitStateString.Split(',');
		var maxSplitsLowState = maxSplitsState[0].Split(':');
		if (maxSplitsLowState.Length >= expectedRuleCount)
			_currentRateLimit = new RuleDefinition(
				int.Parse(maxSplitsLowState[0]),
				int.Parse(maxSplitsLowState[1]),
				int.Parse(maxSplitsLowState[2]));
		else
			// Something went wrong - our rule parsing did not match the API format
			HeaderParsingError = true;
	}

	public static bool CheckForBan()
	{
		if (_currentRateLimit.RestrictionTime > 0)
		{
			BanTime = _currentRateLimit.RestrictionTime;
			return true;
		}

		return false;
	}

	public static void Reset()
	{
		_currentRateLimit.ClearHitCount();
		rateLimitExceeded = false;
	}

	public static int GetSecondsToWait()
	{
		return _maxRateLimit.RestrictionTime - ResponseSeconds;
	}

	public struct RuleDefinition
	{
		public int HitCount
		{
			get; private set;
		}
		public int Period
		{
			get;
		}
		public int RestrictionTime
		{
			get;
		}

		public RuleDefinition(int hits, int period, int restriction)
		{
			HitCount = hits;
			Period = period;
			RestrictionTime = restriction;
		}

		public void IncrementHitCounter()
		{
			HitCount++;
		}

		public void ClearHitCount()
		{
			HitCount = 0;
		}
	}
}
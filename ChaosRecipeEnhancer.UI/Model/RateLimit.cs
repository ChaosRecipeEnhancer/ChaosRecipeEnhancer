namespace ChaosRecipeEnhancer.UI.Model
{
    internal class RateLimit
    {
        private static RuleDefinition _currentRateLimit;

        private static RuleDefinition
            _maxRateLimit =
                new RuleDefinition(int.MaxValue, 0, 0); // initialize to max, this will be updated on our first fetch

        public static bool RateLimitExceeded;

        public static bool HeaderParsingError { get; set; }

        public static int CurrentRequests => _currentRateLimit.HitCount;

        public static int MaximumRequests => _maxRateLimit.HitCount;

        public static int BanTime { get; set; }

        public static int ResponseSeconds { get; set; }

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
            const int ExpectedRuleCount = 3; // defined by the PoE API

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
            if (maxSplitsLowState.Length >= ExpectedRuleCount)
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

        public static void IncreaseRequestCounter()
        {
            _currentRateLimit.IncrementHitCounter();
        }

        public static void Reset()
        {
            _currentRateLimit.ClearHitCount();
            RateLimitExceeded = false;
        }

        public static int GetSecondsToWait()
        {
            return _maxRateLimit.RestrictionTime - ResponseSeconds;
        }

        public struct RuleDefinition
        {
            public int HitCount { get; private set; }
            public int Period { get; }
            public int RestrictionTime { get; }

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
}
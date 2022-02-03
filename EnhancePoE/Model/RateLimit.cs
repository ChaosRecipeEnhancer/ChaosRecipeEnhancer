namespace EnhancePoE.Model
{
    internal class RateLimit
    {
        //public static int MaximumRequestsHigh { get; set; } = 240;
        //public static int ResetTime { get; set; } = 60;
        //public static int ResetTimeHigh { get; set; } = 240;

        public static bool RateLimitExceeded = false;
        //public static int[] RateLimitMax { get; set; } = new int[3];
        //public static int[] RateLimitMaxHigh { get; set; } = new int[3];

        public static int[] RateLimitState { get; set; } = new int[3];

        //public static int[] RateLimitStateHigh { get; set; } = new int[3];
        public static bool HeaderParsingError { get; set; }

        //public static System.Timers.Timer RateLimitTimer { get; set; } = new System.Timers.Timer();
        //public static System.Timers.Timer RateLimitTimerHigh { get; set; } = new System.Timers.Timer();
        public static int RequestCounter { get; set; } = 0;

        //public static int RequestCounterHigh { get; set; } = 0;

        // TODO: update this value if poe server response headers change
        public static int MaximumRequests { get; set; } = 10;
        public static int BanTime { get; set; }
        public static int ResponseSeconds { get; set; }

        //public static int ResponseMinutes { get; set; } = 0;

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
            HeaderParsingError = false;

            var maxSplits = rateLimitMaxString.Split(',');
            var maxSplitsLow = maxSplits[0].Split(':');

            var maxSplitsState = rateLimitStateString.Split(',');
            var maxSplitsLowState = maxSplitsState[0].Split(':');

            for (var i = 0; i < maxSplitsLowState.Length; i++)
                if (int.TryParse(maxSplitsLowState[i], out var splitInt))
                    RateLimitState[i] = splitInt;
                else
                    HeaderParsingError = true;
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

        public static void IncreaseRequestCounter()
        {
            RateLimitState[0]++;
        }

        public static int GetSecondsToWait()
        {
            // TODO: update this value if poe server response headers change
            return 60 - ResponseSeconds;
        }
    }
}
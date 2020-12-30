using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Model
{
    class RateLimit
    {
        //public static int[] RateLimitMax { get; set; } = new int[3];
        //public static int[] RateLimitMaxHigh { get; set; } = new int[3];
        public static int[] RateLimitState { get; set; } = new int[3];
        //public static int[] RateLimitStateHigh { get; set; } = new int[3];
        public static bool HeaderParsingError { get; set; } = false;
        //public static System.Timers.Timer RateLimitTimer { get; set; } = new System.Timers.Timer();
        //public static System.Timers.Timer RateLimitTimerHigh { get; set; } = new System.Timers.Timer();
        public static int RequestCounter { get; set; } = 0;
        //public static int RequestCounterHigh { get; set; } = 0;

        // TODO: update this value if poe server response headers change
        public static int MaximumRequests { get; set; } = 45;
        //public static int MaximumRequestsHigh { get; set; } = 240;
        //public static int ResetTime { get; set; } = 60;
        //public static int ResetTimeHigh { get; set; } = 240;
        public static bool RateLimitExceeded = false;
        public static int BanTime { get; set; } = 0;
        public static int ResponseSeconds { get; set; } = 0;
        //public static int ResponseMinutes { get; set; } = 0;

        public static void DeserializeResponseSeconds(string responseTime)
        {
            string[] split = responseTime.Split(':');
            string[] split2 = split[2].Split(' ');
            if (Int32.TryParse(split2[0], out int s))
            {
                ResponseSeconds = s;
            }
            else
            {
                HeaderParsingError = true;
            }
            //Trace.WriteLine(ResponseSeconds, "res secs");
        }
        public static void DeserializeRateLimits(string rateLimitMaxString, string rateLimitStateString)
        {
            HeaderParsingError = false;
            string[] maxSplits = rateLimitMaxString.Split(',');
            string[] maxSplitsLow = maxSplits[0].Split(':');
            //string[] maxSplitsHigh = maxSplits[1].Split(':');

            string[] maxSplitsState = rateLimitStateString.Split(',');
            string[] maxSplitsLowState = maxSplitsState[0].Split(':');
            //string[] maxSplitsHighState = maxSplitsState[1].Split(':');

            //Trace.WriteLine(RateLimitState.Length, "rate limit lenght");

            //for(int i = 0; i < maxSplitsLow.Length; i++)
            //{
            //    if (Int32.TryParse(maxSplitsLow[i], out int splitInt))
            //    {
            //        RateLimitMax[i] = splitInt;
            //    }
            //    else
            //    {
            //        HeaderParsingError = true;
            //    }
            //}
            //for (int i = 0; i < maxSplitsHigh.Length; i++)
            //{
            //    if (Int32.TryParse(maxSplitsHigh[i], out int splitInt))
            //    {
            //        RateLimitMaxHigh[i] = splitInt;
            //    }
            //    else
            //    {
            //        HeaderParsingError = true;
            //    }
            //}
            for (int i = 0; i < maxSplitsLowState.Length; i++)
            {
                if (Int32.TryParse(maxSplitsLowState[i], out int splitInt))
                {
                    RateLimitState[i] = splitInt;
                }
                else
                {
                    HeaderParsingError = true;
                }
            }
            //for (int i = 0; i < maxSplitsHighState.Length; i++)
            //{
            //    if (Int32.TryParse(maxSplitsHighState[i], out int splitInt))
            //    {
            //        RateLimitStateHigh[i] = splitInt;
            //    }
            //    else
            //    {
            //        HeaderParsingError = true;
            //    }
            //}


            //foreach(int i in RateLimitMax)
            //{
            //    Trace.WriteLine(i, "rate limit max");
            //}
            //foreach (int i in RateLimitMaxHigh)
            //{
            //    Trace.WriteLine(i, "rate limit max high");
            //}
            //foreach (int i in RateLimitState)
            //{
            //    Trace.WriteLine(i, "rate limit state");
            //}
            //foreach (int i in RateLimitStateHigh)
            //{
            //    Trace.WriteLine(i, "rate limit steat high");
            //}

        }

        //public static void CheckTimerValues()
        //{

        //}

        public static bool CheckForBan()
        {
            if(RateLimitState[2] > 0)
            {
                // TODO: show ban in ui
                //int maxWaiting = Math.Max(RateLimitState[2], RateLimitStateHigh[2]);
                //BanTime = maxWaiting;
                BanTime = RateLimitState[2];
                return true;
                //await Task.Delay(maxWaiting * 1000);
            }
            return false;
        }

        //public static void InitializeRateLimitTimer()
        //{
        //    RateLimitTimer.Elapsed += OnTimedEvent;
        //    RateLimitTimer.Interval = 60 * 1000;
        //    RateLimitTimer.AutoReset = true;
        //    RateLimitTimerHigh.Elapsed += OnTimedEventHigh;
        //    RateLimitTimerHigh.Interval = 240 * 1000;
        //    RateLimitTimerHigh.AutoReset = true;
        //}
        //private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        //{
        //    RequestCounter = 0;
        //    //Trace.WriteLine("resetting Request Counter");
        //}
        //private static void OnTimedEventHigh(Object source, System.Timers.ElapsedEventArgs e)
        //{
        //    RequestCounterHigh = 0;
        //    //Trace.WriteLine("resetting Request Counter high");
        //}

        public static void IncreaseRequestCounter()
        {
            RateLimitState[0]++;
            //RequestCounter++;
            //RequestCounterHigh++;
        }

        //public static void CheckRateLimitThresholds()
        //{
        //    if(MaximumRequests != RateLimitMax[0])
        //    {
        //        MaximumRequests = RateLimitMax[0];
        //    }
        //    if (MaximumRequestsHigh != RateLimitMaxHigh[0])
        //    {
        //        MaximumRequestsHigh = RateLimitMaxHigh[0];
        //    }
        //    if (ResetTime != RateLimitMax[1])
        //    {
        //        ResetTime = RateLimitMax[1];
        //        //RateLimitTimer.Enabled = false;
        //        //RateLimitTimer.Interval = ResetTime;
        //        //RateLimitTimer.Enabled = true;
        //    }
        //    if (ResetTimeHigh != RateLimitMaxHigh[1])
        //    {
        //        ResetTimeHigh = RateLimitMaxHigh[1];
        //        //RateLimitTimerHigh.Enabled = false;
        //        //RateLimitTimerHigh.Interval = ResetTime;
        //        //RateLimitTimerHigh.Enabled = true;
        //    }
        //}

        public static int GetSecondsToWait()
        {
            // TODO: update this value if poe server response headers change
            return 60 - ResponseSeconds;
        }
    }
}

using System;

namespace ChaosRecipeEnhancer.UI.Models.Exceptions;

public class RateLimitException : Exception
{
    public int SecondsToWait { get; set; }

    public RateLimitException(int secondsToWait) : base($"Rate limit exceeded. Please wait {secondsToWait} seconds before trying again.")
    {
        SecondsToWait = secondsToWait;
    }
}
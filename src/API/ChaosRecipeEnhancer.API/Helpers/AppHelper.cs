namespace ChaosRecipeEnhancer.API.Helpers;

/// <summary>
/// Provides helper methods for the application.
/// </summary>
public static class AppHelper
{
    /// <summary>
    /// Determines whether the application is running in an AWS Lambda environment.
    /// </summary>
    /// <returns>True if the application is running in an AWS Lambda environment, otherwise false.</returns>
    public static bool IsRunningInLambda()
    {
        var lambdaRuntimeApi = Environment.GetEnvironmentVariable("AWS_LAMBDA_RUNTIME_API");
        return !string.IsNullOrEmpty(lambdaRuntimeApi);
    }
}

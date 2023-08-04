using Amazon.CDK;

namespace ChaosRecipeEnhancer.API.Infra;

public class EnvironmentHelper
{
    public static IEnvironment MakeEnvironment(string? accountId = null, string? region = null)
    {
        return new Amazon.CDK.Environment
        {
            Account = accountId?.Length > 0 ? accountId : accountId ??
                System.Environment.GetEnvironmentVariable("CDK_DEPLOY_ACCOUNT") ??
                System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
            Region = region?.Length > 0 ? region : region ??
                System.Environment.GetEnvironmentVariable("CDK_DEPLOY_REGION") ??
                System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
        };
    }
}

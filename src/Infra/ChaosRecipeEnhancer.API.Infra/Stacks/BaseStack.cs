using Amazon.CDK;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra;

public class BaseStack : Stack
{
    protected Environment CurrentEnvironment
    {
        get;
    }

    public BaseStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var accountId = Of(this).Account;
        CurrentEnvironment = InfraConfigHelper.GetCurrentEnvironmentById(accountId);
    }
}

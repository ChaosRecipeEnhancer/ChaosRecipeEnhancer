using Amazon.CDK;
using ChaosRecipeEnhancer.API.Infra.Helpers;
using Constructs;
using Environment = ChaosRecipeEnhancer.API.Infra.Config.Environment;

namespace ChaosRecipeEnhancer.API.Infra.Stacks;

public abstract class BaseStack : Stack
{
    protected Environment CurrentEnvironment { get; }

    protected BaseStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
    {
        var accountId = Of(this).Account;
        CurrentEnvironment = InfraConfigHelper.GetCurrentInfraEnvironmentById(accountId);
    }
}

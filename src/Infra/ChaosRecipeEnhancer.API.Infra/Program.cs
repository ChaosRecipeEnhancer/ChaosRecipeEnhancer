using Amazon.CDK;
using Cdklabs.CdkNag;
using ChaosRecipeEnhancer.API.Infra.Helpers;
using ChaosRecipeEnhancer.API.Infra.Stacks;

namespace ChaosRecipeEnhancer.API.Infra;

sealed class Program
{
    public static void Main()
    {
        var app = new App();

        // Enable CdkNag checks
        // Aspects.Of(app).Add(new AwsSolutionsChecks());

        _ = new InfraStack(
            app,
            "CRE-InfraStack",
            new StackProps { Env = EnvironmentHelper.MakeEnvironment() }
        );

        app.Synth();
    }
}

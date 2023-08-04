using Amazon.CDK;
using Cdklabs.CdkNag;
using ChaosRecipeEnhancer.API.Infra;

namespace ChaosRecipeEnhancer.Infra;

sealed class Program
{
    private static readonly string API_VERSION = InfraConfigHelper.GetApiVersion();

    public static void Main(string[] args)
    {
        var app = new App();
        Aspects.Of(app).Add(new AwsSolutionsChecks());

        _ = InfraConfigHelper.GetCurrentEnvironmentById(System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"));
        _ = new InfraStack(app, $"CRE-InfraStack", new InfraStackProps()
        {
            ApiVersion = API_VERSION,
            Env = EnvironmentHelper.MakeEnvironment()
        });

        app.Synth();
    }
}

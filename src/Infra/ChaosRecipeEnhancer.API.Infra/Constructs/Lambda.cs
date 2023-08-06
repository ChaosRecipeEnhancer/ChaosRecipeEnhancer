using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SecretsManager;
using ChaosRecipeEnhancer.API.Infra.Helpers;
using Construct = Constructs.Construct;

namespace ChaosRecipeEnhancer.API.Infra.Constructs;

public class LambdaProps
{
    public bool IsSandbox { get; init; }
    public string AssetPath { get; init; } = default!;
    public string Handler { get; init; } = default!;
}

public class Lambda : Construct
{
    internal Function Function { get; }

    public Lambda(Construct scope, string id, LambdaProps props) : base(scope, id)
    {
        var role = new Role(
            this,
            "FunctionRole",
            new RoleProps { AssumedBy = new ServicePrincipal("lambda.amazonaws.com") }
        );

        var lambdaBasicExecution = ManagedPolicy.FromAwsManagedPolicyName(
            "service-role/AWSLambdaBasicExecutionRole"
        );
        var xRayDaemonWriteAccess = ManagedPolicy.FromAwsManagedPolicyName(
            "AWSXRayDaemonWriteAccess"
        );

        // Read access to the secret that contains the CRE client id / secret to communicate with the GGG API
        GrantSecretReadAccess("CRE-Credentials", role);

        role.AddManagedPolicy(lambdaBasicExecution);
        role.AddManagedPolicy(xRayDaemonWriteAccess);

        var environmentVariables = new Dictionary<string, string>
        {
            { "API_VERSION", InfraConfigHelper.GetApiVersion() },

            // Lambda PowerTools for .NET Configurations
            // See the following reference for more details:
            //  - https://docs.powertools.aws.dev/lambda-dotnet/core/logging/#full-list-of-environment-variables
            //  - https://docs.powertools.aws.dev/lambda-dotnet/core/metrics/#full-list-of-environment-variables
            //  - https://docs.powertools.aws.dev/lambda-dotnet/core/tracing/#full-list-of-environment-variables
            { "POWERTOOLS_SERVICE_NAME", id },
            { "POWERTOOLS_TRACER_CAPTURE_HTTP_REQUESTS", props.IsSandbox ? "true" : "" },
            { "POWERTOOLS_LOG_LEVEL", props.IsSandbox ? "Debug" : "Information" },
            { "POWERTOOLS_LOGGER_LOG_EVENT", props.IsSandbox ? "true" : "false" },
            { "POWERTOOLS_TRACE_DISABLED", props.IsSandbox ? "false" : "true" },
            { "POWERTOOLS_TRACER_CAPTURE_RESPONSE", props.IsSandbox ? "true" : "false" },
            { "POWERTOOLS_TRACER_CAPTURE_ERROR", props.IsSandbox ? "true" : "false" },
            { "POWERTOOLS_METRICS_NAMESPACE", "ChaosRecipeEnhancer-API" },

            // For the .NET runtime, set this variable to enable or disable .NET specific runtime optimizations.
            { "AWS_LAMBDA_DOTNET_PREJIT", "always" }
        };

        Function = new Function(
            this,
            "Lambda",
            new FunctionProps
            {
                Role = role,
                Architecture = Architecture.X86_64,
                Code = Code.FromAsset(props.AssetPath),
                Runtime = Runtime.DOTNET_6,
                Handler = props.Handler,
                Environment = environmentVariables,
                Tracing = Tracing.ACTIVE,

                // API Gateway has a hard limit of 29 seconds for Lambda integrations.
                Timeout = Duration.Seconds(29),
                MemorySize = 256,
            }
        );
    }

    private void GrantSecretReadAccess(string secretId, Role role)
    {
        var secret = Secret.FromSecretNameV2(this, $"{secretId}Grant", secretId);
        secret.GrantRead(role);
    }
}

using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra;

public class LambdaProps
{
	public string LambdaName
	{
		get; set;
	}
	public string AssetPath
	{
		get; set;
	}
	public string Handler
	{
		get; set;
	}
	public int? Timeout
	{
		get; set;
	}
	public int? MemorySize
	{
		get; set;
	}
	public Dictionary<string, string>? Environment
	{
		get; set;
	}
}

public class Lambda : Construct
{
	internal Function Function
	{
		get; set;
	}

	public Lambda(Construct scope, string id, LambdaProps props) : base(scope, id)
	{
		var role = new Role(
			this,
			$"{props.LambdaName}-Role",
			new RoleProps() { AssumedBy = new ServicePrincipal("lambda.amazonaws.com") }
		);

		IManagedPolicy lambdaBasicExecution = ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole");
		IManagedPolicy xrayDaemonWriteAccess = ManagedPolicy.FromAwsManagedPolicyName("AWSXRayDaemonWriteAccess");

		role.AddManagedPolicy(lambdaBasicExecution);
		role.AddManagedPolicy(xrayDaemonWriteAccess);

		var isSandbox = Stack.Of(this).StackName.Contains("Sandbox");
		var environment = props.Environment ?? new Dictionary<string, string>();

		// Lambda Powertools for .NET Configurations
		// See the following reference for more details:
		//  - https://docs.powertools.aws.dev/lambda-dotnet/core/logging/#full-list-of-environment-variables
		//  - https://docs.powertools.aws.dev/lambda-dotnet/core/metrics/#full-list-of-environment-variables
		//  - https://docs.powertools.aws.dev/lambda-dotnet/core/tracing/#full-list-of-environment-variables
		environment.Add("POWERTOOLS_SERVICE_NAME", id);
		environment.Add("POWERTOOLS_LOG_LEVEL", isSandbox ? "Debug" : "Information");
		environment.Add("POWERTOOLS_LOGGER_LOG_EVENT", isSandbox ? "true" : "false");
		environment.Add("POWERTOOLS_TRACE_DISABLED", isSandbox ? "false" : "true");
		environment.Add("POWERTOOLS_TRACER_CAPTURE_RESPONSE", isSandbox ? "true" : "false");
		environment.Add("POWERTOOLS_TRACER_CAPTURE_ERROR", isSandbox ? "true" : "true");
		environment.Add("POWERTOOLS_METRICS_NAMESPACE", "ChaosRecipeEnhancer-API");

		// For the .NET 3.1 runtime, set this variable to enable or disable .NET 3.1 specific runtime optimizations.
		environment.Add("AWS_LAMBDA_DOTNET_PREJIT", "always");

		Function = new Function(
			this,
			props.LambdaName,
			new FunctionProps()
			{
				Code = Code.FromAsset(props.AssetPath),
				Architecture = Architecture.X86_64,
				Runtime = Runtime.DOTNET_6,
				Handler = props.Handler,
				MemorySize = props.MemorySize ?? 256,
				Environment = props.Environment,
				Role = role,

				// API Gateway has a hard limit of 29 seconds for Lambda integrations.
				Timeout = Duration.Seconds(props.Timeout ?? 29),
			}
		);
	}
}

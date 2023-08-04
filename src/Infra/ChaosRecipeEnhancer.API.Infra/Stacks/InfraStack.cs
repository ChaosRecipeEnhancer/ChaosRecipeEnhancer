using System;
using Amazon.CDK;
using Cdklabs.CdkNag;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra;

public class InfraStackProps : StackProps
{
	public string ApiVersion
	{
		get; set;
	}
}

public class InfraStack : BaseStack
{
	internal InfraStack(Construct scope, string id, InfraStackProps props = null) : base(scope, id, props)
	{
		var lambda = new Lambda(this, "CRE-Lambda", new LambdaProps()
		{
			LambdaName = $"{CurrentEnvironment.Name}-CRE-Lambda",
			AssetPath = "../../dist/API/release/publish",
			Handler = "ChaosRecipeEnhancer.API",
			MemorySize = 256,
		});

		lambda.Function.AddEnvironment("API_VERSION", props.ApiVersion);

		var gateway = new Gateway(this, "CRE-Api-Gateway", new GatewayProps()
		{
			GatewayName = $"{CurrentEnvironment.Name}-CRE-API-Gateway",
			ApiVersion = props.ApiVersion,
			BaseEnvironmentVariables = CurrentEnvironment,
			ApiFunction = lambda.Function
		});

		#region CDK Nag Suppressions

		NagSuppressions.AddStackSuppressions(
			this,
			new[]
			{
				new NagPackSuppression
				{
					Id = "AwsSolutions-APIG2",
					Reason = "TODO: Will review this suppression and resolve it if possible."
				},
				new NagPackSuppression
				{
					Id = "AwsSolutions-APIG3",
					Reason = "TODO: Will review this suppression and resolve it if possible."
				},
				new NagPackSuppression
				{
					Id = "AwsSolutions-APIG4",
					Reason = "TODO: Will review this suppression and resolve it if possible."
				},
				new NagPackSuppression
				{
					Id = "AwsSolutions-COG4",
					Reason = "TODO: Will review this suppression and resolve it if possible."
				},
				new NagPackSuppression
				{
					Id = "AwsSolutions-IAM4",
					Reason = "TODO: Will review this suppression and resolve it if possible."
				},
			}
		);

		#endregion
	}
}

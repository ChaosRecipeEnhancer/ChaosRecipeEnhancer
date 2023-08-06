using Amazon.CDK;
using ChaosRecipeEnhancer.API.Infra.Constructs;
// using Cdklabs.CdkNag;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra.Stacks;

public class InfraStack : BaseStack
{
    internal InfraStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
    {
        var lambda = new Lambda(this, "CRE-Lambda", new LambdaProps
        {
            AssetPath = "../../dist/ChaosRecipeEnhancer.API/release/publish",
            Handler = "ChaosRecipeEnhancer.API",
            IsSandbox = CurrentEnvironment.Name == "sandbox"
        });

        var gateway = new Gateway(this, "CRE-Api-Gateway", new GatewayProps
        {
            ApiFunction = lambda.Function,
            ApiGatewayUrl = CurrentEnvironment.ApiGatewayUrl,
            CertificateArn = CurrentEnvironment.CertificateArn
        });

        _ = new DnsRecord(
            this,
            "DnsRecord",
            new DnsRecordProps
            {
                HostedZoneId = CurrentEnvironment.HostedZoneId,
                RecordName = CurrentEnvironment.ApiGatewayUrl,
                TargetApiGateway = gateway.RestApiGateway
            }
        );

        _ = new WebApplicationFirewall(
            this,
            "WebApplicationFirewall",
            new WebApplicationFirewallProps
            {
                GatewayArn = gateway.GatewayArn
            }
        );

        #region CDK Nag Suppressions

        // NagSuppressions.AddStackSuppressions(
        //     this,
        //     new[]
        //     {
        //         new NagPackSuppression
        //         {
        //             Id = "AwsSolutions-APIG2",
        //             Reason = "TODO: Will review this suppression and resolve it if possible."
        //         },
        //         new NagPackSuppression
        //         {
        //             Id = "AwsSolutions-APIG3",
        //             Reason = "TODO: Will review this suppression and resolve it if possible."
        //         },
        //         new NagPackSuppression
        //         {
        //             Id = "AwsSolutions-APIG4",
        //             Reason = "TODO: Will review this suppression and resolve it if possible."
        //         },
        //         new NagPackSuppression
        //         {
        //             Id = "AwsSolutions-COG4",
        //             Reason = "TODO: Will review this suppression and resolve it if possible."
        //         },
        //         new NagPackSuppression
        //         {
        //             Id = "AwsSolutions-IAM4",
        //             Reason = "TODO: Will review this suppression and resolve it if possible."
        //         },
        //     }
        // );

        #endregion
    }
}

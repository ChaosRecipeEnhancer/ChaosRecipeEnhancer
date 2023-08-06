using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra.Constructs;

public class DnsRecordProps
{
    public string HostedZoneId { get; set; }
    public string RecordName { get; set; }
    public LambdaRestApi TargetApiGateway { get; set; }
}

public class DnsRecord : Construct
{
    public DnsRecord(Construct scope, string id, DnsRecordProps props) : base(scope, id)
    {
        // Note: The R53 Hosted Zone and the Certificate associated with the ChaosRecipe.com URL are the only parts of
        // our Infra that has to be manually created due to its coupling with the custom domain name. The management of
        // the domain name is something that will be managed by the administrator (preferably on Route 53). You can plug
        // in the values (ID & Name) for the Hosted Zone after you have created it on your `infraConfig.json` file.
        var hostedZone = HostedZone.FromHostedZoneAttributes(
            this,
            "HostedZone",
            new HostedZoneAttributes
            {
                HostedZoneId = props.HostedZoneId,
                // Zone name should always be the same as the record name
                ZoneName = props.RecordName
            }
        );

        new ARecord(
            this,
            "GatewayARecord",
            new ARecordProps
            {
                RecordName = props.RecordName,
                Zone = hostedZone,
                Target = RecordTarget.FromAlias(new ApiGateway(props.TargetApiGateway))
            }
        );
    }
}
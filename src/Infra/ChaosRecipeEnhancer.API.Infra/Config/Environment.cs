namespace ChaosRecipeEnhancer.API.Infra.Config;

public class Environment
{
    public string Name { get; set; } = default!;
    public string AccountId { get; set; } = default!;
    public string ApiGatewayUrl { get; set; } = default!;
    public string HostedZoneId { get; set; } = default!;
    public string CertificateArn { get; set; } = default!;
}

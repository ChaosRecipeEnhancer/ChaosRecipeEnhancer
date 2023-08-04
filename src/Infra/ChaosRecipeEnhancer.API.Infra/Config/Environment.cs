using System;

namespace ChaosRecipeEnhancer.API.Infra;

public class Environment
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string AccountId { get; set; } = default!;
    public string ApiGatewayUrl { get; set; } = default!;
    public string CertificateArn { get; set; } = default!;
    public EnvironmentType EnvironmentType
    {
        get
        {
            if (Name == "sandbox")
            {
                return EnvironmentType.Sandbox;
            }
            else if (Name == "dev")
            {
                return EnvironmentType.Dev;
            }
            else if (Name == "test")
            {
                return EnvironmentType.Test;
            }
            else if (Name == "stage")
            {
                return EnvironmentType.Stage;
            }
            else if (Name == "prod")
            {
                return EnvironmentType.Prod;
            }
            else
            {
                throw new Exception($"Environment {Name} is not supported");
            }
        }
    }
}

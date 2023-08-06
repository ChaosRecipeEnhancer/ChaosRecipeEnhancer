using System.Collections.Generic;

namespace ChaosRecipeEnhancer.API.Infra.Config;

public class InfraConfig
{
    public string ApiVersion { get; set; }
    public List<Environment> Environments { get; set; } = new();
}

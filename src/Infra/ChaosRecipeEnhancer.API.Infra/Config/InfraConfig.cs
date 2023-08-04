using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.API.Infra;

public class InfraConfig
{
    public string ApiVersion { get; set; } = "v0";
    public List<Environment> Environments { get; set; } = new List<Environment>();
    public Environment GetStandboxEnvironment()
    {
        return Environments.First(e => e.EnvironmentType == EnvironmentType.Sandbox);
    }
}

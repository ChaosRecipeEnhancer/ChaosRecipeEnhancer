using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ChaosRecipeEnhancer.API.Infra.Config;
using Environment = ChaosRecipeEnhancer.API.Infra.Config.Environment;

namespace ChaosRecipeEnhancer.API.Infra.Helpers;

public static class InfraConfigHelper
{
    private static Environment? _currentEnvironment = null;

    public static string GetApiVersion()
    {
        var infraConfig = GetInfraConfig();
        return infraConfig.ApiVersion;
    }

    public static Environment GetCurrentInfraEnvironmentById(string accountId)
    {
        if (_currentEnvironment != null)
        {
            return _currentEnvironment;
        }

        var infraConfig = GetInfraConfig();

        _currentEnvironment = infraConfig.Environments.FirstOrDefault(env => env.AccountId.Equals(accountId));
        return _currentEnvironment ?? throw new Exception($"Could not find infraConfig for account with an AWS Account ID of '{accountId}'; Ensure you've properly configured your `infraConfig.json`.");
    }

    private static InfraConfig GetInfraConfig()
    {
        // Read `infraConfig.json` file from assembly location
        var assemblyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var infraConfigFilePath = Path.Combine(assemblyDirectoryPath, "infraConfig.json");

        var infraConfig = JsonSerializer.Deserialize<InfraConfig>(File.ReadAllText(infraConfigFilePath));
        return infraConfig ?? throw new Exception("Could not load `infraConfig.json` file; Ensure it's properly configured and exists in the root of the project.");
    }

}

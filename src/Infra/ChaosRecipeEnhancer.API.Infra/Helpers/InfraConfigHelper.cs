using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace ChaosRecipeEnhancer.API.Infra;

public class InfraConfigHelper
{
    private static Environment _currentEnvironment = null;

    public static InfraConfig GetInfraConfig()
    {
        // Read `infraConfig.json` file from assembly location
        string assemblyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string infraConfigFilePath = Path.Combine(assemblyDirectoryPath, "infraConfig.json");

        var infraConfig = JsonSerializer.Deserialize<InfraConfig>(File.ReadAllText(infraConfigFilePath));

        return infraConfig ?? throw new Exception("Could not load `infraConfig.json` file; Ensure it's properly configured and exists in the root of the project.");
    }

    public static Environment GetCurrentEnvironmentById(string accountId = null)
    {
        if (_currentEnvironment is not null)
        {
            return _currentEnvironment;
        }

        var infraConfig = GetInfraConfig();

        if (string.IsNullOrEmpty(accountId))
        {
            // if environment variable is not set, use Sandbox
            accountId = infraConfig.GetStandboxEnvironment().AccountId;
        }

        Console.WriteLine($"AWS Account ID: {accountId}");

        // map `config.json` to a dictionary
        _currentEnvironment = infraConfig.Environments.FirstOrDefault(e => e.AccountId == accountId, infraConfig.GetStandboxEnvironment());

        Console.WriteLine($"Current Environment: ${_currentEnvironment.DisplayName}");

        return _currentEnvironment;
    }

    public static Environment GetCurrentEnvironmentByName(string accountName = null)
    {
        if (_currentEnvironment is not null)
        {
            return _currentEnvironment;
        }

        var infraConfig = GetInfraConfig();

        if (string.IsNullOrEmpty(accountName))
        {
            // if environment variable is not set, use Sandbox
            accountName = infraConfig.GetStandboxEnvironment().Name;
        }

        Console.WriteLine($"AWS Account ID: {accountName}");

        // map `config.json` to a dictionary
        _currentEnvironment = infraConfig.Environments.FirstOrDefault(e => e.Name == accountName, infraConfig.GetStandboxEnvironment());

        Console.WriteLine($"Current Environment: ${_currentEnvironment.DisplayName}");

        return _currentEnvironment;
    }

    public static string GetApiVersion()
    {
        var infraConfig = GetInfraConfig();
        return infraConfig.ApiVersion;
    }
}

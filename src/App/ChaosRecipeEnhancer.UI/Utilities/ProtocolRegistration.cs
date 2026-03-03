using Microsoft.Win32;
using Serilog;
using System;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Handles registration and removal of the "chaosrecipe://" custom URL protocol in the Windows registry.
/// This enables the OS to route OAuth callback URIs back to the application.
/// </summary>
public static class ProtocolRegistration
{
    private const string ProtocolName = "chaosrecipe";
    private const string ProtocolDescription = "URL:Chaos Recipe Enhancer Protocol";

    /// <summary>
    /// Registers the "chaosrecipe://" URL protocol handler in HKCU\Software\Classes.
    /// Uses HKCU so no admin elevation is required.
    /// </summary>
    /// <param name="exePath">Full path to the application executable.</param>
    public static void Register(string exePath)
    {
        try
        {
            // HKCU\Software\Classes\chaosrecipe
            using var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{ProtocolName}");
            if (key == null) return;

            key.SetValue("", ProtocolDescription);
            key.SetValue("URL Protocol", "");

            // shell\open\command
            using var commandKey = key.CreateSubKey(@"shell\open\command");
            commandKey?.SetValue("", $"\"{exePath}\" \"%1\"");

            Log.Information("Registered {Protocol}:// protocol handler pointing to {ExePath}", ProtocolName, exePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to register {Protocol}:// protocol handler", ProtocolName);
        }
    }

    /// <summary>
    /// Removes the "chaosrecipe://" URL protocol handler from the registry.
    /// Called during uninstall cleanup.
    /// </summary>
    public static void Unregister()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{ProtocolName}", throwOnMissingSubKey: false);
            Log.Information("Unregistered {Protocol}:// protocol handler", ProtocolName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to unregister {Protocol}:// protocol handler", ProtocolName);
        }
    }
}

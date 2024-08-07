﻿using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
using System;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class SettingsConfiguration
{
    public static void UpgradeSettings()
    {
        if (Settings.Default.UpgradeSettingsAfterUpdate)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeSettingsAfterUpdate = false;

            // [3.25] one time settings 'migration
            Settings.Default.LeagueName = "Standard";
            Settings.Default.StashTabIdentifiers = string.Empty;

            // Reset sensitive auth-related settings after an upgrade
            Settings.Default.PathOfExileApiAuthToken = string.Empty;
            Settings.Default.LegacyAuthSessionId = string.Empty;

            Settings.Default.Save();
        }
    }

    public static void OnClose()
    {
        if (Settings.Default.RateLimitExpiresOn > DateTime.Now)
        {
            Log.Information("Rate limit expires on: {RateLimitExpiresOn}", Settings.Default.RateLimitExpiresOn);
        }

        if (Settings.Default.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.AttemptingLogin)
        {
            // Reset the connection status if the application is closed while attempting to log in.
            Log.Information("Resetting connection status to ConnectionNotValidated.");
            Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;
            Settings.Default.Save();
        }
    }
}

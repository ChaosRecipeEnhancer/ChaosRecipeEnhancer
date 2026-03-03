﻿using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class SettingsConfiguration
{
    // The exe name used to locate legacy user.config directories in %LocalAppData%.
    // .NET stores user settings under folders that start with this prefix, followed by
    // an evidence type and a hash derived from the exe's install path. When we switched
    // from MSI/ClickOnce to Velopack the install path changed, which changed the hash,
    // so Settings.Default.Upgrade() can no longer find the old settings. This constant
    // lets us search for ANY folder that belonged to this app regardless of evidence hash.
    internal const string SettingsFolderPrefix = "ChaosRecipeEnhancer";

    // The version users are migrating FROM when moving to the first Velopack release.
    // We look for a user.config under this version folder to confirm it's genuinely
    // from the pre-Velopack era and not some unrelated directory.
    internal const string LegacyVersion = "3.27.1000.0";

    // Settings we intentionally skip during migration:
    // - UpgradeSettingsAfterUpdate: internal flag, not a user preference.
    // - LegacySettingsMigrated: internal flag for this migration feature itself.
    // - V325MigrationCompleted: internal flag for one-time [3.25] data migrations.
    internal static readonly HashSet<string> SkipSettings = new(StringComparer.Ordinal)
    {
        "UpgradeSettingsAfterUpdate",
        "LegacySettingsMigrated",
        "V325MigrationCompleted",
    };

    public static void UpgradeSettings()
    {
        // --- Standard .NET settings upgrade (works within the SAME evidence hash) ---
        // When UpgradeSettingsAfterUpdate is true (its default), we call Upgrade() to
        // copy settings from the previous version's user.config into the current version.
        // This handles the normal Velopack-to-Velopack update path where the exe location
        // (and therefore evidence hash) stays the same.
        if (Settings.Default.UpgradeSettingsAfterUpdate)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeSettingsAfterUpdate = false;

            Settings.Default.Save();
        }

        // --- One-time [3.25] data migrations ---
        // These must only run ONCE, not on every version upgrade. Previously they
        // lived inside the UpgradeSettingsAfterUpdate block, which re-runs every
        // time a new version is deployed (because the flag defaults to true for any
        // new user.config folder). This caused LeagueName, StashTabIdentifiers, and
        // ActiveRecipeType to be overwritten on every upgrade.
        if (!Settings.Default.V325MigrationCompleted)
        {
            Settings.Default.LeagueName = "Standard";
            Settings.Default.StashTabIdentifiers = string.Empty;

            // Migrate ChaosRecipeTrackingEnabled to ActiveRecipeType
            // ChaosRecipeTrackingEnabled = true → ChaosOrb (0)
            // ChaosRecipeTrackingEnabled = false → RegalOrb (1)
            if (Settings.Default.ChaosRecipeTrackingEnabled)
            {
                Settings.Default.ActiveRecipeType = 0; // ChaosOrb
            }
            else
            {
                Settings.Default.ActiveRecipeType = 1; // RegalOrb
            }

            Settings.Default.V325MigrationCompleted = true;
            Settings.Default.Save();
        }

        // --- Legacy migration (MSI/ClickOnce → Velopack, one-time only) ---
        // When we switched from MSI/ClickOnce to Velopack, the exe moved to a new
        // directory (%LocalAppData%\ChaosRecipeEnhancer\current\). This changed the
        // "evidence hash" in the .NET user.config path, so Upgrade() above couldn't
        // find the old settings — they're in a sibling folder with a different hash.
        // This block searches for the old user.config, parses its XML, and imports
        // the values into the current settings. The LegacySettingsMigrated flag
        // ensures we only attempt this expensive search once.
        if (!Settings.Default.LegacySettingsMigrated)
        {
            try
            {
                MigrateLegacySettings();
            }
            catch (Exception ex)
            {
                // Migration is best-effort — if it fails (permissions, corrupt XML, etc.)
                // we log and continue with defaults. Users can still reconfigure manually.
                Log.Warning(ex, "Legacy settings migration failed — continuing with defaults");
            }

            // Mark as migrated regardless of success/failure so we don't re-attempt.
            Settings.Default.LegacySettingsMigrated = true;
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

    /// <summary>
    /// Searches %LocalAppData% for user.config files from the pre-Velopack install
    /// (version 3.27.1000.0) and imports their values into the current settings.
    /// </summary>
    private static void MigrateLegacySettings()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrEmpty(localAppData) || !Directory.Exists(localAppData))
        {
            return;
        }

        var candidateDirs = Directory.GetDirectories(localAppData, SettingsFolderPrefix + "*");

        if (candidateDirs.Length == 0)
        {
            Log.Information("Legacy migration: no candidate settings directories found");
            return;
        }

        // Get the current settings directory so we don't accidentally "migrate" from ourselves.
        var currentConfigPath = ConfigurationManager
            .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)
            .FilePath;
        var currentSettingsDir = Path.GetDirectoryName(Path.GetDirectoryName(currentConfigPath));

        var bestLegacyConfig = FindBestLegacyConfig(candidateDirs, currentSettingsDir);

        if (bestLegacyConfig == null)
        {
            Log.Information("Legacy migration: no user.config found for version {Version}", LegacyVersion);
            return;
        }

        Log.Information("Legacy migration: importing settings from {Path}", bestLegacyConfig);
        ImportUserConfig(bestLegacyConfig);
    }

    /// <summary>
    /// Searches an array of candidate directories for the most recently modified
    /// legacy user.config file, skipping the current app's own settings directory.
    /// </summary>
    /// <param name="candidateDirs">Directories in %LocalAppData% matching the app name prefix.</param>
    /// <param name="currentSettingsDir">
    /// The current app's settings root directory (two levels above user.config),
    /// or null if unknown. This directory is excluded from the search to prevent
    /// the app from importing its own settings.
    /// </param>
    /// <returns>
    /// Full path to the best legacy user.config, or null if no valid candidate was found.
    /// </returns>
    internal static string FindBestLegacyConfig(string[] candidateDirs, string currentSettingsDir)
    {
        string bestLegacyConfig = null;
        var bestWriteTime = DateTime.MinValue;

        foreach (var dir in candidateDirs)
        {
            // Skip our own current settings directory — we don't want to import from ourselves.
            if (currentSettingsDir != null && dir.Equals(currentSettingsDir, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var legacyConfigPath = Path.Combine(dir, LegacyVersion, "user.config");

            if (File.Exists(legacyConfigPath))
            {
                var writeTime = File.GetLastWriteTimeUtc(legacyConfigPath);

                if (writeTime > bestWriteTime)
                {
                    bestWriteTime = writeTime;
                    bestLegacyConfig = legacyConfigPath;
                }
            }
        }

        return bestLegacyConfig;
    }

    /// <summary>
    /// Parses a legacy user.config XML file and applies matching values to the current
    /// <see cref="Settings.Default"/> instance.
    /// </summary>
    private static void ImportUserConfig(string configPath)
    {
        var doc = XDocument.Load(configPath);
        var legacyValues = ParseUserConfigXml(doc);

        if (legacyValues.Count == 0)
        {
            Log.Information("Legacy migration: no settings values found in file");
            return;
        }

        var imported = ApplyLegacyValues(legacyValues, Settings.Default.Properties, SkipSettings, (name, value) =>
        {
            Settings.Default[name] = value;
        });

        Log.Information("Legacy migration: imported {Count} settings from legacy config", imported);

        if (imported > 0)
        {
            Settings.Default.Save();
        }
    }

    /// <summary>
    /// Parses the user.config XML document and extracts setting name/value pairs.
    /// </summary>
    /// <remarks>
    /// The user.config XML format:
    /// <code>
    /// &lt;configuration&gt;
    ///   &lt;userSettings&gt;
    ///     &lt;ChaosRecipeEnhancer.UI.Properties.Settings&gt;
    ///       &lt;setting name="LeagueName" serializeAs="String"&gt;
    ///         &lt;value&gt;Settlers&lt;/value&gt;
    ///       &lt;/setting&gt;
    ///     &lt;/ChaosRecipeEnhancer.UI.Properties.Settings&gt;
    ///   &lt;/userSettings&gt;
    /// &lt;/configuration&gt;
    /// </code>
    /// </remarks>
    /// <returns>
    /// Dictionary mapping setting names to their string values, or an empty dictionary
    /// if the settings section was not found or contained no valid entries.
    /// </returns>
    internal static Dictionary<string, string> ParseUserConfigXml(XDocument doc)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        // Navigate to the settings section — the element name matches our fully qualified settings class.
        var settingsSection = doc
            .Descendants("ChaosRecipeEnhancer.UI.Properties.Settings")
            .FirstOrDefault();

        if (settingsSection == null)
        {
            return result;
        }

        foreach (var setting in settingsSection.Elements("setting"))
        {
            var name = setting.Attribute("name")?.Value;
            var value = setting.Element("value")?.Value;

            // We accept empty string values (some settings legitimately default to ""),
            // but skip entries with missing or empty names, or missing <value> elements.
            if (!string.IsNullOrEmpty(name) && value != null)
            {
                result[name] = value;
            }
        }

        return result;
    }

    /// <summary>
    /// Applies parsed legacy values to the current settings, converting string values
    /// to their proper types using the settings property metadata.
    /// </summary>
    /// <param name="legacyValues">Name/value pairs parsed from the legacy user.config XML.</param>
    /// <param name="currentProperties">The current settings schema (Settings.Default.Properties).</param>
    /// <param name="skipSettings">Setting names that should not be migrated (auth tokens, internal flags).</param>
    /// <param name="applySetting">Callback to apply a converted value. Separated for testability.</param>
    /// <returns>The number of settings successfully imported.</returns>
    internal static int ApplyLegacyValues(
        Dictionary<string, string> legacyValues,
        SettingsPropertyCollection currentProperties,
        HashSet<string> skipSettings,
        Action<string, object> applySetting)
    {
        int imported = 0;

        foreach (var property in currentProperties)
        {
            if (property is not SettingsProperty prop)
            {
                continue;
            }

            if (skipSettings.Contains(prop.Name))
            {
                continue;
            }

            if (!legacyValues.TryGetValue(prop.Name, out var legacyValue))
            {
                continue;
            }

            try
            {
                // Convert the string value from XML to the property's actual type
                // using the same converter .NET uses internally for settings.
                var converted = System.ComponentModel.TypeDescriptor
                    .GetConverter(prop.PropertyType)
                    .ConvertFromInvariantString(legacyValue);

                applySetting(prop.Name, converted);
                imported++;
            }
            catch (Exception ex)
            {
                // If a single value fails to convert (e.g., type changed between versions),
                // skip it and keep going — partial migration is better than no migration.
                Log.Warning(ex, "Legacy migration: failed to import setting {Name}", prop.Name);
            }
        }

        return imported;
    }
}

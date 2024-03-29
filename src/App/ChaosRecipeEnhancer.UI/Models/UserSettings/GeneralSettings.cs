using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Chaos Recipe Enhancer General Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public string LeagueName
    {

        get => Settings.Default.LeagueName;
        set
        {
            if (Settings.Default.LeagueName != value)
            {
                Settings.Default.LeagueName = value;
                Save();
            }
        }
    }

    public string StashTabPrefix
    {
        get => Settings.Default.StashTabPrefix;
        set
        {
            if (Settings.Default.StashTabPrefix != value)
            {
                Settings.Default.StashTabPrefix = value;
                Save();
            }
        }
    }

    public HashSet<string> StashTabIndices
    {
        get => new((Settings.Default.StashTabIndices ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries));
        set
        {
            var indicesString = value != null ? string.Join(",", value) : "";
            if (Settings.Default.StashTabIndices != indicesString)
            {
                Settings.Default.StashTabIndices = indicesString;
                Save();
            }
        }
    }

    public HashSet<string> StashTabIds
    {
        get => new((Settings.Default.StashTabIdentifiers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries));
        set
        {
            var idsString = value != null ? string.Join(",", value) : "";
            if (Settings.Default.StashTabIdentifiers != idsString)
            {
                Settings.Default.StashTabIdentifiers = idsString;
                Save();
            }
        }
    }

    public int StashTabQueryMode
    {
        get => Settings.Default.StashTabQueryMode;
        set
        {
            if (Settings.Default.StashTabQueryMode != value)
            {
                Settings.Default.StashTabQueryMode = value;
                Save();
            }
        }
    }

    public bool AutoFetchOnRezoneEnabled
    {
        get => Settings.Default.AutoFetchOnRezoneEnabled;
        set
        {
            if (Settings.Default.AutoFetchOnRezoneEnabled != value)
            {
                Settings.Default.AutoFetchOnRezoneEnabled = value;
                Save();
            }
        }
    }

    public int PathOfExileClientLogLocationMode
    {
        get => Settings.Default.PathOfExileClientLogLocationMode;
        set
        {
            if (Settings.Default.PathOfExileClientLogLocationMode != value)
            {
                Settings.Default.PathOfExileClientLogLocationMode = value;
                Save();
            }
        }
    }

    public string PathOfExileClientLogLocation
    {
        get => Settings.Default.PathOfExileClientLogLocation;
        set
        {
            if (Settings.Default.PathOfExileClientLogLocation != value)
            {
                Settings.Default.PathOfExileClientLogLocation = value;
                Save();
            }
        }
    }
}

using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Chaos Recipe Enhancer Stash Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool GuildStashMode
    {

        get => Settings.Default.GuildStashMode;
        set
        {
            if (Settings.Default.GuildStashMode != value)
            {
                Settings.Default.GuildStashMode = value;
                Save();
            }
        }
    }

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

    public bool CustomLeagueEnabled
    {
        get => Settings.Default.CustomLeagueEnabled;
        set
        {
            if (Settings.Default.CustomLeagueEnabled != value)
            {
                Settings.Default.CustomLeagueEnabled = value;
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

    public bool HideRemoveOnlyTabs
    {
        get => Settings.Default.HideRemoveOnlyTabs;
        set
        {
            if (Settings.Default.HideRemoveOnlyTabs != value)
            {
                Settings.Default.HideRemoveOnlyTabs = value;
                Save();
            }
        }
    }
}

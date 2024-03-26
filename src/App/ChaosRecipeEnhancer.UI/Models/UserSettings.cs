using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System;

namespace ChaosRecipeEnhancer.UI.Models;

public interface IUserSettings
{
    // PoE Account & Client settings
    string PathOfExileAccountName { get; set; }
    string PathOfExileApiAuthToken { get; set; }
    DateTime PathOfExileApiAuthTokenExpiration { get; set; }
    ConnectionStatusTypes PoEAccountConnectionStatus { get; set; }
    string PathOfExileClientLogLocation { get; set; }

    // CRE General Settings
    int PathOfExileClientLogLocationMode { get; set; }
    string LeagueName { get; set; }
    int StashTabQueryMode { get; set; }
    string StashTabIndices { get; set; }
    string StashTabPrefix { get; set; }
    bool AutoFetchOnRezoneEnabled { get; set; }
    int FullSetThreshold { get; set; }
    bool DoNotPreserveLowItemLevelGear { get; set; }

    // CRE Window Settings
    int SettingsWindowNavIndex { get; set; }

    // CRE Overlay Settings
    SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode { get; set; }

    // CRE Loot Filter Settings
    bool LootFilterRingsAlwaysActive { get; set; }
    bool LootFilterAmuletsAlwaysActive { get; set; }
    bool LootFilterBeltsAlwaysActive { get; set; }
    bool LootFilterGlovesAlwaysActive { get; set; }
    bool LootFilterBootsAlwaysActive { get; set; }
    bool LootFilterHelmetsAlwaysActive { get; set; }
    bool LootFilterBodyArmourAlwaysActive { get; set; }
    bool LootFilterWeaponsAlwaysActive { get; set; }

    void Reset();
}


public class UserSettings : IUserSettings
{
    #region PoE Account & Client Settings

    public string PathOfExileAccountName
    {
        get => Settings.Default.PathOfExileAccountName;
        set
        {
            if (Settings.Default.PathOfExileAccountName != value)
            {
                Settings.Default.PathOfExileAccountName = value;
                Save();
            }
        }
    }

    public string PathOfExileApiAuthToken
    {
        get => Settings.Default.PathOfExileApiAuthToken;
        set
        {
            if (Settings.Default.PathOfExileApiAuthToken != value)
            {
                Settings.Default.PathOfExileApiAuthToken = value;
                Save();
            }
        }
    }

    public DateTime PathOfExileApiAuthTokenExpiration
    {
        get => Settings.Default.PathOfExileApiAuthTokenExpiration;
        set
        {
            if (Settings.Default.PathOfExileApiAuthTokenExpiration != value)
            {
                Settings.Default.PathOfExileApiAuthTokenExpiration = value;
                Save();
            }
        }
    }

    public ConnectionStatusTypes PoEAccountConnectionStatus
    {
        get => (ConnectionStatusTypes)Settings.Default.PoEAccountConnectionStatus;
        set
        {
            if (Settings.Default.PoEAccountConnectionStatus != (int)value)
            {
                Settings.Default.PoEAccountConnectionStatus = (int)value;
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

    #endregion

    #region CRE General Settings

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

    public string StashTabIndices
    {
        get => Settings.Default.StashTabIndices;
        set
        {
            if (Settings.Default.StashTabIndices != value)
            {
                Settings.Default.StashTabIndices = value;
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

    public int FullSetThreshold
    {
        get => Settings.Default.FullSetThreshold;
        set
        {
            if (Settings.Default.FullSetThreshold != value)
            {
                Settings.Default.FullSetThreshold = value;
                Save();
            }
        }
    }

    public bool DoNotPreserveLowItemLevelGear
    {
        get => Settings.Default.DoNotPreserveLowItemLevelGear;
        set
        {
            if (Settings.Default.DoNotPreserveLowItemLevelGear != value)
            {
                Settings.Default.DoNotPreserveLowItemLevelGear = value;
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


    #endregion

    #region CRE Window Settings

    public int SettingsWindowNavIndex
    {
        get => Settings.Default.SettingsWindowNavIndex;
        set
        {
            if (Settings.Default.SettingsWindowNavIndex != value)
            {
                Settings.Default.SettingsWindowNavIndex = value;
                Save();
            }
        }
    }

    #endregion

    #region CRE Overlay Settings

    public SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode
    {
        get => (SetTrackerOverlayItemCounterDisplayMode)Settings.Default.SetTrackerOverlayItemCounterDisplayMode;
        set
        {
            if (Settings.Default.SetTrackerOverlayItemCounterDisplayMode != (int)value)
            {
                Settings.Default.SetTrackerOverlayItemCounterDisplayMode = (int)value;
                Save();
            }
        }
    }

    #endregion

    #region CRE Loot Filter Settings

    public bool LootFilterRingsAlwaysActive
    {
        get => Settings.Default.LootFilterRingsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterRingsAlwaysActive != value)
            {
                Settings.Default.LootFilterRingsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterAmuletsAlwaysActive
    {
        get => Settings.Default.LootFilterAmuletsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterAmuletsAlwaysActive != value)
            {
                Settings.Default.LootFilterAmuletsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterBeltsAlwaysActive
    {
        get => Settings.Default.LootFilterBeltsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBeltsAlwaysActive != value)
            {
                Settings.Default.LootFilterBeltsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterGlovesAlwaysActive
    {
        get => Settings.Default.LootFilterGlovesAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterGlovesAlwaysActive != value)
            {
                Settings.Default.LootFilterGlovesAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterBootsAlwaysActive
    {
        get => Settings.Default.LootFilterBootsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBootsAlwaysActive != value)
            {
                Settings.Default.LootFilterBootsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterHelmetsAlwaysActive
    {
        get => Settings.Default.LootFilterHelmetsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterHelmetsAlwaysActive != value)
            {
                Settings.Default.LootFilterHelmetsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterBodyArmourAlwaysActive
    {
        get => Settings.Default.LootFilterBodyArmourAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBodyArmourAlwaysActive != value)
            {
                Settings.Default.LootFilterBodyArmourAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterWeaponsAlwaysActive
    {
        get => Settings.Default.LootFilterWeaponsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterWeaponsAlwaysActive != value)
            {
                Settings.Default.LootFilterWeaponsAlwaysActive = value;
                Save();
            }
        }
    }

    #endregion

    public void Reset()
    {
        Settings.Default.Reset();
        Settings.Default.Save();
    }

    private static void Save() => Settings.Default.Save();
}

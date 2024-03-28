using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
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
    bool ChaosRecipeTrackingEnabled { get; set; }
    bool IncludeIdentifiedItemsEnabled { get; set; }
    bool VendorSetsEarly { get; set; }
    int PathOfExileClientLogLocationMode { get; set; }
    string LeagueName { get; set; }
    int StashTabQueryMode { get; set; }
    string StashTabIndices { get; set; }
    string StashTabPrefix { get; set; }
    bool AutoFetchOnRezoneEnabled { get; set; }
    int FullSetThreshold { get; set; }
    bool DoNotPreserveLowItemLevelGear { get; set; }

    // CRE Window Settings
    bool CloseToTrayEnabled { get; set; }
    int SettingsWindowNavIndex { get; set; }
    Language Language { get; set; }

    // CRE Overlay Settings
    SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode { get; set; }
    SetTrackerOverlayMode SetTrackerOverlayMode { get; set; }
    bool SetTrackerOverlayOverlayLockPositionEnabled { get; set; }
    double SetTrackerOverlayTopPosition { get; set; }
    double SetTrackerOverlayLeftPosition { get; set; }
    bool SilenceNeedItemsMessage { get; set; }
    bool SilenceSetsFullMessage { get; set; }

    // CRE Loot Filter Settings
    bool LootFilterRingsAlwaysActive { get; set; }
    bool LootFilterAmuletsAlwaysActive { get; set; }
    bool LootFilterBeltsAlwaysActive { get; set; }
    bool LootFilterGlovesAlwaysActive { get; set; }
    bool LootFilterBootsAlwaysActive { get; set; }
    bool LootFilterHelmetsAlwaysActive { get; set; }
    bool LootFilterBodyArmourAlwaysActive { get; set; }
    bool LootFilterWeaponsAlwaysActive { get; set; }

    // CRE Sound Settings
    bool SoundEnabled { get; set; }
    double SoundLevel { get; set; }

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

    public bool ChaosRecipeTrackingEnabled
    {
        get => Settings.Default.ChaosRecipeTrackingEnabled;
        set
        {
            if (Settings.Default.ChaosRecipeTrackingEnabled != value)
            {
                Settings.Default.ChaosRecipeTrackingEnabled = value;
                Save();
            }
        }
    }

    public bool IncludeIdentifiedItemsEnabled
    {
        get => Settings.Default.IncludeIdentifiedItemsEnabled;
        set
        {
            if (Settings.Default.IncludeIdentifiedItemsEnabled != value)
            {
                Settings.Default.IncludeIdentifiedItemsEnabled = value;
                Save();
            }
        }
    }

    public bool VendorSetsEarly
    {
        get => Settings.Default.VendorSetsEarly;
        set
        {
            if (Settings.Default.VendorSetsEarly != value)
            {
                Settings.Default.VendorSetsEarly = value;
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
            Log.Information("StashTabIndices: {StashTabIndices}", value);
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

    public bool CloseToTrayEnabled
    {
        get => Settings.Default.CloseToTrayEnabled;
        set
        {
            if (Settings.Default.CloseToTrayEnabled != value)
            {
                Settings.Default.CloseToTrayEnabled = value;
                Save();
            }
        }
    }

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

    public Language Language
    {
        get => (Language)Settings.Default.Language;
        set
        {
            if (Settings.Default.Language != (int)value)
            {
                Settings.Default.Language = (int)value;
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

    public SetTrackerOverlayMode SetTrackerOverlayMode
    {
        get => (SetTrackerOverlayMode)Settings.Default.SetTrackerOverlayDisplayMode;
        set
        {
            if (Settings.Default.SetTrackerOverlayDisplayMode != (int)value)
            {
                Settings.Default.SetTrackerOverlayDisplayMode = (int)value;
                Save();
            }
        }
    }

    public bool SetTrackerOverlayOverlayLockPositionEnabled
    {
        get => Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled;
        set
        {
            if (Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled != value)
            {
                Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled = value;
                Save();
            }
        }
    }

    public double SetTrackerOverlayTopPosition
    {
        get => Settings.Default.SetTrackerOverlayTopPosition;
        set
        {
            if (Settings.Default.SetTrackerOverlayTopPosition != value)
            {
                Settings.Default.SetTrackerOverlayTopPosition = value;
                Save();
            }
        }
    }

    public double SetTrackerOverlayLeftPosition
    {
        get => Settings.Default.SetTrackerOverlayLeftPosition;
        set
        {
            if (Settings.Default.SetTrackerOverlayLeftPosition != value)
            {
                Settings.Default.SetTrackerOverlayLeftPosition = value;
                Save();
            }
        }
    }

    public bool SilenceNeedItemsMessage
    {
        get => Settings.Default.SilenceNeedItemsMessage;
        set
        {
            if (Settings.Default.SilenceNeedItemsMessage != value)
            {
                Settings.Default.SilenceNeedItemsMessage = value;
                Save();
            }
        }
    }

    public bool SilenceSetsFullMessage
    {
        get => Settings.Default.SilenceSetsFullMessage;
        set
        {
            if (Settings.Default.SilenceSetsFullMessage != value)
            {
                Settings.Default.SilenceSetsFullMessage = value;
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

    #region CRE Sound Settings

    public bool SoundEnabled
    {
        get => Settings.Default.SoundEnabled;
        set
        {
            if (Settings.Default.SoundEnabled != value)
            {
                Settings.Default.SoundEnabled = value;
                Save();
            }
        }
    }

    public double SoundLevel
    {
        get => Settings.Default.SoundLevel;
        set
        {
            if (Settings.Default.SoundLevel != value)
            {
                Settings.Default.SoundLevel = value;
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

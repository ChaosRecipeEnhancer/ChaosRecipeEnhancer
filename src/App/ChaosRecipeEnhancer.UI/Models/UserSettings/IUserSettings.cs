using ChaosRecipeEnhancer.UI.Models.Enums;
using System;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

public interface IUserSettings
{
    bool UpgradeSettingsAfterUpdate { get; set; }
    bool DebugMode { get; set; }

    # region PoE Account Settings

    string PathOfExileAccountName { get; set; }
    string PathOfExileApiAuthToken { get; set; }
    DateTime PathOfExileApiAuthTokenExpiration { get; set; }
    ConnectionStatusTypes PoEAccountConnectionStatus { get; set; }
    string PathOfExileClientLogLocation { get; set; }

    # endregion

    #region Legacy Auth Settings

    bool LegacyAuthMode { get; set; }
    string LegacyAuthAccountName { get; set; }
    string LegacyAuthSessionId { get; set; }

    #endregion

    # region Recipe Settings

    bool ChaosRecipeTrackingEnabled { get; set; }
    bool IncludeIdentifiedItemsEnabled { get; set; }
    bool VendorSetsEarly { get; set; }
    int PathOfExileClientLogLocationMode { get; set; }
    bool GuildStashMode { get; set; }
    string LeagueName { get; set; }
    bool CustomLeagueEnabled { get; set; }
    int StashTabQueryMode { get; set; }
    public HashSet<string> StashTabIds { get; set; }
    public HashSet<string> StashTabIndices { get; set; }
    string StashTabPrefix { get; set; }
    bool AutoFetchOnRezoneEnabled { get; set; }
    int FullSetThreshold { get; set; }
    bool DoNotPreserveLowItemLevelGear { get; set; }

    # endregion

    #region Settings Window Settings

    bool CloseToTrayEnabled { get; set; }
    int SettingsWindowNavIndex { get; set; }
    Language Language { get; set; }

    #endregion

    #region Set Tracker Overlay Settings

    double SetTrackerOverlayTopPosition { get; set; }
    double SetTrackerOverlayLeftPosition { get; set; }
    SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode { get; set; }
    SetTrackerOverlayMode SetTrackerOverlayMode { get; set; }
    bool SetTrackerOverlayOverlayLockPositionEnabled { get; set; }
    bool SilenceNeedItemsMessage { get; set; }
    bool SilenceSetsFullMessage { get; set; }

    #endregion

    #region Stash Tab Overlay Settings

    float StashTabOverlayOpacity { get; set; }
    string StashTabOverlayBackgroundColor { get; set; }
    string StashTabOverlayHighlightColor { get; set; }
    int StashTabOverlayHighlightMode { get; set; }
    double StashTabOverlayIndividualTabMargin { get; set; }
    double StashTabOverlayIndividualTabHeaderPadding { get; set; }
    double StashTabOverlayIndividualTabHeaderGap { get; set; }
    double StashTabOverlayTopPosition { get; set; }
    double StashTabOverlayLeftPosition { get; set; }
    double StashTabOverlayHeight { get; set; }
    double StashTabOverlayWidth { get; set; }
    string StashTabOverlayTabDefaultBackgroundColor { get; set; }
    double StashTabOverlayTabGroupBottomMargin { get; set; }
    float StashTabOverlayTabOpacity { get; set; }

    #endregion

    #region Loot Filter Settings

    bool LootFilterManipulationEnabled { get; set; }
    string LootFilterFileLocation { get; set; }
    bool LootFilterIconsEnabled { get; set; }
    bool LootFilterRingsAlwaysActive { get; set; }
    bool LootFilterAmuletsAlwaysActive { get; set; }
    bool LootFilterBeltsAlwaysActive { get; set; }
    bool LootFilterGlovesAlwaysActive { get; set; }
    bool LootFilterBootsAlwaysActive { get; set; }
    bool LootFilterHelmetsAlwaysActive { get; set; }
    bool LootFilterBodyArmourAlwaysActive { get; set; }
    bool LootFilterWeaponsAlwaysActive { get; set; }

    #endregion

    #region Hotkey Settings

    string FetchStashHotkey { get; set; }
    string ToggleSetTrackerOverlayHotkey { get; set; }
    string ToggleStashTabOverlayHotkey { get; set; }
    string ReloadFilterHotkey { get; set; }

    #endregion

    #region Sound Settings

    bool SoundEnabled { get; set; }
    double SoundLevel { get; set; }

    #endregion

    #region Color Settings

    string LootFilterRingColor { get; set; }
    string LootFilterAmuletColor { get; set; }
    string LootFilterBeltColor { get; set; }
    string LootFilterBootsColor { get; set; }
    string LootFilterGlovesColor { get; set; }
    string LootFilterHelmetColor { get; set; }
    string LootFilterBodyArmourColor { get; set; }
    string LootFilterWeaponColor { get; set; }

    #endregion

    public void Reset();
}
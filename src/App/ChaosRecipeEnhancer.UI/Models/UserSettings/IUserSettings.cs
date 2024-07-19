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
    string StashTabPrefix { get; set; }
    bool HideRemoveOnlyTabs { get; set; }
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

    #region Space Saving Settings

    bool LootFilterSpaceSavingHideLargeWeapons { get; set; }
    bool LootFilterSpaceSavingHideOffHand { get; set; }

    #endregion

    #region Loot Filter Settings - Ring

    int LootFilterStylesRingTextFontSize { get; set; }
    string LootFilterStylesRingTextColor { get; set; }
    string LootFilterStylesRingBorderColor { get; set; }
    string LootFilterStylesRingBackgroundColor { get; set; } // Formerly: LootFilterRingColor
    bool LootFilterStylesRingAlwaysActive { get; set; }
    bool LootFilterStylesRingAlwaysDisabled { get; set; }
    bool LootFilterStylesRingMapIconEnabled { get; set; }
    int LootFilterStylesRingMapIconColor { get; set; }
    int LootFilterStylesRingMapIconSize { get; set; }
    int LootFilterStylesRingMapIconShape { get; set; }
    bool LootFilterStylesRingBeamEnabled { get; set; }
    bool LootFilterStylesRingBeamTemporary { get; set; }
    int LootFilterStylesRingBeamColor { get; set; }
    bool LootFilterStylesRingTextColorEnabled { get; set; }
    bool LootFilterStylesRingBorderColorEnabled { get; set; }
    bool LootFilterStylesRingBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Amulet

    int LootFilterStylesAmuletTextFontSize { get; set; }
    string LootFilterStylesAmuletTextColor { get; set; }
    string LootFilterStylesAmuletBorderColor { get; set; }
    string LootFilterStylesAmuletBackgroundColor { get; set; } // Formerly: LootFilterAmuletColor
    bool LootFilterStylesAmuletAlwaysActive { get; set; }
    bool LootFilterStylesAmuletAlwaysDisabled { get; set; }
    bool LootFilterStylesAmuletMapIconEnabled { get; set; }
    int LootFilterStylesAmuletMapIconColor { get; set; }
    int LootFilterStylesAmuletMapIconSize { get; set; }
    int LootFilterStylesAmuletMapIconShape { get; set; }
    bool LootFilterStylesAmuletBeamEnabled { get; set; }
    bool LootFilterStylesAmuletBeamTemporary { get; set; }
    int LootFilterStylesAmuletBeamColor { get; set; }
    bool LootFilterStylesAmuletTextColorEnabled { get; set; }
    bool LootFilterStylesAmuletBorderColorEnabled { get; set; }
    bool LootFilterStylesAmuletBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Belt

    int LootFilterStylesBeltTextFontSize { get; set; }
    string LootFilterStylesBeltTextColor { get; set; }
    string LootFilterStylesBeltBorderColor { get; set; }
    string LootFilterStylesBeltBackgroundColor { get; set; } // Formerly: LootFilterBeltColor
    bool LootFilterStylesBeltAlwaysActive { get; set; }
    bool LootFilterStylesBeltAlwaysDisabled { get; set; }
    bool LootFilterStylesBeltMapIconEnabled { get; set; }
    int LootFilterStylesBeltMapIconColor { get; set; }
    int LootFilterStylesBeltMapIconSize { get; set; }
    int LootFilterStylesBeltMapIconShape { get; set; }
    bool LootFilterStylesBeltBeamEnabled { get; set; }
    bool LootFilterStylesBeltBeamTemporary { get; set; }
    int LootFilterStylesBeltBeamColor { get; set; }
    bool LootFilterStylesBeltTextColorEnabled { get; set; }
    bool LootFilterStylesBeltBorderColorEnabled { get; set; }
    bool LootFilterStylesBeltBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Boots

    int LootFilterStylesBootsTextFontSize { get; set; }
    string LootFilterStylesBootsTextColor { get; set; }
    string LootFilterStylesBootsBorderColor { get; set; }
    string LootFilterStylesBootsBackgroundColor { get; set; } // Formerly: LootFilterBootsColor
    bool LootFilterStylesBootsAlwaysActive { get; set; }
    bool LootFilterStylesBootsAlwaysDisabled { get; set; }
    bool LootFilterStylesBootsMapIconEnabled { get; set; }
    int LootFilterStylesBootsMapIconColor { get; set; }
    int LootFilterStylesBootsMapIconSize { get; set; }
    int LootFilterStylesBootsMapIconShape { get; set; }
    bool LootFilterStylesBootsBeamEnabled { get; set; }
    bool LootFilterStylesBootsBeamTemporary { get; set; }
    int LootFilterStylesBootsBeamColor { get; set; }
    bool LootFilterStylesBootsTextColorEnabled { get; set; }
    bool LootFilterStylesBootsBorderColorEnabled { get; set; }
    bool LootFilterStylesBootsBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Gloves

    int LootFilterStylesGlovesTextFontSize { get; set; }
    string LootFilterStylesGlovesTextColor { get; set; }
    string LootFilterStylesGlovesBorderColor { get; set; }
    string LootFilterStylesGlovesBackgroundColor { get; set; } // Formerly: LootFilterGlovesColor
    bool LootFilterStylesGlovesAlwaysActive { get; set; }
    bool LootFilterStylesGlovesAlwaysDisabled { get; set; }
    bool LootFilterStylesGlovesMapIconEnabled { get; set; }
    int LootFilterStylesGlovesMapIconColor { get; set; }
    int LootFilterStylesGlovesMapIconSize { get; set; }
    int LootFilterStylesGlovesMapIconShape { get; set; }
    bool LootFilterStylesGlovesBeamEnabled { get; set; }
    bool LootFilterStylesGlovesBeamTemporary { get; set; }
    int LootFilterStylesGlovesBeamColor { get; set; }
    bool LootFilterStylesGlovesTextColorEnabled { get; set; }
    bool LootFilterStylesGlovesBorderColorEnabled { get; set; }
    bool LootFilterStylesGlovesBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Helmet

    int LootFilterStylesHelmetTextFontSize { get; set; }
    string LootFilterStylesHelmetTextColor { get; set; }
    string LootFilterStylesHelmetBorderColor { get; set; }
    string LootFilterStylesHelmetBackgroundColor { get; set; } // Formerly: LootFilterHelmetColor
    bool LootFilterStylesHelmetAlwaysActive { get; set; }
    bool LootFilterStylesHelmetAlwaysDisabled { get; set; }
    bool LootFilterStylesHelmetMapIconEnabled { get; set; }
    int LootFilterStylesHelmetMapIconColor { get; set; }
    int LootFilterStylesHelmetMapIconSize { get; set; }
    int LootFilterStylesHelmetMapIconShape { get; set; }
    bool LootFilterStylesHelmetBeamEnabled { get; set; }
    bool LootFilterStylesHelmetBeamTemporary { get; set; }
    int LootFilterStylesHelmetBeamColor { get; set; }
    bool LootFilterStylesHelmetTextColorEnabled { get; set; }
    bool LootFilterStylesHelmetBorderColorEnabled { get; set; }
    bool LootFilterStylesHelmetBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Body Armour

    int LootFilterStylesBodyArmourTextFontSize { get; set; }
    string LootFilterStylesBodyArmourTextColor { get; set; }
    string LootFilterStylesBodyArmourBorderColor { get; set; }
    string LootFilterStylesBodyArmourBackgroundColor { get; set; } // Formerly: LootFilterBodyArmourColor
    bool LootFilterStylesBodyArmourAlwaysActive { get; set; }
    bool LootFilterStylesBodyArmourAlwaysDisabled { get; set; }
    bool LootFilterStylesBodyArmourMapIconEnabled { get; set; }
    int LootFilterStylesBodyArmourMapIconColor { get; set; }
    int LootFilterStylesBodyArmourMapIconSize { get; set; }
    int LootFilterStylesBodyArmourMapIconShape { get; set; }
    bool LootFilterStylesBodyArmourBeamEnabled { get; set; }
    bool LootFilterStylesBodyArmourBeamTemporary { get; set; }
    int LootFilterStylesBodyArmourBeamColor { get; set; }
    bool LootFilterStylesBodyArmourTextColorEnabled { get; set; }
    bool LootFilterStylesBodyArmourBorderColorEnabled { get; set; }
    bool LootFilterStylesBodyArmourBackgroundColorEnabled { get; set; }

    #endregion

    #region Loot Filter Settings - Weapon

    int LootFilterStylesWeaponTextFontSize { get; set; }
    string LootFilterStylesWeaponTextColor { get; set; }
    string LootFilterStylesWeaponBorderColor { get; set; }
    string LootFilterStylesWeaponBackgroundColor { get; set; } // Formerly: LootFilterWeaponColor
    bool LootFilterStylesWeaponAlwaysActive { get; set; }
    bool LootFilterStylesWeaponAlwaysDisabled { get; set; }
    bool LootFilterStylesWeaponMapIconEnabled { get; set; }
    int LootFilterStylesWeaponMapIconColor { get; set; }
    int LootFilterStylesWeaponMapIconSize { get; set; }
    int LootFilterStylesWeaponMapIconShape { get; set; }
    bool LootFilterStylesWeaponBeamEnabled { get; set; }
    bool LootFilterStylesWeaponBeamTemporary { get; set; }
    int LootFilterStylesWeaponBeamColor { get; set; }
    bool LootFilterStylesWeaponTextColorEnabled { get; set; }
    bool LootFilterStylesWeaponBorderColorEnabled { get; set; }
    bool LootFilterStylesWeaponBackgroundColorEnabled { get; set; }

    #endregion

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

    public void Reset();
}
using System;
using System.IO;
using ChaosRecipeEnhancer.App.Models.Settings;
using ConfOxide;

namespace ChaosRecipeEnhancer.App.Services
{
    public class SettingsService
    {
        #region Fields

        private Settings _settings;
        private string FolderName => "ChaosRecipeEnhancer";
        private string FileName => "Settings.json";
        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string SettingsFolderPath => Path.Combine(AppDataFolderPath, FolderName);
        private string SettingsFilePath => Path.Combine(SettingsFolderPath, FileName);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService()
        {
            _settings = new Settings();
            if (!Directory.Exists(SettingsFolderPath))
            {
                Directory.CreateDirectory(SettingsFolderPath);
            }

            if (!File.Exists(SettingsFilePath))
            {
                CreateDefaultSettings();
            }
            else
            {
                try
                {
                    _settings.ReadJsonFile(SettingsFilePath);
                }
                catch
                {
                    File.Delete(SettingsFilePath);
                    CreateDefaultSettings();
                }
            }

            if (string.IsNullOrEmpty(_settings.CREClientId))
            {
                _settings.CREClientId = Guid.NewGuid().ToString();
                Save();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [on save].
        /// </summary>
        public event EventHandler OnSave;

        #endregion

        #region Properties

        #region User Account Settings

        public string AccountName
        {
            get => _settings.accName;
            set => _settings.accName = value;
        }

        public string SessionId
        {
            get => _settings.SessionId;
            set => _settings.SessionId = value;
        }

        public string CREClientId
        {
            get => _settings.CREClientId;
            set => _settings.CREClientId = value;
        }

        #endregion

        #region General App Settings

        public string GGGClientLogFileLocation
        {
            get => _settings.LogLocation;
            set => _settings.LogLocation = value;
        }

        public string CurrentLeague
        {
            get => _settings.League;
            set => _settings.League = value;
        }

        public bool SelectFromMainLeagues
        {
            get => _settings.MainLeague;
            set => _settings.MainLeague = value;
        }

        public bool InputCustomLeagueName
        {
            get => _settings.CustomLeague;
            set => _settings.CustomLeague = value;
        }

        public int SetsThreshold
        {
            get => _settings.Sets;
            set => _settings.Sets = value;
        }

        public int StashOverlayHighlightMode
        {
            get => _settings.HighlightMode;
            set => _settings.HighlightMode = value;
        }

        // Name may be the opposite of what it does
        public bool DoNotPreserveLowItemLevelGear
        {
            get => _settings.DoNotPreserveLowItemLevelGear;
            set => _settings.DoNotPreserveLowItemLevelGear = value;
        }

        public int MainOverlayMode
        {
            get => _settings.OverlayMode;
            set => _settings.OverlayMode = value;
        }

        public int MainOverlayItemAmountDisplayMode
        {
            get => _settings.ShowItemAmount;
            set => _settings.ShowItemAmount = value;
        }

        public bool IncludeIdentifiedItems
        {
            get => _settings.IncludeIdentified;
            set => _settings.IncludeIdentified = value;
        }

        public bool ChaosRecipe
        {
            get => _settings.ChaosRecipe;
            set => _settings.ChaosRecipe = value;
        }

        public bool RegalRecipe
        {
            get => _settings.RegalRecipe;
            set => _settings.RegalRecipe = value;
        }

        public bool ExaltRecipe
        {
            get => _settings.ExaltRecipe;
            set => _settings.ExaltRecipe = value;
        }

        public bool CloseToTray
        {
            get => _settings.hideOnClose;
            set => _settings.hideOnClose = value;
        }

        public bool AutoFetchStashContentsOnRezone
        {
            get => _settings.AutoFetch;
            set => _settings.AutoFetch = value;
        }

        // TODO Should be enum
        public int Language
        {
            get => _settings.Language;
            set => _settings.Language = value;
        }

        public string StashTabOverlayBackgroundColor
        {
            get => _settings.StashTabBackgroundColor;
            set => _settings.StashTabBackgroundColor = value;
        }

        public int StashTabQueryMode
        {
            get => _settings.StashTabMode;
            set => _settings.StashTabMode = value;
        }

        public string StashTabQueryString
        {
            get => _settings.StashTabName;
            set => _settings.StashTabName = value;
        }

        public string StashTabQueryIndices
        {
            get => _settings.StashTabIndices;
            set => _settings.StashTabIndices = value;
        }

        public bool LootFilterIcons
        {
            get => _settings.LootFilterIcons;
            set => _settings.LootFilterIcons = value;
        }

        public bool LootFilterManipulationActive
        {
            get => _settings.LootFilterActive;
            set => _settings.LootFilterActive = value;
        }

        public string LootFilterFileLocation
        {
            get => _settings.LootFilterLocation;
            set => _settings.LootFilterLocation = value;
        }

        public bool Sound
        {
            get => _settings.Sound;
            set => _settings.Sound = value;
        }

        public int Volume
        {
            get => _settings.Volume;
            set => _settings.Volume = value;
        }

        public string ItemPickupSoundFileLocation
        {
            get => _settings.ItemPickupSoundFileLocation;
            set => _settings.ItemPickupSoundFileLocation = value;
        }

        public string FilterChangeSoundFileLocation
        {
            get => _settings.FilterChangeSoundFileLocation;
            set => _settings.FilterChangeSoundFileLocation = value;
        }

        // The icons on the main overlay will stay at 100% opacity (i.e. active state) if these are set to true

        public bool HelmetIconAlwaysActive
        {
            get => _settings.HelmetsAlwaysActive;
            set => _settings.HelmetsAlwaysActive = value;
        }

        public bool ChestIconAlwaysActive
        {
            get => _settings.ChestsAlwaysActive;
            set => _settings.ChestsAlwaysActive = value;
        }

        public bool GloveIconAlwaysActive
        {
            get => _settings.GlovesAlwaysActive;
            set => _settings.GlovesAlwaysActive = value;
        }

        public bool BootIconAlwaysActive
        {
            get => _settings.BootsAlwaysActive;
            set => _settings.BootsAlwaysActive = value;
        }

        public bool WeaponIconAlwaysActive
        {
            get => _settings.WeaponsAlwaysActive;
            set => _settings.WeaponsAlwaysActive = value;
        }

        public bool RingIconAlwaysActive
        {
            get => _settings.RingsAlwaysActive;
            set => _settings.RingsAlwaysActive = value;
        }

        public bool AmuletIconAlwaysActive
        {
            get => _settings.AmuletsAlwaysActive;
            set => _settings.AmuletsAlwaysActive = value;
        }

        public bool BeltIconAlwaysActive
        {
            get => _settings.BeltsAlwaysActive;
            set => _settings.BeltsAlwaysActive = value;
        }

        #endregion

        // TODO: Rename a ton of these, because they're confusing

        #region App UI Settings

        public bool LockOverlayPosition
        {
            get => _settings.LockOverlayPosition;
            set => _settings.LockOverlayPosition = value;
        }

        public double TopMain
        {
            get => _settings.TopMain;
            set => _settings.TopMain = value;
        }

        public double LeftMain
        {
            get => _settings.LeftMain;
            set => _settings.LeftMain = value;
        }

        public double TopOverlay
        {
            get => _settings.TopOverlay;
            set => _settings.TopOverlay = value;
        }

        public double LeftOverlay
        {
            get => _settings.LeftOverlay;
            set => _settings.LeftOverlay = value;
        }

        public float Opacity
        {
            get => _settings.Opacity;
            set => _settings.Opacity = value;
        }

        public double TopStashTabOverlay
        {
            get => _settings.TopStashTabOverlay;
            set => _settings.TopStashTabOverlay = value;
        }

        public double LeftStashTabOverlay
        {
            get => _settings.LeftStashTabOverlay;
            set => _settings.LeftStashTabOverlay = value;
        }

        public double XStashTabOverlay
        {
            get => _settings.XStashTabOverlay;
            set => _settings.XStashTabOverlay = value;
        }

        public double YStashTabOverlay
        {
            get => _settings.YStashTabOverlay;
            set => _settings.YStashTabOverlay = value;
        }

        public float OpacityStashTab
        {
            get => _settings.OpacityStashTab;
            set => _settings.OpacityStashTab = value;
        }

        public double TabHeaderWidth
        {
            get => _settings.TabHeaderWidth;
            set => _settings.TabHeaderWidth = value;
        }

        public double TabHeaderGap
        {
            get => _settings.TabHeaderGap;
            set => _settings.TabHeaderGap = value;
        }

        public double TabMargin
        {
            get => _settings.TabMargin;
            set => _settings.TabMargin = value;
        }

        public string ColorStash
        {
            get => _settings.ColorStash;
            set => _settings.ColorStash = value;
        }

        #endregion

        #region Hotkey Settings

        // Is this even used?
        public string HotkeyToggle
        {
            get => _settings.HotkeyToggle;
            set => _settings.HotkeyToggle = value;
        }

        public string HotkeyRefresh
        {
            get => _settings.HotkeyRefresh;
            set => _settings.HotkeyRefresh = value;
        }

        public string HotkeyStashTab
        {
            get => _settings.HotkeyStashTab;
            set => _settings.HotkeyStashTab = value;
        }

        public string HotkeyReloadFilter
        {
            get => _settings.HotkeyReloadFilter;
            set => _settings.HotkeyReloadFilter = value;
        }

        #endregion

        #region Filter Style Changes

        public string ColorHelmet
        {
            get => _settings.ColorHelmet;
            set => _settings.ColorHelmet = value;
        }

        public string ColorChest
        {
            get => _settings.ColorChest;
            set => _settings.ColorChest = value;
        }

        public string ColorGloves
        {
            get => _settings.ColorGloves;
            set => _settings.ColorGloves = value;
        }

        public string ColorBoots
        {
            get => _settings.ColorBoots;
            set => _settings.ColorBoots = value;
        }

        public string ColorWeapon
        {
            get => _settings.ColorWeapon;
            set => _settings.ColorWeapon = value;
        }

        public string ColorAmulet
        {
            get => _settings.ColorAmulet;
            set => _settings.ColorAmulet = value;
        }

        public string ColorRing
        {
            get => _settings.ColorAmulet;
            set => _settings.ColorAmulet = value;
        }

        public string ColorBelt
        {
            get => _settings.ColorBelt;
            set => _settings.ColorBelt = value;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
        public void Save(bool raiseEvent = true)
        {
            _settings.WriteJsonFile(SettingsFilePath);
            if (raiseEvent)
            {
                OnSave?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates the default settings.
        /// </summary>
        private void CreateDefaultSettings()
        {
            using (File.Create(SettingsFilePath))
            {
            }

            Save();
        }

        public void ResetSettings()
        {
            File.Delete(SettingsFilePath);

            CreateDefaultSettings();

            Save();
        }

        #endregion
    }
}
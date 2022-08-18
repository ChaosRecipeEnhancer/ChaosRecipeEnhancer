using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ChaosRecipeEnhancer.App.Helpers;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;
using Serilog;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.View
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsView : INotifyPropertyChanged
    {
        #region Constructors

        public SettingsView(SetTrackerOverlayView setTrackerOverlayView, StashTabOverlayView stashTabOverlayView)
        {
            _logger = Log.ForContext<SettingsView>();
            _logger.Debug("Constructing SettingsView");

            _setTrackerOverlayView = setTrackerOverlayView;
            _stashTabOverlayView = stashTabOverlayView;

            InitializeComponent();
            DataContext = this;
            AutoUpdateHelper.InitializeAutoUpdater(AppVersion);
            
            InitializeColors();
            InitializeHotkeys();
            InitializeTray();

            // add Action to MouseHook
            MouseHook.MouseAction += (s, e) => Coordinates.OverlayClickEvent(_stashTabOverlayView);

            _logger.Debug("SettingsView constructed successfully");
        }

        #endregion

        #region Fields

        private readonly ILogger _logger;

        private readonly SetTrackerOverlayView _setTrackerOverlayView;
        private readonly StashTabOverlayView _stashTabOverlayView;

        // This version # should match up with the format for Assembly version # (3 dots, 4 digits), or else you'll get spammed for AutoUpdates
        private const string AppVersion = "1.6.0.1";
        
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();

        // ReSharper disable once UnusedMember.Local
        private static string RunButtonContent { get; set; } = "Run Overlay";

        private ContextMenu _contextMenu;
        private MenuItem _menuItem;
        private MenuItem _menuItemUpdate;
        private bool _trayClose;

        #endregion

        #region Properties

        public static bool SettingsComplete { get; set; }

        // ReSharper disable once UnusedMember.Global
        public static string AppVersionText { get; set; } = "v." + AppVersion;

        // TODO: [Refactor] Query by folder name stuff (doesn't work; not supported by API)
        // public Visibility FolderNameVisible
        // {
        //     get => _folderNameVisible;
        //     set
        //     {
        //         if (_folderNameVisible != value)
        //         {
        //             _folderNameVisible = value;
        //             OnPropertyChanged("FolderNameVisible");
        //         }
        //     }
        // }

        #endregion

        #region Event Handlers

        // Minimize to system tray when application is closed.
        protected override void OnClosing(CancelEventArgs e)
        {
            // if hideOnClose
            // setting cancel to true will cancel the close request
            // so the application is not closed
            if (Settings.Default.CloseToTrayEnabled && !_trayClose)
            {
                e.Cancel = true;

                Hide();

                base.OnClosing(e);
            }

            if (!Settings.Default.CloseToTrayEnabled || _trayClose)
            {
                _notifyIcon.Visible = false;

                MouseHook.Stop();

                HotkeysManager.ShutdownSystemHook();

                Settings.Default.Save();

                if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive)
                    LogWatcher.StopWatchingLogFile();

                Application.Current.Shutdown();
            }
        }

        private void CheckForUpdates_Click(object Sender, EventArgs e)
        {
            AutoUpdateHelper.CheckForUpdates();
        }

        // Close the form, which closes the application.
        private void MenuItem_Click(object Sender, EventArgs e)
        {
            _trayClose = true;
            Close();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunOverlay();
        }

        private void ColorBootsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterBootsColor = ColorBootsPicker.SelectedColor.ToString();
        }

        private void ColorGlovesPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterGlovesColor = ColorGlovesPicker.SelectedColor.ToString();
        }

        private void ColorHelmetPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterHelmetColor = ColorHelmetPicker.SelectedColor.ToString();
        }

        private void ColorChestPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterBodyArmourColor = ColorChestPicker.SelectedColor.ToString();
        }

        private void ColorWeaponsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterWeaponColor = ColorWeaponsPicker.SelectedColor.ToString();
        }

        private void ColorStashPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.StashTabOverlayHighlightColor = ColorStashPicker.SelectedColor.ToString();
        }

        private void ColorRingPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterRingColor = ColorRingPicker.SelectedColor.ToString();
        }

        private void ColorAmuletPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterAmuletColor = ColorAmuletPicker.SelectedColor.ToString();
        }

        private void ColorBeltPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.LootFilterBeltColor = ColorBeltPicker.SelectedColor.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        private void VolumeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Data.PlayNotificationSound();
        }

        private void ColorStashBackgroundPicker_SelectedColorChanged(object sender,
            RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.StashTabOverlayBackgroundColor = ColorStashBackgroundPicker.SelectedColor.ToString();
        }

        private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyView)
                    isWindowOpen = true;

            if (isWindowOpen) return;
            var hotkeyDialog = new HotkeyView(this, "toggle");
            hotkeyDialog.Show();
        }

        private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyView)
                    isWindowOpen = true;

            if (isWindowOpen) return;
            var hotkeyDialog = new HotkeyView(this, "refresh");
            hotkeyDialog.Show();
        }

        private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyView)
                    isWindowOpen = true;

            if (!isWindowOpen)
            {
                var hotkeyDialog = new HotkeyView(this, "stashtab");
                hotkeyDialog.Show();
            }
        }

        private void LootFilterFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "LootFilter|*.filter";
            var res = open.ShowDialog();

            if (res != System.Windows.Forms.DialogResult.OK) return;

            var filename = open.FileName;
            Settings.Default.LootFilterFileLocation = filename;
            LootFilterFileDialog.Content = filename;
        }

        private void TabHeaderGapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _stashTabOverlayView.StashTabOverlayIndividualTabHeaderGap =
                new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderGap, 0,
                    Settings.Default.StashTabOverlayIndividualTabHeaderGap, 0);
        }

        private void TabHeaderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StashTabList.StashTabs.Count <= 0) return;

            foreach (var s in StashTabList.StashTabs)
                s.TabHeaderWidth =
                    new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2,
                        Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2);
        }

        private void TabHeaderMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _stashTabOverlayView.StashTabOverlayIndividualTabMargin =
                new Thickness(Settings.Default.StashTabOverlayIndividualTabMargin, 0, 0, 0);
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void SetTrackerOverlayDisplayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Settings.Default.SetTrackerOverlayDisplayMode)
            {
                case 0:
                    Trace.WriteLine(
                        $"OverlayModeComboBox_SelectionChanged: Case 0 {Settings.Default.SetTrackerOverlayDisplayMode}");
                    _setTrackerOverlayView.MainOverlayContentControl.Content =
                        new StandardDisplay(this, _setTrackerOverlayView);
                    break;
                case 1:
                    Trace.WriteLine(
                        $"OverlayModeComboBox_SelectionChanged: Case 1 {Settings.Default.SetTrackerOverlayDisplayMode}");
                    _setTrackerOverlayView.MainOverlayContentControl.Content =
                        new MinifiedDisplay(this, _setTrackerOverlayView);
                    break;
                case 2:
                    Trace.WriteLine(
                        $"OverlayModeComboBox_SelectionChanged: Case 2 {Settings.Default.SetTrackerOverlayDisplayMode}");
                    _setTrackerOverlayView.MainOverlayContentControl.Content =
                        new OnlyButtonsDisplay(this, _setTrackerOverlayView);
                    break;
                case 3:
                    Trace.WriteLine(
                        $"OverlayModeComboBox_SelectionChanged: Case 3 {Settings.Default.SetTrackerOverlayDisplayMode}");
                    _setTrackerOverlayView.MainOverlayContentControl.Content =
                        new VerticalStandardDisplay(this, _setTrackerOverlayView);
                    break;
                case 4:
                    Trace.WriteLine(
                        $"OverlayModeComboBox_SelectionChanged: Case 4 {Settings.Default.SetTrackerOverlayDisplayMode}");
                    _setTrackerOverlayView.MainOverlayContentControl.Content =
                        new VerticalMinifiedDisplay(this, _setTrackerOverlayView);
                    break;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This will reset all of your settings!", "Reset Settings",
                MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Settings.Default.Reset();
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                    break;
                case MessageBoxResult.Cancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowNumbersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _setTrackerOverlayView.AmountsVisibility = Settings.Default.SetTrackerOverlayItemCounterDisplayMode != 0
                ? Visibility.Visible
                : Visibility.Hidden;
        }
        
        #endregion

        #region Methods

        private void InitializeHotkeys()
        {
            HotkeysManager.SetupSystemHook();
            HotkeysManager.GetRefreshHotkey();
            HotkeysManager.GetToggleHotkey();
            HotkeysManager.GetStashTabHotkey();
            AddAllHotkeys();
        }

        private void InitializeColors()
        {
            if (Settings.Default.LootFilterBootsColor != "")
                ColorBootsPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBootsColor);
            if (Settings.Default.LootFilterBodyArmourColor != "")
                ColorChestPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBodyArmourColor);
            if (Settings.Default.LootFilterWeaponColor != "")
                ColorWeaponsPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterWeaponColor);
            if (Settings.Default.LootFilterGlovesColor != "")
                ColorGlovesPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterGlovesColor);
            if (Settings.Default.LootFilterHelmetColor != "")
                ColorHelmetPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterHelmetColor);
            if (Settings.Default.StashTabOverlayHighlightColor != "")
                ColorStashPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor);
            if (Settings.Default.StashTabOverlayBackgroundColor != "")
                ColorStashBackgroundPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayBackgroundColor);
            if (Settings.Default.LootFilterRingColor != "")
                ColorRingPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterRingColor);
            if (Settings.Default.LootFilterAmuletColor != "")
                ColorAmuletPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterAmuletColor);
            if (Settings.Default.LootFilterBeltColor != "")
                ColorBeltPicker.SelectedColor =
                    (Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBeltColor);
        }

        // creates tray icon with menu
        private void InitializeTray()
        {
            _notifyIcon.Icon = Properties.Resources.coin;
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            new Container();
            _contextMenu = new ContextMenu();
            _menuItem = new MenuItem();
            _menuItemUpdate = new MenuItem();

            // Initialize contextMenu1
            _contextMenu.MenuItems.AddRange(new[] { _menuItem, _menuItemUpdate });

            // Initialize menuItem1
            _menuItem.Index = 1;
            _menuItem.Text = "E&xit";
            _menuItem.Click += MenuItem_Click;

            // Initialize menuItemUpdate
            _menuItemUpdate.Index = 0;
            _menuItemUpdate.Text = "C&heck for Updates";
            _menuItemUpdate.Click += CheckForUpdates_Click;

            _notifyIcon.ContextMenu = _contextMenu;
        }

        public void RunOverlay()
        {
            if (_setTrackerOverlayView.IsOpen)
            {
                _setTrackerOverlayView.Hide();
                if (_stashTabOverlayView.IsOpen) _stashTabOverlayView.Hide();
                RunButton.Content = "Run Overlay";
            }
            else
            {
                if (CheckAllSettings())
                {
                    _setTrackerOverlayView.Show();
                    RunButton.Content = "Stop Overlay";
                }
            }
        }

        public void RunStashTabOverlay()
        {
            var ready = CheckAllSettings();
            if (ready)
            {
                if (_stashTabOverlayView.IsOpen)
                    _stashTabOverlayView.Hide();
                else
                    _stashTabOverlayView.Show();
            }
        }

        public void AddAllHotkeys()
        {
            if (Settings.Default.FetchStashHotkey != "< not set >")
                HotkeysManager.AddHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey,
                    _setTrackerOverlayView.RunFetching);
            if (Settings.Default.ToggleSetTrackerOverlayHotkey != "< not set >")
                HotkeysManager.AddHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey, RunOverlay);
            if (Settings.Default.ToggleStashTabOverlayHotkey != "< not set >")
                HotkeysManager.AddHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey,
                    RunStashTabOverlay);
        }

        public void RemoveAllHotkeys()
        {
            HotkeysManager.RemoveRefreshHotkey();
            HotkeysManager.RemoveStashTabHotkey();
            HotkeysManager.RemoveToggleHotkey();
        }

        private string GetSoundFilePath()
        {
            var open = new OpenFileDialog();
            open.Filter = "MP3|*.mp3";
            var res = open.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK) return open.FileName;

            return null;
        }

        public static bool CheckAllSettings()
        {
            var accName = Settings.Default.PathOfExileAccountName;
            var sessId = Settings.Default.PathOfExileWebsiteSessionId;
            var league = Settings.Default.LeagueName;
            var lootFilterLocation = Settings.Default.LootFilterFileLocation;
            var lootFilterActive = Settings.Default.LootFilterManipulationEnabled;
            var logLocation = Settings.Default.PathOfExileClientLogLocation;
            var autoFetch = Settings.Default.AutoFetchOnRezoneEnabled;

            var missingSettings = new List<string>();

            if (accName == "") missingSettings.Add("- Account Name \n");
            if (sessId == "") missingSettings.Add("- PoE Session ID \n");
            if (league == "") missingSettings.Add("- League \n");
            if (lootFilterActive)
                if (lootFilterLocation == "")
                    missingSettings.Add("- Loot Filter Location \n");

            if (autoFetch)
                if (logLocation == "")
                    missingSettings.Add("- Log File Location \n");

            switch (Settings.Default.StashTabQueryMode)
            {
                case 0:
                {
                    if (Settings.Default.StashTabIndices == "") missingSettings.Add("- StashTab Index");
                    break;
                }
                case 1:
                {
                    if (Settings.Default.StashTabPrefix == "") missingSettings.Add("- StashTab Prefix");
                    break;
                }
                case 2:
                {
                    if (Settings.Default.StashTabSuffix == "") missingSettings.Add("- StashTab Suffix");
                    break;
                }
                // TODO: [Refactor] Query by folder name stuff (doesn't work; not supported by API)
                // case 3:
                //     {
                //         if (Settings.Default.StashFolderName == "") missingSettings.Add("- StashFolder Name");
                //         break;
                //     }
            }

            if (missingSettings.Count > 0)
            {
                SettingsComplete = false;
            }
            else
            {
                SettingsComplete = true;
                return true;
            }

            var errorMessage = missingSettings.Aggregate("Please add: \n", (current, setting) => current + setting);

            MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        #region INotifyPropertyChanged implementation

        // Basically, the UI thread subscribes to this event and update the binding if the received Property Name correspond to the Binding Path element
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
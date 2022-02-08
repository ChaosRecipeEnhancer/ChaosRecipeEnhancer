using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using EnhancePoE.Model;
using EnhancePoE.Properties;
using EnhancePoE.UserControls;
using EnhancePoE.View;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;

//using EnhancePoE.TabItemViewModel;

namespace EnhancePoE
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly string appVersion = "1.3.2.0";

        public static ChaosRecipeEnhancer overlay = new ChaosRecipeEnhancer();

        public static StashTabWindow stashTabOverlay = new StashTabWindow();

        public static MainWindow instance;

        private Visibility _indicesVisible = Visibility.Hidden;
        private Visibility _nameVisible = Visibility.Hidden;
        private IContainer components;
        private ContextMenu contextMenu;
        private MenuItem menuItem;
        private MenuItem menuItemUpdate;

        private readonly NotifyIcon ni = new NotifyIcon();


        private bool trayClose;

        public MainWindow()
        {
            instance = this;
            //Properties.Settings.Default.Reset();
            InitializeComponent();
            DataContext = this;
            NETAutoupdater.InitializeAutoupdater(appVersion);

            //Data.InitializeBases();
            if (!string.IsNullOrEmpty(Settings.Default.FilterChangeSoundFileLocation) && !FilterSoundLocationDialog.Content.Equals("Default Sound"))
                Data.Player.Open(new Uri(Settings.Default.FilterChangeSoundFileLocation));
            else
                //Data.Player.Open(new Uri(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Sounds\filterchanged.mp3")));
                Data.Player.Open(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\filterchanged.mp3")));

            if (!string.IsNullOrEmpty(Settings.Default.ItemPickupSoundFileLocation) && !ItemPickupLocationDialog.Content.Equals("Default Sound"))
                Data.PlayerSet.Open(new Uri(Settings.Default.ItemPickupSoundFileLocation));
            else
                //Data.PlayerSet.Open(new Uri(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Sounds\itemsPickedUp.mp3")));
                Data.PlayerSet.Open(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\itemsPickedUp.mp3")));

            //TESTING SETTINGS RESET
            //if (Debugger.IsAttached)
            //    Properties.Settings.Default.Reset();

            // initialize stashtabs
            //DataContext = stashTabsModel;

            // Populate the league dropdown
            LeagueComboBox.ItemsSource = ApiAdapter.GetAllLeagueNames();

            InitializeColors();
            InitializeHotkeys();
            InitializeTray();
            LoadModeVisibility();
            // add Action to MouseHook
            MouseHook.MouseAction += Coordinates.Event;

            //throw new NullReferenceException();
        }

        public static string AppVersionText { get; set; } = "v." + appVersion;

        public static bool SettingsComplete { get; set; }

        private static string RunButtonContent { get; set; } = "Run Overlay";

        public Visibility IndicesVisible
        {
            get => _indicesVisible;
            set
            {
                if (_indicesVisible != value)
                {
                    _indicesVisible = value;
                    OnPropertyChanged("IndicesVisible");
                }
            }
        }

        public Visibility NameVisible
        {
            get => _nameVisible;
            set
            {
                if (_nameVisible != value)
                {
                    _nameVisible = value;
                    OnPropertyChanged("NameVisible");
                }
            }
        }

        public Visibility LootfilterFileDialogVisible => Settings.Default.LootfilterOnline
            ? Visibility.Collapsed
            : Visibility.Visible;

        public Visibility LootfilterOnlineFilterNameVisible => Settings.Default.LootfilterOnline
            ? Visibility.Visible
            : Visibility.Collapsed;

        private void InitializeHotkeys()
        {
            HotkeysManager.SetupSystemHook();
            //HotkeysManager.RequiresModifierKey = false;
            HotkeysManager.GetRefreshHotkey();
            HotkeysManager.GetToggleHotkey();
            HotkeysManager.GetStashTabHotkey();
            //HotkeysManager.GetReloadFilterHotkey();
            AddAllHotkeys();
        }

        private void InitializeColors()
        {
            if (Settings.Default.ColorBoots != "") ColorBootsPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorBoots);
            if (Settings.Default.ColorChest != "") ColorChestPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorChest);
            if (Settings.Default.ColorWeapon != "") ColorWeaponsPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorWeapon);
            if (Settings.Default.ColorGloves != "") ColorGlovesPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorGloves);
            if (Settings.Default.ColorHelmet != "") ColorHelmetPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorHelmet);
            if (Settings.Default.ColorStash != "") ColorStashPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorStash);
            if (Settings.Default.StashTabBackgroundColor != "") ColorStashBackgroundPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.StashTabBackgroundColor);
            if (Settings.Default.ColorRing != "") ColorRingPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorRing);
            if (Settings.Default.ColorAmulet != "") ColorAmuletPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorAmulet);
            if (Settings.Default.ColorBelt != "") ColorBeltPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Default.ColorBelt);
        }


        // creates tray icon with menu
        private void InitializeTray()
        {
            ni.Icon = Properties.Resources.coin;
            ni.Visible = true;
            ni.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            components = new Container();
            contextMenu = new ContextMenu();
            menuItem = new MenuItem();
            menuItemUpdate = new MenuItem();

            // Initialize contextMenu1
            contextMenu.MenuItems.AddRange(new[] { menuItem, menuItemUpdate });


            // Initialize menuItem1
            menuItem.Index = 1;
            menuItem.Text = "E&xit";
            menuItem.Click += MenuItem_Click;

            // Initialize menuItemUpdate
            menuItemUpdate.Index = 0;
            menuItemUpdate.Text = "C&heck for Updates";
            menuItemUpdate.Click += CheckForUpdates_Click;


            ni.ContextMenu = contextMenu;
        }

        private void CheckForUpdates_Click(object Sender, EventArgs e)
        {
            NETAutoupdater.CheckForUpdates();
        }

        // Close the form, which closes the application.
        private void MenuItem_Click(object Sender, EventArgs e)
        {
            trayClose = true;
            Close();
        }


        // Minimize to system tray when application is closed.
        protected override void OnClosing(CancelEventArgs e)
        {
            // if hideOnClose
            // setting cancel to true will cancel the close request
            // so the application is not closed
            if (Settings.Default.hideOnClose && !trayClose)
            {
                e.Cancel = true;
                Hide();
                base.OnClosing(e);
            }

            if (!Settings.Default.hideOnClose || trayClose)
            {
                ni.Visible = false;
                MouseHook.Stop();
                HotkeysManager.ShutdownSystemHook();
                Settings.Default.Save();
                if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive) LogWatcher.StopWatchingLogFile();
                //overlay.Close();
                //stashTabOverlay.Close();
                Application.Current.Shutdown();
            }
        }


        public void RunOverlay()
        {
            //Trace.WriteLine(ForegroundWindows.GetForegroundProcessName(), "focused");

            if (overlay.IsOpen)
            {
                overlay.Hide();
                if (stashTabOverlay.IsOpen) stashTabOverlay.Hide();
                RunButton.Content = "Run Overlay";
            }
            else
            {
                if (CheckAllSettings())
                {
                    overlay.Show();
                    RunButton.Content = "Stop Overlay";
                }
            }
            //bool ready = CheckAllSettings();
            //if (ready)
            //{

            //    if (RunButton.Content.ToString() == "Run Overlay")
            //    {
            //        RunButton.Content = "Stop Overlay";
            //        overlay.Show();
            //    }
            //    else
            //    {
            //        RunButton.Content = "Run Overlay";
            //        overlay.Hide();
            //        if (stashTabOverlay.IsOpen)
            //        {
            //            stashTabOverlay.Hide();
            //        }
            //    }
            //}
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunOverlay();
        }

        public static void RunStashTabOverlay()
        {
            var ready = CheckAllSettings();
            if (ready)
            {
                if (stashTabOverlay.IsOpen)
                    //MouseHook.Stop();
                    stashTabOverlay.Hide();
                else
                    //if (ChaosRecipeEnhancer.FetchingActive == true)
                    //{
                    //    overlay.RunFetching();
                    //}
                    //MouseHook.Start();
                    stashTabOverlay.Show();
            }
        }

        public void AddAllHotkeys()
        {
            if (Settings.Default.HotkeyRefresh != "< not set >") HotkeysManager.AddHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey, overlay.RunFetching);
            if (Settings.Default.HotkeyToggle != "< not set >") HotkeysManager.AddHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey, RunOverlay);
            if (Settings.Default.HotkeyStashTab != "< not set >") HotkeysManager.AddHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey, RunStashTabOverlay);
            //if (Properties.Settings.Default.HotkeyReloadFilter != "< not set >")
            //{
            //    HotkeysManager.AddHotkey(HotkeysManager.reloadFilterModifier, HotkeysManager.reloadFilterKey, ReloadItemFilter);
            //}
        }

        public void RemoveAllHotkeys()
        {
            HotkeysManager.RemoveRefreshHotkey();
            HotkeysManager.RemoveStashTabHotkey();
            HotkeysManager.RemoveToggleHotkey();
            //HotkeysManager.RemoveReloadFilterHotkey();
        }

        private string GetSoundFilePath()
        {
            var open = new OpenFileDialog();
            open.Filter = "MP3|*.mp3";
            var res = open.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK) return open.FileName;

            return null;
        }


        private void ColorBootsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorBoots = ColorBootsPicker.SelectedColor.ToString();
        }

        private void ColorGlovesPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorGloves = ColorGlovesPicker.SelectedColor.ToString();
        }

        private void ColorHelmetPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorHelmet = ColorHelmetPicker.SelectedColor.ToString();
        }

        private void ColorChestPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorChest = ColorChestPicker.SelectedColor.ToString();
        }

        private void ColorWeaponsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorWeapon = ColorWeaponsPicker.SelectedColor.ToString();
        }

        private void ColorStashPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorStash = ColorStashPicker.SelectedColor.ToString();
        }

        private void ColorRingPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorRing = ColorRingPicker.SelectedColor.ToString();
        }

        private void ColorAmuletPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorAmulet = ColorAmuletPicker.SelectedColor.ToString();
        }

        private void ColorBeltPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.ColorBelt = ColorBeltPicker.SelectedColor.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        public static bool CheckAllSettings()
        {
            var accName = Settings.Default.accName;
            var sessId = Settings.Default.SessionId;
            var league = Settings.Default.League;
            var lootfilterLocation = Settings.Default.LootfilterLocation;
            var lootfilterOnline = Settings.Default.LootfilterOnline;
            var lootfilterOnlineName = Settings.Default.LootfilterOnlineName;
            var lootfilterActive = Settings.Default.LootfilterActive;
            var logLocation = Settings.Default.LogLocation;
            var autoFetch = Settings.Default.AutoFetch;

            var missingSettings = new List<string>();
            var errorMessage = "Please add: \n";

            if (accName == "") missingSettings.Add("- Account Name \n");
            if (sessId == "") missingSettings.Add("- PoE Session ID \n");
            if (league == "") missingSettings.Add("- League \n");
            if (lootfilterActive)
            {
                if (!lootfilterOnline && lootfilterLocation == "") missingSettings.Add("- Lootfilter Location \n");

                if (lootfilterOnline && lootfilterOnlineName == "") missingSettings.Add("- Lootfilter Name \n");
            }

            if (autoFetch)
                if (logLocation == "")
                    missingSettings.Add("- Log File Location \n");
            if (Settings.Default.StashtabMode == 0)
            {
                if (Settings.Default.StashTabIndices == "") missingSettings.Add("- StashTab Index");
            }
            else if (Settings.Default.StashtabMode == 1)
            {
                if (Settings.Default.StashTabName == "") missingSettings.Add("- StashTab Name");
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

            foreach (var setting in missingSettings) errorMessage += setting;

            MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void VolumeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Data.PlayNotificationSound();
        }

        private void ColorStashBackgroundPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Default.StashTabBackgroundColor = ColorStashBackgroundPicker.SelectedColor.ToString();
        }

        private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyWindow)
                    isWindowOpen = true;

            if (!isWindowOpen)
            {
                var hotkeyDialog = new HotkeyWindow(this, "toggle");
                hotkeyDialog.Show();
            }
        }

        private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyWindow)
                    isWindowOpen = true;

            if (!isWindowOpen)
            {
                var hotkeyDialog = new HotkeyWindow(this, "refresh");
                hotkeyDialog.Show();
            }
        }

        private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyWindow)
                    isWindowOpen = true;

            if (!isWindowOpen)
            {
                var hotkeyDialog = new HotkeyWindow(this, "stashtab");
                hotkeyDialog.Show();
            }
        }

        private void ReloadFilterHotkey_Click(object sender, RoutedEventArgs e)
        {
            var isWindowOpen = false;
            foreach (Window w in Application.Current.Windows)
                if (w is HotkeyWindow)
                    isWindowOpen = true;

            if (!isWindowOpen)
            {
                var hotkeyDialog = new HotkeyWindow(this, "reloadFilter");
                hotkeyDialog.Show();
            }
        }

        private void LootfilterFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Lootfilter|*.filter";
            var res = open.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                var filename = open.FileName;
                //LootfilterFileDialog.Text = filename;
                Settings.Default.LootfilterLocation = filename;
                //LootfilterFileDialog.Select(LootfilterFileDialog.Text.Length, 0);
                LootfilterFileDialog.Content = filename;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadModeVisibility();
        }

        private void LoadModeVisibility()
        {
            if (Settings.Default.StashtabMode == 0)
            {
                IndicesVisible = Visibility.Visible;
                NameVisible = Visibility.Hidden;
            }
            else
            {
                NameVisible = Visibility.Visible;
                IndicesVisible = Visibility.Hidden;
            }
        }

        private void TabHeaderGapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stashTabOverlay.TabHeaderGap = new Thickness(Settings.Default.TabHeaderGap, 0, Settings.Default.TabHeaderGap, 0);
        }

        private void TabHeaderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StashTabList.StashTabs.Count > 0)
                foreach (var s in StashTabList.StashTabs)
                    s.TabHeaderWidth = new Thickness(Settings.Default.TabHeaderWidth, 2, Settings.Default.TabHeaderWidth, 2);
        }

        private void TabHeaderMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stashTabOverlay.TabMargin = new Thickness(Settings.Default.TabMargin, 0, 0, 0);
        }

        public static void GenerateNewOverlay()
        {
            overlay = new ChaosRecipeEnhancer();
        }

        public static void GenerateNewStashtabOverlay()
        {
            stashTabOverlay = new StashTabWindow();
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void OverlayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Settings.Default.OverlayMode == 0)
                overlay.MainOverlayContentControl.Content = new MainOverlayContent();
            else if (Settings.Default.OverlayMode == 1)
                overlay.MainOverlayContentControl.Content = new MainOverlayContentMinified();
            else if (Settings.Default.OverlayMode == 2) overlay.MainOverlayContentControl.Content = new MainOverlayOnlyButtons();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This will reset all of your settings!", "Reset Settings", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Settings.Default.Reset();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void ChaosRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.RegalRecipe = false;
        }

        private void RegalRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ChaosRecipe = false;
        }

        private void LogLocationDialog_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Text|Client.txt";
            var res = open.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                var filename = open.FileName;
                //LootfilterFileDialog.Text = filename;
                Settings.Default.LogLocation = filename;
                //LootfilterFileDialog.Select(LootfilterFileDialog.Text.Length, 0);
                LogLocationDialog.Content = filename;
            }
        }

        private void AutoFetchCheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void AutoFetchCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive) LogWatcher.WorkerThread.Abort();
        }

        private void ShowNumbersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Settings.Default.ShowItemAmount != 0)
                overlay.AmountsVisibility = Visibility.Visible;
            else
                overlay.AmountsVisibility = Visibility.Hidden;
        }

        private void LootfilterOnlineCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(LootfilterFileDialogVisible));
            OnPropertyChanged(nameof(LootfilterOnlineFilterNameVisible));
        }

        private void Hyperlink_RequestNavigateByAccName(object sender, RequestNavigateEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.accName))
            {
                const string messageBoxText = "You first need enter your account name";
                MessageBox.Show(messageBoxText, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var url = string.Format(e.Uri.ToString(), Settings.Default.accName);
                Process.Start(url);
            }
        }

        private void FilterSoundLocationDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var soundFilePath = GetSoundFilePath();

            if (soundFilePath != null)
            {
                Settings.Default.FilterChangeSoundFileLocation = soundFilePath;
                FilterSoundLocationDialog.Content = soundFilePath;
                Data.Player.Open(new Uri(soundFilePath));

                Data.PlayNotificationSound();
            }
        }

        private void ItemPickupLocationDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var soundFilePath = GetSoundFilePath();

            if (soundFilePath != null)
            {
                Settings.Default.ItemPickupSoundFileLocation = soundFilePath;
                ItemPickupLocationDialog.Content = soundFilePath;
                Data.PlayerSet.Open(new Uri(soundFilePath));

                Data.PlayNotificationSoundSetPicked();
            }
        }
        //private void ReloadItemFilter()
        //{
        //    //hotkeys causing problems? 
        //    RemoveAllHotkeys();
        //    string filterName = GetFilterName();
        //    //System.Diagnostics.Trace.WriteLine(filterName);
        //    SendInputs.SendInsert(filterName);
        //    //HotkeysManager.AddHotkey(HotkeysManager.reloadFilterModifier, HotkeysManager.reloadFilterKey, ReloadItemFilter);
        //    AddAllHotkeys();
        //}

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
    }
}
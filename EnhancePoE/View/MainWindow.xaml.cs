using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Navigation;
using EnhancePoE.Model;
using EnhancePoE.Model.Storage;
using EnhancePoE.View;
using System.IO;
using System.Reflection;

//using EnhancePoE.TabItemViewModel;

namespace EnhancePoE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private static readonly string appVersion = "1.0.11.0";
        public static string AppVersionText { get; set; } = "v." + appVersion;

        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem;
        private System.Windows.Forms.MenuItem menuItemUpdate;
        private System.ComponentModel.IContainer components;

        public static bool SettingsComplete { get; set; }

        public static ChaosRecipeEnhancer overlay = new ChaosRecipeEnhancer();

        public static StashTabWindow stashTabOverlay = new StashTabWindow();

        private static string RunButtonContent { get; set; } = "Run Overlay";

        private Visibility _indicesVisible = Visibility.Hidden;
        public Visibility IndicesVisible
        {
            get { return _indicesVisible; }
            set
            {
                if (_indicesVisible != value)
                {
                    _indicesVisible = value;
                    OnPropertyChanged("IndicesVisible");
                }
            }
        }
        private Visibility _nameVisible = Visibility.Hidden;
        public Visibility NameVisible
        {
            get { return _nameVisible; }
            set
            {
                if (_nameVisible != value)
                {
                    _nameVisible = value;
                    OnPropertyChanged("NameVisible");
                }
            }
        }

        public Visibility LootfilterFileDialogVisible => Properties.Settings.Default.LootfilterOnline
            ? Visibility.Collapsed
            : Visibility.Visible;

        public Visibility LootfilterOnlineFilterNameVisible => Properties.Settings.Default.LootfilterOnline
            ? Visibility.Visible
            : Visibility.Collapsed;


        private bool trayClose = false;

        public static MainWindow instance;

        public MainWindow()
        {
            instance = this;
            //Properties.Settings.Default.Reset();
            InitializeComponent();
            DataContext = this;
            NETAutoupdater.InitializeAutoupdater(appVersion);


            //check for updates
            //AutoUpdater.RunUpdateAsAdmin = true;

            //Data.InitializeBases();
            if (!String.IsNullOrEmpty(Properties.Settings.Default.FilterChangeSoundFileLocation) && !FilterSoundLocationDialog.Content.Equals("Default Sound"))
            {
                Data.Player.Open(new Uri(Properties.Settings.Default.FilterChangeSoundFileLocation));
            }
            else
            {
                //Data.Player.Open(new Uri(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Sounds\filterchanged.mp3")));
                Data.Player.Open(new Uri(System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\filterchanged.mp3")));
            }

            if (!String.IsNullOrEmpty(Properties.Settings.Default.ItemPickupSoundFileLocation) && !ItemPickupLocationDialog.Content.Equals("Default Sound"))
            {
                Data.PlayerSet.Open(new Uri(Properties.Settings.Default.ItemPickupSoundFileLocation));
            }
            else
            {
                //Data.PlayerSet.Open(new Uri(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Sounds\itemsPickedUp.mp3")));
                Data.PlayerSet.Open(new Uri(System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\itemsPickedUp.mp3")));
            }

            //TESTING SETTINGS RESET
            //if (Debugger.IsAttached)
            //    Properties.Settings.Default.Reset();

            // initialize stashtabs
            //DataContext = stashTabsModel;

            InitializeColors();
            InitializeHotkeys();
            InitializeTray();
            LoadModeVisibility();
            // add Action to MouseHook
            MouseHook.MouseAction += new EventHandler(Coordinates.Event);

            //throw new NullReferenceException();
        }

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
            if (Properties.Settings.Default.ColorBoots != "")
            {
                ColorBootsPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorBoots);
            }
            if (Properties.Settings.Default.ColorChest != "")
            {
                ColorChestPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorChest);
            }
            if (Properties.Settings.Default.ColorWeapon != "")
            {
                ColorWeaponsPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorWeapon);
            }
            if (Properties.Settings.Default.ColorGloves != "")
            {
                ColorGlovesPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorGloves);
            }
            if (Properties.Settings.Default.ColorHelmet != "")
            {
                ColorHelmetPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorHelmet);
            }
            if (Properties.Settings.Default.ColorStash != "")
            {
                ColorStashPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash);
            }
            if (Properties.Settings.Default.StashTabBackgroundColor != "")
            {
                ColorStashBackgroundPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.StashTabBackgroundColor);
            }
            if (Properties.Settings.Default.ColorRing != "")
            {
                ColorRingPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorRing);
            }
            if (Properties.Settings.Default.ColorAmulet != "")
            {
                ColorAmuletPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorAmulet);
            }
            if (Properties.Settings.Default.ColorBelt != "")
            {
                ColorBeltPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorBelt);
            }

        }


        // creates tray icon with menu
        private void InitializeTray()
        {
            ni.Icon = Properties.Resources.coin;
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem = new System.Windows.Forms.MenuItem();
            this.menuItemUpdate = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem, this.menuItemUpdate });


            // Initialize menuItem1
            this.menuItem.Index = 1;
            this.menuItem.Text = "E&xit";
            this.menuItem.Click += new System.EventHandler(this.MenuItem_Click);

            // Initialize menuItemUpdate
            this.menuItemUpdate.Index = 0;
            this.menuItemUpdate.Text = "C&eck for Updates";
            this.menuItemUpdate.Click += new System.EventHandler(CheckForUpdates_Click);



            ni.ContextMenu = this.contextMenu;
        }

        private void CheckForUpdates_Click(object Sender, EventArgs e)
        {
            NETAutoupdater.CheckForUpdates();
        }

        // Close the form, which closes the application.
        private void MenuItem_Click(object Sender, EventArgs e)
        {
            this.trayClose = true;
            this.Close();
        }


        // Minimize to system tray when application is closed.
        protected override void OnClosing(CancelEventArgs e)
        {

            // if hideOnClose
            // setting cancel to true will cancel the close request
            // so the application is not closed
            if (Properties.Settings.Default.hideOnClose && !this.trayClose)
            {
                e.Cancel = true;
                this.Hide();
                base.OnClosing(e);
            }

            if (!Properties.Settings.Default.hideOnClose || this.trayClose)
            {
                ni.Visible = false;
                MouseHook.Stop();
                HotkeysManager.ShutdownSystemHook();
                Properties.Settings.Default.Save();
                if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive)
                {
                    LogWatcher.StopWatchingLogFile();
                }
                //overlay.Close();
                //stashTabOverlay.Close();
                App.Current.Shutdown();
            }
        }


        public void RunOverlay()
        {
            //Trace.WriteLine(ForegroundWindows.GetForegroundProcessName(), "focused");

            if (overlay.IsOpen)
            {
                overlay.Hide();
                if (stashTabOverlay.IsOpen)
                {
                    stashTabOverlay.Hide();
                }
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
            bool ready = CheckAllSettings();
            if (ready)
            {
                if (stashTabOverlay.IsOpen)
                {
                    //MouseHook.Stop();
                    stashTabOverlay.Hide();
                }
                else
                {
                    //if (ChaosRecipeEnhancer.FetchingActive == true)
                    //{
                    //    overlay.RunFetching();
                    //}
                    //MouseHook.Start();
                    stashTabOverlay.Show();
                }
            }
        }

        public void AddAllHotkeys()
        {
            if (Properties.Settings.Default.HotkeyRefresh != "< not set >")
            {
                HotkeysManager.AddHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey, overlay.RunFetching);
            }
            if (Properties.Settings.Default.HotkeyToggle != "< not set >")
            {
                HotkeysManager.AddHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey, RunOverlay);
            }
            if (Properties.Settings.Default.HotkeyStashTab != "< not set >")
            {
                HotkeysManager.AddHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey, RunStashTabOverlay);
            }
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
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "MP3|*.mp3";
            DialogResult res = open.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                return open.FileName;
            }

            return null;
        }


        private void ColorBootsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorBoots = ColorBootsPicker.SelectedColor.ToString();
        }

        private void ColorGlovesPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorGloves = ColorGlovesPicker.SelectedColor.ToString();
        }

        private void ColorHelmetPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorHelmet = ColorHelmetPicker.SelectedColor.ToString();
        }

        private void ColorChestPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorChest = ColorChestPicker.SelectedColor.ToString();
        }

        private void ColorWeaponsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorWeapon = ColorWeaponsPicker.SelectedColor.ToString();
        }

        private void ColorStashPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorStash = ColorStashPicker.SelectedColor.ToString();
        }

        private void ColorRingPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorRing = ColorRingPicker.SelectedColor.ToString();
        }

        private void ColorAmuletPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorAmulet = ColorAmuletPicker.SelectedColor.ToString();
        }

        private void ColorBeltPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorBelt = ColorBeltPicker.SelectedColor.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        public static bool CheckAllSettings()
        {
            string accName = Properties.Settings.Default.accName;
            string sessId = Properties.Settings.Default.SessionId;
            string league = Properties.Settings.Default.League;
            string lootfilterLocation = Properties.Settings.Default.LootfilterLocation;
            bool lootfilterOnline = Properties.Settings.Default.LootfilterOnline;
            string lootfilterOnlineName = Properties.Settings.Default.LootfilterOnlineName;
            bool lootfilterActive = Properties.Settings.Default.LootfilterActive;
            string logLocation = Properties.Settings.Default.LogLocation;
            bool autoFetch = Properties.Settings.Default.AutoFetch;

            List<string> missingSettings = new List<string>();
            string errorMessage = "Please add: \n";

            if (accName == "")
            {
                missingSettings.Add("- Account Name \n");
            }
            if (sessId == "")
            {
                missingSettings.Add("- PoE Session ID \n");
            }
            if (league == "")
            {
                missingSettings.Add("- League \n");
            }
            if (lootfilterActive)
            {
                if (!lootfilterOnline && lootfilterLocation == "")
                {
                    missingSettings.Add("- Lootfilter Location \n");
                }

                if (lootfilterOnline && lootfilterOnlineName == "")
                {
                    missingSettings.Add("- Lootfilter Name \n");
                }
            }
            if (autoFetch)
            {
                if (logLocation == "")
                {
                    missingSettings.Add("- Log File Location \n");
                }
            }
            if (Properties.Settings.Default.StashtabMode == 0)
            {
                if (Properties.Settings.Default.StashTabIndices == "")
                {
                    missingSettings.Add("- StashTab Index");
                }
            }
            else if (Properties.Settings.Default.StashtabMode == 1)
            {
                if (Properties.Settings.Default.StashTabName == "")
                {
                    missingSettings.Add("- StashTab Name");
                }
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

            foreach (string setting in missingSettings)
            {
                errorMessage += setting;
            }

            System.Windows.MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void VolumeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Data.PlayNotificationSound();
        }
        private void ColorStashBackgroundPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.StashTabBackgroundColor = ColorStashBackgroundPicker.SelectedColor.ToString();
        }

        private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w is HotkeyWindow)
                {
                    isWindowOpen = true;
                }
            }

            if (!isWindowOpen)
            {
                HotkeyWindow hotkeyDialog = new HotkeyWindow(this, "toggle");
                hotkeyDialog.Show();
            }
        }

        private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w is HotkeyWindow)
                {
                    isWindowOpen = true;
                }
            }

            if (!isWindowOpen)
            {
                HotkeyWindow hotkeyDialog = new HotkeyWindow(this, "refresh");
                hotkeyDialog.Show();
            }
        }

        private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w is HotkeyWindow)
                {
                    isWindowOpen = true;
                }
            }

            if (!isWindowOpen)
            {
                HotkeyWindow hotkeyDialog = new HotkeyWindow(this, "stashtab");
                hotkeyDialog.Show();
            }
        }

        private void ReloadFilterHotkey_Click(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w is HotkeyWindow)
                {
                    isWindowOpen = true;
                }
            }

            if (!isWindowOpen)
            {
                HotkeyWindow hotkeyDialog = new HotkeyWindow(this, "reloadFilter");
                hotkeyDialog.Show();
            }
        }

        private void LootfilterFileDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Lootfilter|*.filter";
            DialogResult res = open.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                string filename = open.FileName;
                //LootfilterFileDialog.Text = filename;
                Properties.Settings.Default.LootfilterLocation = filename;
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
            if (Properties.Settings.Default.StashtabMode == 0)
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
            stashTabOverlay.TabHeaderGap = new Thickness(Properties.Settings.Default.TabHeaderGap, 0, Properties.Settings.Default.TabHeaderGap, 0);
        }

        private void TabHeaderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StashTabList.StashTabs.Count > 0)
            {
                foreach (StashTab s in StashTabList.StashTabs)
                {
                    s.TabHeaderWidth = new Thickness(Properties.Settings.Default.TabHeaderWidth, 2, Properties.Settings.Default.TabHeaderWidth, 2);
                }
            }
        }

        private void TabHeaderMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stashTabOverlay.TabMargin = new Thickness(Properties.Settings.Default.TabMargin, 0, 0, 0);
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
            Properties.Settings.Default.Save();
        }

        private void OverlayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Properties.Settings.Default.OverlayMode == 0)
            {
                overlay.MainOverlayContentControl.Content = new UserControls.MainOverlayContent();
            }
            else if (Properties.Settings.Default.OverlayMode == 1)
            {
                overlay.MainOverlayContentControl.Content = new UserControls.MainOverlayContentMinified();
            }
            else if (Properties.Settings.Default.OverlayMode == 2)
            {
                overlay.MainOverlayContentControl.Content = new UserControls.MainOverlayOnlyButtons();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("This will reset all of your settings!", "Reset Settings", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Properties.Settings.Default.Reset();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void ChaosRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RegalRecipe = false;
        }

        private void RegalRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ChaosRecipe = false;
        }

        private void LogLocationDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Text|Client.txt";
            DialogResult res = open.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                string filename = open.FileName;
                //LootfilterFileDialog.Text = filename;
                Properties.Settings.Default.LogLocation = filename;
                //LootfilterFileDialog.Select(LootfilterFileDialog.Text.Length, 0);
                LogLocationDialog.Content = filename;
            }
        }

        private void AutoFetchCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AutoFetchCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive)
            {
                LogWatcher.WorkerThread.Abort();
            }
        }

        private void ShowNumbersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Properties.Settings.Default.ShowItemAmount != 0)
            {
                overlay.AmountsVisibility = Visibility.Visible;
            }
            else
            {
                overlay.AmountsVisibility = Visibility.Hidden;
            }
        }

        private void LootfilterOnlineCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(LootfilterFileDialogVisible));
            OnPropertyChanged(nameof(LootfilterOnlineFilterNameVisible));
        }

        private void Hyperlink_RequestNavigateByAccName(object sender, RequestNavigateEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.accName))
            {
                const string messageBoxText = "You first need enter your account name";
                System.Windows.MessageBox.Show(messageBoxText, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string url = string.Format(e.Uri.ToString(), Properties.Settings.Default.accName);
                System.Diagnostics.Process.Start(url);
            }
        }

        private void FilterSoundLocationDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var soundFilePath = GetSoundFilePath();

            if (soundFilePath != null)
            {
                Properties.Settings.Default.FilterChangeSoundFileLocation = soundFilePath;
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
                Properties.Settings.Default.ItemPickupSoundFileLocation = soundFilePath;
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}

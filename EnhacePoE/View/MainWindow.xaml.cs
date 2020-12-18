using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using EnhancePoE.Model;
using EnhancePoE.View;

//using EnhancePoE.TabItemViewModel;

namespace EnhancePoE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem;
        private System.ComponentModel.IContainer components;

        public static bool SettingsComplete { get; set; }

        public static ChaosRecipeEnhancer overlay = new ChaosRecipeEnhancer();

        public static StashTabWindow stashTabOverlay = new StashTabWindow();


        private Visibility _indicesVisible = Visibility.Hidden;
        public Visibility IndicesVisible
        {
            get { return _indicesVisible; }
            set { if(_indicesVisible != value)
                {
                    _indicesVisible = value;
                    OnPropertyChanged("IndicesVisible");
                } }
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

        //public static TabItemViewModel stashTabsModel = new TabItemViewModel();


        private bool trayClose = false;



        public MainWindow()
        {
            //Properties.Settings.Default.Reset();
            InitializeComponent();
            DataContext = this;

            //Data.InitializeBases();
            Data.Player.Open(new Uri(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Sounds\filterchanged.mp3")));
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
        }

        private void InitializeHotkeys()
        {
            HotkeysManager.SetupSystemHook();
            HotkeysManager.RequiresModifierKey = false;
            HotkeysManager.GetRefreshHotkey();
            HotkeysManager.GetToggleHotkey();
            HotkeysManager.GetStashTabHotkey();
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
            ni.Icon = Properties.Resources.gold_removebg_preview;
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

            // Initialize contextMenu1
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem });

            // Initialize menuItem1
            this.menuItem.Index = 0;
            this.menuItem.Text = "E&xit";
            this.menuItem.Click += new System.EventHandler(this.MenuItem_Click);

            ni.ContextMenu = this.contextMenu;
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

            if(!Properties.Settings.Default.hideOnClose || this.trayClose)
            {
                ni.Visible = false;
                MouseHook.Stop();
                HotkeysManager.ShutdownSystemHook();
                Properties.Settings.Default.Save();
                //overlay.Close();
                //stashTabOverlay.Close();
                App.Current.Shutdown();
            }
        }

        // save all settings, recreate hotkeys
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(RefreshRate.Text, out int refresh))
            {
                if(refresh < 15)
                {
                    System.Windows.MessageBox.Show("Refreshrate has to be greater than 15!");
                }
                else
                {
                    Properties.Settings.Default.RefreshRate = refresh;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Refreshrate has to be a number!");
            }
            if (Int32.TryParse(Sets.Text, out int sets))
            {
                if (sets < 0)
                {
                    System.Windows.MessageBox.Show("Refreshrate has to be greater than 0!");
                }
                else
                {
                    Properties.Settings.Default.Sets = sets;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Number of Sets has to be a number!");
            }

            RemoveAllHotkeys();
            AddAllHotkeys();

            Trace.WriteLine(Properties.Settings.Default.YStashTabOverlay, "y");
            Trace.WriteLine(Properties.Settings.Default.XStashTabOverlay, "x");
            Trace.WriteLine(Properties.Settings.Default.LeftStashTabOverlay, "left");
            Trace.WriteLine(Properties.Settings.Default.TopStashTabOverlay, "top");

            Properties.Settings.Default.accName = accountName.Text.ToString();
            //Properties.Settings.Default.StashTabsString = SettingsSerializer.SerializeStashTab(stashTabsModel);
            //Properties.Settings.Default.StashTabs = stashTabsModel;
            Properties.Settings.Default.Save();
            System.Windows.MessageBox.Show("Settings saved!");
        }

        public void RunOverlay()
        {
            bool ready = CheckAllSettings();
            if (ready)
            {
                if (RunButton.Content.ToString() == "Run Overlay")
                {
                    RunButton.Content = "Stop Overlay";
                    overlay.Show();
                }
                else
                {
                    RunButton.Content = "Run Overlay";
                    overlay.Hide();
                    if (stashTabOverlay.IsOpen)
                    {
                        stashTabOverlay.Hide();
                    }
                }
            }
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
                    if (ChaosRecipeEnhancer.FetchingActive == true)
                    {
                        overlay.RunFetching();
                    }
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
        }

        public void RemoveAllHotkeys()
        {
            HotkeysManager.RemoveRefreshHotkey();
            HotkeysManager.RemoveStashTabHotkey();
            HotkeysManager.RemoveToggleHotkey();
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






        //private void LootfilterFileDialog_MouseDown(object sender, MouseButtonEventArgs e)
        //{

        //}

        public static bool CheckAllSettings()
        {
            string accName = Properties.Settings.Default.accName;
            string sessId = Properties.Settings.Default.SessionId;
            string league = Properties.Settings.Default.League;
            int refreshRate = Properties.Settings.Default.RefreshRate;
            string lootfilterLocation = Properties.Settings.Default.LootfilterLocation;
            bool lootfilterActive = Properties.Settings.Default.LootfilterActive;

            List<string> missingSettings = new List<string>();
            string errorMessage = "Please add: \n";

            if(accName == "")
            {
                missingSettings.Add("- Account Name \n");
            }
            if(sessId == "")
            {
                missingSettings.Add("- PoE Session ID \n");
            }
            if(league == "")
            {
                missingSettings.Add("- League \n");
            }
            if(refreshRate < 15)
            {
                missingSettings.Add("- Refresh Rate \n");
            }
            if (lootfilterActive)
            {
                if(lootfilterLocation == "")
                {
                    missingSettings.Add("- Lootfilter Location \n");
                }
            }
            if(Properties.Settings.Default.StashtabMode == 0)
            {
                if(Properties.Settings.Default.StashTabIndices == "")
                {
                    missingSettings.Add("- StashTab Index");
                }
            }
            else if(Properties.Settings.Default.StashtabMode == 1)
            {
                if (Properties.Settings.Default.StashTabName == "")
                {
                    missingSettings.Add("- StashTab Name");
                }
            }

            if(missingSettings.Count > 0)
            {
                SettingsComplete = false;
            }
            else
            {
                SettingsComplete = true;
                return true; 
            }

            foreach(string setting in missingSettings)
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
                HotkeyWindow hotkeyDialog = new HotkeyWindow("toggle");
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
                HotkeyWindow hotkeyDialog = new HotkeyWindow("refresh");
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
                HotkeyWindow hotkeyDialog = new HotkeyWindow("stashtab");
                hotkeyDialog.Show();
            }
        }

        private void LootfilterFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
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


        private void TabHeaderGapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stashTabOverlay.TabHeaderGap = new Thickness(Properties.Settings.Default.TabHeaderGap, 0, Properties.Settings.Default.TabHeaderGap, 0);
        }


        // TODO: make tabheaderwidth single instance for only changing once
        private void TabHeaderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(StashTabList.StashTabs != null)
            {
                foreach(StashTab s in StashTabList.StashTabs)
                {
                    s.TabHeaderWidth = new Thickness(Properties.Settings.Default.TabHeaderWidth, 2, Properties.Settings.Default.TabHeaderWidth, 2);
                }
            }
        }

        private void TabHeaderMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stashTabOverlay.TabMargin = new Thickness(Properties.Settings.Default.TabMargin, 0, 0, 0);
        }
    }
}

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
    public partial class MainWindow : Window
    {

        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem;
        private System.ComponentModel.IContainer components;

        public static bool SettingsComplete { get; set; }

        public static ChaosRecipeEnhancer overlay = new ChaosRecipeEnhancer();

        public static StashTabWindow stashTabOverlay = new StashTabWindow();


        public static TabItemViewModel stashTabsModel = new TabItemViewModel();



        private bool trayClose = false;



        public MainWindow()
        {
            InitializeComponent();

            //TESTING SETTINGS RESET
            //if (Debugger.IsAttached)
            //    Properties.Settings.Default.Reset();

            // initialize stashtabs
            DataContext = stashTabsModel;

            InitializeColors();
            InitializeHotkeys();
            InitializeTray();

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
            if (Properties.Settings.Default.ColorJewellery != "")
            {
                ColorJewelleryPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorJewellery);
            }
            if (Properties.Settings.Default.ColorStash != "")
            {
                ColorStashPicker.SelectedColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash);
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


        // 
        //protected override void OnStateChanged(EventArgs e)
        //{
        //    Trace.WriteLine("minimize");
        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        Trace.WriteLine("Windowstate minimized");
        //        this.Hide();
        //        if()
        //    }
        //    base.OnStateChanged(e);
        //}

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

            Properties.Settings.Default.accName = accountName.Text.ToString();
            Properties.Settings.Default.StashTabsString = SettingsSerializer.SerializeStashTab(stashTabsModel);
            Properties.Settings.Default.StashTabs = stashTabsModel;
            Properties.Settings.Default.Save();
            System.Windows.MessageBox.Show("Settings saved!");
        }

        public void RunOverlay()
        {
            bool ready = CheckAllSettings();
            if (ready)
            {
                if (RunButton.Content.ToString() == "Run")
                {
                    RunButton.Content = "Stop";
                    overlay.Show();
                }
                else
                {
                    RunButton.Content = "Run";
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




        private void CustomHotkeyToggle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void RefreshHotkey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void StashTabHotkey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void ColorJewelleryPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorJewellery = ColorJewelleryPicker.SelectedColor.ToString();
        }

        private void ColorStashPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Properties.Settings.Default.ColorStash = ColorStashPicker.SelectedColor.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }






        private void LootfilterFileDialog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Lootfilter|*.filter";
            DialogResult res = open.ShowDialog();
            if(res == System.Windows.Forms.DialogResult.OK)
            {
                string filename = open.FileName;
                //LootfilterFileDialog.Text = filename;
                Properties.Settings.Default.LootfilterLocation = filename;
                LootfilterFileDialog.Select(LootfilterFileDialog.Text.Length, 0);

            }
        }

        public static bool CheckAllSettings()
        {
            string accName = Properties.Settings.Default.accName;
            string sessId = Properties.Settings.Default.SessionId;
            string league = Properties.Settings.Default.League;
            int refreshRate = Properties.Settings.Default.RefreshRate;
            string lootfilterLocation = Properties.Settings.Default.LootfilterLocation;
            bool lootfilterActive = Properties.Settings.Default.LootfilterActive;
            int numberOfTabs = stashTabsModel.StashTabs.Count;

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
            if(numberOfTabs <= 0) 
            {
                missingSettings.Add("- At least 1 Tab \n");
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
    }

}

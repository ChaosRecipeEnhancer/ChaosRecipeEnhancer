using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Timers;
using System.ComponentModel;
using EnhancePoE.Model;
using System.Collections.Specialized;
using System.Threading;

namespace EnhancePoE
{
    /// <summary>
    /// Interaction logic for ChaosRecipeEnhancer.xaml
    /// </summary>
    public partial class ChaosRecipeEnhancer : Window
    {

        public static Data currentData { get; set; } = new Data();

        public static bool FetchingActive { get; set;} = false;
        private static System.Timers.Timer aTimer;

        private static readonly double deactivatedOpacity = .1;
        private static readonly double activatedOpacity = 1;

        public static int FullSets { get; set; } = 0;

        private static bool isOpen = false;

        public ChaosRecipeEnhancer()
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            this.FullSetsTextBlock.Text = "0";
        }


        private async void FetchData()
        {
            GetFrequency();
            ApiAdapter.GenerateUri();
            await this.Dispatcher.Invoke(async() =>
            {
                await ApiAdapter.GetItems();
            });
            currentData.CheckActives();
            SetOpacity();

        }


        public void RunFetching()
        {
            if (MainWindow.SettingsComplete)
            {
                if (!isOpen)
                {
                    return;
                }
                if (aTimer.Enabled)
                {
                    aTimer.Enabled = false;
                    FetchingActive = false;
                    RefreshButton.Content = "Fetch";
                }
                else
                {
                    FetchData();
                    //aTimer.Interval = 1000;
                    aTimer.Enabled = true;
                    FetchingActive = true;
                    RefreshButton.Content = "Stop";
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RunFetching();
        }

        private static void GetFrequency()
        {
            int addedTime = 0;
            if(MainWindow.stashTabsModel.StashTabs.Count > 0)
            {
                addedTime += MainWindow.stashTabsModel.StashTabs.Count * 1000;
            }
            aTimer.Interval = (Properties.Settings.Default.RefreshRate * 1000) + addedTime;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            FetchData();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void SetOpacity()
        {

            Dispatcher.Invoke(() =>
            {
                if (!currentData.HelmetActive)
                {
                    Helmet.Opacity = deactivatedOpacity;
                }
                else
                {
                    Helmet.Opacity = activatedOpacity;
                }
                if (!currentData.GlovesActive)
                {
                    Gloves.Opacity = deactivatedOpacity;
                }
                else
                {
                    Gloves.Opacity = activatedOpacity;
                }
                if (!currentData.BootsActive)
                {
                    Boots.Opacity = deactivatedOpacity;
                }
                else
                {
                    Boots.Opacity = activatedOpacity;
                }
                if (!currentData.WeaponActive)
                {
                    Weapon.Opacity = deactivatedOpacity;
                }
                else
                {
                    Weapon.Opacity = activatedOpacity;
                }
                if (!currentData.ChestActive)
                {
                    Chest.Opacity = deactivatedOpacity;
                }
                else
                {
                    Chest.Opacity = activatedOpacity;
                }
            });
        }

        public new virtual void Hide()
        {
            isOpen = false;

            aTimer.Enabled = false;
            base.Hide();
        }

        public new virtual void Show()
        {


            isOpen = true;
            if (FetchingActive)
            {
                aTimer.Enabled = true;
                FetchData();
            }

            base.Show();

        }

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunStashTabOverlay();
        }

        private void HandleEditButton()
        {
            if (MainWindow.stashTabOverlay.IsEditing)
            {
                MainWindow.stashTabOverlay.IsEditing = false;
                MainWindow.stashTabOverlay.Transparentize();
                EditStashTabOverlay.Content = "Edit";
                MouseHook.Start();
            }
            else
            {
                MouseHook.Stop();
                MainWindow.stashTabOverlay.IsEditing = true;
                MainWindow.stashTabOverlay.Normalize();
                EditStashTabOverlay.Content = "Save";
            }
        }

        private void EditStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.stashTabOverlay.IsOpen)
            {
                HandleEditButton();
            }
            else
            {
                MainWindow.RunStashTabOverlay();
                HandleEditButton();
            }
        }
    }
}

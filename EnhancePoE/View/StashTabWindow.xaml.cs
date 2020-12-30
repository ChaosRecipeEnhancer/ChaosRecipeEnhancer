using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using EnhancePoE.Model;
using Microsoft.Win32;

namespace EnhancePoE.View
{
    /// <summary>
    /// Interaction logic for StashTabWindow.xaml
    /// </summary>
    /// 

    public partial class StashTabWindow : Window, INotifyPropertyChanged
    {
        public bool IsOpen { get; set; } = false;
        public bool IsEditing { get; set; } = false;

        private Thickness _tabHeaderGap;
        public Thickness TabHeaderGap
        {
            get { return _tabHeaderGap; }
            set 
            { 
                if (value != _tabHeaderGap) 
                {
                    _tabHeaderGap = value;
                    OnPropertyChanged("TabHeaderGap");
                } 
            }
        }

        private Thickness _tabMargin;
        public Thickness TabMargin
        {
            get { return _tabMargin; }
            set
            {
                if (value != _tabMargin)
                {
                    _tabMargin = value;
                    OnPropertyChanged("TabMargin");
                }
            }
        }

        private Visibility _stashBorderVisibility = Visibility.Hidden;
        public Visibility StashBorderVisibility
        {
            get { return _stashBorderVisibility; }
            set
            {
                _stashBorderVisibility = value;
                OnPropertyChanged("StashBorderVisibility");
            }
        }


        //public double Gap { get; set; } = 0;

        public static ObservableCollection<TabItem> OverlayStashTabList = new ObservableCollection<TabItem>();
        public StashTabWindow()
        {
            InitializeComponent();
            DataContext = this;
            StashTabOverlayTabControl.ItemsSource = OverlayStashTabList;

        }


        public new virtual void Hide()
        {
            Transparentize();
            //MainWindow.overlay.EditStashTabOverlay.Content = "Edit";
            EditModeButton.Content = "Edit";
            IsEditing = false;
            MouseHook.Stop();

            foreach (StashTab i in StashTabList.StashTabs)
            {
                i.OverlayCellsList.Clear();
                i.TabHeader = null;
            }

            IsOpen = false;
            IsEditing = false;
            MainWindow.overlay.OpenStashOverlayButtonContent = "Stash";
            //MainWindow.overlay.EditStashTabOverlay.Content = "Edit";
            //TextBlockList.Clear();

            if (ChaosRecipeEnhancer.FetchingActive)
            {
                ChaosRecipeEnhancer.aTimer.Enabled = true;
            }


            base.Hide();
        }

        // TODO: rework tabitems, tabheaders
        public new virtual void Show()
        {
            if (ChaosRecipeEnhancer.FetchingActive)
            {
                ChaosRecipeEnhancer.aTimer.Enabled = false;
            }
            if (StashTabList.StashTabs.Count != 0)
            {
                IsOpen = true;
                OverlayStashTabList.Clear();
                _tabHeaderGap.Right = Properties.Settings.Default.TabHeaderGap;
                _tabHeaderGap.Left = Properties.Settings.Default.TabHeaderGap;
                TabMargin = new Thickness(Properties.Settings.Default.TabMargin, 0, 0, 0);
                //TabHeaderWidth = new Thickness(Properties.Settings.Default.TabHeaderWidth, 2, Properties.Settings.Default.TabHeaderWidth, 2);

                foreach (StashTab i in StashTabList.StashTabs)
                {
                    //i.PrepareOverlayList();
                    //i.ActivateNextCell(true);
                    TabItem newStashTabItem;
                    TextBlock tbk = new TextBlock() { Text = i.TabName };

                    //TextBlock tbk = new TextBlock() { Text = i.TabName};
                    //if (i.ItemOrderList.Count > 0)
                    //{
                    //    if (Properties.Settings.Default.ColorStash != "")
                    //    {
                    //        tbk.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                    //    }
                    //    else
                    //    {
                    //        tbk.Background = Brushes.Red;
                    //    }
                    //}

                    //tbk.Background = i.TabHeaderColor;
                    tbk.DataContext = i;
                    tbk.SetBinding(TextBlock.BackgroundProperty, new System.Windows.Data.Binding("TabHeaderColor"));
                    tbk.SetBinding(TextBlock.PaddingProperty, new System.Windows.Data.Binding("TabHeaderWidth"));

                    //tbk.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("TabName"));

                    //tbk.SetBinding(TextBlock.PaddingProperty, new System.Windows.Data.Binding("TabHeaderThickness"));
                    tbk.FontSize = 16;
                    //if(i..Co > 0)
                    //{
                    //    if (Properties.Settings.Default.ColorStash != "")
                    //    {
                    //        i.TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                    //    }
                    //    else
                    //    {
                    //        i.TabHeaderColor = Brushes.Red;
                    //    }
                    //}

                    i.TabHeader = tbk;

                    //TextBlockList.Add(tbk, i.TabIndex);

                    //string name = i.TabName + "GridControl";
                    if (i.Quad)
                    {
                        newStashTabItem = new TabItem
                        {
                            Header = tbk,
                            Content = new UserControls.DynamicGridControlQuad
                            {
                                ItemsSource = i.OverlayCellsList,

                            }
                        };
                    }
                    else
                    {
                        newStashTabItem = new TabItem
                        {
                            Header = tbk,
                            Content = new UserControls.DynamicGridControl
                            {
                                ItemsSource = i.OverlayCellsList
                            }
                        };
                    }

                    OverlayStashTabList.Add(newStashTabItem);
                }

                StashTabOverlayTabControl.SelectedIndex = 0;

                Data.PrepareSelling();
                Data.ActivateNextCell(true, null);
                if(Properties.Settings.Default.HighlightMode == 2)
                {
                    foreach(ItemSet set in Data.ItemSetListHighlight)
                    {
                        foreach (Item i in set.ItemList)
                        {
                            StashTab currTab = Data.GetStashTabFromItem(i);
                            currTab.ActivateItemCells(i);
                        }
                    }
                }

                MainWindow.overlay.OpenStashOverlayButtonContent = "Hide";

                MouseHook.Start();
                base.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("No StashTabs Available! Fetch before opening Overlay.", "Stashtab Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartEditMode()
        {
            MouseHook.Stop();
            //MainWindow.overlay.EditStashTabOverlay.Content = "Save";
            EditModeButton.Content = "Save";
            StashBorderVisibility = Visibility.Visible;
            Normalize();
            IsEditing = true;
        }

        public void StopEditMode()
        {
            Transparentize();
            //MainWindow.overlay.EditStashTabOverlay.Content = "Edit";
            EditModeButton.Content = "Edit";
            StashBorderVisibility = Visibility.Hidden;
            MouseHook.Start();
            IsEditing = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeTransparent(hwnd);
        }

        public void Transparentize()
        {
            Trace.WriteLine("make transparent");
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeTransparent(hwnd);
        }

        public void Normalize()
        {
            Trace.WriteLine("make normal");
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeNormal(hwnd);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //if (IsOpen)
            //{
            //    ((MainWindow)System.Windows.Application.Current.MainWindow).Close();

            //}
        }

        public void HandleEditButton()
        {
            if (MainWindow.stashTabOverlay.IsEditing)
            {
                StopEditMode();
            }
            else
            {
                StartEditMode();
            }
        }

        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            HandleEditButton();
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

        //private void hook_MouseUp(object sender, MouseHookEventArgs e)
        //{
        //    Trace.WriteLine("Mouse clicked");
        //    // do some stuff with your exciting new mouse hook data
        //}

        //private void Get

    }
}

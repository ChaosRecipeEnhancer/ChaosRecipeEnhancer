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
    // TODO: fix bug where transparentize not applying after editing
    // TODO: update tab header gap and tab header width dynamically

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
            MouseHook.Stop();

            foreach (StashTab i in StashTabList.StashTabs)
            {
                i.OverlayCellsList.Clear();
                i.TabHeader = null;
            }

            IsOpen = false;
            IsEditing = false;
            MainWindow.overlay.OpenStashTabOverlay.Content = "Stash";
            MainWindow.overlay.EditStashTabOverlay.Content = "Edit";
            //TextBlockList.Clear();
            base.Hide();
        }

        // TODO: rework tabitems, tabheaders
        public new virtual void Show()
        {

            if(StashTabList.StashTabs != null && StashTabList.StashTabs.Count != 0)
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

                    //TabItem newStashTabItem = new TabItem;
                    //newStashTabItem.Header = i.TabName;
                    ////newStashTabItem.DataContext = i.ItemList;
                    //newStashTabItem.Content = i.TabNumber;
                    OverlayStashTabList.Add(newStashTabItem);

                    //Trace.WriteLine("works");


                    //OverlayStashTabList.Add(i);
                }

                StashTabOverlayTabControl.SelectedIndex = 0;

                Data.PrepareSelling();
                Data.ActivateNextCell(true);

                MainWindow.overlay.OpenStashTabOverlay.Content = "Hide";

                MouseHook.Start();
                base.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("No StashTabs Available!", "Stashtab Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeTransparent(hwnd);
        }

        public void Normalize()
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeNormal(hwnd);
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

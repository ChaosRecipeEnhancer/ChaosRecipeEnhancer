using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class StashTabWindow : Window
    {

        public bool IsOpen { get; set; } = false;
        public bool IsEditing { get; set; } = false;
        //public Dictionary<TextBlock, int> TextBlockList { get; set; } = new Dictionary<TextBlock, int>();



        public static ObservableCollection<TabItem> OverlayStashTabList = new ObservableCollection<TabItem>();
        public StashTabWindow()
        {
            InitializeComponent();
            StashTabOverlayTabControl.ItemsSource = OverlayStashTabList;
        }

        public new virtual void Hide()
        {
            MouseHook.Stop();

            foreach (StashTab i in MainWindow.stashTabsModel.StashTabs)
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

        public new virtual void Show()
        {

            IsOpen = true;
            OverlayStashTabList.Clear();

            foreach (StashTab i in MainWindow.stashTabsModel.StashTabs)
            {
                //i.PrepareOverlayList();
                //i.ActivateNextCell(true);
                TabItem newStashTabItem;
                TextBlock tbk = new TextBlock() { Text = i.TabName, Padding = new Thickness(22, 2, 22, 2) };
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

            ChaosRecipeEnhancer.currentData.PrepareSelling();
            ChaosRecipeEnhancer.currentData.ActivateNextCell(true);

            MainWindow.overlay.OpenStashTabOverlay.Content = "Hide";

            MouseHook.Start();
            base.Show();

            //Test();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        //public void Test()
        //{

        //    //StashTabOverlayTabControl.SelectedContent
        //    //Dispatcher.BeginInvoke((Action)(() => StashTabOverlayTabControl.SelectedIndex = 0));
        //    var ctrl = StashTabOverlayTabControl.SelectedContent as UserControls.DynamicGridControl;

        //    //Trace.WriteLine(ctrl.Items[5]);
        //    //ctrl.GetButtonFromCell(ctrl.Items[5]);



        //    //StashTabOverlayTabControl.SelectedIndex = 0;



        //    int index = StashTabOverlayTabControl.SelectedIndex;

        //    Trace.WriteLine(index, "selected index");


        //    foreach (TabItem i in OverlayStashTabList)
        //    {



        //        //Trace.WriteLine(i.Content, "test");
        //        //Trace.WriteLine(i.Content.Items, " test");

        //        //UserControls.DynamicGridControl test = (UserControls.DynamicGridControl)i.Content;

        //        //if(test != null)
        //        //{
        //        //    Trace.WriteLine("not null");

        //        //    test.Test();
        //        //}
        //        //else
        //        //{
        //        //    Trace.WriteLine("null");
        //        //}


        //        //var ctrl = i.SelectedContent as UserControls.DynamicGridControl;
        //        //test.Test();

        //        //Trace.WriteLine(OverlayStashTabList.Count(), "count overlay stashtbalist");



        //        //    ContentPresenter cp = test.ItemContainerGenerator.ContainerFromItem(c) as ContentPresenter;
        //        //    Button tb = Utility.FindVisualChild<Button>(cp);


        //        //foreach (Cell c in test.Items)
        //        //{
        //        //    //Trace.WriteLine(c.Active);
        //        //    ContentPresenter cp = test.ItemContainerGenerator.ContainerFromItem(test) as ContentPresenter;
        //        //    Button tb = Utility.FindVisualChild<Button>(cp);

        //        //    Trace.WriteLine(tb.Content);
        //        //}



        //        //Trace.WriteLine(tb.Count());


        //        //int count = VisualTreeHelper.GetChildrenCount(test);

        //        //Trace.WriteLine(count, "children count");

        //        //var test = Utility.FindVisualChild<ItemsControl>(i);


        //        //Trace.WriteLine(test.Count(), "test");
        //        //Trace.WriteLine(firstStackPanelInTabControl);
        //        //foreach (Button btn in i.Content)
        //        //{

        //        //}
        //    }
        //}

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








        //private void hook_MouseUp(object sender, MouseHookEventArgs e)
        //{
        //    Trace.WriteLine("Mouse clicked");
        //    // do some stuff with your exciting new mouse hook data
        //}

        //private void Get

    }
}

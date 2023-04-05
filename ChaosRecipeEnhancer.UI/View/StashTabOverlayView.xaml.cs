using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Extensions.Native;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;
using Serilog;

namespace ChaosRecipeEnhancer.UI.View
{
    /// <summary>
    ///     Interaction logic for StashTabOverlayView.xaml
    /// </summary>
    public partial class StashTabOverlayView : INotifyPropertyChanged
    {
        #region Constructors

        public StashTabOverlayView(SetTrackerOverlayView setTrackerOverlayView)
        {
            _logger = Log.ForContext<StashTabOverlayView>();
            _logger.Debug("Constructing StashTabOverlayView");

            _setTrackerOverlayView = setTrackerOverlayView;

            InitializeComponent();
            DataContext = this;
            StashTabOverlayTabControl.ItemsSource = OverlayStashTabList;

            _logger.Debug("StashTabOverlayView constructed successfully");
        }

        #endregion

        #region Fields

        private readonly ILogger _logger;
        private readonly SetTrackerOverlayView _setTrackerOverlayView;

        private static readonly ObservableCollection<TabItem> OverlayStashTabList = new ObservableCollection<TabItem>();
        private Visibility _stashBorderVisibility = Visibility.Hidden;
        private Thickness _stashTabOverlayIndividualTabHeaderGap;
        private Thickness _stashTabOverlayIndividualTabMargin;

        #endregion

        #region Properties

        public bool IsOpen { get; set; }
        private bool IsEditing { get; set; }

        public Thickness StashTabOverlayIndividualTabHeaderGap
        {
            get => _stashTabOverlayIndividualTabHeaderGap;
            set
            {
                if (value != _stashTabOverlayIndividualTabHeaderGap)
                {
                    _stashTabOverlayIndividualTabHeaderGap = value;
                    OnPropertyChanged("StashTabOverlayIndividualTabHeaderGap");
                }
            }
        }

        public Thickness StashTabOverlayIndividualTabMargin
        {
            get => _stashTabOverlayIndividualTabMargin;
            set
            {
                if (value != _stashTabOverlayIndividualTabMargin)
                {
                    _stashTabOverlayIndividualTabMargin = value;
                    OnPropertyChanged("StashTabOverlayIndividualTabMargin");
                }
            }
        }

        public Visibility StashBorderVisibility
        {
            get => _stashBorderVisibility;
            set
            {
                _stashBorderVisibility = value;
                OnPropertyChanged("StashBorderVisibility");
            }
        }

        #endregion

        #region Event Handlers

        public new virtual void Hide()
        {
            Transparentize();
            EditModeButton.Content = "Edit";
            IsEditing = false;
            NativeMouseExtensions.Stop();

            foreach (var i in StashTabList.StashTabs)
            {
                i.OverlayCellsList.Clear();
                i.TabNameContainer = null;
            }

            IsOpen = false;
            IsEditing = false;

            _setTrackerOverlayView.OpenStashOverlayButtonContent = "Stash";

            base.Hide();
        }

        // TODO: rework tab items and tab headers
        public new virtual void Show()
        {
            // Ensure the user has fetched stash data before populating our Stash Tab Overlay
            if (StashTabList.StashTabs.Count != 0)
            {
                IsOpen = true;

                OverlayStashTabList.Clear();

                // Read user settings that affect how some of the UI components are rendered
                _stashTabOverlayIndividualTabHeaderGap.Right = Settings.Default.StashTabOverlayIndividualTabHeaderGap;
                _stashTabOverlayIndividualTabHeaderGap.Left = Settings.Default.StashTabOverlayIndividualTabHeaderGap;
                StashTabOverlayIndividualTabMargin = new Thickness(Settings.Default.StashTabOverlayIndividualTabMargin, 0, 0, 0);

                // For each individual stash tab in our query results
                foreach (var stashTabData in StashTabList.StashTabs)
                {
                    // Creating an object that represents a Stash Tab (the physical tab that you interact with)
                    TabItem newStashTab;
                    
                    // Creating a text block that will contain the name of said Stash Tab
                    var textBlock = new TextBlock
                    {
                        Text = stashTabData.TabName,
                        DataContext = stashTabData
                    };

                    textBlock.SetBinding(TextBlock.BackgroundProperty, new Binding("TabHeaderColor"));
                    textBlock.SetBinding(TextBlock.PaddingProperty, new Binding("TabHeaderWidth"));
                    textBlock.FontSize = 16;
                    
                    stashTabData.TabNameContainer = textBlock;

                    if (stashTabData.Quad)
                    {
                        newStashTab = new TabItem
                        {
                            Header = textBlock,
                            Content = new QuadStashGrid
                            {
                                ItemsSource = stashTabData.OverlayCellsList
                            }
                        };
                    }
                    else
                    {
                        newStashTab = new TabItem
                        {
                            Header = textBlock,
                            Content = new NormalStashGrid
                            {
                                ItemsSource = stashTabData.OverlayCellsList
                            }
                        };
                    }

                    OverlayStashTabList.Add(newStashTab);
                }

                StashTabOverlayTabControl.SelectedIndex = 0;

                Data.PrepareSelling();
                
                Data.ActivateNextCell(true, null, StashTabOverlayTabControl);
                
                // If "All Items" highlight mode enabled, paint all Stash Tab Headers to their respective colors
                if (Settings.Default.StashTabOverlayHighlightMode == 2)
                {
                    foreach (var set in Data.ItemSetListHighlight)
                    {
                        foreach (var i in set.ItemList)
                        {
                            var currTab = Data.GetStashTabFromItem(i);
                            currTab.ActivateItemCells(i);
                            
                            currTab.TabHeaderColor =  new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                        }
                    }
                }

                _setTrackerOverlayView.OpenStashOverlayButtonContent = "Hide";

                NativeMouseExtensions.Start();
                base.Show();
            }
            else
            {
                MessageBox.Show("No StashTabs Available! Fetch before opening overlay.", "Error: Stash Tab Overlay",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            var hWnd = new WindowInteropHelper(this).Handle;

            NativeWindowExtensions.MakeTransparent(hWnd);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            HandleEditButton(this);
        }

        #endregion

        #region Methods

        public void StartEditMode()
        {
            NativeMouseExtensions.Stop();
            EditModeButton.Content = "Save";
            StashBorderVisibility = Visibility.Visible;
            Normalize();
            IsEditing = true;
        }

        public void StopEditMode()
        {
            Transparentize();
            EditModeButton.Content = "Edit";
            StashBorderVisibility = Visibility.Hidden;
            NativeMouseExtensions.Start();
            IsEditing = false;
        }

        public void Transparentize()
        {
            Trace.WriteLine("make transparent");
            var hwnd = new WindowInteropHelper(this).Handle;

            NativeWindowExtensions.MakeTransparent(hwnd);
        }

        public void Normalize()
        {
            Trace.WriteLine("make normal");
            var hwnd = new WindowInteropHelper(this).Handle;

            NativeWindowExtensions.MakeNormal(hwnd);
        }

        public void HandleEditButton(StashTabOverlayView stashTabOverlayView)
        {
            if (stashTabOverlayView.IsEditing)
                StopEditMode();
            else
                StartEditMode();
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
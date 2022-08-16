using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using ChaosRecipeEnhancer.App;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls;
using Serilog;

namespace ChaosRecipeEnhancer.UI.View
{
    /// <summary>
    ///     Interaction logic for StashTabOverlayView.xaml
    /// </summary>
    public partial class StashTabOverlayView : INotifyPropertyChanged
    {
        #region Constructors

        public StashTabOverlayView(ChaosRecipeEnhancerWindow chaosRecipeEnhancerWindow)
        {
            _logger = Log.ForContext<StashTabOverlayView>();
            _logger.Debug("Constructing StashTabOverlayView");

            _chaosRecipeEnhancerWindow = chaosRecipeEnhancerWindow;

            InitializeComponent();
            DataContext = this;
            StashTabOverlayTabControl.ItemsSource = OverlayStashTabList;

            _logger.Debug("StashTabOverlayView constructed successfully");
        }

        #endregion

        #region Fields

        private readonly ILogger _logger;
        private readonly ChaosRecipeEnhancerWindow _chaosRecipeEnhancerWindow;

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

        private Visibility StashBorderVisibility
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
            MouseHook.Stop();

            foreach (var i in StashTabList.StashTabs)
            {
                i.OverlayCellsList.Clear();
                i.TabHeader = null;
            }

            IsOpen = false;
            IsEditing = false;

            _chaosRecipeEnhancerWindow.OpenStashOverlayButtonContent = "Stash";

            base.Hide();
        }

        // TODO: rework tab items and tab headers
        public new virtual void Show()
        {
            if (StashTabList.StashTabs.Count != 0)
            {
                IsOpen = true;
                OverlayStashTabList.Clear();
                _stashTabOverlayIndividualTabHeaderGap.Right = Settings.Default.StashTabOverlayIndividualTabHeaderGap;
                _stashTabOverlayIndividualTabHeaderGap.Left = Settings.Default.StashTabOverlayIndividualTabHeaderGap;
                StashTabOverlayIndividualTabMargin =
                    new Thickness(Settings.Default.StashTabOverlayIndividualTabMargin, 0, 0, 0);

                foreach (var i in StashTabList.StashTabs)
                {
                    TabItem newStashTabItem;
                    var tbk = new TextBlock
                    {
                        Text = i.TabName,
                        DataContext = i
                    };

                    tbk.SetBinding(TextBlock.BackgroundProperty, new Binding("TabHeaderColor"));
                    tbk.SetBinding(TextBlock.PaddingProperty, new Binding("TabHeaderWidth"));
                    tbk.FontSize = 16;
                    i.TabHeader = tbk;

                    if (i.Quad)
                        newStashTabItem = new TabItem
                        {
                            Header = tbk,
                            Content = new DynamicGridControlQuad
                            {
                                ItemsSource = i.OverlayCellsList
                            }
                        };
                    else
                        newStashTabItem = new TabItem
                        {
                            Header = tbk,
                            Content = new DynamicGridControl
                            {
                                ItemsSource = i.OverlayCellsList
                            }
                        };

                    OverlayStashTabList.Add(newStashTabItem);
                }

                StashTabOverlayTabControl.SelectedIndex = 0;

                Data.PrepareSelling();
                Data.ActivateNextCell(true, null);
                if (Settings.Default.StashTabOverlayHighlightMode == 2)
                    foreach (var set in Data.ItemSetListHighlight)
                    foreach (var i in set.ItemList)
                    {
                        var currTab = Data.GetStashTabFromItem(i);
                        currTab.ActivateItemCells(i);
                    }

                _chaosRecipeEnhancerWindow.OpenStashOverlayButtonContent = "Hide";

                MouseHook.Start();
                base.Show();
            }
            else
            {
                MessageBox.Show("No StashTabs Available! Fetch before opening Overlay.", "Stashtab Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            var hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeTransparent(hwnd);
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
            MouseHook.Stop();
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
            MouseHook.Start();
            IsEditing = false;
        }

        public void Transparentize()
        {
            Trace.WriteLine("make transparent");
            var hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeTransparent(hwnd);
        }

        public void Normalize()
        {
            Trace.WriteLine("make normal");
            var hwnd = new WindowInteropHelper(this).Handle;

            Win32.makeNormal(hwnd);
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
﻿using System;
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
    /// Interaction logic for StashTabOverlayView.xaml
    /// </summary>
    public partial class StashTabOverlayWindow : INotifyPropertyChanged
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly SetTrackerOverlayWindow _setTrackerOverlayWindow;

        private static readonly ObservableCollection<TabItem> OverlayStashTabList = new ObservableCollection<TabItem>();
        private Visibility _stashBorderVisibility = Visibility.Hidden;
        private Thickness _tabHeaderGap;
        private Thickness _tabMargin;

        #endregion

        #region Constructors

        public StashTabOverlayWindow(SetTrackerOverlayWindow setTrackerOverlayWindow)
        {
            _logger = Log.ForContext<StashTabOverlayWindow>();
            _logger.Debug("Constructing StashTabOverlayView");

            _setTrackerOverlayWindow = setTrackerOverlayWindow;

            InitializeComponent();
            DataContext = this;
            StashTabOverlayTabControl.ItemsSource = OverlayStashTabList;

            _logger.Debug("StashTabOverlayView constructed successfully");
        }

        #endregion

        #region Properties

        public bool IsOpen { get; set; }
        private bool IsEditing { get; set; }

        public Thickness TabHeaderGap
        {
            get => _tabHeaderGap;
            set
            {
                if (value != _tabHeaderGap)
                {
                    _tabHeaderGap = value;
                    OnPropertyChanged("TabHeaderGap");
                }
            }
        }

        public Thickness TabMargin
        {
            get => _tabMargin;
            set
            {
                if (value != _tabMargin)
                {
                    _tabMargin = value;
                    OnPropertyChanged("TabMargin");
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

            _setTrackerOverlayWindow.OpenStashOverlayButtonContent = "Stash";

            base.Hide();
        }

        // TODO: rework tab items and tab headers
        public new virtual void Show()
        {
            if (StashTabList.StashTabs.Count != 0)
            {
                IsOpen = true;
                OverlayStashTabList.Clear();
                _tabHeaderGap.Right = Settings.Default.TabHeaderGap;
                _tabHeaderGap.Left = Settings.Default.TabHeaderGap;
                TabMargin = new Thickness(Settings.Default.TabMargin, 0, 0, 0);

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
                if (Settings.Default.HighlightMode == 2)
                    foreach (var set in Data.ItemSetListHighlight)
                    foreach (var i in set.ItemList)
                    {
                        var currTab = Data.GetStashTabFromItem(i);
                        currTab.ActivateItemCells(i);
                    }

                _setTrackerOverlayWindow.OpenStashOverlayButtonContent = "Hide";

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

        public void HandleEditButton(StashTabOverlayWindow stashTabOverlayWindow)
        {
            if (stashTabOverlayWindow.IsEditing)
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
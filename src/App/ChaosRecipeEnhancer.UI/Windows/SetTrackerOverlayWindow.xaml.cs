using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class SetTrackerOverlayWindow
{
    private readonly SetTrackerOverlayViewModel _model;
    private readonly StashTabOverlayWindow _stashTabOverlay;
    private LogWatcherManager _logWatcherManager;

    public SetTrackerOverlayWindow()
    {
        DataContext = _model = new SetTrackerOverlayViewModel();

        // initialize stash tab overlay window and log watcher alongside this window
        _stashTabOverlay = new StashTabOverlayWindow();

        InitializeComponent();
        UpdateOverlayType();

        Settings.Default.PropertyChanged += OnSettingsChanged;
    }

    public bool IsOpen { get; private set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Win32.MakeToolWindow(this);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.SetTrackerOverlayDisplayMode))
            UpdateOverlayType();
        else if (e.PropertyName == nameof(Settings.FullSetThreshold)
                 || e.PropertyName == nameof(Settings.StashTabIndices)
                 || e.PropertyName == nameof(Settings.StashTabPrefix))
            _model.UpdateNotificationMessage();
    }

    private void UpdateOverlayType()
    {
        if (Settings.Default.SetTrackerOverlayDisplayMode == (int)SetTrackerOverlayMode.Standard)
            MainOverlayContentControl.Content = new StandardDisplay(this);
        if (Settings.Default.SetTrackerOverlayDisplayMode == (int)SetTrackerOverlayMode.VerticalStandard)
            MainOverlayContentControl.Content = new VerticalStandardDisplay(this);
        if (Settings.Default.SetTrackerOverlayDisplayMode == (int)SetTrackerOverlayMode.Minified)
            MainOverlayContentControl.Content = new MinifiedDisplay(this);
        if (Settings.Default.SetTrackerOverlayDisplayMode == (int)SetTrackerOverlayMode.VerticalMinified)
            MainOverlayContentControl.Content = new VerticalMinifiedDisplay(this);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && !Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled && Mouse.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    public new virtual void Hide()
    {
        IsOpen = false;
        _stashTabOverlay.Hide();

        if (_logWatcherManager is not null)
        {
            _logWatcherManager.StopWatchingLogFile();
            _logWatcherManager.Dispose();
        }

        base.Hide();
    }

    public new virtual void Show()
    {

        if (_model.Settings.AutoFetchOnRezoneEnabled &&
            string.IsNullOrWhiteSpace(_model.Settings.PathOfExileClientLogLocation))
        {
            IsOpen = false;
            _stashTabOverlay.Hide();
            base.Hide();

            ErrorWindow.Spawn(
                "You have enabled Auto-Fetching, but have no PoE Client.txt file path set. Please set the file path or disable the 'General Tab > General Section > Fetch on New Map' setting.",
                "Error: Auto-Fetching"
            );
        }
        else
        {
            IsOpen = true;
            if (_model.Settings.AutoFetchOnRezoneEnabled &&
                !string.IsNullOrWhiteSpace(_model.Settings.PathOfExileClientLogLocation))
            {
                _logWatcherManager = new LogWatcherManager(this);
            }
            base.Show();
        }
    }

    public async void RunFetchingAsync()
    {
        if (!IsOpen) return;

        var successfulResult = await _model.FetchDataAsync(); // Fire and forget async

        if (!successfulResult)
        {
            Hide();
        }
    }

    public void RunReloadFilter()
    {
        if (!IsOpen) return;

        _model.RunReloadFilter();
    }

    public void RunStashTabOverlay()
    {
        if (_stashTabOverlay.IsOpen)
        {
            _stashTabOverlay.Hide();
        }
        else
        {
            _stashTabOverlay.Show();
        }
    }
}
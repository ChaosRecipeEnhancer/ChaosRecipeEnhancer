using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Utilities.Native;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class SetTrackerOverlayWindow : Window
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
        WindowsUtilitiesForOverlays.MakeToolWindow(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.SetTrackerOverlayDisplayMode) ||
            e.PropertyName == nameof(Settings.SetTrackerOverlayItemCounterDisplayMode))
            UpdateOverlayType(); // HACK: force update the item counters when we change the display mode
        else if (e.PropertyName == nameof(Settings.FullSetThreshold) ||
                 e.PropertyName == nameof(Settings.StashTabIndices) ||
                 e.PropertyName == nameof(Settings.StashTabPrefix) ||
                 e.PropertyName == nameof(Settings.SilenceSetsFullMessage) ||
                 e.PropertyName == nameof(Settings.SilenceNeedItemsMessage))
            _model.UpdateStashButtonAndWarningMessage(false);
    }

    private void UpdateOverlayType()
    {
        MainOverlayContentControl.Content = Settings.Default.SetTrackerOverlayDisplayMode switch
        {
            (int)SetTrackerOverlayMode.Standard => new StandardDisplay(this),
            (int)SetTrackerOverlayMode.VerticalStandard => new VerticalStandardDisplay(this),
            (int)SetTrackerOverlayMode.Minified => new MinifiedDisplay(this),
            (int)SetTrackerOverlayMode.VerticalMinified => new VerticalMinifiedDisplay(this),
            (int)SetTrackerOverlayMode.OnlyButtons => new OnlyButtonsDisplay(this),
            (int)SetTrackerOverlayMode.OnlyButtonsVertical => new VerticalOnlyButtons(this),
            (int)SetTrackerOverlayMode.OnlyMinifiedButtons => new MinifiedOnlyButtonsDisplay(this),
            (int)SetTrackerOverlayMode.OnlyMinifiedButtonsVertical => new VerticalMinifiedOnlyButtonsDisplay(this),
            _ => MainOverlayContentControl.Content
        };
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

        if (_logWatcherManager != null)
        {
            _logWatcherManager.StopWatchingLogFile();
            _logWatcherManager.Dispose();
            _logWatcherManager = null;
        }

        base.Hide();
    }

    public new virtual void Show()
    {
        if (_model.GlobalUserSettings.AutoFetchOnRezoneEnabled &&
            string.IsNullOrWhiteSpace(_model.GlobalUserSettings.PathOfExileClientLogLocation))
        {
            IsOpen = false;
            _stashTabOverlay.Hide();
            base.Hide();

            GlobalErrorHandler.Spawn(
                "You have enabled Auto-Fetching, but have no PoE Client.txt file path set. Please set the file path or disable the 'General Tab > General Section > Fetch on New Map' setting.",
                "Error: Auto-Fetching"
            );
        }
        else
        {
            IsOpen = true;
            if (_model.GlobalUserSettings.AutoFetchOnRezoneEnabled &&
                !string.IsNullOrWhiteSpace(_model.GlobalUserSettings.PathOfExileClientLogLocation))
            {
                _logWatcherManager = new LogWatcherManager(this);
            }
            base.Show();
        }
    }

    public async void RunFetchingAsync()
    {
        if (!IsOpen) return;

        var successfulResult = await _model.FetchStashDataAsync();

        if (!successfulResult)
        {
            Hide();
        }
    }

    public async void RunReloadFilter()
    {
        if (!IsOpen) return;

        if (_model.GlobalUserSettings.LootFilterManipulationEnabled)
        {
            await _model.RunReloadFilter();
        }
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

    public void RunSetTrackerOverlay()
    {
        if (IsOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}
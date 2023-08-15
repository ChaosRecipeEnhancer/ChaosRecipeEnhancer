using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Windows;

internal partial class SetTrackerOverlayWindow
{
    private readonly SetTrackerOverlayViewModel _model;

    private readonly StashTabOverlayWindow _stashTabOverlay;

    public SetTrackerOverlayWindow()
    {
        DataContext = _model = new SetTrackerOverlayViewModel();
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
        else if (e.PropertyName == nameof(Settings.FullSetThreshold) || e.PropertyName == nameof(Settings.SelectedStashTabs))
            _model.UpdateNotificationMessage();
    }

    private void UpdateOverlayType()
    {
        if (Settings.Default.SetTrackerOverlayDisplayMode == 0)
            MainOverlayContentControl.Content = new StandardDisplay(this);
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
        base.Hide();
    }

    public new virtual void Show()
    {
        IsOpen = true;
        base.Show();
    }

    public void RunFetching()
    {
        if (!IsOpen) return;

        _model.FetchDataAsync(); // Fire and forget async
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
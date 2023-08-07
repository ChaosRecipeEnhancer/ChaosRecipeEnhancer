using System.Windows;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for MinifiedDisplay.xaml
/// </summary>
public partial class MinifiedDisplay
{
    #region Fields

    private readonly SetTrackerOverlayView _setTrackerOverlay;
    private readonly SettingsView _settingsView;

    #endregion

    #region Constructors

    public MinifiedDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
    {
        _settingsView = settingsView;
        _setTrackerOverlay = setTrackerOverlay;
        InitializeComponent();
    }

    #endregion

    #region Event Handlers

    private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
    {
        _settingsView.RunStashTabOverlay();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        _setTrackerOverlay.RunFetching();
    }

    private void ReloadItemFilterButton_Click(object sender, RoutedEventArgs e)
    {
        _setTrackerOverlay.ReloadItemFilter();
    }

    #endregion
}
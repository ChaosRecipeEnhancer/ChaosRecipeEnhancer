using System.Configuration;
using System.Windows;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

internal partial class StandardDisplay
{
    private readonly SetTrackerOverlayWindow _parent;

    public StandardDisplay(SetTrackerOverlayWindow parent)
    {
        InitializeComponent();
        _parent = parent;
    }

    private void OnStashTabOverlayButtonClicked(object sender, RoutedEventArgs e)
    {
        _parent.RunStashTabOverlay();
    }

    private void OnFetchButtonClicked(object sender, RoutedEventArgs e)
    {
        _parent.RunFetching();
    }

    private void OnReloadFilterButtonClicked(object sender, RoutedEventArgs e)
    {
        _parent.RunReloadFilter();
    }
}
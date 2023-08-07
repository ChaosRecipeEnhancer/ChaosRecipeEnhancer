using System.Windows;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for MinifiedDisplay.xaml
/// </summary>
internal partial class MinifiedDisplay
{
    private readonly SetTrackerOverlayView _parent;

    public MinifiedDisplay(SetTrackerOverlayView parent)
    {
        _parent = parent;
        InitializeComponent();
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
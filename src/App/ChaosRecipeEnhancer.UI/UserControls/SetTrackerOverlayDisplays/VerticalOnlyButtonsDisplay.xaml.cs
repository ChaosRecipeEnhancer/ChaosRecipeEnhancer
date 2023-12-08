using System.Windows;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

public partial class VerticalOnlyButtons
{
    private readonly SetTrackerOverlayWindow _parent;

    public VerticalOnlyButtons(SetTrackerOverlayWindow parent)
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
        _parent.RunFetchingAsync();
    }

    private void OnReloadFilterButtonClicked(object sender, RoutedEventArgs e)
    {
        _parent.RunReloadFilter();
    }
}
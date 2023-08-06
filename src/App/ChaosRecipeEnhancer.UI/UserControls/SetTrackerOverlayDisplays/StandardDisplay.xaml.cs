using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for SetTrackerOverlayStandardDisplay.xaml
/// </summary>
public partial class StandardDisplay
{
	private readonly SetTrackerOverlayView _setTrackerOverlay;

	public StandardDisplay(SetTrackerOverlayView setTrackerOverlay)
	{
		_setTrackerOverlay = setTrackerOverlay;
		InitializeComponent();
	}

	private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
	{
		_settingsView.RunStashTabOverlay();
	}

	private void FetchButton_Click(object sender, RoutedEventArgs e)
	{
		_setTrackerOverlay.RunFetching();
	}

	private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
	{
		_setTrackerOverlay.ReloadItemFilter();
	}
}
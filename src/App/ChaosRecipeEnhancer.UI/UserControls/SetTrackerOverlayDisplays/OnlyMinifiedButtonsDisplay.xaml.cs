using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for OnlyMinifiedButtonsDisplay.xaml
/// </summary>
public partial class OnlyMinifiedButtonsDisplay
{
	#region Fields

	private readonly ILogger _logger;
	private readonly SetTrackerOverlayView _setTrackerOverlay;
	private readonly SettingsView _settingsView;

	#endregion

	#region Constructors

	public OnlyMinifiedButtonsDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
	{
		_logger = Log.ForContext<OnlyMinifiedButtonsDisplay>();
		_logger.Debug("Constructing OnlyMinifiedButtonsDisplay");

		_settingsView = settingsView;
		_setTrackerOverlay = setTrackerOverlay;
		InitializeComponent();

		_logger.Debug("OnlyMinifiedButtonsDisplay constructed successfully");
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
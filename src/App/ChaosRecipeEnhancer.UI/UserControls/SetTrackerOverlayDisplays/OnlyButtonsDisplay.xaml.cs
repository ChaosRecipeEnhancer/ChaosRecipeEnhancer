using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for OnlyButtonsDisplay.xaml
/// </summary>
public partial class OnlyButtonsDisplay
{
	#region Constructors

	public OnlyButtonsDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
	{
		_logger = Log.ForContext<OnlyButtonsDisplay>();
		_logger.Debug("Constructing OnlyButtonsDisplay");

		_settingsView = settingsView;
		_setTrackerOverlay = setTrackerOverlay;

		InitializeComponent();

		_logger.Debug("OnlyButtonsDisplay constructed successfully");
	}

	#endregion

	#region Fields

	private readonly ILogger _logger;
	private readonly SetTrackerOverlayView _setTrackerOverlay;
	private readonly SettingsView _settingsView;

	#endregion

	#region Event Handlers

	private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
	{
		_settingsView.RunStashTabOverlay();
	}

	private void RefreshButton_Click_1(object sender, RoutedEventArgs e)
	{
		_setTrackerOverlay.RunFetching();
	}

	private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
	{
		_setTrackerOverlay.ReloadItemFilter();
	}

	#endregion
}
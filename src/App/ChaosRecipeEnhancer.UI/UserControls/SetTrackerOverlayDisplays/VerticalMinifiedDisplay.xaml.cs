using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for VerticalMinifiedDisplay.xaml
/// </summary>
public partial class VerticalMinifiedDisplay
{
	#region Constructors

	public VerticalMinifiedDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
	{
		_logger = Log.ForContext<VerticalMinifiedDisplay>();
		_logger.Debug("Constructing VerticalMinifiedDisplay");

		_settingsView = settingsView;
		_setTrackerOverlay = setTrackerOverlay;
		InitializeComponent();

		_logger.Debug("VerticalMinifiedDisplay  constructed successfully");
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
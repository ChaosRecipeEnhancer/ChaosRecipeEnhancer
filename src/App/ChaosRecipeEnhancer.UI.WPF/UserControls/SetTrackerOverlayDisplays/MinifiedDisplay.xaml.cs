using System.Windows;

using Serilog;

using ChaosRecipeEnhancer.UI.WPF.View;

namespace ChaosRecipeEnhancer.UI.WPF.UserControls.SetTrackerOverlayDisplays;

/// <summary>
///     Interaction logic for MinifiedDisplay.xaml
/// </summary>
public partial class MinifiedDisplay
{
	#region Fields

	private readonly ILogger _logger;
	private readonly SetTrackerOverlayView _setTrackerOverlay;
	private readonly SettingsView _settingsView;

	#endregion

	#region Constructors

	public MinifiedDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
	{
		_logger = Log.ForContext<MinifiedDisplay>();
		_logger.Debug("Constructing MinifiedDisplay");

		_settingsView = settingsView;
		_setTrackerOverlay = setTrackerOverlay;
		InitializeComponent();

		_logger.Debug("MinifiedDisplay  constructed successfully");
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
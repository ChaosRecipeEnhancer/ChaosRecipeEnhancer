using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays
{
    /// <summary>
    ///     Interaction logic for SetTrackerOverlayStandardDisplay.xaml
    /// </summary>
    public partial class StandardDisplay
    {
        #region Constructors

        public StandardDisplay(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
        {
            _logger = Log.ForContext<StandardDisplay>();
            _logger.Debug("Constructing StandardDisplay");

            _settingsView = settingsView;
            _setTrackerOverlay = setTrackerOverlay;
            InitializeComponent();

            _logger.Debug("StandardDisplay constructed successfully");
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

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            _setTrackerOverlay.RunFetching();
        }

        private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _setTrackerOverlay.ReloadItemFilter();
        }

        #endregion
    }
}
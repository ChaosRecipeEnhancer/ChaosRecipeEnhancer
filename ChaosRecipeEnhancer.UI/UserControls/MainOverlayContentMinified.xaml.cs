using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    ///     Interaction logic for MainOverlayContentMinified.xaml
    /// </summary>
    public partial class MainOverlayContentMinified
    {
        #region Constructors

        public MainOverlayContentMinified(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Constructing MainOverlayContentMinified");

            _settingsView = settingsView;
            _setTrackerOverlay = setTrackerOverlay;
            InitializeComponent();

            _logger.Debug("MainOverlayContentMinified  constructed successfully");
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
}
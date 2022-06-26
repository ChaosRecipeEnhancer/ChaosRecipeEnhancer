using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for MainOverlayContentMinified.xaml
    /// </summary>
    public partial class MainOverlayContentMinified
    {
        #region Fields

        private ILogger _logger;
        private readonly SetTrackerOverlayWindow _setTrackerOverlay;
        private readonly SettingsWindow _settingsWindow;

        #endregion

        #region Constructors

        public MainOverlayContentMinified(SettingsWindow settingsWindow, SetTrackerOverlayWindow setTrackerOverlay)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Constructing MainOverlayContentMinified");

            _settingsWindow = settingsWindow;
            _setTrackerOverlay = setTrackerOverlay;
            InitializeComponent();

            _logger.Debug("MainOverlayContentMinified  constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.RunStashTabOverlay();
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
using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for MainOverlayOnlyButtons.xaml
    /// </summary>
    public partial class MainOverlayOnlyButtons
    {
        #region Fields

        private ILogger _logger;
        private readonly SetTrackerOverlayWindow _setTrackerOverlay;
        private readonly SettingsWindow _settingsWindow;

        #endregion

        #region Constructors

        public MainOverlayOnlyButtons(SettingsWindow settingsWindow, SetTrackerOverlayWindow setTrackerOverlay)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Constructing MainOverlayOnlyButtons");

            _settingsWindow = settingsWindow;
            _setTrackerOverlay = setTrackerOverlay;

            InitializeComponent();

            _logger.Debug("MainOverlayOnlyButtons constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.RunStashTabOverlay();
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
}
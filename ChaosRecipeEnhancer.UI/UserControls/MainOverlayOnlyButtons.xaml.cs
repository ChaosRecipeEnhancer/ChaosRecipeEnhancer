using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    ///     Interaction logic for MainOverlayOnlyButtons.xaml
    /// </summary>
    public partial class MainOverlayOnlyButtons
    {
        #region Constructors

        public MainOverlayOnlyButtons(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
        {
            _logger = Log.ForContext<MainOverlayOnlyButtons>();
            _logger.Debug("Constructing MainOverlayOnlyButtons");

            _settingsView = settingsView;
            _setTrackerOverlay = setTrackerOverlay;

            InitializeComponent();

            _logger.Debug("MainOverlayOnlyButtons constructed successfully");
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
}
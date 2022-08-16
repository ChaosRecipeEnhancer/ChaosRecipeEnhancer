using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    ///     Interaction logic for VerticalMainOverlayContent.xaml
    /// </summary>
    public partial class VerticalMainOverlayContent
    {
        #region Constructors

        public VerticalMainOverlayContent(SettingsView settingsView, SetTrackerOverlayView setTrackerOverlay)
        {
            _logger = Log.ForContext<VerticalMainOverlayContent>();
            _logger.Debug("Constructing VerticalMainOverlayContent");

            _settingsView = settingsView;
            _setTrackerOverlay = setTrackerOverlay;
            InitializeComponent();

            _logger.Debug("MainOverlayContent constructed successfully");
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
using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for MainOverlayContent.xaml
    /// </summary>
    public partial class MainOverlayContent
    {
        #region Fields

        private ILogger _logger;
        private readonly SetTrackerOverlayWindow _setTrackerOverlay;
        private readonly SettingsWindow _settingsWindow;

        #endregion

        #region Constructors

        public MainOverlayContent(SettingsWindow settingsWindow, SetTrackerOverlayWindow setTrackerOverlay)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Constructing MainOverlayContent");

            _settingsWindow = settingsWindow;
            _setTrackerOverlay = setTrackerOverlay;
            InitializeComponent();

            _logger.Debug("MainOverlayContent constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.RunStashTabOverlay();
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
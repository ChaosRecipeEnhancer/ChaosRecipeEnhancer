using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for VerticalMainOverlayContentMinified.xaml
    /// </summary>
    public partial class VerticalMainOverlayContentMinified
    {
        #region Fields

        private ILogger _logger;
        private readonly ChaosRecipeEnhancerWindow _chaosRecipeEnhancer;
        private readonly MainWindow _mainWindow;

        #endregion

        #region Constructors

        public VerticalMainOverlayContentMinified(MainWindow mainWindow, ChaosRecipeEnhancerWindow chaosRecipeEnhancer)
        {
            _logger = Log.ForContext<VerticalMainOverlayContentMinified>();
            _logger.Debug("Constructing VerticalMainOverlayContentMinified");

            _mainWindow = mainWindow;
            _chaosRecipeEnhancer = chaosRecipeEnhancer;
            InitializeComponent();

            _logger.Debug("MainOverlayContentMinified  constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.RunStashTabOverlay();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _chaosRecipeEnhancer.RunFetching();
        }

        private void ReloadItemFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _chaosRecipeEnhancer.ReloadItemFilter();
        }

        #endregion
    }
}
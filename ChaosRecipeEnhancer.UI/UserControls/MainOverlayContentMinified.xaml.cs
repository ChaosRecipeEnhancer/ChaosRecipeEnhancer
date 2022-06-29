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
        private readonly ChaosRecipeEnhancerWindow _chaosRecipeEnhancer;
        private readonly MainWindow _mainWindow;

        #endregion

        #region Constructors

        public MainOverlayContentMinified(MainWindow mainWindow, ChaosRecipeEnhancerWindow chaosRecipeEnhancer)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Constructing MainOverlayContentMinified");

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
using System.Windows;
using ChaosRecipeEnhancer.UI.View;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for VerticalMainOverlayContent.xaml
    /// </summary>
    public partial class VerticalMainOverlayContent
    {
        #region Fields

        private ILogger _logger;
        private readonly ChaosRecipeEnhancerWindow _chaosRecipeEnhancer;
        private readonly MainWindow _mainWindow;

        #endregion

        #region Constructors

        public VerticalMainOverlayContent(MainWindow mainWindow, ChaosRecipeEnhancerWindow chaosRecipeEnhancer)
        {
            _logger = Log.ForContext<VerticalMainOverlayContent>();
            _logger.Debug("Constructing VerticalMainOverlayContent");

            _mainWindow = mainWindow;
            _chaosRecipeEnhancer = chaosRecipeEnhancer;
            InitializeComponent();

            _logger.Debug("MainOverlayContent constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.RunStashTabOverlay();
        }

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            _chaosRecipeEnhancer.RunFetching();
        }

        private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _chaosRecipeEnhancer.ReloadItemFilter();
        }

        #endregion
    }
}
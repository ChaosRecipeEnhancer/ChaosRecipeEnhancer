using System.Windows;
using EnhancePoE.UI.View;
using Serilog;

namespace EnhancePoE.UI.UserControls
{
    /// <summary>
    /// Interaction logic for MainOverlayContent.xaml
    /// </summary>
    public partial class MainOverlayContent
    {
        #region Fields

        private ILogger _logger;
        private readonly ChaosRecipeEnhancer _chaosRecipeEnhancer;
        private readonly MainWindow _mainWindow;

        #endregion

        #region Constructors

        public MainOverlayContent(MainWindow mainWindow, ChaosRecipeEnhancer chaosRecipeEnhancer)
        {
            _logger = Log.ForContext<MainOverlayContentMinified>();
            _logger.Debug("Initializing MainOverlayContent");

            _mainWindow = mainWindow;
            _chaosRecipeEnhancer = chaosRecipeEnhancer;
            InitializeComponent();

            _logger.Debug("MainOverlayContent initialized");
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
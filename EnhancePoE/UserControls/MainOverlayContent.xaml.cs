using System.Windows;

namespace EnhancePoE.UserControls
{
    /// <summary>
    ///     Interaction logic for MainOverlayContent.xaml
    /// </summary>
    public partial class MainOverlayContent
    {
        public MainOverlayContent()
        {
            InitializeComponent();
        }

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunStashTabOverlay();
        }

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.overlay.RunFetching();
        }

        private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.overlay.ReloadItemFilter();
        }
    }
}
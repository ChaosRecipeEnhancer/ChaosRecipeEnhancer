using EnhancePoE.UI.View;
using System.Windows;
using System.Windows.Controls;

namespace EnhancePoE.UI.UserControls
{
    /// <summary>
    ///     Interaction logic for MainOverlayOnlyButtons.xaml
    /// </summary>
    public partial class MainOverlayOnlyButtons : UserControl
    {
        public MainOverlayOnlyButtons()
        {
            InitializeComponent();
        }

        private void OpenStashTabOverlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RunStashTabOverlay();
        }

        private void RefreshButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.Overlay.RunFetching();
        }

        private void ReloadFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Overlay.ReloadItemFilter();
        }
    }
}
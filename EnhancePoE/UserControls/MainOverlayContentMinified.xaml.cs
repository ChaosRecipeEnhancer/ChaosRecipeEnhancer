using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnhancePoE.UserControls
{
    /// <summary>
    ///     Interaction logic for MainOverlayContentMinified.xaml
    /// </summary>
    public partial class MainOverlayContentMinified : UserControl
    {
        public MainOverlayContentMinified()
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

        private void ReloadItemFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Overlay.ReloadItemFilter();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.RunStashTabOverlay();
        }
    }
}
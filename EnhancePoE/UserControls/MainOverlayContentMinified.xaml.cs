using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnhancePoE.UserControls
{
    /// <summary>
    /// Interaction logic for MainOverlayContentMinified.xaml
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
            MainWindow.overlay.RunFetching();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.RunStashTabOverlay();

        }
    }
}

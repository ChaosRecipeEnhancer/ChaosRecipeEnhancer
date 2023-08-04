using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms
{
    public partial class RecipesForm
    {
        public RecipesForm()
        {
            InitializeComponent();
        }

        private void ChaosRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.RegalRecipeTrackingEnabled = false;
        }

        private void RegalRecipeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ChaosRecipeTrackingEnabled = false;
        }
    }
}
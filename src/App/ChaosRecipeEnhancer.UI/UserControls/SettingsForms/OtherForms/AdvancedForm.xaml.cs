using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public partial class AdvancedForm
{
    public AdvancedForm()
    {
        DataContext = new AdvancedFormViewModel();
        InitializeComponent();
    }

    private void OnResetButtonClicked(object sender, RoutedEventArgs e)
    {
        switch (MessageBox.Show("This will reset all of your settings!", "Reset Settings", MessageBoxButton.YesNo))
        {
            case MessageBoxResult.Yes:
                Settings.Default.Reset();
                break;
            case MessageBoxResult.No:
                break;
        }
    }
}
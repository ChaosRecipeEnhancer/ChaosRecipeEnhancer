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
        switch (MessageBox.Show("This will reset all of your settings. You will lose all your user configurations if decide not to manually back them up.", "Warning: Reset Settings", MessageBoxButton.YesNo))
        {
            case MessageBoxResult.Yes:
                // Reset settings to default
                Settings.Default.Reset();
                // Disable the upgrade setting
                Settings.Default.UpgradeSettingsAfterUpdate = false;
                // Save the settings
                Settings.Default.Save();
                break;
            case MessageBoxResult.No:
                break;
        }
    }
}
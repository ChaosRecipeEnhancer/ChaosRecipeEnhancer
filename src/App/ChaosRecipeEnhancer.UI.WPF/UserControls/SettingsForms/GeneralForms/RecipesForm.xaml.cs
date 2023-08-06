using System.Windows;

using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.UserControls.SettingsForms.GeneralForms;

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
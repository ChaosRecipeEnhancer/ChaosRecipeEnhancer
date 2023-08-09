using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms;

public partial class LootFilterManipulationForm
{
    public LootFilterManipulationForm()
    {
        DataContext = new LootFilterManipulationFormViewModel();
        InitializeComponent();
    }

    private void OnLootFilterFileDialogInputClicked(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.Filter = "LootFilter|*.filter";
        var res = open.ShowDialog();

        if (res != DialogResult.OK) return;

        var filename = open.FileName;
        Settings.Default.LootFilterFileLocation = filename;
        LootFilterFileDialog.Content = filename;
    }
}
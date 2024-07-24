using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using System.Windows.Forms;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms;

public partial class LootFilterManipulationForm
{
    private readonly LootFilterManipulationFormViewModel _model;
    public LootFilterManipulationForm()
    {
        DataContext = _model = Ioc.Default.GetService<LootFilterManipulationFormViewModel>();
        InitializeComponent();
    }

    private void LootFilterFileDialogInput_Clicked(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog
        {
            Filter = "LootFilter|*.filter"
        };

        var res = open.ShowDialog();

        if (res != DialogResult.OK) return;

        var filename = open.FileName;
        _model.LootFilterFileLocation = filename;
        LootFilterFileDialog.Content = filename;
    }
}
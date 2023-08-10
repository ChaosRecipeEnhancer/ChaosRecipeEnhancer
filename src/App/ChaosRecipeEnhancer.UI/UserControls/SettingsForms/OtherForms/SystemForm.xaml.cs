using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public partial class SystemForm
{
    private readonly SystemFormViewModel _model;

    public SystemForm()
    {
        DataContext = _model = new SystemFormViewModel();
        InitializeComponent();
    }

    private void ComboBox_ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _model.OnAppThemeChanged();
    }
}
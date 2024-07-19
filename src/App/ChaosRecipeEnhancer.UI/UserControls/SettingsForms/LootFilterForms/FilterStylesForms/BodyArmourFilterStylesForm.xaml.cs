using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class BodyArmourFilterStylesForm : UserControl
{
    private readonly BodyArmourFilterStylesFormViewModel _model;

    public BodyArmourFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<BodyArmourFilterStylesFormViewModel>();
        InitializeComponent();
    }
}

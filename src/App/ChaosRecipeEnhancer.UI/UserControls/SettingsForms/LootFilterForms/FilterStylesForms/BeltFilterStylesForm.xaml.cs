using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class BeltFilterStylesForm : UserControl
{
    private readonly BeltFilterStylesFormViewModel _model;

    public BeltFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<BeltFilterStylesFormViewModel>();
        InitializeComponent();
    }
}

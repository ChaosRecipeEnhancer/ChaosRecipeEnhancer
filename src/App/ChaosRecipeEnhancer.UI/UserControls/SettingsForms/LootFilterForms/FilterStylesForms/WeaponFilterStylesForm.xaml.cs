using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class WeaponFilterStylesForm : UserControl
{
    private readonly WeaponFilterStylesFormViewModel _model;

    public WeaponFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<WeaponFilterStylesFormViewModel>();
        InitializeComponent();
    }
}

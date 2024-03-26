using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

public partial class PathOfExileAccountOAuthForm : UserControl
{
    public PathOfExileAccountOAuthForm()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<PathOfExileAccountOAuthFormViewModel>();
    }
}

using System.Windows;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

/// <summary>
/// Interaction logic for PathOfExileAccountOAuthForm.xaml
/// </summary>
public partial class PathOfExileAccountOAuthForm : UserControl
{
    private readonly PathOfExileAccountOAuthFormViewModel _model;

    public PathOfExileAccountOAuthForm()
    {
        InitializeComponent();
        DataContext = _model = new PathOfExileAccountOAuthFormViewModel();
    }

    private void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        PathOfExileAccountOAuthFormViewModel.LoginToPoEWebsite();
    }

    private void OnLogoutClicked(object sender, RoutedEventArgs e)
    {
        PathOfExileAccountOAuthFormViewModel.Logout();
    }
}

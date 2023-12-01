using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Properties;

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

        Settings.Default.PropertyChanged += OnSettingsChanged;
    }

    private void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        PathOfExileAccountOAuthFormViewModel.LoginToPoEWebsite();
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.PathOfExileWebsiteSessionId) ||
            e.PropertyName == nameof(Settings.PathOfExileAccountName))
        {
            _model.Settings.PoEAccountConnectionStatus = 0;
            _model.Settings.Save();
        }
    }
}


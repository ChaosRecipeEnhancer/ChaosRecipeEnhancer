using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public partial class LegacyStashForm
{
    private readonly LegacyStashFormViewModel _model;

    public LegacyStashForm()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<LegacyStashFormViewModel>();
    }

    private void LeagueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        _model.UpdateUserSettingsForSelectedLeague(comboBox.SelectedItem);
    }

    private void TabsCheckComboBox_SelectionChanged(object sender, ItemSelectionChangedEventArgs itemSelectionChangedEventArgs)
    {
        var checkComboBox = (CheckComboBox)sender;
        _model.UpdateUserSettingsForSelectedTabIdentifiers(checkComboBox.SelectedItems);
    }
}

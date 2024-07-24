using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public partial class LegacyStashForm
{
    private readonly LegacyStashFormViewModel _model;
    private const int MaxSelectedItems = 10;

    public LegacyStashForm()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<LegacyStashFormViewModel>();
    }

    #region Methods

    private void LeagueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        _model.UpdateUserSettingsForSelectedLeague(comboBox.SelectedItem);
    }

    private void TabsCheckComboBox_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
    {
        var checkComboBox = (CheckComboBox)sender;

        if (checkComboBox.SelectedItems.Count > MaxSelectedItems)
        {
            // Remove items from the start until we're at the limit
            while (checkComboBox.SelectedItems.Count > MaxSelectedItems)
            {
                checkComboBox.SelectedItems.RemoveAt(0);
            }
        }

        // Update the ViewModel
        _model.UpdateUserSettingsForSelectedTabIdentifiers(checkComboBox.SelectedItems);
    }

    #endregion
}

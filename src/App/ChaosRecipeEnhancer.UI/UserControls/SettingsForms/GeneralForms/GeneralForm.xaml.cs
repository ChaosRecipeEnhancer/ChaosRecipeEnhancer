using CommunityToolkit.Mvvm.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public partial class GeneralForm
{
    private readonly GeneralFormViewModel _model;

    public GeneralForm()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<GeneralFormViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_model.ReadyToFetchData())
        {
            await Task.Run(async () =>
            {
                // Dispatch any UI updates back to the UI thread
                await Dispatcher.InvokeAsync(async () =>
                {
                    await _model.LoadLeagueListAsync();
                    await _model.LoadStashTabNamesIndicesAsync();
                });
            });
        }
    }

    private void OnStashTabSelectionChanged(object sender, ItemSelectionChangedEventArgs itemSelectionChangedEventArgs)
    {
        var checkComboBox = (CheckComboBox)sender;
        _model.UpdateSelectedTabList(checkComboBox.SelectedItems);
    }
}
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Serilog;
using System;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms;

public partial class FilterStylesForm
{
    public FilterStylesForm()
    {
        InitializeComponent();
    }

    private void OnReloadFilterClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var reloadService = Ioc.Default.GetService<IReloadFilterService>();
            reloadService?.ReloadFilter();
        }
        catch (Exception ex)
        {
            Log.ForContext<FilterStylesForm>()
                .Warning(ex, "Failed to reload filter from style editor");
        }
    }
}

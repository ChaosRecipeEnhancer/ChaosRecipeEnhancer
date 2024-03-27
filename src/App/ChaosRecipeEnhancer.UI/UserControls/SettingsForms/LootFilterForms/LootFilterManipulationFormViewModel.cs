using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms;

public class LootFilterManipulationFormViewModel : CreViewModelBase
{
    private readonly IFilterManipulationService _filterManipulationService = Ioc.Default.GetRequiredService<IFilterManipulationService>();
    private readonly IReloadFilterService _reloadFilterService = Ioc.Default.GetRequiredService<IReloadFilterService>();

    public void RunCleanFilter()
    {
        _filterManipulationService.RemoveChaosRecipeSectionAsync();
        _reloadFilterService.ReloadFilter();
    }
}
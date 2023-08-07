using ChaosRecipeEnhancer.UI.Api;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class PathOfExileForm
{
    private readonly LeagueGetter _leagueGetter = new();
    private readonly PathOfExileFormViewModel _model;

    public PathOfExileForm()
    {
        DataContext = _model = new PathOfExileFormViewModel();
        InitializeComponent();

        // Populate the league dropdown
        LoadLeagueList();
    }

    private async void LoadLeagueList()
    {
        var leagues = await _leagueGetter.GetLeaguesAsync();
        _model.UpdateLeagueList(leagues);
    }
}
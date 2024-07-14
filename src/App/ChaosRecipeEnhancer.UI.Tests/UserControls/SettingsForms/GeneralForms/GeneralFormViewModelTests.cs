using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

namespace ChaosRecipeEnhancer.UI.Tests.UserControls.SettingsForms.GeneralForms;

public class GeneralFormViewModelTests
{
    private readonly GeneralFormViewModel _sut;

    public GeneralFormViewModelTests()
    {
        _sut = new GeneralFormViewModel(null, new UserSettings());
    }

    [Fact]
    public void OnLeagueChange_WhenLeagueIsChanged_SelectedStashTabIndexShouldClear()
    {
        // Arrange
        _sut.StashTabIndices = new HashSet<string> { "1", "2", "3" };
        _sut.SelectedLeagueName = "Some Old League";
        
        // Act
        _sut.SelectedLeagueName = "Some New League";
        
        // Assert
        _sut.StashTabIndices.Should().BeEmpty();
    }
}
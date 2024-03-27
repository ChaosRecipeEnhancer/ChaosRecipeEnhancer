using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls;

namespace ChaosRecipeEnhancer.UI.Tests.UserControls;

public class CreViewModelBaseTests
{
    [Fact]
    public void GlobalUserSettings_ShouldReturnDefaultSettings()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act
        var settings = viewModel.GlobalUserSettings;

        // Assert
        Assert.NotNull(settings);
        Assert.IsType<Settings>(settings);
    }

    private class TestViewModel : CreViewModelBase
    {
        // Empty test implementation
    }
}
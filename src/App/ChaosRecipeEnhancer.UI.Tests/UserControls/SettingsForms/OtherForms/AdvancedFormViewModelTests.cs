using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;
using Moq;

namespace ChaosRecipeEnhancer.UI.Tests.UserControls.SettingsForms.OtherForms;

public class AdvancedFormViewModelTests
{
    private readonly Mock<IAuthStateManager> _mockAuthStateManager;
    private readonly Mock<IUserSettings> _mockUserSettings;
    private readonly AdvancedFormViewModel _viewModel;

    public AdvancedFormViewModelTests()
    {
        _mockAuthStateManager = new Mock<IAuthStateManager>();
        _mockUserSettings = new Mock<IUserSettings>();
        _viewModel = new AdvancedFormViewModel(_mockAuthStateManager.Object, _mockUserSettings.Object);
    }

    [Fact]
    public void DoNotPreserveLowItemLevelGearIsChecked_ShouldSetUserSettingsValue()
    {
        // Arrange
        bool expectedValue = true;

        // Act
        _viewModel.DoNotPreserveLowItemLevelGearIsChecked = expectedValue;

        // Assert
        _mockUserSettings.VerifySet(us => us.DoNotPreserveLowItemLevelGear = expectedValue, Times.Once);
    }
}
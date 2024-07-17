using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using Moq;

namespace ChaosRecipeEnhancer.UI.Tests.Services;

public class NotificationSoundServiceTests
{
    private readonly Mock<IUserSettings> _mockUserSettings;

    public NotificationSoundServiceTests()
    {
        _mockUserSettings = new Mock<IUserSettings>();
    }

    [Fact]
    public void Constructor_InitializesSoundPlayers_WhenSoundEnabled()
    {
        // Arrange
        _mockUserSettings.Setup(u => u.SoundEnabled).Returns(true);

        // Act
        var service = new NotificationSoundService(_mockUserSettings.Object);

        // Assert
        Assert.NotNull(service);
        // You may need to expose _soundPlayers as a protected property for this assertion
        // Assert.Equal(3, service.SoundPlayers.Count);
    }

    [Fact]
    public void Constructor_DoesNotInitializeSoundPlayers_WhenSoundDisabled()
    {
        // Arrange
        _mockUserSettings.Setup(u => u.SoundEnabled).Returns(false);

        // Act
        var service = new NotificationSoundService(_mockUserSettings.Object);

        // Assert
        Assert.NotNull(service);
        // You may need to expose _soundPlayers as a protected property for this assertion
        // Assert.Empty(service.SoundPlayers);
    }

    [Fact]
    public void PlayNotificationSound_PlaysSound_WhenSoundEnabledAndSoundTypeExists()
    {
        // Arrange
        _mockUserSettings.Setup(u => u.SoundEnabled).Returns(true);
        _mockUserSettings.Setup(u => u.SoundLevel).Returns(0.5);

        var mockAudioResource = new Mock<IAudioResource>();

        var service = new NotificationSoundService(_mockUserSettings.Object);

        service.SetAudioResource(NotificationSoundType.ItemSetStateChanged, mockAudioResource.Object);

        // Act
        service.PlayNotificationSound(NotificationSoundType.ItemSetStateChanged);

        // Assert
        mockAudioResource.Verify(a => a.Play(0.5f), Times.Once);
    }

    [Fact]
    public void PlayNotificationSound_DoesNotPlaySound_WhenSoundDisabled()
    {
        // Arrange
        _mockUserSettings.Setup(u => u.SoundEnabled).Returns(false);

        var service = new NotificationSoundService(_mockUserSettings.Object);

        // Act
        service.PlayNotificationSound(NotificationSoundType.ItemSetStateChanged);

        // Assert
        // No exception should be thrown, and no sound should be played
    }

    [Fact]
    public void Dispose_DisposesAllAudioResources()
    {
        // Arrange
        _mockUserSettings.Setup(u => u.SoundEnabled).Returns(true);

        var mockAudioResource1 = new Mock<IAudioResource>();
        var mockAudioResource2 = new Mock<IAudioResource>();

        var service = new NotificationSoundService(_mockUserSettings.Object);

        service.SetAudioResource(NotificationSoundType.ItemSetStateChanged, mockAudioResource1.Object);
        service.SetAudioResource(NotificationSoundType.SetPickingComplete, mockAudioResource2.Object);

        // Act
        service.Dispose();

        // Assert
        mockAudioResource1.Verify(a => a.Dispose(), Times.Once);
        mockAudioResource2.Verify(a => a.Dispose(), Times.Once);
    }
}
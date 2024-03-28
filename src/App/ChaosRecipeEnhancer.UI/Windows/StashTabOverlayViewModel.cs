using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class StashTabOverlayViewModel : ViewModelBase
{
    private readonly INotificationSoundService _notificationSoundService = Ioc.Default.GetRequiredService<INotificationSoundService>();
    private bool _isEditing;

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public void PlayItemSetStateChangedNotificationSound()
    {
        _notificationSoundService.PlayNotificationSound(NotificationSoundType.ItemSetStateChanged);
    }

    public void PlaySetPickingCompletedNotificationSound()
    {
        _notificationSoundService.PlayNotificationSound(NotificationSoundType.SetPickingComplete);
    }
}
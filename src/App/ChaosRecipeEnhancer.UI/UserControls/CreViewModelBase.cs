using ChaosRecipeEnhancer.UI.Properties;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChaosRecipeEnhancer.UI.UserControls;

/// <summary>
/// Base class for view models in the Chaos Recipe Enhancer application. Implements <see cref="ObservableObject"/>.
/// </summary>
public abstract class CreViewModelBase : ObservableObject
{
    /// <summary>
    /// Gets the global user settings.
    /// </summary>
    public Settings GlobalUserSettings { get; } = Settings.Default;
}
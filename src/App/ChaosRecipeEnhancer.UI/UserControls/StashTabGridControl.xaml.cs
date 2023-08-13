using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls;

/// <summary>
/// This Control represents the internal grid that will contain all the clickable cells within the Stash Tab.
/// There will only ever be one of these per app instance.
/// </summary>
internal partial class StashTabGridControl
{
    public static readonly DependencyProperty IsQuadProperty = DependencyProperty.Register(
        nameof(IsQuad),
        typeof(bool),
        typeof(StashTabGridControl),
        new PropertyMetadata(false)
    );

    public StashTabGridControl()
    {
        InitializeComponent();
    }

    public bool IsQuad
    {
        get => (bool)GetValue(IsQuadProperty);
        set => SetValue(IsQuadProperty, value);
    }
}
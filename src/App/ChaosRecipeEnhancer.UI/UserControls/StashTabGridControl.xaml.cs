using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls;

/// <summary>
///     Interaction logic for StashTabGridControl.xaml
/// </summary>
internal partial class StashTabGridControl
{
    public static readonly DependencyProperty IsQuadProperty = DependencyProperty.Register(
        nameof(IsQuad),
        typeof(bool),
        typeof(StashTabGridControl),
        new PropertyMetadata(false));

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
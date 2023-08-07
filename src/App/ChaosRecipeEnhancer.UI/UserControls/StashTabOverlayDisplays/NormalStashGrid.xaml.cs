using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Extensions;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;

/// <summary>
///     Interaction logic for NormalStashGrid.xaml
/// </summary>
internal partial class NormalStashGrid
{
    #region Constructors

    public NormalStashGrid()
    {
        InitializeComponent();
    }

    #endregion

    #region Methods

    public Button GetButtonFromCell(object cell)
    {
        for (var i = 0; i < Items.Count; i++)
            if (Items[i] == cell)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i);
                var t = ControlExtensions.GetChild<Button>(container);
                return t;
            }

        return null;
    }

    #endregion
}
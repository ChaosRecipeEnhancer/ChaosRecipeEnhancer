using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Extensions;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;

/// <summary>
///     Interaction logic for QuadStashGrid.xaml
/// </summary>
internal partial class QuadStashGrid
{
    #region Constructors

    public QuadStashGrid()
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
                //Trace.WriteLine(cell.XIndex + " x " + cell.YIndex + " y");

                var container = ItemContainerGenerator.ContainerFromIndex(i);
                var t = ControlExtensions.GetChild<Button>(container);
                return t;
            }

        return null;
    }

    #endregion
}
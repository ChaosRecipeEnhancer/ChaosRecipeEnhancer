using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Extensions;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public partial class QuadStashGrid
{
    public QuadStashGrid()
    {
        InitializeComponent();
    }

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
}
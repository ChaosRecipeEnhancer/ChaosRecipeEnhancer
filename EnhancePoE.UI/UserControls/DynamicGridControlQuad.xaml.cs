using System.Windows.Controls;
using EnhancePoE.UI.Model;

namespace EnhancePoE.UI.UserControls
{
    /// <summary>
    ///     Interaction logic for DynamicGridControlQuad.xaml
    /// </summary>
    public partial class DynamicGridControlQuad : ItemsControl
    {
        public DynamicGridControlQuad()
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
                    var t = Utility.GetChild<Button>(container);
                    return t;
                }

            return null;
        }
    }
}
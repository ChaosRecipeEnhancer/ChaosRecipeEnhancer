using System.Windows.Controls;
using EnhancePoE.UI.Model;

namespace EnhancePoE.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DynamicGridControl.xaml
    /// </summary>
    public partial class DynamicGridControl
    {
        #region Constructors

        public DynamicGridControl()
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
                    var t = Utility.GetChild<Button>(container);
                    return t;
                }

            return null;
        }

        #endregion
    }
}
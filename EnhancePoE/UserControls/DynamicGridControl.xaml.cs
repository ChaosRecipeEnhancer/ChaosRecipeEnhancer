using EnhancePoE.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnhancePoE.UserControls
{
    /// <summary>
    /// Interaction logic for DynamicGridControl.xaml
    /// </summary>
    public partial class DynamicGridControl : ItemsControl
    {
        public DynamicGridControl()
        {
            InitializeComponent();
        }

        public Button GetButtonFromCell(object cell)
        {

            for(int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] == cell)
                {
                    var container = this.ItemContainerGenerator.ContainerFromIndex(i);
                    Button t = Model.Utility.GetChild<Button>(container);
                    return t;
                }
            }
            return null;
        }
    }
}

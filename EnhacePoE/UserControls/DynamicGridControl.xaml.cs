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
                    //Trace.WriteLine(cell.XIndex + " x " + cell.YIndex + " y");

                    var container = this.ItemContainerGenerator.ContainerFromIndex(i);
                    Button t = Model.Utility.GetChild<Button>(container);
                    return t;
                }
            }
            return null;
        }

        //public void GetAllButtons()
        //{

        //    for (int i = 0; i < this.Items.Count; i++)
        //    {
        //        // Note this part is only working for controls where all items are loaded  
        //        // and generated. You can check that with ItemContainerGenerator.Status
        //        // If you are planning to use VirtualizingStackPanel make sure this 
        //        // part of code will be only executed on generated items.
        //        var container = this.ItemContainerGenerator.ContainerFromIndex(i);
        //        Button t = Model.Utility.GetChild<Button>(container);

        //        Trace.WriteLine(t.Content);
        //    }
        //}

        public void Test()
        {
            //ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
            //ContentPresenter cp = this.ItemContainerGenerator.ContainerFromItem(this) as ContentPresenter;
            //IEnumerable<Button> tb = Model.Utility.FindVisualChildren<Button>(cp);

            Trace.WriteLine(this.Items.Count, "test");


            //Trace.WriteLine(this.Items[0].GetType());

            //for (int i = 0; i < this.Items.Count; i++)
            //{
            //    // Note this part is only working for controls where all items are loaded  
            //    // and generated. You can check that with ItemContainerGenerator.Status
            //    // If you are planning to use VirtualizingStackPanel make sure this 
            //    // part of code will be only executed on generated items.
            //    var container = this.ItemContainerGenerator.ContainerFromIndex(i);
            //    Button t = Model.Utility.GetChild<Button>(container);

            //    Trace.WriteLine(t.Content);
            //}

            //foreach(Model.Cell c in this.Items)
            //{

            //    Trace.WriteLine(c.CellName);
            //}

            //ContentPresenter contentPresenter = Model.Utility.FindVisualChild<ContentPresenter>(this);

            //if(contentPresenter != null)
            //{


            //    //DataTemplate yourDataTemplate = contentPresenter.ContentTemplate;

            //    //Trace.WriteLine(yourDataTemplate);

            //    //MediaElement yourMediaElement = yourDataTemplate.FindName("vidList", contentPresenter) as MediaElement;
            //    //if (yourMediaElement != null)
            //    //{
            //    //    // Do something with yourMediaElement here
            //    //}
            //}
            //else
            //{
            //    Trace.WriteLine("contentpresenter null");
            //}

            //for (int i = 0; i < this.Items.Count; i++)
            //{

            //    ContentPresenter contentPresenter = (ContentPresenter)(this.ItemContainerGenerator.ContainerFromItem(this.Items[i]));
            //    //ContentPresenter myContentPresenter = Model.Utility.FindVisualChild<ContentPresenter>(this.Items[i]);

            //    if (contentPresenter != null)
            //    {
            //        Button btn = contentPresenter.ContentTemplate.LoadContent() as Button;
            //        Trace.WriteLine(btn.Content);
            //    }
            //    else
            //    {
            //        Trace.WriteLine("contentpresenter null");
            //    }
            //    //ContentPresenter contentPresenter = this.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;


            //    //Trace.WriteLine(contentPresenter.Content);
            //}
        }

    }
}

using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Model;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DynamicGridControlQuad.xaml
    /// </summary>
    public partial class DynamicGridControlQuad
    {
        #region Fields

        private ILogger _logger;

        #endregion

        #region Constructors

        public DynamicGridControlQuad()
        {
            _logger = Log.ForContext<HotkeyEditorControl>();
            _logger.Debug("Constructing DynamicGridControlQuad");

            InitializeComponent();

            _logger.Debug("DynamicGridControlQuad constructed successfully");
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
                    var t = Utility.GetChild<Button>(container);
                    return t;
                }

            return null;
        }

        #endregion
    }
}
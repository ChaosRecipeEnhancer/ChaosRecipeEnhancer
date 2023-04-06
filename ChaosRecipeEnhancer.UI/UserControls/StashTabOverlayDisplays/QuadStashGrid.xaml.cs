using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Extensions;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays
{
    /// <summary>
    ///     Interaction logic for QuadStashGrid.xaml
    /// </summary>
    public partial class QuadStashGrid
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public QuadStashGrid()
        {
            _logger = Log.ForContext<HotkeyEditorControl>();
            _logger.Debug("Constructing QuadStashGrid");

            InitializeComponent();

            _logger.Debug("QuadStashGrid constructed successfully");
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
}
using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Extensions;
using Serilog;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays
{
    /// <summary>
    ///     Interaction logic for NormalStashGrid.xaml
    /// </summary>
    public partial class NormalStashGrid
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public NormalStashGrid()
        {
            _logger = Log.ForContext<HotkeyEditorControl>();
            _logger.Debug("Constructing NormalStashGrid");

            InitializeComponent();

            _logger.Debug("NormalStashGrid constructed successfully");
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
}
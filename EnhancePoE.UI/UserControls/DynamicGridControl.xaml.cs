﻿using System.Windows.Controls;
using EnhancePoE.UI.Model;
using Serilog;

namespace EnhancePoE.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DynamicGridControl.xaml
    /// </summary>
    public partial class DynamicGridControl : IDynamicGridControl
    {
        #region Fields

        private ILogger _logger;

        #endregion
        
        #region Constructors

        public DynamicGridControl()
        {
            _logger = Log.ForContext<HotkeyEditorControl>();
            _logger.Debug("Constructing DynamicGridControl");

            InitializeComponent();
            
            _logger.Debug("DynamicGridControl constructed successfully");
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
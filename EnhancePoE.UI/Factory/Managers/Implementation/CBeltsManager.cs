using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
{
    internal class CBeltsManager : ABaseItemClassManager
    {
        #region Constructors

        public CBeltsManager()
        {
            ClassName = "Belts";
            ClassFilterName = "\"Belts\"";
            ClassColor = Settings.Default.ColorBelt;
            AlwaysActive = Settings.Default.BeltsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.BeltActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
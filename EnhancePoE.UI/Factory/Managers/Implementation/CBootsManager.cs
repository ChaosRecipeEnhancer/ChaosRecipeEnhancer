using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
{
    internal class CBootsManager : CBaseItemClassManager
    {
        #region Constructors

        public CBootsManager()
        {
            ClassName = "Boots";
            ClassFilterName = "\"Boots\"";
            ClassColor = Settings.Default.ColorBoots;
            AlwaysActive = Settings.Default.BootsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.BootsActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
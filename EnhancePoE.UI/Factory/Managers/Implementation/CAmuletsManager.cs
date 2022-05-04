using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
{
    internal class CAmuletsManager : CBaseItemClassManager
    {
        #region Constructors

        public CAmuletsManager()
        {
            ClassName = "Amulets";
            ClassFilterName = "\"Amulets\"";
            ClassColor = Settings.Default.ColorAmulet;
            AlwaysActive = Settings.Default.AmuletsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.AmuletActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
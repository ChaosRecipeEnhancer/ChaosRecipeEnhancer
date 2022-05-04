using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
{
    internal class CRingsManager : CBaseItemClassManager
    {
        #region Constructors

        public CRingsManager()
        {
            ClassName = "Rings";
            ClassFilterName = "\"Rings\"";
            ClassColor = Settings.Default.ColorRing;
            AlwaysActive = Settings.Default.RingsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.RingActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
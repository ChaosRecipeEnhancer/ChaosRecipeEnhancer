using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Factory.Managers.Implementation
{
    internal class CRingsManager : ABaseItemClassManager
    {
        #region Constructors

        public CRingsManager()
        {
            ClassName = "Rings";
            ClassFilterName = "\"Rings\"";
            ClassColor = Settings.Default.LootFilterRingColor;
            AlwaysActive = Settings.Default.LootFilterRingsAlwaysActive;
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
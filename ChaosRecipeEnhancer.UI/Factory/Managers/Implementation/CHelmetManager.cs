using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Factory.Managers.Implementation
{
    internal class CHelmetManager : ABaseItemClassManager
    {
        #region Constructors

        public CHelmetManager()
        {
            ClassName = "Helmets";
            ClassFilterName = "\"Helmets\"";
            ClassColor = Settings.Default.LootFilterHelmetColor;
            AlwaysActive = Settings.Default.LootFilterHelmetsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.HelmetActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
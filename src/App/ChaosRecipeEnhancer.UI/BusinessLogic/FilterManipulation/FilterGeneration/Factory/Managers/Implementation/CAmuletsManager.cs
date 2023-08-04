using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation
{
    internal class CAmuletsManager : ABaseItemClassManager
    {
        #region Constructors

        public CAmuletsManager()
        {
            ClassName = "Amulets";
            ClassFilterName = "\"Amulets\"";
            ClassColor = Settings.Default.LootFilterAmuletColor;
            AlwaysActive = Settings.Default.LootFilterAmuletsAlwaysActive;
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
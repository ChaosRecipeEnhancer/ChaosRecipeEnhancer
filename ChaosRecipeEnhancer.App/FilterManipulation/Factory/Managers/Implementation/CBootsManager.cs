using ChaosRecipeEnhancer.App.Models;
using ChaosRecipeEnhancer.App.Models.Settings;

namespace ChaosRecipeEnhancer.App.FilterManipulation.Factory.Managers.Implementation
{
    internal class CBootsManager : ABaseItemClassManager
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
using ChaosRecipeEnhancer.App.Models;
using ChaosRecipeEnhancer.App.Models.Settings;

namespace ChaosRecipeEnhancer.App.FilterManipulation.Factory.Managers.Implementation
{
    internal class CHelmetManager : ABaseItemClassManager
    {
        #region Constructors

        public CHelmetManager()
        {
            ClassName = "Helmets";
            ClassFilterName = "\"Helmets\"";
            ClassColor = Settings.Default.ColorHelmet;
            AlwaysActive = Settings.Default.HelmetsAlwaysActive;
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
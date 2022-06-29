using ChaosRecipeEnhancer.App.Models;
using ChaosRecipeEnhancer.App.Models.Settings;

namespace ChaosRecipeEnhancer.App.FilterManipulation.Factory.Managers.Implementation
{
    internal class CRingsManager : ABaseItemClassManager
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
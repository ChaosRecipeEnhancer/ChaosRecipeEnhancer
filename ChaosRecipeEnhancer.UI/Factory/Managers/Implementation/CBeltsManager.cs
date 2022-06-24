using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Factory.Managers.Implementation
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
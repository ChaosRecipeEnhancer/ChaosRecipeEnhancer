using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Factory.Managers.Implementation
{
    internal class CBodyArmoursManager : ABaseItemClassManager
    {
        #region Constructors

        public CBodyArmoursManager()
        {
            ClassName = "BodyArmours";
            ClassFilterName = "\"Body Armours\"";
            ClassColor = Settings.Default.ColorChest;
            AlwaysActive = Settings.Default.ChestsAlwaysActive;
        }

        #endregion

        #region Methods

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.ChestActive = newValue;
            return activeItems;
        }

        #endregion
    }
}
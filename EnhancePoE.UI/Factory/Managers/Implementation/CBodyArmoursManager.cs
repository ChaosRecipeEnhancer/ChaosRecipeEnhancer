using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
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
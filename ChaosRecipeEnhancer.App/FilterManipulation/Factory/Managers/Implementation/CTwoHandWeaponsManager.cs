using System.Collections.Generic;
using ChaosRecipeEnhancer.App.Models;
using ChaosRecipeEnhancer.App.Models.Settings;
using ChaosRecipeEnhancer.DataModels.Constants;

namespace ChaosRecipeEnhancer.App.FilterManipulation.Factory.Managers.Implementation
{
    internal class CTwoHandWeaponsManager : ABaseItemClassManager
    {
        #region Constructors

        public CTwoHandWeaponsManager()
        {
            ClassName = "TwoHandWeapons";
            ClassFilterName = "\"Two Hand\"";
            ClassColor = Settings.Default.ColorWeapon;
            AlwaysActive = Settings.Default.WeaponsAlwaysActive;
        }

        #endregion

        #region Methods

        public override string SetBaseType()
        {
            var baseType = "Class ";
            baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
            baseType += FilterConstants.NewLine + FilterConstants.Tab + "Width <= 2" + FilterConstants.NewLine +
                        FilterConstants.Tab + "Height <= 3";
            return baseType;
        }

        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.WeaponActive = newValue;
            return activeItems;
        }

        public override bool CheckIfMissing(HashSet<string> missingItemClasses)
        {
            // bad, dont like, no good ideas for now tho
            return missingItemClasses.Contains(ClassName) || missingItemClasses.Contains("OneHandWeapons");
        }

        #endregion
    }
}
using System.Collections.Generic;
using EnhancePoE.UI.Const;
using EnhancePoE.UI.Model;
using EnhancePoE.UI.Properties;

namespace EnhancePoE.UI.Factory.Managers.Implementation
{
    internal class COneHandWeaponsManager : ABaseItemClassManager
    {
        #region Constructors

        public COneHandWeaponsManager()
        {
            ClassName = "OneHandWeapons";
            ClassFilterName = "\"One Hand\"";
            ClassColor = Settings.Default.ColorWeapon;
            AlwaysActive = Settings.Default.WeaponsAlwaysActive;
        }

        #endregion

        #region Methods

        public override string SetBaseType()
        {
            // Seems like we omit claws by design as they don't fit the sizing rules (2 x 2 = 4 stash units)
            var baseType = "Class ";
            baseType +=
                "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
            baseType += CConst.newLine + CConst.tab + "Width <= 1" + CConst.newLine + CConst.tab + "Height <= 3";
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
            return missingItemClasses.Contains(ClassName) || missingItemClasses.Contains("TwoHandWeapons");
        }

        #endregion
    }
}
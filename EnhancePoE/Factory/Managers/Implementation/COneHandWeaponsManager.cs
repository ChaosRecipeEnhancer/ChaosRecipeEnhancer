using EnhancePoE.Const;
using EnhancePoE.Properties;

namespace EnhancePoE.Visitors
{
    internal class COneHandWeaponsManager : CBaseItemClassManager
    {
        public COneHandWeaponsManager()
        {
            ClassName = "OneHandWeapons";
            ClassFilterName = "\"One Hand\"";
            ClassColor = Settings.Default.ColorWeapon;
            AlwaysActive = Settings.Default.WeaponsAlwaysActive;            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override string SetBaseType()
        {
            // Seems like we omit claws by design as they don't fit the rule of 
            var baseType = "Class ";
            baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
            baseType += CConst.newLine + CConst.tab + "Width <= 1" + CConst.newLine + CConst.tab + "Height <= 3";
            return baseType;
        }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.WeaponActive = newValue;
            return activeItems;
        }
    }
}
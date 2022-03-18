using EnhancePoE.Const;
using EnhancePoE.Properties;

namespace EnhancePoE.Visitors
{
    internal class CTwoHandWeaponsManager : CBaseItemClassManager
    {
        public CTwoHandWeaponsManager()
        {
            ClassName = "TwoHandWeapons";
            ClassFilterName = "\"Two Hand\"";
            ClassColor = Settings.Default.ColorWeapon;
            AlwaysActive = Settings.Default.WeaponsAlwaysActive;            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override string SetBaseType()
        {
            // TODO: There have been issues reported with users not being able to fit 2 sets in their due to the size of some 2-handers, but looks like we have the WxH rules set here...
            var baseType = "Class ";
            baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
            baseType += CConst.newLine + CConst.tab + "Width <= 2" + CConst.newLine + CConst.tab + "Height <= 3";
            baseType += CConst.newLine + CConst.tab + "Sockets <= 5" + CConst.newLine + CConst.tab + "LinkedSockets <= 5";
            return baseType;
        }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.WeaponActive = newValue;
            return activeItems;
        }
    }
}
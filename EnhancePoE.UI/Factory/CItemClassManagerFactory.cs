using System;
using EnhancePoE.UI.Enums;
using EnhancePoE.UI.Factory.Managers;
using EnhancePoE.UI.Factory.Managers.Implementation;

namespace EnhancePoE.UI.Factory
{
    public class CItemClassManagerFactory
    {
        #region Methods

        public ABaseItemClassManager GetItemClassManager(EnumItemClass itemClass)
        {
            switch (itemClass)
            {
                case EnumItemClass.Helmets:
                    return new CHelmetManager();
                case EnumItemClass.BodyArmours:
                    return new CBodyArmoursManager();
                case EnumItemClass.Gloves:
                    return new CGlovesManager();
                case EnumItemClass.Boots:
                    return new CBootsManager();
                case EnumItemClass.Rings:
                    return new CRingsManager();
                case EnumItemClass.Amulets:
                    return new CAmuletsManager();
                case EnumItemClass.Belts:
                    return new CBeltsManager();
                case EnumItemClass.OneHandWeapons:
                    return new COneHandWeaponsManager();
                case EnumItemClass.TwoHandWeapons:
                    return new CTwoHandWeaponsManager();
                default:
                    throw new Exception("Wrong item class.");
            }
        }

        #endregion
    }
}
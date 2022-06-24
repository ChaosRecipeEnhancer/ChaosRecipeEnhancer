using System;
using ChaosRecipeEnhancer.UI.Enums;
using ChaosRecipeEnhancer.UI.Factory.Managers;
using ChaosRecipeEnhancer.UI.Factory.Managers.Implementation;

namespace ChaosRecipeEnhancer.UI.Factory
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
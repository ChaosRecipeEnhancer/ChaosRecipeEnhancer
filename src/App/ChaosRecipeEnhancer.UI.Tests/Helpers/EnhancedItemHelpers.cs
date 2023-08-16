using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Models;

namespace ChaosRecipeEnhancer.UI.Tests.Helpers;

public static class EnhancedItemHelpers
{
    public static EnhancedItem GetOneHandedItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.WandUrl);
    }

    public static EnhancedItem GetShieldItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.ShieldUrl);
    }

    public static EnhancedItem GetTwoHandedItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.StaffUrl);
    }

    public static EnhancedItem GetAmuletItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.AmuletUrl);
    }

    public static EnhancedItem GetBodyArmourModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.BodyArmourUrl);
    }

    public static EnhancedItem GetItemWithXYAndTabIndex(string url, int x, int y, int tabIndex = 0)
    {
        var itemModel = new EnhancedItem(1, 1, false, 74, 0, x, y, new BaseItemInfluences(), url);
        itemModel.StashTabIndex = tabIndex;
        itemModel.GetItemClass();
        return itemModel;
    }

    private static EnhancedItem GetWithUrlAndDerivedItemClassSet(string url)
    {
        var itemModel = new EnhancedItem(1, 1, false, 74, 0, 25, 25, new BaseItemInfluences(), url);
        itemModel.GetItemClass();
        return itemModel;
    }
}
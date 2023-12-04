using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

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
        var itemModel = new EnhancedItem(1, 1, false, 74, 0, x, y, new BaseItemInfluences(), url)
        {
            StashTabIndex = tabIndex
        };
        return itemModel;
    }

    private static EnhancedItem GetWithUrlAndDerivedItemClassSet(string url)
    {
        var itemModel = new EnhancedItem(1, 1, false, 74, 0, 25, 25, new BaseItemInfluences(), url);
        return itemModel;
    }
}
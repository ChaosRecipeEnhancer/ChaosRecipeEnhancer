using ChaosRecipeEnhancer.UI.BusinessLogic.Items;

namespace ChaosRecipeEnhancer.UI.Tests.Helpers;

public static class EnhancedItemHelpers
{
    public static EnhancedItemModel GetOneHandedItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.WandUrl);
    }

    public static EnhancedItemModel GetShieldItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.ShieldUrl);
    }
    
    public static EnhancedItemModel GetTwoHandedItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.StaffUrl);
    }
    
    
    public static EnhancedItemModel GetAmuletItemModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.AmuletUrl);
    }

    public static EnhancedItemModel GetBodyArmourModel()
    {
        return GetWithUrlAndDerivedItemClassSet(ItemIconConstants.BodyArmourUrl);
    }
    
    public static EnhancedItemModel GetItemWithXYAndTabIndex(string url, int x, int y, int tabIndex = 0)
    {
        var itemModel = new EnhancedItemModel(1, 1, false, 74, 0, x, y, new ItemInfluencesModel(), url);
        itemModel.StashTabIndex = tabIndex;
        itemModel.GetItemClass();
        return itemModel;
    }

    private static EnhancedItemModel GetWithUrlAndDerivedItemClassSet(string url)
    {
        var itemModel = new EnhancedItemModel(1, 1, false, 74, 0, 25, 25, new ItemInfluencesModel(), url);
        itemModel.GetItemClass();
        return itemModel;
    }
}
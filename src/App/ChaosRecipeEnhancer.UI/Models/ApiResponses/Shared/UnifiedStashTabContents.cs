using ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.SessionIdEndpointResponses;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;

public class UnifiedStashTabContents
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Index { get; set; }
    public string Type { get; set; }
    public bool IsQuadLayout { get; set; }
    public List<BaseItem> Items { get; set; }
}

public static class StashTabContentsExtensions
{
    public static UnifiedStashTabContents ToUnifiedContents(this GetStashResponse response)
    {
        return new UnifiedStashTabContents
        {
            Id = response.Stash.Id,
            Name = response.Stash.Name,
            Index = response.Stash.Index,
            Type = response.Stash.Type,
            IsQuadLayout = response.Stash.Type == "QuadStash",
            Items = response.Stash.Items
        };
    }

    public static UnifiedStashTabContents ToUnifiedContents(this BaseStashTabContents response, string id, string name, int index, string type)
    {
        return new UnifiedStashTabContents
        {
            Id = id,
            Name = name,
            Index = index,
            Type = type,
            IsQuadLayout = response.IsQuadLayout,
            Items = response.Items
        };
    }
}
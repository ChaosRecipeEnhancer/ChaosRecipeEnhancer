using ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.SessionIdEndpointResponses;
using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;

public class UnifiedStashTabMetadata
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Index { get; set; }
    public List<UnifiedStashTabMetadata> Children { get; set; }

    public override string ToString()
    {
        // If the Id is not just the Index as a string, include it in the output
        if (Id != Index.ToString())
        {
            return $"[Index {Index}] {Name} (Id: {Id})";
        }
        else
        {
            return $"[Index {Index}] {Name}";
        }
    }
}

public static class StashTabMetadataExtensions
{
    public static List<UnifiedStashTabMetadata> ToUnifiedMetadata(this ListStashesResponse response)
    {
        return response.StashTabs.Select(tab => new UnifiedStashTabMetadata
        {
            Id = tab.Id,
            Name = tab.Name,
            Type = tab.Type,
            Index = tab.Index,
            Children = tab.Children?.Select(child => new UnifiedStashTabMetadata
            {
                Id = child.Id,
                Name = child.Name,
                Type = child.Type,
                Index = child.Index
            }).ToList()
        }).ToList();
    }

    public static List<UnifiedStashTabMetadata> ToUnifiedMetadata(this BaseStashTabMetadataList response)
    {
        return response.StashTabs.Select(tab => new UnifiedStashTabMetadata
        {
            Id = tab.Index.ToString(), // Since there's no Id in this version, use Index as a string
            Name = tab.Name,
            Type = tab.Type,
            Index = tab.Index
        }).ToList();
    }
}
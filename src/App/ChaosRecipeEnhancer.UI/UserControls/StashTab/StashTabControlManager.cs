using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

/// <summary>
///     CRE-specific model of a stash tab within our app (NOT the GGG ToggleStashTabOverlay object model).
///     Represents various UI elements and additional metadata used in our app that is derived from the original
///     stash tab JSON object requested from GGG's API.
/// </summary>
public static class StashTabControlManager
{
    public static List<StashTabControl> StashTabControls { get; set; } = [];
    public static List<int> StashTabIndices { get; private set; }

    public static void GetStashTabIndicesFromSettings(List<UnifiedStashTabMetadata> stashData)
    {
        if (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.TabsByNamePrefix)
        {
            GetStashTabIndicesFromSettingsForQueryByNamePrefix(stashData);
        }
        else if (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.TabsById && !string.IsNullOrWhiteSpace(Settings.Default.StashTabIdentifiers))
        {
            GetStashTabIndicesFromSettingsForQueryById(stashData);
        }
    }

    private static void GetStashTabIndicesFromSettingsForQueryByNamePrefix(List<UnifiedStashTabMetadata> stashData)
    {
        List<int> selectedTabIndices = [];

        var tabNamePrefix = Settings.Default.StashTabPrefix;

        // iterate through the stashData and find the tabs that start with the name prefix
        foreach (var tab in stashData)
        {
            if (tab.Name.StartsWith(tabNamePrefix))
            {
                selectedTabIndices.Add(tab.Index);
            }
        }

        // update the StashTabIndices property with the selected tab indices
        StashTabIndices = selectedTabIndices;

        // ensure that the selected tab indices are unique
        foreach (var s in selectedTabIndices)
        {
            if (!selectedTabIndices.Contains(s)) selectedTabIndices.Add(s);
        }
    }

    private static void GetStashTabIndicesFromSettingsForQueryById(List<UnifiedStashTabMetadata> stashData)
    {
        List<int> selectedTabIndices = [];

        // convert the comma-separated string of stash tab IDs to a list of integers
        var selectedStashIds = Settings.Default.StashTabIdentifiers.Split(',').ToList();

        // iterate through the list of stash tab IDs and find the corresponding index in the stashData list
        foreach (var id in selectedStashIds)
        {
            var tab = stashData.FirstOrDefault(x => x.Id == id);
            if (tab != null)
            {
                selectedTabIndices.Add(tab.Index);
            }
        }

        // update the StashTabIndices property with the selected tab indices
        StashTabIndices = selectedTabIndices;

        // ensure that the selected tab indices are unique
        foreach (var s in selectedTabIndices)
        {
            if (!selectedTabIndices.Contains(s)) selectedTabIndices.Add(s);
        }
    }
}
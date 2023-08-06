using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.Items;

public class EnhancedItemListModel : ItemListModel
{
	[JsonPropertyName("items")]
	public new List<EnhancedItemModel> Items
	{
		get; set;
	}
}
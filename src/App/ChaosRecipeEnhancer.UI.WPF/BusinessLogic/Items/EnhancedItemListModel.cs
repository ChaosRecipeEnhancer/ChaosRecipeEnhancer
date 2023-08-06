using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;

public class EnhancedItemListModel : ItemListModel
{
	[JsonPropertyName("items")]
	public new List<EnhancedItemModel> Items
	{
		get; set;
	}
}
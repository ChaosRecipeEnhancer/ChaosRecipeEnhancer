using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosRecipeEnhancer.UI.Api.Data;

// names are from api
public class Item
{
	// poe json props
	public int w
	{
		get; set;
	}
	public int h
	{
		get; set;
	}
	public string typeLine
	{
		get; set;
	}
	public string baseType
	{
		get; set;
	}
	public string name
	{
		get; set;
	}
	public bool identified
	{
		get; set;
	}
	public int ilvl
	{
		get; set;
	}
	public int frameType
	{
		get; set;
	}
	public int x
	{
		get; set;
	}
	public int y
	{
		get; set;
	}
	public string icon
	{
		get; set;
	}

	// own prop
	public string ItemType
	{
		get; set;
	}
	public int StashTabIndex
	{
		get; set;
	}

	public void GetItemClass()
	{
		//https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9IZWxtZXRzL0hlbG1ldFN0ckRleDciLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/0884b27765/HelmetStrDex7.png
		var urlParts = icon.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string encodedPart = urlParts[4];
		while (encodedPart.Length % 4 != 0)
		{
			encodedPart += "=";
		}
		string decodedItemData = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));
		var iconParts = decodedItemData.Split('/');
		string itemClass = iconParts[1];
		switch (itemClass)
		{
			case "Armours" when iconParts[2] == "Shields":
				itemClass = "OneHandWeapons";
				break;
			case "Weapons":
			case "Armours":
				itemClass = iconParts[2];
				break;
		}
		ItemType = itemClass;
	}
}

public class ItemList
{
	public List<Item> items
	{
		get; set;
	}
	public bool quadLayout
	{
		get; set;
	}
}

using System.Collections.Generic;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;

public abstract class ABaseItemClassManager
{
	#region Properties

	public string ClassName
	{
		get; set;
	}
	public string ClassFilterName
	{
		get; set;
	}
	public string ClassColor
	{
		get; set;
	}
	public bool AlwaysActive
	{
		get; set;
	}

	#endregion

	#region Methods

	public virtual string SetBaseType()
	{
		var baseType = "Class " + ClassFilterName;
		return baseType;
	}

	public virtual bool CheckIfMissing(HashSet<string> missingItemClasses)
	{
		return missingItemClasses.Contains(ClassName);
	}

	public abstract ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue);

	#endregion
}
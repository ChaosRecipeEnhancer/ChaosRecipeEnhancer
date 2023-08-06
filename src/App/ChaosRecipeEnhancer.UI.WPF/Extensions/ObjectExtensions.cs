namespace ChaosRecipeEnhancer.UI.WPF.Extensions;

public static class ObjectExtensions
{
	public static object GetPropertyValue(object src, string propertyName)
	{
		return src.GetType().GetProperty(propertyName)?.GetValue(src);
	}
}
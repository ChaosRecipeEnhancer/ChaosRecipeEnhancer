using System.Windows;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Extensions;

internal abstract class ControlExtensions
{
	public static T GetChild<T>(DependencyObject obj) where T : DependencyObject
	{
		if (obj == null) return null;

		DependencyObject child = null;
		for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
		{
			child = VisualTreeHelper.GetChild(obj, i);

			if (child.GetType() == typeof(T)) break;

			child = GetChild<T>(child);

			if (child != null && child.GetType() == typeof(T)) break;
		}

		return child as T;
	}
}
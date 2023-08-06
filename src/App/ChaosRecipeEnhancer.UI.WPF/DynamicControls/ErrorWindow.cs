using System.Windows;

namespace ChaosRecipeEnhancer.UI.WPF.DynamicControls;

public static class ErrorWindow
{
	public static void Spawn(string content, string title)
	{
		MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
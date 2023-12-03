using System.Windows;

namespace ChaosRecipeEnhancer.UI.Windows;

public static class ErrorWindow
{
    public static void Spawn(string content, string title)
    {
        MessageBox.Show(
            $"{content}\n\nYou can copy the contents of this window by" +
            $"hitting CTRL + C while this window is focused. This will make pasting" +
            $"error messages into Discord, GitHub issues, etc., much easier.",
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
}
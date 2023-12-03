using System.Windows;

namespace ChaosRecipeEnhancer.UI.Windows;

public static class ErrorWindow
{
    public static void Spawn(string content, string title, bool showCopyHint = false)
    {
        // this is a hint i want to include for long exception messages
        // makes it easier to do some interactive troubleshooting with users
        var hint = showCopyHint
            ? "You can copy the contents of this window by hitting CTRL + C while " +
              "this window is focused. This will make pasting error messages into " +
              "Discord, GitHub issues, etc., much easier."
            : string.Empty;

        MessageBox.Show(
            hint + content,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
}
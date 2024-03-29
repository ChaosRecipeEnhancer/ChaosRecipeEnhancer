using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.Services;

public static class GlobalErrorHandler
{
    public static void Spawn(string content, string title, string preamble = null)
    {
        var errorDialog = new ErrorWindow(title, content, preamble);
        errorDialog.ShowDialog();
    }
}

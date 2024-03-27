using System.Windows;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Utilities;

public static class VisualUtilities
{
    /// <summary>
    /// Converts the specified visual element's location to screen coordinates.
    /// </summary>
    /// <param name="visual">The visual element whose coordinates are to be converted.</param>
    /// <returns>The point representing the element's location in screen coordinates. Returns (0,0) if the element is null or not connected.</returns>
    public static Point GetScreenCoordinates(Visual visual)
    {
        // Check if the visual is null or not connected to the presentation source.
        if (visual == null || !IsVisualConnected(visual))
        {
            return new Point(0, 0);
        }

        // Convert the visual element's location to screen coordinates.
        var screenCoordinates = visual.PointToScreen(new Point(0, 0));

        return screenCoordinates;
    }

    /// <summary>
    /// Determines whether a given visual element is connected to a presentation source that is,
    /// it is part of a visual tree that leads to a rendered window.
    /// </summary>
    /// <param name="visual">The visual element to check.</param>
    /// <returns>
    /// <c>true</c> if the visual element is connected to a rendered window;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// A visual element is considered connected if it is part of a visual tree that leads to a window currently being displayed to the user.
    /// This method is useful for verifying the render status of elements before performing operations that require the element to be visible.
    /// </remarks>
    public static bool IsVisualConnected(Visual visual)
    {
        // Attempts to retrieve the PresentationSource associated with the visual element.
        // The PresentationSource represents the connection of the visual element to the windowing system.
        // If the visual element is part of a rendered visual tree, it will have a non-null PresentationSource.
        var isConnected = PresentationSource.FromVisual(visual) != null;

        // Returns true if the visual element is connected to a rendered window, false otherwise.
        return isConnected;
    }
}

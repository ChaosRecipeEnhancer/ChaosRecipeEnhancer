using System.Windows;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Contains utility methods for working with visual elements in a WPF application.
/// </summary>
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

    /// <summary>
    /// Searches recursively for a child of a given <see cref="DependencyObject"/> that is of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This method performs a depth-first search through the visual tree of the provided <see cref="DependencyObject"/>,
    /// looking for a child element of the type specified by <typeparamref name="T"/>. If no such child is found, null is returned.
    ///
    /// Note that this method will return the first encountered child of the specified type in the visual tree. If the visual tree
    /// contains multiple children of the type <typeparamref name="T"/>, consider using a different approach if you need to retrieve
    /// a specific instance other than the first one found.
    /// </remarks>
    /// <typeparam name="T">The type of the visual child to search for. This type must inherit from <see cref="DependencyObject"/>.</typeparam>
    /// <param name="obj">The root <see cref="DependencyObject"/> from which to start the search.</param>
    /// <returns>The first child of type <typeparamref name="T"/> found within the visual tree rooted at <paramref name="obj"/>, or null if no such child exists.</returns>
    /// <example>
    /// How to use <see cref="GetChild{T}"/> to find a <see cref="TextBox"/> within a parent control:
    /// <code>
    /// var parentControl = someControl;
    /// var textBoxChild = VisualUtilities.GetChild&lt;TextBox&gt;(parentControl);
    /// if (textBoxChild != null)
    /// {
    ///     // Do something with textBoxChild
    /// }
    /// </code>
    /// </example>
    public static T GetChild<T>(DependencyObject obj) where T : DependencyObject
    {
        // Check if the provided DependencyObject is null, return null to indicate no child can be found.
        if (obj is null)
        {
            return null;
        }

        // Iterate through all children of the provided DependencyObject.
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            // Attempt to cast the child to the specified type T. If successful, return the cast child.
            // If the cast is unsuccessful, recursively search the child's descendants.
            var result = child as T ?? GetChild<T>(child);
            if (result is not null)
            {
                // Return the first child (or descendant) of the specified type T found.
                return result;
            }
        }

        // If no child of the specified type T is found in the visual tree, return null.
        return null;
    }
}

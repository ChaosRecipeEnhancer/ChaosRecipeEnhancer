using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Utilities;

internal static class ControlHelpers
{
    public static bool HitTest(FrameworkElement frameworkElement, Point location)
    {
        if (frameworkElement is null)
        {
            return false;
        }

        var pt = frameworkElement.PointToScreen(new Point(0, 0));
        pt.X--;
        pt.Y--;

        double btnX = Math.Ceiling(pt.X + frameworkElement.ActualWidth) + 2;
        double btnY = Math.Ceiling(pt.Y + frameworkElement.ActualHeight) + 2;

        return location.X >= pt.X && location.Y >= pt.Y && location.X <= btnX && location.Y <= btnY;
    }

    public static T GetContainerForDataObject<T>(ItemsControl itemsControl, object data) where T : DependencyObject
    {
        for (int i = 0; i < itemsControl.Items.Count; i++)
        {
            if (itemsControl.Items[i] == data)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                return GetChild<T>(container);
            }
        }

        return null;
    }

    private static T GetChild<T>(DependencyObject obj) where T : DependencyObject
    {
        if (obj is null)
        {
            return null;
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            var result = child as T ?? GetChild<T>(child);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }
}

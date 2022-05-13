using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace EnhancePoE.UI.Model
{
    internal class Utility
    {
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) return (T)child;

                    var childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }

            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject) where T : DependencyObject
        {
            if (rootObject != null)
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
                {
                    var child = VisualTreeHelper.GetChild(rootObject, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
        }

        public static T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                DependencyObject child = null;
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child.GetType() == typeof(T))
                    {
                        break;
                    }

                    if (child != null)
                    {
                        child = GetChild<T>(child);
                        if (child != null && child.GetType() == typeof(T)) break;
                    }
                }

                return child as T;
            }

            return null;
        }

        public static object GetPropertyValue(object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName).GetValue(src);
        }
    }
}
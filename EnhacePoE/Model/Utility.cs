using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EnhancePoE.Model
{
    class Utility
    {
        //public childItem FindVisualChild<childItem>(DependencyObject obj)
        //    where childItem : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is childItem)
        //        {
        //            return (childItem)child;
        //        }
        //        else
        //        {
        //            childItem childOfChild = FindVisualChild<childItem>(child);
        //            if (childOfChild != null)
        //                return childOfChild;
        //        }
        //    }
        //    return null;
        //}

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject) where T : DependencyObject
        {
            if (rootObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(rootObject, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public static T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if(obj != null)
            {
                DependencyObject child = null;
                for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child.GetType() == typeof(T))
                    {
                        break;
                    }
                    else if (child != null)
                    {
                        child = GetChild<T>(child);
                        if (child != null && child.GetType() == typeof(T))
                        {
                            break;
                        }
                    }
                }
                return child as T;
            }
            return null;
        }
    }
}

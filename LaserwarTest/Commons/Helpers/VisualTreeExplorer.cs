﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LaserwarTest.Commons.Helpers
{
    public static class VisualTreeExplorer
    {
        public static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent is T target)
                return target;

            if (parent == null) return null;

            return FindParent<T>(parent);
        }
    }
}

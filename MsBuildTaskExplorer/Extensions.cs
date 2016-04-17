using System.Windows;
using System.Windows.Media;

namespace MsBuildTaskExplorer
{
    internal static class Extensions
    {
        public static T FindVisualParent<T>(this DependencyObject child, string parentName = null) where T : DependencyObject
        {

            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
            {
                return null;
            }
            else
            {
                var parentElement = parent as FrameworkElement;
                if (parent is T && (parentName == null || (parentElement != null && parentElement.Name == parentName)))
                {
                    return parent as T;
                }
                else
                {
                    return parent.FindVisualParent<T>(parentName);
                }
            }
        }
    }
}

using System.Windows;
using System.Windows.Media;

namespace MsBuildTaskExplorer
{
    public static class WindowUtils
    {
        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            while (element != null && !(element is T))
                element = VisualTreeHelper.GetParent(element);

            return (T)element;
        }

        public static T GetVisualChild<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element is T t) return t;
            var count = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < count; i++)
            {
                var item = GetVisualChild<T>(VisualTreeHelper.GetChild(element, i));
                if (item != null)
                    return item;
            }
            return null;
        }

        public static void ShowChildWindow(this Window owner, Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Height = owner.Height;
            window.Width = owner.Width;
            window.Left = owner.Left + 15;
            window.Top = owner.Top + 15;
            window.Owner = owner;
            window.ShowDialog();
        }

        public static void ShowCenter(this Window owner, Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = owner;
            window.ShowDialog();
        }
    }
}

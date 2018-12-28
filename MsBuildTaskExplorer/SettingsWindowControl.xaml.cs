using System.Windows;
using System.Windows.Controls;
using MsBuildTaskExplorer.AppSettings;

namespace MsBuildTaskExplorer
{
    public partial class SettingsWindowControl : UserControl
    {
        public SettingsWindowControl()
        {
            InitializeComponent();
            Loaded += (sender, args) => SupportedExtTb.Text = Settings.Instance.SupportedFileExtensions;
        }

        private void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            var taskExplorerWindow = this.GetVisualParent<TaskExplorerWindowControl>();
            Settings.Instance.SupportedFileExtensions = SupportedExtTb.Text;
            taskExplorerWindow.SettingsControl.Visibility = Visibility.Collapsed;
            taskExplorerWindow.MainControl.Visibility = Visibility.Visible;
            taskExplorerWindow.UpdateTaskList();
        }
    }
}

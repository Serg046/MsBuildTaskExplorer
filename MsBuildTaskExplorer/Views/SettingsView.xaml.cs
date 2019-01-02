using System.Windows;
using System.Windows.Controls;

namespace MsBuildTaskExplorer.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            Loaded += (sender, args) => SupportedExtTb.Text = Settings.Instance.SupportedFileExtensions;
        }

        private void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            var taskExplorerView = this.GetVisualParent<TaskExplorerView>();
            Settings.Instance.SupportedFileExtensions = SupportedExtTb.Text;
            taskExplorerView.SettingsControl.Visibility = Visibility.Collapsed;
            taskExplorerView.MainControl.Visibility = Visibility.Visible;
            taskExplorerView.UpdateTaskList();
        }
    }
}

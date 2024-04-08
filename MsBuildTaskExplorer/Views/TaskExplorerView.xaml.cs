using System.Windows.Controls;
using MsBuildTaskExplorer.ViewModels;

namespace MsBuildTaskExplorer.Views
{
    
    public partial class TaskExplorerView : UserControl
    {
        public TaskExplorerView()
        {
            InitializeComponent();
            DataContext = ViewModelFactory.Create<TaskExplorerViewModel>();
        }

        public static TaskExplorerView Instance { get; } = new TaskExplorerView();
    }
}
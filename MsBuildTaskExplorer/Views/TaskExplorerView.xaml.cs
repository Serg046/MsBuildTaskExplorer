using System.Windows.Controls;
using MsBuildTaskExplorer.ViewModels;

namespace MsBuildTaskExplorer.Views
{
    
    public partial class TaskExplorerView : UserControl
    {
        public TaskExplorerView()
        {
            this.InitializeComponent();
            DataContext = ViewModelFactory.Create<TaskExplorerViewModel>();
        }
    }
}
using MsBuildTaskExplorer.Views;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace MsBuildTaskExplorer
{
    [Guid("5b66e03c-6a61-434c-b7a1-ab7d3e3b0a6a")]
    public class TaskExplorerWindow : ToolWindowPane
    {
        public TaskExplorerWindow() : base(null)
        {
            this.Caption = "MsBuild Task Explorer";
            this.Content = new TaskExplorerView();
        }
    }
}

using MsBuildTaskExplorer.Views;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace MsBuildTaskExplorer
{
    [Guid("5b66e03c-6a61-434c-b7a1-ab7d3e3b0a6a")]
    public class TaskExplorerWindow : ToolWindowPane
    {
	    public const string TITLE = "MsBuild Task Explorer";

        public TaskExplorerWindow() : base(null)
        {
	        Caption = TITLE;
            Content = TaskExplorerView.Instance;
        }
    }
}

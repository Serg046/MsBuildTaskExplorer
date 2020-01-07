using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MsBuildTaskExplorer.Views;

namespace MsBuildTaskExplorer
{
    internal sealed class TaskExplorerWindowCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("f5de2864-d950-4d39-9e08-6f6a934c08a2");
        private readonly Package _package;

        private TaskExplorerWindowCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is IMenuCommandService commandService)
            {
                var menuCommandId = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(ShowToolWindow, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        public static TaskExplorerWindowCommand Instance { get; private set; }

        public TaskExplorerWindow Control { get; set; }

        public IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new TaskExplorerWindowCommand(package);
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = _package.FindToolWindow(typeof(TaskExplorerWindow), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}

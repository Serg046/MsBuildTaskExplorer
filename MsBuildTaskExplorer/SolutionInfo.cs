using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;

namespace MsBuildTaskExplorer
{
    internal class SolutionInfo
    {
        private const string OUTPUT_WINDOW_NAME = "MsBuildTaskExplorer";

        private DTE2 _dte;
        private SolutionEvents _solutionEvents;
        private string _solutionPath;
        private OutputWindowPane _outputWindow;

        public bool IsOpen { get; private set; }

        private OutputWindowPane OutputWindow
        {
            get
            {
                if (_outputWindow == null)
                {
                    var windows = _dte.ToolWindows.OutputWindow.OutputWindowPanes;

                    //see https://msdn.microsoft.com/en-us/library/bb166236.aspx
                    try
                    {
                        _outputWindow = windows.Item(OUTPUT_WINDOW_NAME);
                    }
                    catch (ArgumentException)
                    {
                        _outputWindow = windows.Add(OUTPUT_WINDOW_NAME);
                    }
                }
                return _outputWindow;
            }
        }

        public void Initialize()
        {
            _dte = TaskExplorerWindowCommand.Instance.ServiceProvider.GetService(typeof(DTE)) as DTE2;
            if (_dte == null)
                throw new InvalidOperationException("Solution info cannot be loaded");
            UpdateState();

            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () =>
            {
                UpdateState();
                SolutionOpened?.Invoke(this);
            };
            _solutionEvents.AfterClosing += () =>
            {
                UpdateState();
                SolutionClosed?.Invoke(this);
            };
        }

        private void UpdateState()
        {
            IsOpen = _dte.Solution.IsOpen;
            _solutionPath = IsOpen ? Path.GetDirectoryName(_dte.Solution.FullName) : null;
        }

        public IEnumerable<MsBuildTask> GetMsBuildTasks()
        {
            if (_dte?.Solution == null)
                throw new InvalidOperationException("SolutionInfo is not initialized");
            if (!IsOpen)
                throw new InvalidOperationException("Solution is closed");

            return GetMsBuildTasks(new DirectoryInfo(_solutionPath));
        }

        private IEnumerable<MsBuildTask> GetMsBuildTasks(DirectoryInfo directory)
        {
            var tasks = directory.GetDirectories()
                .Aggregate(new List<MsBuildTask>(),
                    (current, dir) =>
                    {
                        current.AddRange(GetMsBuildTasks(dir));
                        return current;
                    });

            var projFiles = directory.GetFiles("*.*proj");
            tasks.AddRange(projFiles.Select(projFile => BuildMsBuildTask(projFile.FullName)));
            return tasks;
        }

        private MsBuildTask BuildMsBuildTask(string filePath)
        {
            var project = ProjectCollection.GlobalProjectCollection.LoadProject(filePath);
            var targets = project.Targets.Values
                .Where(t => t.Location.File.StartsWith(_solutionPath)
                            || t.Name == "Build" || t.Name == "Clean" || t.Name == "Rebuild")
                .Select(t => t.Name);
            ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            return new MsBuildTask(filePath, targets);
        }

        public void WriteOutputLine(string value)
        {
            OutputWindow.OutputString(value + Environment.NewLine);
        }

        public IEnumerable<ProjectProperty> GetAllProperties(string projFilePath)
        {
            var project = ProjectCollection.GlobalProjectCollection.LoadProject(projFilePath);
            var props = project.Properties;
            ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            return props;
        }

        public event SolutionEventHandler SolutionOpened;
        public event SolutionEventHandler SolutionClosed;
    }

    internal delegate void SolutionEventHandler(SolutionInfo solutionInfo);
}

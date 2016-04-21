using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;

namespace MsBuildTaskExplorer
{
    internal class SolutionInfo
    {
        private DTE _dte;
        private SolutionEvents _solutionEvents;
        private string _solutionPath;

        public bool IsOpen { get; private set; }

        public void Initialize()
        {
            _dte = TaskExplorerWindowCommand.Instance.ServiceProvider.GetService(typeof(SDTE)) as DTE;
            if (_dte == null)
                throw new InvalidOperationException("Solution info cannot be loaded");
            UpdateState();

            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () =>
            {
                UpdateState();
                SolutionOpened?.Invoke(this);
            };
        }

        private void UpdateState()
        {
            IsOpen = _dte.Solution.IsOpen;
            _solutionPath = Path.GetDirectoryName(_dte.Solution.FullName);
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

        public event SolutionOpenedEventHandler SolutionOpened;
    }

    internal delegate void SolutionOpenedEventHandler(SolutionInfo solutionInfo);
}

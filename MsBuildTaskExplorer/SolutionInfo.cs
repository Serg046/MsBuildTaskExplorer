using System;
using System.Collections.Generic;
using Pri.LongPath;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using System.Text;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer
{
    internal class SolutionInfo
    {
        private const string OUTPUT_WINDOW_NAME = "MsBuildTaskExplorer";

        private DTE2 _dte;
        private SolutionEvents _solutionEvents;
        private string _solutionPath;
        private OutputWindowPane _outputWindow;
        public event SolutionEventHandler SolutionOpened;
        public event SolutionEventHandler SolutionClosed;

        public SolutionInfo()
        {
            Initialize();
        }

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
        
        private void Initialize()
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

        public Task<IReadOnlyList<MsBuildTask>> GetMsBuildTasksAsync()
        {
            if (!IsOpen)
                throw new InvalidOperationException("Solution is closed");

            return Task.Run(() => GetMsBuildTasks(new DirectoryInfo(_solutionPath)));
        }

        private IReadOnlyList<MsBuildTask> GetMsBuildTasks(DirectoryInfo directory)
        {
            var tasks = directory.GetDirectories()
                .Aggregate(new List<MsBuildTask>(),
                    (current, dir) =>
                    {
                        current.AddRange(GetMsBuildTasks(dir));
                        return current;
                    });

            foreach (var projFile in directory.GetFiles("*.*proj"))
            {
                try
                {
                    tasks.Add(BuildMsBuildTask(projFile.FullName));
                }
                catch(InvalidProjectFileException ex)
                {
                    var errSb = new StringBuilder("Exception: ")
                        .AppendLine(ex.GetType().FullName)
                        .AppendLine($"The project \"{projFile.FullName}\" was not loaded.")
                        .AppendLine(ex.Message);
                    if (ex.StackTrace != null)
                    {
                        errSb.AppendLine("Stack trace:").Append(ex.StackTrace.ToString());
                    }
                    WriteOutputLine(errSb.ToString());
                }
            }
            return tasks;
        }

        private MsBuildTask BuildMsBuildTask(string fullPath)
        {
            var project = ProjectCollection.GlobalProjectCollection.LoadProject(fullPath);
            var targets = project.Targets.Values
                .Where(t => t.Location.File.StartsWith(_solutionPath)
                            || t.Name == "Build" || t.Name == "Clean" || t.Name == "Rebuild")
                .Select(t => t.Name);
            ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            return new MsBuildTask(fullPath, fullPath.Replace(_solutionPath, string.Empty).TrimStart('\\'), targets);
        }

        public void WriteOutputLine(string value)
        {
            OutputWindow.OutputString(value + Environment.NewLine);
        }

        public IEnumerable<ProjectProperty> GetAllProperties(string projFilePath)
        {
            if (!IsOpen)
                throw new InvalidOperationException("Solution is closed");

            var project = ProjectCollection.GlobalProjectCollection.LoadProject(projFilePath, GetGlobalProperties(), null);
            var props = project.Properties;
            ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            return props;
        }

        public Dictionary<string, string> GetGlobalProperties()
        {
            return new Dictionary<string, string>
            {
                ["Configuration"] = _dte.Solution.SolutionBuild.ActiveConfiguration.Name,
            };
        }
    }

    internal delegate void SolutionEventHandler(SolutionInfo solutionInfo);
}

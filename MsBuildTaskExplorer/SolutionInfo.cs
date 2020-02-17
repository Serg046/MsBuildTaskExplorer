using System;
using System.Collections.Generic;
using Pri.LongPath;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer
{
    internal class SolutionInfo
    {
        private const string OUTPUT_WINDOW_NAME = "MsBuildTaskExplorer";
        private const string ENSURE_NUGET_PACKAGE_BUILD_IMPORTS = "EnsureNuGetPackageBuildImports";

        private readonly object _projSync = new object();
        private DTE2 _dte;
        private SolutionEvents _solutionEvents;
        private string _solutionFolder;
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
            _solutionFolder = IsOpen ? Path.GetDirectoryName(_dte.Solution.FullName) : null;
        }

        public Task<IReadOnlyList<MsBuildTask>> GetMsBuildTasksAsync(string filter)
        {
            if (!IsOpen)
                throw new InvalidOperationException("Solution is closed");

            return Task.Run(() => GetMsBuildTasks(new DirectoryInfo(_solutionFolder), filter));
        }

        private IReadOnlyList<MsBuildTask> GetMsBuildTasks(DirectoryInfo directory, string filter)
        {
            var tasks = directory.GetDirectories()
                .Aggregate(new List<MsBuildTask>(),
                    (current, dir) =>
                    {
                        current.AddRange(GetMsBuildTasks(dir, filter));
                        return current;
                    });

            foreach (var mask in Settings.Instance.SupportedFileExtensions.Split(';'))
            {
                foreach (var projFile in directory.GetFiles(mask))
                {
                    try
                    {
                        tasks.Add(BuildMsBuildTask(projFile.FullName, filter));
                    }
                    catch (InvalidProjectFileException ex)
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
            }
            return tasks;
        }

        private MsBuildTask BuildMsBuildTask(string fullPath, string filter)
        {
	        lock (_projSync)
	        {
	            var project = ProjectCollection.GlobalProjectCollection.LoadProject(fullPath);
	            var targets = project.Targets.Values
	                .Where(t => t.Location.File.StartsWith(_solutionFolder)
	                            || t.Name == "Build" || t.Name == "Clean" || t.Name == "Rebuild")
	                .Select(t => t.Name);
	            ProjectCollection.GlobalProjectCollection.UnloadProject(project);
	            return new MsBuildTask(
		            fullPath,
		            fullPath.Replace(_solutionFolder, string.Empty).TrimStart('\\'),
		            (relativeFilePath, targetName) => GetFilter(relativeFilePath, targetName, filter),
		            targets);
            }
	    }

        private bool GetFilter(string relativeFilePath, string targetName, string originalFilter)
        {
	        try
	        {
		        var filter = targetName != ENSURE_NUGET_PACKAGE_BUILD_IMPORTS;
		        var filters = originalFilter?.Split('|');
		        if (filter && filters != null)
		        {
			        if (filters.Length == 1)
			        {
				        filter = Regex.IsMatch(targetName, filters[0], RegexOptions.IgnoreCase);
			        }
			        else if (filters.Length == 2)
			        {
				        filter = Regex.IsMatch(targetName, filters[1], RegexOptions.IgnoreCase)
				                 && Regex.IsMatch(relativeFilePath, filters[0], RegexOptions.IgnoreCase);
			        }
		        }

		        return filter;
	        }
	        catch (ArgumentException ex)
	        {
		        WriteOutputLine(ex.ToString());
                return false;
	        }
        }

        public void WriteOutputLine(string value)
        {
            OutputWindow.OutputString(value + Environment.NewLine);
        }

        public ICollection<ProjectProperty> GetAllProperties(string projFilePath)
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
            var solutionDir = _solutionFolder;
            if (!solutionDir.EndsWith('\\'))
            {
                solutionDir += "\\";
            }
            return new Dictionary<string, string>
            {
                ["Configuration"] = _dte.Solution.SolutionBuild.ActiveConfiguration.Name,
                ["SolutionDir"] = solutionDir,
                ["SolutionExt"] = Path.GetExtension(_dte.Solution.FileName),
                ["SolutionFileName"] = Path.GetFileName(_dte.Solution.FileName),
                ["SolutionName"] = Path.GetFileNameWithoutExtension(_dte.Solution.FileName),
                ["SolutionPath"] = _dte.Solution.FullName
            };
        }

        public void ShowOutputWindow()
        {
            OutputWindow.Activate();
            _dte.ToolWindows.OutputWindow.Parent.SetFocus();
        }
    }

    internal delegate void SolutionEventHandler(SolutionInfo solutionInfo);
}

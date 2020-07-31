using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using AopInpc;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class TaskExplorerViewModel : INotifyPropertyChangedCaller
    {
        private const string SEPARATOR = "<`~`>";
        private const string ENSURE_NUGET_PACKAGE_BUILD_IMPORTS = "EnsureNuGetPackageBuildImports";

        private SolutionInfo _solutionInfo;
        private bool _isInitialized;
        private bool _isTargetRunning;
        private IReadOnlyList<MsBuildTask> _msBuildTasks;

        public TaskExplorerViewModel()
        {
            Tasks = new ObservableCollection<MsBuildTaskViewModel>();
            SettingsViewModel = ViewModelFactory.Create<SettingsViewModel>(this);
        }

        [Inpc]
        public virtual string Filter { get; set; }

        public ObservableCollection<MsBuildTaskViewModel> Tasks { get; }

        [Inpc]
        public virtual Visibility ProgressBarVisibility { get; set; }

        [Inpc]
        public virtual Visibility MainViewVisibility { get; set; }

        public SettingsViewModel SettingsViewModel { get; }

        public virtual async Task Initialize()
        {
            if (!_isInitialized)
            {
                var filter = Settings.Instance.Filter;
                if (!string.IsNullOrEmpty(filter))
                    Filter = filter;
                _solutionInfo = new SolutionInfo();
                await UpdateTaskList();
                _solutionInfo.SolutionOpened += info => UpdateTaskList();
                _solutionInfo.SolutionClosed += info =>
                {
	                Tasks.Clear();
	                _msBuildTasks = null;
                };
                _isInitialized = true;
            }
            ProgressBarVisibility = Visibility.Collapsed;
        }

        public virtual void Unload() => SaveSettings();

        public virtual async Task Execute(MsBuildTargetViewModel targetVm)
        {
            if (!_isTargetRunning)
            {
                _isTargetRunning = true;
                ProgressBarVisibility = Visibility.Visible;
                _solutionInfo.ShowOutputWindow();

                await Task.Run(() => BuildManager.DefaultBuildManager.Build(CreateBuildParameters(),
                    CreateBuildRequest(targetVm.Parent.FullFilePath, targetVm.Target)));

                ProgressBarVisibility = Visibility.Collapsed;
                _isTargetRunning = false;
            }
        }

        public virtual void PrintAllProps(MsBuildTargetViewModel targetVm)
        {
            _solutionInfo.ShowOutputWindow();

            var properties = _solutionInfo.GetAllProperties(targetVm.Parent.FullFilePath);
            if (properties == null || properties.Count == 0)
            {
                _solutionInfo.WriteOutputLine("There is no any property");
            }
            else
            {
                foreach (var projectProperty in properties.OrderBy(p => p.Name))
                {
                    _solutionInfo.WriteOutputLine($"{projectProperty.Name} = {projectProperty.EvaluatedValue}");
                }
            }
        }

        public virtual void Abort()
        {
            _solutionInfo.ShowOutputWindow();
            BuildManager.DefaultBuildManager.CancelAllSubmissions();
        }

        public virtual void OpenSettings()
        {
            MainViewVisibility = Visibility.Collapsed;
            SettingsViewModel.SettingsViewVisibility = Visibility.Visible;
        }

        public virtual async Task ApplyFilterAsync(string filter)
        {
            SaveSettings(filter);
	        await UpdateTaskList(filter);
        }

        public virtual async Task UpdateTaskList(string filter = null)
        {
            if (_solutionInfo?.IsOpen == true)
            {
                ProgressBarVisibility = Visibility.Visible;
                if (_msBuildTasks == null)
                {
	                _msBuildTasks = await _solutionInfo.GetMsBuildTasksAsync();
                }


                MsBuildTask.FilterCallback filterCallback = (relativeFilePath, targetName)
	                => GetFilter(relativeFilePath, targetName, filter ?? Filter);
                foreach (var msBuildTask in _msBuildTasks)
                {
	                msBuildTask.Filter = filterCallback;
                }
                var orderedTasks = _msBuildTasks
                    .Where(t => t.Targets != null && t.Targets.Any())
                    .OrderBy(t => t.FullFilePath);
                Tasks.Clear();

                var expandedTargets = Settings.Instance.ExpandedTargets?.Split(new[] { SEPARATOR }, StringSplitOptions.None);

                foreach (var task in orderedTasks)
                {
                    var taskViewModel = ViewModelFactory.Create<MsBuildTaskViewModel>(task, this);
                    if (expandedTargets?.Length > 0)
                    {
                        if (expandedTargets.Contains(taskViewModel.FullFilePath))
                        {
                            taskViewModel.IsExpanded = true;
                        }
                    }
                    else
                    {
                        taskViewModel.IsExpanded = true;
                    }
                    Tasks.Add(taskViewModel);
                }

                ProgressBarVisibility = Visibility.Collapsed;
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
				        filter = Regex.IsMatch(relativeFilePath, filters[0], RegexOptions.IgnoreCase);
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
		        _solutionInfo.WriteOutputLine(ex.ToString());
		        return false;
	        }
        }

        private void SaveSettings(string filter = null)
        {
	        var actualFilter = filter ?? Filter ?? string.Empty;
            if (_solutionInfo?.IsOpen == true)
            {
	            Settings.Instance.Filter = actualFilter;

                var expandedItems = Tasks.Where(t => t.IsExpanded).Select(t => t.FullFilePath)
                    .Where(path => !Regex.IsMatch(path, SEPARATOR)).ToList();
                Settings.Instance.ExpandedTargets = expandedItems.Any() ? string.Join(SEPARATOR, expandedItems) : string.Empty;
            }
        }

        private BuildParameters CreateBuildParameters()
        {
            return new BuildParameters(new ProjectCollection())
            {
                Loggers = new[] { new MsBuildLogger(_solutionInfo.WriteOutputLine) }
            };
        }

        private BuildRequestData CreateBuildRequest(string projFilePath, string target)
        {
            return new BuildRequestData(projFilePath, _solutionInfo.GetGlobalProperties(), null,
                new[] { target }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

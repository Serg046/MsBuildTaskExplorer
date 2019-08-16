using System;
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

        public TaskExplorerViewModel()
        {
            Tasks = new ObservableCollection<MsBuildTaskViewModel>();
            SettingsViewModel = ViewModelFactory.Create<SettingsViewModel>(this);
            Initialize = new AsyncLambdaCommand(async () =>
            {
                if (!_isInitialized)
                {
                    var filter = Settings.Instance.Filter;
                    if (!string.IsNullOrEmpty(filter))
                        Filter = filter;
                    _solutionInfo = new SolutionInfo();
                    await UpdateTaskList();
                    _solutionInfo.SolutionOpened += async info => await UpdateTaskList();
                    _solutionInfo.SolutionClosed += info => Tasks.Clear();
                    _isInitialized = true;
                }
                ProgressBarVisibility = Visibility.Collapsed;
            });
            Refresh = new AsyncLambdaCommand(async () =>
            {
                SaveSettings();
                await UpdateTaskList();
            });
            ExecuteTask = new AsyncLambdaCommand<MsBuildTargetViewModel>(async (vm) =>
            {
                if (!_isTargetRunning)
                {
                    _isTargetRunning = true;
                    ProgressBarVisibility = Visibility.Visible;
                    _solutionInfo.ShowOutputWindow();

                    await Task.Run(() => BuildManager.DefaultBuildManager.Build(CreateBuildParameters(),
                        CreateBuildRequest(vm.Parent.FullFilePath, vm.Target)));

                    ProgressBarVisibility = Visibility.Collapsed;
                    _isTargetRunning = false;
                }
            });
            PrintAllProps = new AsyncLambdaCommand<MsBuildTargetViewModel>((vm) =>
            {
                _solutionInfo.ShowOutputWindow();
                
                var properties = _solutionInfo.GetAllProperties(vm.Parent.FullFilePath);
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
                return Task.CompletedTask;
            });
            AbortTask = new AsyncLambdaCommand(() =>
            {
                _solutionInfo.ShowOutputWindow();
                BuildManager.DefaultBuildManager.CancelAllSubmissions();
                return Task.CompletedTask;
            });
        }

        [Inpc]
        public virtual string Filter { get; set; }

        public virtual ObservableCollection<MsBuildTaskViewModel> Tasks { get; }

        [Inpc]
        public virtual Visibility ProgressBarVisibility { get; set; }

        [Inpc]
        public virtual Visibility MainViewVisibility { get; set; }

        public SettingsViewModel SettingsViewModel { get; }

        public virtual AsyncLambdaCommand Initialize { get; }
        public virtual Action Unload => () => SaveSettings();
        public virtual AsyncLambdaCommand Refresh { get; }
        public virtual AsyncLambdaCommand<MsBuildTargetViewModel> ExecuteTask { get; }
        public virtual AsyncLambdaCommand<MsBuildTargetViewModel> PrintAllProps { get; }
        public virtual AsyncLambdaCommand AbortTask { get; }
        public Action OpenSettings => () =>
        {
            MainViewVisibility = Visibility.Collapsed;
            SettingsViewModel.SettingsViewVisibility = Visibility.Visible;
        };

        public async Task UpdateTaskList()
        {
            if (_solutionInfo?.IsOpen == true)
            {
                ProgressBarVisibility = Visibility.Visible;
                Func<string, bool> filter;
                if (string.IsNullOrEmpty(Filter))
                    filter = targetName => targetName != ENSURE_NUGET_PACKAGE_BUILD_IMPORTS;
                else
                    filter = targetName => targetName != ENSURE_NUGET_PACKAGE_BUILD_IMPORTS
                                           && Regex.IsMatch(targetName, Filter, RegexOptions.IgnoreCase);

                var tasks = await _solutionInfo.GetMsBuildTasksAsync();
                var orderedTasks = tasks
                    .Where(t => t.Targets != null && t.Targets.Any())
                    .OrderBy(t => t.FullFilePath);
                Tasks.Clear();

                var expandedTargets = Settings.Instance.ExpandedTargets?.Split(new[] { SEPARATOR }, StringSplitOptions.None);

                foreach (var task in orderedTasks)
                {
                    task.Filter = filter;
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

        private void SaveSettings()
        {
            if (_solutionInfo?.IsOpen == true)
            {
                Settings.Instance.Filter = !string.IsNullOrEmpty(Filter) ? Filter : string.Empty;

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
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

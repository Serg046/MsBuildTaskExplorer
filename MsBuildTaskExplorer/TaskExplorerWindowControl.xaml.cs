using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using MsBuildTaskExplorer.AppSettings;

namespace MsBuildTaskExplorer
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    public partial class TaskExplorerWindowControl : UserControl
    {
        private SolutionInfo _solutionInfo;
        private bool _isInitialized;
        private const string SEPARATOR = "<`~`>";

        public TaskExplorerWindowControl()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += (sender, args) => SaveSettings();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_isInitialized)
            {
                var filter = Settings.Instance.Filter;
                if (!string.IsNullOrEmpty(filter))
                    FilterTb.Text = filter;
                _solutionInfo = new SolutionInfo();
                if (_solutionInfo.IsOpen)
                {
                    UpdateTaskList();
                }
                _solutionInfo.SolutionOpened += info => UpdateTaskList();
                _solutionInfo.SolutionClosed += info => TasksItemsControl.ItemsSource = null;
                _isInitialized = true;
            }
        }

        private void ShowProgressBar()
        {
            ProgressBar.IsIndeterminate = true;
            ProgressBar.Visibility = Visibility.Visible;
        }

        private void HideProgressBar()
        {
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void UpdateTaskList()
        {
            ShowProgressBar();
            Func<string, bool> filter;
            if (string.IsNullOrEmpty(FilterTb.Text))
                filter = targetName => targetName != "EnsureNuGetPackageBuildImports";
            else
                filter = targetName => targetName != "EnsureNuGetPackageBuildImports"
                && Regex.IsMatch(targetName, FilterTb.Text, RegexOptions.IgnoreCase);

            var tasks = await _solutionInfo.GetMsBuildTasksAsync();
            TasksItemsControl.ItemsSource = tasks
                .Select(t =>
                {
                    t.Filter = filter;
                    return t;
                })
                .Where(t => t.Targets != null && t.Targets.Any())
                .OrderBy(t => t.FullFilePath);
            
            ExpandTargetsIfRequired();
            HideProgressBar();
        }

        private void ExpandTargetsIfRequired()
        {
            var expandedTargets = Settings.Instance.ExpandedTargets;
            if (!string.IsNullOrEmpty(expandedTargets))
            {
                var targetPaths = expandedTargets.Split(new[] {SEPARATOR}, StringSplitOptions.None);
                var itemsToExpand = TasksItemsControl.Items.Cast<object>()
                    .Select(item => ((TreeViewItem)TasksItemsControl.ItemContainerGenerator.ContainerFromItem(item)))
                    .Where(item => targetPaths.Contains(((MsBuildTask)item.DataContext).FullFilePath));
                foreach (var treeViewItem in itemsToExpand)
                {
                    treeViewItem.IsExpanded = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(FilterTb.Text))
                {
                    foreach (var item in TasksItemsControl.Items)
                    {
                        var dObject = TasksItemsControl.ItemContainerGenerator.ContainerFromItem(item);
                        ((TreeViewItem)dObject).IsExpanded = true;
                    }
                }
            }
        }

        private async void RunButtonOnClick(object sender, RoutedEventArgs e)
        {
            ShowProgressBar();
            var btn = (sender as FrameworkElement);
            var targetName = btn.DataContext.ToString();
            var currentTreeViewItem = btn.FindVisualParent<TreeViewItem>();
            var msBuildTask = currentTreeViewItem.FindVisualParent<TreeViewItem>().DataContext as MsBuildTask;

            using (var buildManager = new BuildManager())
            {
                await Task.Run(() => buildManager.Build(CreateBuildParameters(),
                    CreateBuildRequest(msBuildTask.FullFilePath, targetName)));
            }
            HideProgressBar();
        }

        private BuildParameters CreateBuildParameters()
        {
            return new BuildParameters(new ProjectCollection())
            {
                Loggers = new [] { new MsBuildLogger(_solutionInfo.WriteOutputLine) }
            };
        }

        private BuildRequestData CreateBuildRequest(string projFilePath, string target)
        {
            return new BuildRequestData(projFilePath, _solutionInfo.GetGlobalProperties(), null,
                new[] { target }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance);
        }

        private void RefreshButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (_solutionInfo.IsOpen)
            {
                SaveSettings();
                UpdateTaskList();
            }
        }

        private void SaveSettings()
        {
            Settings.Instance.Filter = !string.IsNullOrEmpty(FilterTb.Text) ? FilterTb.Text : string.Empty;
            
            var expandedItems = TasksItemsControl.Items.Cast<object>()
                .Select(item => ((TreeViewItem)TasksItemsControl.ItemContainerGenerator.ContainerFromItem(item)))
                .Where(item => item.IsExpanded)
                .Select(item => ((MsBuildTask)item.DataContext).FullFilePath)
                .Where(path => !Regex.IsMatch(path, SEPARATOR))
                .ToList();
            Settings.Instance.ExpandedTargets = expandedItems.Any() ? string.Join(SEPARATOR, expandedItems) : string.Empty;
        }

        private void PrintAllPropsButtonOnClick(object sender, RoutedEventArgs e)
        {
            var msBuildTask = (sender as FrameworkElement)
                ?.FindVisualParent<TreeViewItem>()
                ?.FindVisualParent<TreeViewItem>()
                .DataContext as MsBuildTask;

            foreach (var projectProperty in _solutionInfo.GetAllProperties(msBuildTask.FullFilePath).OrderBy(p => p.Name))
            {
                _solutionInfo.WriteOutputLine($"{projectProperty.Name} = {projectProperty.EvaluatedValue}");
            }
        }
    }
}
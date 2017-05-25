using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace MsBuildTaskExplorer
{
    using System.Windows;
    using System.Windows.Controls;
    public partial class TaskExplorerWindowControl : UserControl
    {
        private readonly SolutionInfo _solutionInfo = new SolutionInfo();
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
                _solutionInfo.Initialize();
                if (_solutionInfo.IsOpen)
                {
                    UpdateTaskList();
                }
                _solutionInfo.SolutionOpened += info => UpdateTaskList();
                _solutionInfo.SolutionClosed += info => TasksItemsControl.ItemsSource = null;
                _isInitialized = true;
            }
        }

        private async void UpdateTaskList()
        {
            ProgressBar.IsIndeterminate = true;
            ProgressBar.Visibility = Visibility.Visible;

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
                .OrderBy(t => t.FilePath);
            
            ExpandTargetsIfRequired();

            ProgressBar.IsIndeterminate = false;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ExpandTargetsIfRequired()
        {
            var expandedTargets = Settings.Instance.ExpandedTargets;
            if (!string.IsNullOrEmpty(expandedTargets))
            {
                var targetPaths = expandedTargets.Split(new[] {SEPARATOR}, StringSplitOptions.None);
                var itemsToExpand = TasksItemsControl.Items.Cast<object>()
                    .Select(item => ((TreeViewItem)TasksItemsControl.ItemContainerGenerator.ContainerFromItem(item)))
                    .Where(item => targetPaths.Contains(((MsBuildTask)item.DataContext).FilePath));
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

        private void RunButtonOnClick(object sender, RoutedEventArgs e)
        {
            var btn = (sender as FrameworkElement);
            var targetName = btn.DataContext.ToString();
            var currentTreeViewItem = btn.FindVisualParent<TreeViewItem>();
            var msBuildTask = currentTreeViewItem.FindVisualParent<TreeViewItem>().DataContext as MsBuildTask;

            using (var buildManager = new BuildManager())
            {
                buildManager.Build(CreateBuildParameters(), CreateBuildRequest(msBuildTask.FilePath, targetName));
            }

        }

        private BuildParameters CreateBuildParameters()
        {
            var projectCollection = new ProjectCollection();
            var buildParameters = new BuildParameters(projectCollection)
            {
                Loggers = new List<ILogger>() { new MsBuildLogger(_solutionInfo.WriteOutputLine) }
            };
            return buildParameters;
        }

        private BuildRequestData CreateBuildRequest(string projFilePath, string target)
        {
            var globalProperties = new Dictionary<string, string>();
            var buildRequest = new BuildRequestData(projFilePath, globalProperties, null, new[] { target }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance);
            return buildRequest;
        }

        private void RefreshButtonOnClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            UpdateTaskList();
        }

        private void SaveSettings()
        {
            Settings.Instance.Filter = !string.IsNullOrEmpty(FilterTb.Text) ? FilterTb.Text : string.Empty;
            
            var expandedItems = TasksItemsControl.Items.Cast<object>()
                .Select(item => ((TreeViewItem)TasksItemsControl.ItemContainerGenerator.ContainerFromItem(item)))
                .Where(item => item.IsExpanded)
                .Select(item => ((MsBuildTask)item.DataContext).FilePath)
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

            foreach (var projectProperty in _solutionInfo.GetAllProperties(msBuildTask.FilePath).OrderBy(p => p.Name))
            {
                _solutionInfo.WriteOutputLine($"{projectProperty.Name} = {projectProperty.EvaluatedValue}");
            }
        }
    }
}
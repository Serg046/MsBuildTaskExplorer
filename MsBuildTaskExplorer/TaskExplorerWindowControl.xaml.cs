//------------------------------------------------------------------------------
// <copyright file="TaskExplorerWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;

namespace MsBuildTaskExplorer
{
    using System.Windows;
    using System.Windows.Controls;
    public partial class TaskExplorerWindowControl : UserControl
    {
        private readonly SolutionInfo _solutionInfo = new SolutionInfo();
        private bool _isInitialized;

        public TaskExplorerWindowControl()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_isInitialized)
            {
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

        private void UpdateTaskList()
        {
            Func<string, bool> filter;
            if (string.IsNullOrEmpty(FilterTb.Text))
                filter = targetName => targetName != "EnsureNuGetPackageBuildImports";
            else
                filter = targetName => targetName != "EnsureNuGetPackageBuildImports"
                && Regex.IsMatch(targetName, FilterTb.Text, RegexOptions.IgnoreCase);

            TasksItemsControl.ItemsSource = _solutionInfo.GetMsBuildTasks()
                .Select(t =>
                {
                    t.Filter = filter;
                    return t;
                })
                .Where(t => t.Targets != null && t.Targets.Any())
                .OrderBy(t => t.FilePath);
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
            UpdateTaskList();
        }
    }
}
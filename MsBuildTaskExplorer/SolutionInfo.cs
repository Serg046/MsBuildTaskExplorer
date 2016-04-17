using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace MsBuildTaskExplorer
{
    internal class SolutionInfo
    {
        private DTE _dte;
        private SolutionEvents _solutionEvents;

        public bool IsOpen { get; private set; }

        public void Initialize()
        {
            _dte = TaskExplorerWindowCommand.Instance.ServiceProvider.GetService(typeof(SDTE)) as DTE;
            IsOpen = _dte.Solution.IsOpen;
            if (_dte == null)
                throw new InvalidOperationException("Solution info cannot be loaded");
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () =>
            {
                IsOpen = _dte.Solution.IsOpen;
                SolutionOpened?.Invoke(this);
            };
        }

        public IEnumerable<MsBuildTask> GetMsBuildTasks()
        {
            if (_dte?.Solution == null)
                throw new InvalidOperationException("SolutionInfo is not initialized");
            if (!IsOpen)
                throw new InvalidOperationException("Solution is closed");

            var path = Path.GetDirectoryName(_dte.Solution.FullName);
            return GetMsBuildTasks(new DirectoryInfo(path));
        }

        private IEnumerable<MsBuildTask> GetMsBuildTasks(DirectoryInfo directory)
        {
            var tasks = directory.GetDirectories()
                .Aggregate(Enumerable.Empty<MsBuildTask>(),
                    (current, dir) => current.Union(GetMsBuildTasks(dir)));

            var projFiles = directory.GetFiles("*.*proj");
            return tasks.Union(projFiles.Select(projFile => BuildMsBuildTask(projFile.FullName)));
        }

        private MsBuildTask BuildMsBuildTask(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var targets = new List<string>();
            foreach (var target in doc.Root.Descendants().Where(x => x.Name.LocalName == "Target")) //can be refactored if the perfonamce issuies
            {
                var nameAttribute = target.Attributes().SingleOrDefault(a => a.Name == "Name");
                if (nameAttribute != null)
                    targets.Add(nameAttribute.Value);
            }
            return new MsBuildTask(filePath, targets);
        }

        public event SolutionOpenedEventHandler SolutionOpened;
    }

    internal delegate void SolutionOpenedEventHandler(SolutionInfo solutionInfo);
}

using System.Collections.Generic;
using System.Linq;

namespace MsBuildTaskExplorer
{
    internal class MsBuildTask
    {
        private readonly IEnumerable<string> _targets;

        public MsBuildTask(string fullFilePath, string relativeFilePath, FilterCallback filter, IEnumerable<string> targets)
        {
            FullFilePath = fullFilePath;
            RelativeFilePath = relativeFilePath;
            Filter = filter;
            _targets = targets.OrderBy(t => t);
        }

        public string FullFilePath { get; }
        public string RelativeFilePath { get; }
        public FilterCallback Filter { get; }

        public IEnumerable<string> Targets => _targets != null && Filter != null
            ? _targets.Where(target => Filter(RelativeFilePath, target))
            : _targets;

        public delegate bool FilterCallback(string relativeFilePath, string targetName);
    }
}

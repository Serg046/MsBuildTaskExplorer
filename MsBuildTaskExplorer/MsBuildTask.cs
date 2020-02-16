using System.Collections.Generic;
using System.Linq;

namespace MsBuildTaskExplorer
{
    internal class MsBuildTask
    {
        private readonly IEnumerable<string> _targets;

        public MsBuildTask(string fullFilePath, string relativeFilePath, IEnumerable<string> targets)
        {
            FullFilePath = fullFilePath;
            RelativeFilePath = relativeFilePath;
            _targets = targets.OrderBy(t => t);
        }

        public string FullFilePath { get; }
        public string RelativeFilePath { get; }
        public FilterCallback Filter { get; set; }

        public IEnumerable<string> Targets => _targets != null && Filter != null
            ? _targets.Where(target => Filter(FullFilePath, target))
            : _targets;

        public delegate bool FilterCallback(string fullFilePath, string targetName);
    }
}

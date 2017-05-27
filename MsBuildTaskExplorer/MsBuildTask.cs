using System;
using System.Collections.Generic;
using System.Linq;

namespace MsBuildTaskExplorer
{
    internal class MsBuildTask
    {
        private readonly IEnumerable<string> _targets;

        public MsBuildTask(string filePath, string relativeFilePath, IEnumerable<string> targets)
        {
            FullFilePath = filePath;
            RelativeFilePath = relativeFilePath;
            _targets = targets.OrderBy(t => t);
        }

        public string FullFilePath { get; }
        public string RelativeFilePath { get; }
        public Func<string,bool> Filter { get; set; }

        public IEnumerable<string> Targets => _targets != null && Filter != null
            ? _targets.Where(Filter)
            : _targets;
    }
}

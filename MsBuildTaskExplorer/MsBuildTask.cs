using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer
{
    internal class MsBuildTask
    {
        private readonly IEnumerable<string> _targets;

        public MsBuildTask(string filePath, IEnumerable<string> targets)
        {
            FilePath = filePath;
            _targets = targets.OrderBy(t => t);
        }

        public string FilePath { get; }
        public Func<string,bool> Filter { get; set; }

        public IEnumerable<string> Targets => _targets != null && Filter != null
            ? _targets.Where(Filter)
            : _targets;
    }
}

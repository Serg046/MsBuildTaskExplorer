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
            _targets = targets;
            FilePath = filePath;
        }

        public string FilePath { get; }
        public Func<string,bool> Filter { get; set; }

        public IEnumerable<string> Targets => Filter != null
            ? _targets.Where(Filter)
            : _targets;
    }
}

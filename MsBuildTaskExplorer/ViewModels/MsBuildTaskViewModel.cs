using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AopInpc;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class MsBuildTaskViewModel : INotifyPropertyChangedCaller
    {
        public MsBuildTaskViewModel(MsBuildTask task, TaskExplorerViewModel baseViewModel)
        {
            FullFilePath = task.FullFilePath;
            RelativeFilePath = task.RelativeFilePath;
            Targets = task.Targets.Select(target => new MsBuildTargetViewModel(this, baseViewModel, target)).ToList();
        }

        [Inpc]
        public bool IsExpanded { get; set; }

        public string FullFilePath { get; }

        public string RelativeFilePath { get; }

        public IReadOnlyList<MsBuildTargetViewModel> Targets { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

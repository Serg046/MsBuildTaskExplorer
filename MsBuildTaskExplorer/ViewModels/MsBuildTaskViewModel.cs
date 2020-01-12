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
            Targets = task.Targets.Select(target => ViewModelFactory.Create<MsBuildTargetViewModel>(this, baseViewModel, target)).ToList();
        }

        [Inpc]
        public virtual bool IsExpanded { get; set; }

        public string FullFilePath { get; }

        public string RelativeFilePath { get; }

        public IReadOnlyList<MsBuildTargetViewModel> Targets { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

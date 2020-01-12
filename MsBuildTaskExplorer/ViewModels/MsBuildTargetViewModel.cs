using AopInpc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class MsBuildTargetViewModel : INotifyPropertyChangedCaller
    {
        private readonly TaskExplorerViewModel _baseViewModel;

        public MsBuildTargetViewModel(MsBuildTaskViewModel parent, TaskExplorerViewModel baseViewModel, string target)
        {
            _baseViewModel = baseViewModel;
            Parent = parent;
            Target = target;
        }

        public MsBuildTaskViewModel Parent { get; }

        public string Target { get; }

        public virtual async Task Execute()
        {
            await _baseViewModel.Execute(this);
        }

        public virtual void PrintAllProps() => _baseViewModel.PrintAllProps(this);

        public virtual void Abort() => _baseViewModel.Abort();

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

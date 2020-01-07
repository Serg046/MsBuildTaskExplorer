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

        public async Task Execute()
        {
            await _baseViewModel.Execute(this);
        }

        public void PrintAllProps() => _baseViewModel.PrintAllProps(this);

        public void Abort() => _baseViewModel.Abort();

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

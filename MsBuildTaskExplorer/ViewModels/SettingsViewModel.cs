using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using AopInpc;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class SettingsViewModel : INotifyPropertyChangedCaller
    {
        private readonly TaskExplorerViewModel _parentViewModel;

        public SettingsViewModel(TaskExplorerViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            SupportedFileExtensions = Settings.Instance.SupportedFileExtensions;
            SettingsViewVisibility = Visibility.Collapsed;
        }

        [Inpc]
        public virtual string SupportedFileExtensions { get; set; }

        [Inpc]
        public virtual Visibility SettingsViewVisibility { get; set; }

        public async Task NavigateBack()
        {
            SettingsViewVisibility = Visibility.Collapsed;
            _parentViewModel.ProgressBarVisibility = Visibility.Visible;
            _parentViewModel.MainViewVisibility = Visibility.Visible;
            Settings.Instance.SupportedFileExtensions = SupportedFileExtensions ?? "";
            await _parentViewModel.UpdateTaskList();
            _parentViewModel.ProgressBarVisibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

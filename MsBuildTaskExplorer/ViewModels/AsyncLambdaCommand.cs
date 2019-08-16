using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MsBuildTaskExplorer.ViewModels
{
    public class AsyncLambdaCommand : ICommand
    {
        private readonly Func<Task> _command;
        private bool _isEnabled;

        public AsyncLambdaCommand(Func<Task> command)
        {
            _command = command;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (!IsEnabled) throw new NotImplementedException();
            return IsEnabled;
        }

        public async void Execute(object parameter) => await _command();

        public event EventHandler CanExecuteChanged;
    }

    public class AsyncLambdaCommand<T> : ICommand
    {
        private readonly Func<T, Task> _parameterizedCommand;
        private bool _isEnabled;

        public AsyncLambdaCommand(Func<T, Task> command)
        {
            _parameterizedCommand = command;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter) => IsEnabled;

        public async void Execute(object parameter)
        {
            await _parameterizedCommand((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}

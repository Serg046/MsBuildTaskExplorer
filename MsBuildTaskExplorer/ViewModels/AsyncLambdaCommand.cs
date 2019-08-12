using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MsBuildTaskExplorer.ViewModels
{
    public class AsyncLambdaCommand : ICommand
    {
        private readonly Func<Task> _command;

        public AsyncLambdaCommand(Func<Task> command)
        {
            _command = command;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter) => await _command();

        public event EventHandler CanExecuteChanged;
    }

    public class AsyncLambdaCommand<T> : ICommand
    {
        private readonly Func<T, Task> _parameterizedCommand;

        public AsyncLambdaCommand(Func<T, Task> command)
        {
            _parameterizedCommand = command;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter)
        {
            await _parameterizedCommand((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}

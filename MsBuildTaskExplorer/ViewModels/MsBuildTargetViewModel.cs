namespace MsBuildTaskExplorer.ViewModels
{
    internal class MsBuildTargetViewModel
    {

        public MsBuildTargetViewModel(MsBuildTaskViewModel parent, TaskExplorerViewModel baseViewModel, string target)
        {
            Parent = parent;
            Target = target;
            ExecuteTask = baseViewModel.ExecuteTask;
            PrintAllProps = baseViewModel.PrintAllProps;
            AbortTask = baseViewModel.AbortTask;
        }

        public MsBuildTaskViewModel Parent { get; }

        public string Target { get; }

        public virtual AsyncLambdaCommand<MsBuildTargetViewModel> ExecuteTask { get; }
        public virtual AsyncLambdaCommand<MsBuildTargetViewModel> PrintAllProps { get; }
        public virtual AsyncLambdaCommand AbortTask { get; }
    }
}

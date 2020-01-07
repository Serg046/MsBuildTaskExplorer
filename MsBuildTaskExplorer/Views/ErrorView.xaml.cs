using System.Windows;
using MsBuildTaskExplorer.ViewModels;

namespace MsBuildTaskExplorer.Views
{
	public partial class ErrorView : Window
	{
		private readonly ErrorViewModel _viewModel;

		public ErrorView()
		{
			InitializeComponent();
            _viewModel = ViewModelFactory.Create<ErrorViewModel>();
            DataContext = _viewModel;
		}

		public void ShowDialog(string text)
		{
			Title = TaskExplorerWindow.TITLE;
			_viewModel.Error = text;
			ShowDialog();
		}

		private void CloseButtonOnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}

using System.ComponentModel;
using System.Windows;
using AopInpc;

namespace MsBuildTaskExplorer.ViewModels
{
	internal class ErrorViewModel : INotifyPropertyChangedCaller
	{
		[Inpc]
		public virtual string Error { get; set; }

		public void CopyToClipboard()
		{
			Clipboard.SetText(Error);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

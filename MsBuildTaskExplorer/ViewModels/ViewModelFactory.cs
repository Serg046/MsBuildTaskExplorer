using AopInpc;
using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using MsBuildTaskExplorer.Views;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class ViewModelFactory
    {
        private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public static T Create<T>(params object[] args) where T : class, INotifyPropertyChangedCaller
        {
            var inpcType = typeof(T);
            Debug.Assert(Validate(inpcType), "All injected properties must be public virtual read/write allowed");
            return (T)_proxyGenerator.CreateClassProxy(inpcType, args, new InpcInterceptor(), new ExceptionInterceptor());
        }

        internal static bool Validate(Type inpcType)
        {
            foreach (var prop in inpcType.GetProperties())
            {
                if (prop.IsDefined(typeof(InpcAttribute), true) && !(prop.GetGetMethod()?.IsVirtual == true && prop.GetSetMethod()?.IsVirtual == true))
                {
                    return false;
                }
            }
            return true;
        }

        private class ExceptionInterceptor : IInterceptor
        {
            [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods")]
            public async void Intercept(IInvocation invocation)
            {
                try
                {
                    invocation.Proceed();
                    if (invocation.ReturnValue is Task task)
                    {
                        await task;
                    }
                }
                catch (Exception e)
                {
                    new ErrorView().ShowDialog(e.ToString());
                    await ReloadApp();
                }
            }

            private async Task ReloadApp()
            {
                var vm = Create<TaskExplorerViewModel>();
                TaskExplorerView.Instance.DataContext = vm;
                await vm.Initialize();
            }
        }
    }
}

using AopInpc;
using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using MsBuildTaskExplorer.Views;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class ViewModelFactory
    {
        private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public static T Create<T>(params object[] args) where T : class, INotifyPropertyChangedCaller
        {
            var inpcType = typeof(T);
            Debug.Assert(Validate(inpcType), "All injected properties must be public virtual read/write allowed");
            return (T)_proxyGenerator.CreateClassProxy(inpcType, args,
                new InpcInterceptor(), new ExceptionInterceptor().ToInterceptor());
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

        private class ExceptionInterceptor : IAsyncInterceptor
        {
            public void InterceptSynchronous(IInvocation invocation)
            {
                try
                {
                    invocation.Proceed();
                }
                catch (Exception e)
                {
                    new ErrorView().ShowDialog(e.ToString());
                    ReloadApp();
                }
            }

            public void InterceptAsynchronous(IInvocation invocation)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
            }

            private async Task InternalInterceptAsynchronous(IInvocation invocation)
            {
                try
                {
                    invocation.Proceed();
                    var task = (Task)invocation.ReturnValue;
                    await task;
                }
                catch (Exception e)
                {
                    new ErrorView().ShowDialog(e.ToString());
                    await ReloadApp();
                }
            }

            public void InterceptAsynchronous<TResult>(IInvocation invocation)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
            }

            private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
            {
                var result = default(TResult);
                try
                {
                    invocation.Proceed();
                    var task = (Task<TResult>) invocation.ReturnValue;
                    result = await task;
                }
                catch (Exception e)
                {
                    new ErrorView().ShowDialog(e.ToString());
                    await ReloadApp();
                }

                return result;
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

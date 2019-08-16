using AopInpc;
using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Windows;

namespace MsBuildTaskExplorer.ViewModels
{
    internal class ViewModelFactory
    {
        public static T Create<T>(params object[] args) where T : class, INotifyPropertyChangedCaller
        {
            var inpcType = typeof(T);
            Debug.Assert(Validate(inpcType), "All injected properties must be public virtual read/write allowed");
            return (T)new ProxyGenerator().CreateClassProxy(inpcType, args,
                new InpcInterceptor(), new ExceptionInterceptor());
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
            public void Intercept(IInvocation invocation)
            {
                try
                {
                    invocation.Proceed();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }
    }
}

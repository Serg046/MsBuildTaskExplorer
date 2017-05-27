using System.Diagnostics;
using Castle.DynamicProxy;

namespace MsBuildTaskExplorer.AppSettings
{
    internal static class AopSettings
    {
        public static T Create<T>(T settings, SettingsStore settingsStore) where T : class
        {
            Debug.Assert(Validate(settings), "All settings must be public virtual read/write allowed");
            return new ProxyGenerator().CreateClassProxyWithTarget(settings, new Interceptor(settingsStore));
        }

        private static bool Validate(object settings)
        {
            foreach (var prop in settings.GetType().GetProperties())
            {
                if (prop.IsDefined(typeof(SettingAttribute), false) && !(prop.GetGetMethod()?.IsVirtual == true && prop.GetSetMethod()?.IsVirtual == true))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

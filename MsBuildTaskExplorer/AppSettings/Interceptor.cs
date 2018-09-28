using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace MsBuildTaskExplorer.AppSettings
{
    internal class Interceptor : IInterceptor
    {
        private readonly SettingsStore _settingsStore;

        public Interceptor(SettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("get_"))
            {
                var propertyName = invocation.Method.Name.Substring(4);
                var propertyInfo = invocation.TargetType.GetProperty(propertyName);

                if (propertyInfo.IsDefined(typeof(SettingAttribute), false))
                {
                    invocation.ReturnValue = _settingsStore.Read(propertyInfo.PropertyType, propertyName);
                    if (invocation.ReturnValue == null)
                    {
                        var attribute = propertyInfo.GetCustomAttribute<SettingAttribute>();
                        invocation.ReturnValue = attribute.Default;
                    }
                }
            }
            else if (invocation.Method.Name.StartsWith("set_"))
            {
                var propertyName = invocation.Method.Name.Substring(4);
                var propertyInfo = invocation.TargetType.GetProperty(propertyName);

                if (propertyInfo.IsDefined(typeof(SettingAttribute), false))
                {
                    _settingsStore.Write(propertyInfo.PropertyType, propertyName, invocation.Arguments.Single());
                }
            }
        }
    }
}

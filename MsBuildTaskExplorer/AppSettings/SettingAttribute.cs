using System;

namespace MsBuildTaskExplorer.AppSettings
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class SettingAttribute : Attribute
    {
        public object Default { get; set; }
    }
}

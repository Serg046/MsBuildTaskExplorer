using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MsBuildTaskExplorer.AppSettings
{
    public class SettingsStore
    {
        private readonly Dictionary<Type, dynamic> _readers = new Dictionary<Type, dynamic>();
        private readonly Dictionary<Type, dynamic> _writers = new Dictionary<Type, dynamic>();

        public void RegisterReader<T>(SettingReader<T> settingReader)
        {
            _readers.Add(typeof(T), settingReader);
        }

        public void RegisterWriter<T>(SettingWriter<T> settingWriter)
        {
            _writers.Add(typeof(T), settingWriter);
        }

        internal object Read(Type settingType, string settingName)
        {
            Debug.Assert(_readers.ContainsKey(settingType), $"Reader for the type '{settingType.FullName}' is not registered");
            return _readers[settingType](settingName);
        }

        internal void Write(Type settingType, string settingName, object value)
        {
            Debug.Assert(_writers.ContainsKey(settingType), $"Writer for the type '{settingType.FullName}' is not registered");
            _writers[settingType](settingName, (dynamic)value);
        }

        public delegate T SettingReader<out T>(string name);
        public delegate void SettingWriter<in T>(string name, T value);
    }
}

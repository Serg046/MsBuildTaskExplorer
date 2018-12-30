using System.Reflection;
using AopSettings;
using Microsoft.VisualStudio.Settings;

namespace MsBuildTaskExplorer
{
    public class SettingsStoreProvider : ISettingsStoreProvider
    {
        private const string COLLECTION_PATH = "Common";
        private readonly WritableSettingsStore _store;

        public SettingsStoreProvider(WritableSettingsStore store)
        {
            _store = store;
        }

        public bool Contains(string settingName) => _store.PropertyExists(COLLECTION_PATH, settingName);

        public string GetSettingName(PropertyInfo property) => property.Name;

        public object Read(string settingName) => _store.GetString(COLLECTION_PATH, settingName);

        public void Save(string settingName, object value) => _store.SetString(COLLECTION_PATH, settingName, value.ToString());
    }
}

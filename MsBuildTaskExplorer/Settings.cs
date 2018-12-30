using AopSettings;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using SettingsStore = AopSettings.SettingsStore;

namespace MsBuildTaskExplorer
{
    public class Settings
    {
        private const string COLLECTION_PATH = "Common";

        static Settings()
        {
            var settingsManager = new ShellSettingsManager(TaskExplorerWindowCommand.Instance.ServiceProvider);
            var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!userSettingsStore.CollectionExists(COLLECTION_PATH))
                userSettingsStore.CreateCollection(COLLECTION_PATH);

            var provider = new SettingsStoreProvider(userSettingsStore);
            var settingStore = new SettingsStore(provider);
            Instance = AopSettingsFactory.Create<Settings>(settingStore);
        }

        public static Settings Instance { get; }

        [Setting]
        public virtual string Filter { get; set; }
        [Setting]
        public virtual string ExpandedTargets { get; set; }
        [Setting(Default = "*.*proj;*.targets")]
        public virtual string SupportedFileExtensions { get; set; }
    }
}

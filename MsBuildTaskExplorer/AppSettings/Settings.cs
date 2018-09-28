using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace MsBuildTaskExplorer.AppSettings
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

            var settingsStore = new SettingsStore();
            settingsStore.RegisterReader<string>(name => userSettingsStore.GetString(COLLECTION_PATH, name, null));
            settingsStore.RegisterWriter<string>((name, value) => userSettingsStore.SetString(COLLECTION_PATH, name, value));

            Instance = AopSettings.Create(new Settings(), settingsStore);
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

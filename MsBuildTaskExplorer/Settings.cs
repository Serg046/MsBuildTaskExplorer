using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsBuildTaskExplorer
{
    public class Settings
    {
        private static class SettingNames
        {
            public const string FILTER = "Filter";
            public const string EXPANDED_TARGETS = "ExpandedTargets";
        }

        public static Settings Instance { get; } = new Settings();

        public string Filter
        {
            get
            {
                return ReadSetting(SettingNames.FILTER); 
            }
            set
            {
                SaveSetting(SettingNames.FILTER, value);
            }
        }

        public string ExpandedTargets
        {
            get
            {
                return ReadSetting(SettingNames.EXPANDED_TARGETS);
            }
            set
            {
                SaveSetting(SettingNames.EXPANDED_TARGETS, value);
            }
        }

        private string ReadSetting(string settingName) => ConfigurationSettings.AppSettings[settingName];

        private void SaveSetting(string settingName, string value)
            => ConfigurationSettings.AppSettings[settingName] = value;
    }
}

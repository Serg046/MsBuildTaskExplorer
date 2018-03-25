using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MsBuildTaskExplorer
{
    public partial class TaskExplorerSettingControl : Form
    {
        Properties.Settings _settings;

        public TaskExplorerSettingControl()
        {
            InitializeComponent();

            if(_settings == null)
                _settings = new Properties.Settings();
        }

        

        private void _formSettingOkBtn_Click(object sender, EventArgs e)
        {
            _settings.setting_FileMaskPattern = _formSettingFileMaskTxb.Text;
            _settings.Save();
            _settings.Reload();
        }

        private void _formSettingCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _formSettingDefaultBtn_Click(object sender, EventArgs e)
        {
            _settings.setting_FileMaskPattern = _settings.setting_FileMaskPatternDefault;
            _formSettingFileMaskTxb.Text = _settings.setting_FileMaskPattern;

            _settings.Save();
            _settings.Reload();
        }

        private void _formSettingFileMaskTxb_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(_formSettingFileMaskTxb.Text))
                _formSettingFileMaskTxb.Text = _settings.setting_FileMaskPatternDefault;
        }
    }
}

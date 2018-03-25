namespace MsBuildTaskExplorer
{
    partial class TaskExplorerSettingControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._formSettingMaskFileGbox = new System.Windows.Forms.GroupBox();
            this._formSettingFileMaskLbl = new System.Windows.Forms.Label();
            this._formSettingFileMaskTxb = new System.Windows.Forms.TextBox();
            this._formSettingOkBtn = new System.Windows.Forms.Button();
            this._formSettingCancelBtn = new System.Windows.Forms.Button();
            this._formSettingDefaultBtn = new System.Windows.Forms.Button();
            this._formSettingMaskFileGbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _formSettingMaskFileGbox
            // 
            this._formSettingMaskFileGbox.Controls.Add(this._formSettingFileMaskLbl);
            this._formSettingMaskFileGbox.Controls.Add(this._formSettingFileMaskTxb);
            this._formSettingMaskFileGbox.Location = new System.Drawing.Point(12, 12);
            this._formSettingMaskFileGbox.Name = "_formSettingMaskFileGbox";
            this._formSettingMaskFileGbox.Size = new System.Drawing.Size(552, 79);
            this._formSettingMaskFileGbox.TabIndex = 0;
            this._formSettingMaskFileGbox.TabStop = false;
            this._formSettingMaskFileGbox.Text = "Mask";
            // 
            // _formSettingFileMaskLbl
            // 
            this._formSettingFileMaskLbl.AutoSize = true;
            this._formSettingFileMaskLbl.Location = new System.Drawing.Point(7, 22);
            this._formSettingFileMaskLbl.Name = "_formSettingFileMaskLbl";
            this._formSettingFileMaskLbl.Size = new System.Drawing.Size(346, 17);
            this._formSettingFileMaskLbl.TabIndex = 1;
            this._formSettingFileMaskLbl.Text = "For separation use semicolon. Example: *.proj;*.target";
            // 
            // _formSettingFileMaskTxb
            // 
            this._formSettingFileMaskTxb.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MsBuildTaskExplorer.Properties.Settings.Default, "setting_FileMaskPattern", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._formSettingFileMaskTxb.Location = new System.Drawing.Point(6, 51);
            this._formSettingFileMaskTxb.Name = "_formSettingFileMaskTxb";
            this._formSettingFileMaskTxb.Size = new System.Drawing.Size(540, 22);
            this._formSettingFileMaskTxb.TabIndex = 0;
            this._formSettingFileMaskTxb.Text = global::MsBuildTaskExplorer.Properties.Settings.Default.setting_FileMaskPattern;
            this._formSettingFileMaskTxb.Validating += new System.ComponentModel.CancelEventHandler(this._formSettingFileMaskTxb_Validating);
            // 
            // _formSettingOkBtn
            // 
            this._formSettingOkBtn.Location = new System.Drawing.Point(359, 97);
            this._formSettingOkBtn.Name = "_formSettingOkBtn";
            this._formSettingOkBtn.Size = new System.Drawing.Size(80, 31);
            this._formSettingOkBtn.TabIndex = 1;
            this._formSettingOkBtn.Text = "Save";
            this._formSettingOkBtn.UseVisualStyleBackColor = true;
            this._formSettingOkBtn.Click += new System.EventHandler(this._formSettingOkBtn_Click);
            // 
            // _formSettingCancelBtn
            // 
            this._formSettingCancelBtn.Location = new System.Drawing.Point(445, 97);
            this._formSettingCancelBtn.Name = "_formSettingCancelBtn";
            this._formSettingCancelBtn.Size = new System.Drawing.Size(119, 31);
            this._formSettingCancelBtn.TabIndex = 2;
            this._formSettingCancelBtn.Text = "Close";
            this._formSettingCancelBtn.UseVisualStyleBackColor = true;
            this._formSettingCancelBtn.Click += new System.EventHandler(this._formSettingCancelBtn_Click);
            // 
            // _formSettingDefaultBtn
            // 
            this._formSettingDefaultBtn.Location = new System.Drawing.Point(12, 97);
            this._formSettingDefaultBtn.Name = "_formSettingDefaultBtn";
            this._formSettingDefaultBtn.Size = new System.Drawing.Size(89, 31);
            this._formSettingDefaultBtn.TabIndex = 3;
            this._formSettingDefaultBtn.Text = "Default";
            this._formSettingDefaultBtn.UseVisualStyleBackColor = true;
            this._formSettingDefaultBtn.Click += new System.EventHandler(this._formSettingDefaultBtn_Click);
            // 
            // TaskExplorerSettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 140);
            this.Controls.Add(this._formSettingDefaultBtn);
            this.Controls.Add(this._formSettingCancelBtn);
            this.Controls.Add(this._formSettingOkBtn);
            this.Controls.Add(this._formSettingMaskFileGbox);
            this.Name = "TaskExplorerSettingControl";
            this.Text = "MSBuild Task Explorer setting";
            this._formSettingMaskFileGbox.ResumeLayout(false);
            this._formSettingMaskFileGbox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _formSettingMaskFileGbox;
        private System.Windows.Forms.Label _formSettingFileMaskLbl;
        private System.Windows.Forms.TextBox _formSettingFileMaskTxb;
        private System.Windows.Forms.Button _formSettingOkBtn;
        private System.Windows.Forms.Button _formSettingCancelBtn;
        private System.Windows.Forms.Button _formSettingDefaultBtn;
    }
}
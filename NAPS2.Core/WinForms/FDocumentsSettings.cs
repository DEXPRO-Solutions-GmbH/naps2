using NAPS2.Config;
using NAPS2.ImportExport.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public partial class FDocumentsSettings : FormBase
    {
        private readonly DocumentsSettingsContainer documentsSettingsContainer;
        private readonly IUserConfigManager userConfigManager;

        public FDocumentsSettings(DocumentsSettingsContainer documentsSettingsContainer, IUserConfigManager userConfigManager)
        {
            this.documentsSettingsContainer = documentsSettingsContainer;
            this.userConfigManager = userConfigManager;
            InitializeComponent();
        }

        private void FDocumentsSettings_Load(object sender, EventArgs e)
        {
            UpdateValues(documentsSettingsContainer.DocumentsSettings);
            cbRememberSettings.Checked = userConfigManager.Config.DocumentsSettings != null;
        }

        private void UpdateValues(DocumentsSettings documentsSettings)
        {
            txtUrl.Text = documentsSettings.ServerUrl;
            txtPri.Text = documentsSettings.Principal;
            txtUser.Text = documentsSettings.Username;
            txtPassword.Text = documentsSettings.Password;
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            var documentsSettings = new DocumentsSettings
            {
                ServerUrl = txtUrl.Text,
                Principal = txtPri.Text,
                Username = txtUser.Text,
                Password = txtPassword.Text
            };

            documentsSettingsContainer.DocumentsSettings = documentsSettings;
            userConfigManager.Config.DocumentsSettings = cbRememberSettings.Checked ? documentsSettings : null;
            userConfigManager.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                btnOk_Click(sender, e);
            }
        }
    }
}

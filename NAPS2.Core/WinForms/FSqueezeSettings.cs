using NAPS2.Config;
using NAPS2.ImportExport.Squeeze;
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
    public partial class FSqueezeSettings : FormBase
    {
        private readonly SqueezeSettingsContainer settingsContainer;
        private readonly IUserConfigManager userConfigManager;

        public FSqueezeSettings(SqueezeSettingsContainer settingsContainer, IUserConfigManager userConfigManager)
        {
            this.settingsContainer = settingsContainer;
            this.userConfigManager = userConfigManager;
            InitializeComponent();
        }

        private void FSqueezeSettings_Load(object sender, EventArgs e)
        {
            UpdateValues(settingsContainer.SqueezeSettings);
            cbRememberSettings.Checked = userConfigManager.Config.SqueezeSettings != null;
        }

        private void UpdateValues(SqueezeSettings settings)
        {
            txtUrl.Text = settings.ServerUrl;
            txtBatchClass.Text = settings.BatchClassId;
            txtBarcodeFieldName.Text = settings.BarcodeFieldName;
            txtPri.Text = settings.Client;
            txtUser.Text = settings.Username;
            txtPassword.Text = settings.Password;
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            var settings = new SqueezeSettings
            {
                ServerUrl = txtUrl.Text,
                Client = txtPri.Text,
                Username = txtUser.Text,
                Password = txtPassword.Text,
                BatchClassId = txtBatchClass.Text,
                BarcodeFieldName = txtBarcodeFieldName.Text
            };

            settingsContainer.SqueezeSettings = settings;
            userConfigManager.Config.SqueezeSettings = cbRememberSettings.Checked ? settings : null;
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

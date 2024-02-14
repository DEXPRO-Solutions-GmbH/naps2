using NAPS2.Barcode;
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
    public partial class FBarcodeSettings : FormBase
    {
        private readonly BarcodeSettingsContainer barcodeSettingsContainer;
        private readonly IUserConfigManager userConfigManager;

        public FBarcodeSettings(BarcodeSettingsContainer barcodeSettingsContainer, IUserConfigManager userConfigManager)
        {
            this.barcodeSettingsContainer = barcodeSettingsContainer;
            this.userConfigManager = userConfigManager;
            InitializeComponent();
            foreach (var name in Enum.GetValues(typeof(BarcodeFormat)))
                checkListTypes.Items.Add(name);
        }

        private void FDocumentsSettings_Load(object sender, EventArgs e)
        {
            UpdateValues(barcodeSettingsContainer.BarcodeParams);
            cbRememberSettings.Checked = userConfigManager.Config.BarcodeParams != null;
        }

        private void UpdateValues(BarcodeParams barcodeParams)
        {
            txtRegex.Text = barcodeParams?.RegexFilter ?? "";
            txtInvalidValues.Text = string.Join("\n", barcodeParams?.ValueBlacklist ?? new string[0]);
            gridRegions.Rows.Clear();
            foreach (var region in barcodeParams?.RegionFilter)
            {
                var rowIndex = gridRegions.Rows.Add();
                gridRegions.Rows[rowIndex].SetValues((region.X * 100).ToString(), (region.Y * 100).ToString(), (region.Width * 100).ToString(), (region.Height * 100).ToString());
            }
            var checkListItems = checkListTypes.Items.Cast<BarcodeFormat>().ToArray();
            for(var i = 0; i < checkListItems.Length; i++)
            {
                checkListTypes.SetItemChecked(i, barcodeParams?.AllowedFormatFilter?.Contains(checkListItems[i]) ?? false);
            }
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            var barcodeParams = new BarcodeParams
            {
                AllowedFormatFilter = checkListTypes.CheckedItems.Cast<BarcodeFormat>().ToArray(),
                RegexFilter = txtRegex.Text,
                ValueBlacklist = txtInvalidValues.Text.Split('\n'),
                RegionFilter = gridRegions.Rows.Cast<DataGridViewRow>()
                    .Select
                    (
                        r => r.Cells.Cast<DataGridViewCell>()
                            .Select(c => float.TryParse((string)c.Value, out var res) ? res : 0)
                            .ToArray()
                    )
                    .Where(c => c.Any(v => v != 0))
                    .Select(c => new RectangleF(c[0] / 100, c[1] / 100, c[2] / 100, c[3] / 100))
                    .ToArray()
            };

            barcodeSettingsContainer.BarcodeParams = barcodeParams;
            userConfigManager.Config.BarcodeParams = cbRememberSettings.Checked ? barcodeParams : null;
            userConfigManager.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

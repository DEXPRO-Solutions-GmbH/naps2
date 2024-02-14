namespace NAPS2.WinForms
{
    partial class FBarcodeSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FBarcodeSettings));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtRegex = new System.Windows.Forms.TextBox();
            this.txtInvalidValues = new System.Windows.Forms.TextBox();
            this.checkListTypes = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gridRegions = new System.Windows.Forms.DataGridView();
            this.x = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.height = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbRememberSettings = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegions)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.txtRegex, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtInvalidValues, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkListTypes, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.gridRegions, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // txtRegex
            // 
            resources.ApplyResources(this.txtRegex, "txtRegex");
            this.txtRegex.Name = "txtRegex";
            // 
            // txtInvalidValues
            // 
            resources.ApplyResources(this.txtInvalidValues, "txtInvalidValues");
            this.txtInvalidValues.Name = "txtInvalidValues";
            this.txtInvalidValues.UseSystemPasswordChar = true;
            // 
            // checkListTypes
            // 
            resources.ApplyResources(this.checkListTypes, "checkListTypes");
            this.checkListTypes.FormattingEnabled = true;
            this.checkListTypes.Name = "checkListTypes";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // gridRegions
            // 
            resources.ApplyResources(this.gridRegions, "gridRegions");
            this.gridRegions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRegions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.x,
            this.y,
            this.width,
            this.height});
            this.gridRegions.Name = "gridRegions";
            this.gridRegions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // x
            // 
            resources.ApplyResources(this.x, "x");
            this.x.Name = "x";
            // 
            // y
            // 
            resources.ApplyResources(this.y, "y");
            this.y.Name = "y";
            // 
            // width
            // 
            resources.ApplyResources(this.width, "width");
            this.width.Name = "width";
            // 
            // height
            // 
            resources.ApplyResources(this.height, "height");
            this.height.Name = "height";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbRememberSettings
            // 
            resources.ApplyResources(this.cbRememberSettings, "cbRememberSettings");
            this.cbRememberSettings.Name = "cbRememberSettings";
            this.cbRememberSettings.UseVisualStyleBackColor = true;
            // 
            // FBarcodeSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cbRememberSettings);
            this.Name = "FBarcodeSettings";
            this.Load += new System.EventHandler(this.FDocumentsSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRegex;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbRememberSettings;
        private System.Windows.Forms.CheckedListBox checkListTypes;
        private System.Windows.Forms.TextBox txtInvalidValues;
        private System.Windows.Forms.DataGridView gridRegions;
        private System.Windows.Forms.DataGridViewTextBoxColumn x;
        private System.Windows.Forms.DataGridViewTextBoxColumn y;
        private System.Windows.Forms.DataGridViewTextBoxColumn width;
        private System.Windows.Forms.DataGridViewTextBoxColumn height;
    }
}
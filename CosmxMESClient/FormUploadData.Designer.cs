using System.Drawing;
using System.Windows.Forms;

namespace CosmxMESClient
{
    partial class FormUploadData
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.dgvUploadItems = new System.Windows.Forms.DataGridView();
            this.grpItemDetail = new System.Windows.Forms.GroupBox();
            this.txtParameterValue = new System.Windows.Forms.TextBox();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.chkIsNullable = new System.Windows.Forms.CheckBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.chkIsUpload = new System.Windows.Forms.CheckBox();
            this.txtParameterName = new System.Windows.Forms.TextBox();
            this.lblFormatExample = new System.Windows.Forms.Label();
            this.lblLength = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblParameterName = new System.Windows.Forms.Label();
            this.panelActions = new System.Windows.Forms.Panel();
            this.chkEnableMES = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboPLCScanAddress = new System.Windows.Forms.ComboBox();
            this.colParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsUpload = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsNullable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFormatExample = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLCScanAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUploadItems)).BeginInit();
            this.grpItemDetail.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.dgvUploadItems);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.grpItemDetail);
            this.splitContainerMain.Panel2.Controls.Add(this.panelActions);
            this.splitContainerMain.Size = new System.Drawing.Size(1295, 655);
            this.splitContainerMain.SplitterDistance = 1009;
            this.splitContainerMain.TabIndex = 0;
            // 
            // dgvUploadItems
            // 
            this.dgvUploadItems.AllowUserToAddRows = false;
            this.dgvUploadItems.AllowUserToDeleteRows = false;
            this.dgvUploadItems.AllowUserToResizeRows = false;
            this.dgvUploadItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUploadItems.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUploadItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvUploadItems.ColumnHeadersHeight = 40;
            this.dgvUploadItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colParameterName,
            this.colIsUpload,
            this.colDescription,
            this.colType,
            this.colIsNullable,
            this.colLength,
            this.colFormatExample,
            this.PLCScanAddress});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvUploadItems.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvUploadItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUploadItems.GridColor = System.Drawing.Color.LightGray;
            this.dgvUploadItems.Location = new System.Drawing.Point(0, 0);
            this.dgvUploadItems.MultiSelect = false;
            this.dgvUploadItems.Name = "dgvUploadItems";
            this.dgvUploadItems.ReadOnly = true;
            this.dgvUploadItems.RowHeadersVisible = false;
            this.dgvUploadItems.RowTemplate.Height = 35;
            this.dgvUploadItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUploadItems.Size = new System.Drawing.Size(1009, 655);
            this.dgvUploadItems.TabIndex = 0;
            // 
            // grpItemDetail
            // 
            this.grpItemDetail.Controls.Add(this.txtParameterValue);
            this.grpItemDetail.Controls.Add(this.txtLength);
            this.grpItemDetail.Controls.Add(this.cboPLCScanAddress);
            this.grpItemDetail.Controls.Add(this.cboType);
            this.grpItemDetail.Controls.Add(this.chkIsNullable);
            this.grpItemDetail.Controls.Add(this.txtDescription);
            this.grpItemDetail.Controls.Add(this.chkIsUpload);
            this.grpItemDetail.Controls.Add(this.label1);
            this.grpItemDetail.Controls.Add(this.txtParameterName);
            this.grpItemDetail.Controls.Add(this.lblFormatExample);
            this.grpItemDetail.Controls.Add(this.lblLength);
            this.grpItemDetail.Controls.Add(this.lblType);
            this.grpItemDetail.Controls.Add(this.lblDescription);
            this.grpItemDetail.Controls.Add(this.lblParameterName);
            this.grpItemDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpItemDetail.Location = new System.Drawing.Point(0, 0);
            this.grpItemDetail.Name = "grpItemDetail";
            this.grpItemDetail.Padding = new System.Windows.Forms.Padding(10);
            this.grpItemDetail.Size = new System.Drawing.Size(282, 520);
            this.grpItemDetail.TabIndex = 0;
            this.grpItemDetail.TabStop = false;
            this.grpItemDetail.Text = "上传项设置";
            // 
            // txtParameterValue
            // 
            this.txtParameterValue.Location = new System.Drawing.Point(100, 165);
            this.txtParameterValue.Name = "txtParameterValue";
            this.txtParameterValue.Size = new System.Drawing.Size(164, 21);
            this.txtParameterValue.TabIndex = 11;
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(100, 135);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(164, 21);
            this.txtLength.TabIndex = 10;
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(101, 106);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(164, 20);
            this.cboType.TabIndex = 9;
            // 
            // chkIsNullable
            // 
            this.chkIsNullable.AutoSize = true;
            this.chkIsNullable.Location = new System.Drawing.Point(100, 237);
            this.chkIsNullable.Name = "chkIsNullable";
            this.chkIsNullable.Size = new System.Drawing.Size(120, 16);
            this.chkIsNullable.TabIndex = 8;
            this.chkIsNullable.Text = "当前参数是否可空";
            this.chkIsNullable.UseVisualStyleBackColor = true;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(101, 76);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(164, 21);
            this.txtDescription.TabIndex = 7;
            // 
            // chkIsUpload
            // 
            this.chkIsUpload.AutoSize = true;
            this.chkIsUpload.Location = new System.Drawing.Point(101, 21);
            this.chkIsUpload.Name = "chkIsUpload";
            this.chkIsUpload.Size = new System.Drawing.Size(72, 16);
            this.chkIsUpload.TabIndex = 6;
            this.chkIsUpload.Text = "是否上传";
            this.chkIsUpload.UseVisualStyleBackColor = true;
            // 
            // txtParameterName
            // 
            this.txtParameterName.Location = new System.Drawing.Point(100, 46);
            this.txtParameterName.Name = "txtParameterName";
            this.txtParameterName.Size = new System.Drawing.Size(164, 21);
            this.txtParameterName.TabIndex = 5;
            // 
            // lblFormatExample
            // 
            this.lblFormatExample.AutoSize = true;
            this.lblFormatExample.Location = new System.Drawing.Point(15, 170);
            this.lblFormatExample.Name = "lblFormatExample";
            this.lblFormatExample.Size = new System.Drawing.Size(53, 12);
            this.lblFormatExample.TabIndex = 4;
            this.lblFormatExample.Text = "参数值：";
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(15, 140);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(41, 12);
            this.lblLength.TabIndex = 3;
            this.lblLength.Text = "长度：";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(16, 111);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(65, 12);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "参数类型：";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(16, 81);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(65, 12);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "参数描述：";
            // 
            // lblParameterName
            // 
            this.lblParameterName.AutoSize = true;
            this.lblParameterName.Location = new System.Drawing.Point(15, 51);
            this.lblParameterName.Name = "lblParameterName";
            this.lblParameterName.Size = new System.Drawing.Size(53, 12);
            this.lblParameterName.TabIndex = 0;
            this.lblParameterName.Text = "参数名：";
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add(this.chkEnableMES);
            this.panelActions.Controls.Add(this.btnAdd);
            this.panelActions.Controls.Add(this.btnDelete);
            this.panelActions.Controls.Add(this.btnImport);
            this.panelActions.Controls.Add(this.btnExport);
            this.panelActions.Controls.Add(this.btnSave);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(0, 520);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(282, 135);
            this.panelActions.TabIndex = 1;
            // 
            // chkEnableMES
            // 
            this.chkEnableMES.AutoSize = true;
            this.chkEnableMES.Location = new System.Drawing.Point(15, 106);
            this.chkEnableMES.Name = "chkEnableMES";
            this.chkEnableMES.Size = new System.Drawing.Size(114, 16);
            this.chkEnableMES.TabIndex = 5;
            this.chkEnableMES.Text = "启用MES数据上传";
            this.chkEnableMES.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(15, 11);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(102, 30);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "添加项";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(164, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(102, 30);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "删除项";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(15, 47);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(102, 30);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "导入配置";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(163, 47);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(102, 30);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "导出配置";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(163, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 30);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "保存配置";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "PLC扫描地址：";
            // 
            // cboPLCScanAddress
            // 
            this.cboPLCScanAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPLCScanAddress.FormattingEnabled = true;
            this.cboPLCScanAddress.Location = new System.Drawing.Point(101, 196);
            this.cboPLCScanAddress.Name = "cboPLCScanAddress";
            this.cboPLCScanAddress.Size = new System.Drawing.Size(164, 20);
            this.cboPLCScanAddress.TabIndex = 9;
            // 
            // colParameterName
            // 
            this.colParameterName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colParameterName.FillWeight = 109.3466F;
            this.colParameterName.HeaderText = "参数名";
            this.colParameterName.Name = "colParameterName";
            this.colParameterName.ReadOnly = true;
            this.colParameterName.Width = 180;
            // 
            // colIsUpload
            // 
            this.colIsUpload.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colIsUpload.FillWeight = 62.96624F;
            this.colIsUpload.HeaderText = "是否上传";
            this.colIsUpload.Name = "colIsUpload";
            this.colIsUpload.ReadOnly = true;
            this.colIsUpload.Width = 52;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colDescription.FillWeight = 158.972F;
            this.colDescription.HeaderText = "参数描述";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 120;
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colType.FillWeight = 263.5349F;
            this.colType.HeaderText = "参数类型";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colIsNullable
            // 
            this.colIsNullable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colIsNullable.FillWeight = 227.0126F;
            this.colIsNullable.HeaderText = "是否可空";
            this.colIsNullable.Name = "colIsNullable";
            this.colIsNullable.ReadOnly = true;
            this.colIsNullable.Width = 52;
            // 
            // colLength
            // 
            this.colLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colLength.FillWeight = 9.752413F;
            this.colLength.HeaderText = "长度";
            this.colLength.Name = "colLength";
            this.colLength.ReadOnly = true;
            this.colLength.Width = 60;
            // 
            // colFormatExample
            // 
            this.colFormatExample.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colFormatExample.FillWeight = 28.44379F;
            this.colFormatExample.HeaderText = "参数值";
            this.colFormatExample.Name = "colFormatExample";
            this.colFormatExample.ReadOnly = true;
            // 
            // PLCScanAddress
            // 
            this.PLCScanAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PLCScanAddress.HeaderText = "PLC扫描地址";
            this.PLCScanAddress.Name = "PLCScanAddress";
            this.PLCScanAddress.ReadOnly = true;
            this.PLCScanAddress.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // FormUploadData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1295, 655);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "FormUploadData";
            this.Text = "参数上传数据配置";
            this.Load += new System.EventHandler(this.FormUploadData_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUploadItems)).EndInit();
            this.grpItemDetail.ResumeLayout(false);
            this.grpItemDetail.PerformLayout();
            this.panelActions.ResumeLayout(false);
            this.panelActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.DataGridView dgvUploadItems;
        private System.Windows.Forms.GroupBox grpItemDetail;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.CheckBox chkEnableMES;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtParameterValue;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.CheckBox chkIsNullable;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.CheckBox chkIsUpload;
        private System.Windows.Forms.TextBox txtParameterName;
        private System.Windows.Forms.Label lblFormatExample;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblParameterName;
        private ComboBox cboPLCScanAddress;
        private Label label1;
        private DataGridViewTextBoxColumn colParameterName;
        private DataGridViewCheckBoxColumn colIsUpload;
        private DataGridViewTextBoxColumn colDescription;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewCheckBoxColumn colIsNullable;
        private DataGridViewTextBoxColumn colLength;
        private DataGridViewTextBoxColumn colFormatExample;
        private DataGridViewTextBoxColumn PLCScanAddress;
    }
}

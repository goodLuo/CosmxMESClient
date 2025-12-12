namespace CosmxMESClient {
    partial class FormPLCScanAddress {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose( bool disposing ) {
            if (disposing&&( components!=null )) {
                components.Dispose( );
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        private void InitializeComponent( ) {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblPower = new System.Windows.Forms.Label();
            this.numPower = new System.Windows.Forms.NumericUpDown();
            this.lblLength = new System.Windows.Forms.Label();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.grpButtons = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            this.grpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblAddress, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtAddress, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDataType, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbDataType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtDescription, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblInterval, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numInterval, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPower, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.numPower, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblLength, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.numLength, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkEnabled, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 300);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddress.Location = new System.Drawing.Point(3, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(114, 40);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "地址:";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.Location = new System.Drawing.Point(123, 3);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(274, 30);
            this.txtAddress.TabIndex = 1;
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataType.Location = new System.Drawing.Point(3, 40);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(114, 40);
            this.lblDataType.TabIndex = 2;
            this.lblDataType.Text = "数据类型:";
            this.lblDataType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDataType
            // 
            this.cmbDataType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(123, 43);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(274, 32);
            this.cmbDataType.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 80);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(114, 40);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "描述:";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(123, 83);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(274, 30);
            this.txtDescription.TabIndex = 5;
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInterval.Location = new System.Drawing.Point(3, 120);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(114, 40);
            this.lblInterval.TabIndex = 6;
            this.lblInterval.Text = "扫描间隔(ms):";
            this.lblInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numInterval
            // 
            this.numInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numInterval.Location = new System.Drawing.Point(123, 123);
            this.numInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(274, 30);
            this.numInterval.TabIndex = 7;
            this.numInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPower.Location = new System.Drawing.Point(3, 160);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(114, 40);
            this.lblPower.TabIndex = 8;
            this.lblPower.Text = "缩放系数:";
            this.lblPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numPower
            // 
            this.numPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPower.Location = new System.Drawing.Point(123, 163);
            this.numPower.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPower.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPower.Name = "numPower";
            this.numPower.Size = new System.Drawing.Size(274, 30);
            this.numPower.TabIndex = 9;
            this.numPower.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLength.Location = new System.Drawing.Point(3, 200);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(114, 40);
            this.lblLength.TabIndex = 10;
            this.lblLength.Text = "数据长度:";
            this.lblLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numLength
            // 
            this.numLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numLength.Location = new System.Drawing.Point(123, 203);
            this.numLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(274, 30);
            this.numLength.TabIndex = 11;
            this.numLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkEnabled, 2);
            this.chkEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnabled.Location = new System.Drawing.Point(3, 243);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(394, 34);
            this.chkEnabled.TabIndex = 12;
            this.chkEnabled.Text = "启用扫描";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // grpButtons
            // 
            this.grpButtons.Controls.Add(this.btnCancel);
            this.grpButtons.Controls.Add(this.btnOK);
            this.grpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpButtons.Location = new System.Drawing.Point(0, 300);
            this.grpButtons.Name = "grpButtons";
            this.grpButtons.Size = new System.Drawing.Size(400, 50);
            this.grpButtons.TabIndex = 1;
            this.grpButtons.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightGray;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(315, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightBlue;
            this.btnOK.Location = new System.Drawing.Point(234, 15);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormPLCScanAddress
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.grpButtons);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPLCScanAddress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "扫描地址配置";
            this.Load += new System.EventHandler(this.FormPLCScanAddress_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            this.grpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

            }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.NumericUpDown numPower;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.GroupBox grpButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        }
    }
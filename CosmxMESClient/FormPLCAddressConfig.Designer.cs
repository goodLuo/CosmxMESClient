namespace CosmxMESClient {
    partial class FormPLCAddressConfig {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if (disposing&&( components!=null )) {
                components.Dispose( );
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( ) {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "示例键值",
            "示例名称",
            "D100",
            "整数",
            "示例描述",
            "1000",
            "启用",
            "无条件",
            "2024-01-01 10:00:00"}, -1);
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpAddressList = new System.Windows.Forms.GroupBox();
            this.lvAddresses = new System.Windows.Forms.ListView();
            this.grpAddressConfig = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblReadInterval = new System.Windows.Forms.Label();
            this.numReadInterval = new System.Windows.Forms.NumericUpDown();
            this.lblPower = new System.Windows.Forms.Label();
            this.numPower = new System.Windows.Forms.NumericUpDown();
            this.lblLength = new System.Windows.Forms.Label();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.chkAutoSend = new System.Windows.Forms.CheckBox();
            this.lblSendDelay = new System.Windows.Forms.Label();
            this.numSendDelay = new System.Windows.Forms.NumericUpDown();
            this.grpTriggerConditions = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTriggerCondition = new System.Windows.Forms.Label();
            this.cmbTriggerCondition = new System.Windows.Forms.ComboBox();
            this.lblTriggerThreshold = new System.Windows.Forms.Label();
            this.numTriggerThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblTriggerDelay = new System.Windows.Forms.Label();
            this.numTriggerDelay = new System.Windows.Forms.NumericUpDown();
            this.chkTriggerRisingEdge = new System.Windows.Forms.CheckBox();
            this.chkTriggerFallingEdge = new System.Windows.Forms.CheckBox();
            this.lblTriggerHelp = new System.Windows.Forms.Label();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.grpFormActions = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnTestAddress = new System.Windows.Forms.Button();
            this.btnTestTrigger = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labTriggerAddress = new System.Windows.Forms.Label();
            this.cmbTriggerAddress = new System.Windows.Forms.ComboBox();
            this.btnRefreshAddress = new System.Windows.Forms.Button();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtStringThreshold = new System.Windows.Forms.TextBox();
            this.numConsecutiveCount = new System.Windows.Forms.NumericUpDown();
            this.numCooldownSeconds = new System.Windows.Forms.NumericUpDown();
            this.chkResetOnSuccess = new System.Windows.Forms.CheckBox();
            this.lblConsecutiveCount = new System.Windows.Forms.Label();
            this.lblStringThreshold = new System.Windows.Forms.Label();
            this.lblCooldownSeconds = new System.Windows.Forms.Label();
            this.grpAddressList.SuspendLayout();
            this.grpAddressConfig.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSendDelay)).BeginInit();
            this.grpTriggerConditions.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTriggerThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTriggerDelay)).BeginInit();
            this.grpActions.SuspendLayout();
            this.grpFormActions.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConsecutiveCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCooldownSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "键值";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "地址";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "数据类型";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "描述";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "读取间隔(ms)";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "状态";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "触发条件";
            this.columnHeader8.Width = 196;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "触发阈值";
            this.columnHeader9.Width = 157;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "触发状态";
            this.columnHeader10.Width = 206;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "最后触发时间";
            this.columnHeader11.Width = 120;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "触发延迟(ms)";
            this.columnHeader12.Width = 100;
            // 
            // grpAddressList
            // 
            this.grpAddressList.Controls.Add(this.lvAddresses);
            this.grpAddressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAddressList.Location = new System.Drawing.Point(3, 3);
            this.grpAddressList.Name = "grpAddressList";
            this.grpAddressList.Size = new System.Drawing.Size(1476, 342);
            this.grpAddressList.TabIndex = 0;
            this.grpAddressList.TabStop = false;
            this.grpAddressList.Text = "地址列表";
            // 
            // lvAddresses
            // 
            this.lvAddresses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13});
            this.lvAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAddresses.FullRowSelect = true;
            this.lvAddresses.GridLines = true;
            this.lvAddresses.HideSelection = false;
            this.lvAddresses.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lvAddresses.Location = new System.Drawing.Point(3, 26);
            this.lvAddresses.Name = "lvAddresses";
            this.lvAddresses.Size = new System.Drawing.Size(1470, 313);
            this.lvAddresses.TabIndex = 0;
            this.lvAddresses.UseCompatibleStateImageBehavior = false;
            this.lvAddresses.View = System.Windows.Forms.View.Details;
            this.lvAddresses.SelectedIndexChanged += new System.EventHandler(this.lvAddresses_SelectedIndexChanged);
            // 
            // grpAddressConfig
            // 
            this.grpAddressConfig.Controls.Add(this.tableLayoutPanel1);
            this.grpAddressConfig.Location = new System.Drawing.Point(3, 621);
            this.grpAddressConfig.Name = "grpAddressConfig";
            this.grpAddressConfig.Size = new System.Drawing.Size(1192, 204);
            this.grpAddressConfig.TabIndex = 1;
            this.grpAddressConfig.TabStop = false;
            this.grpAddressConfig.Text = "地址基本配置";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAddress, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtAddress, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDataType, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbDataType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtDescription, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblReadInterval, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.numReadInterval, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPower, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.numPower, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblLength, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numLength, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkEnabled, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkAutoSend, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSendDelay, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.numSendDelay, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1186, 175);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(3, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(114, 35);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "名称:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(123, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(467, 30);
            this.txtName.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtName, "地址的唯一标识名称");
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddress.Location = new System.Drawing.Point(596, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(114, 35);
            this.lblAddress.TabIndex = 2;
            this.lblAddress.Text = "PLC地址:";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.Location = new System.Drawing.Point(716, 3);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(467, 30);
            this.txtAddress.TabIndex = 3;
            this.toolTip.SetToolTip(this.txtAddress, "PLC设备地址，如：D100, M200, Y0等");
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataType.Location = new System.Drawing.Point(3, 35);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(114, 35);
            this.lblDataType.TabIndex = 4;
            this.lblDataType.Text = "数据类型:";
            this.lblDataType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDataType
            // 
            this.cmbDataType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Items.AddRange(new object[] {
            "Boolean",
            "Int16",
            "Int32",
            "Single",
            "Double",
            "String"});
            this.cmbDataType.Location = new System.Drawing.Point(123, 38);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(467, 32);
            this.cmbDataType.TabIndex = 5;
            this.toolTip.SetToolTip(this.cmbDataType, "选择要读取的数据类型");
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(596, 35);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(114, 35);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "描述:";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(716, 38);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(467, 30);
            this.txtDescription.TabIndex = 7;
            this.toolTip.SetToolTip(this.txtDescription, "地址的详细描述信息");
            // 
            // lblReadInterval
            // 
            this.lblReadInterval.AutoSize = true;
            this.lblReadInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReadInterval.Location = new System.Drawing.Point(3, 70);
            this.lblReadInterval.Name = "lblReadInterval";
            this.lblReadInterval.Size = new System.Drawing.Size(114, 35);
            this.lblReadInterval.TabIndex = 8;
            this.lblReadInterval.Text = "读取间隔(ms):";
            this.lblReadInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numReadInterval
            // 
            this.numReadInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numReadInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numReadInterval.Location = new System.Drawing.Point(123, 73);
            this.numReadInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numReadInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numReadInterval.Name = "numReadInterval";
            this.numReadInterval.Size = new System.Drawing.Size(467, 30);
            this.numReadInterval.TabIndex = 9;
            this.toolTip.SetToolTip(this.numReadInterval, "数据读取的时间间隔，单位毫秒");
            this.numReadInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPower.Location = new System.Drawing.Point(596, 70);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(114, 35);
            this.lblPower.TabIndex = 10;
            this.lblPower.Text = "缩放系数:";
            this.lblPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numPower
            // 
            this.numPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPower.Location = new System.Drawing.Point(716, 73);
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
            this.numPower.Size = new System.Drawing.Size(467, 30);
            this.numPower.TabIndex = 11;
            this.toolTip.SetToolTip(this.numPower, "数据缩放系数，用于精度调整");
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
            this.lblLength.Location = new System.Drawing.Point(3, 105);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(114, 35);
            this.lblLength.TabIndex = 12;
            this.lblLength.Text = "数据长度:";
            this.lblLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numLength
            // 
            this.numLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numLength.Location = new System.Drawing.Point(123, 108);
            this.numLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(467, 30);
            this.numLength.TabIndex = 13;
            this.toolTip.SetToolTip(this.numLength, "数据长度，用于数组或字符串类型");
            this.numLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnabled.Location = new System.Drawing.Point(596, 108);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(114, 29);
            this.chkEnabled.TabIndex = 14;
            this.chkEnabled.Text = "启用";
            this.toolTip.SetToolTip(this.chkEnabled, "启用或禁用该地址的扫描");
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // chkAutoSend
            // 
            this.chkAutoSend.AutoSize = true;
            this.chkAutoSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAutoSend.Location = new System.Drawing.Point(716, 108);
            this.chkAutoSend.Name = "chkAutoSend";
            this.chkAutoSend.Size = new System.Drawing.Size(467, 29);
            this.chkAutoSend.TabIndex = 15;
            this.chkAutoSend.Text = "自动发送";
            this.toolTip.SetToolTip(this.chkAutoSend, "是否启用自动发送功能");
            this.chkAutoSend.UseVisualStyleBackColor = true;
            // 
            // lblSendDelay
            // 
            this.lblSendDelay.AutoSize = true;
            this.lblSendDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSendDelay.Location = new System.Drawing.Point(3, 140);
            this.lblSendDelay.Name = "lblSendDelay";
            this.lblSendDelay.Size = new System.Drawing.Size(114, 35);
            this.lblSendDelay.TabIndex = 16;
            this.lblSendDelay.Text = "发送延迟(ms):";
            this.lblSendDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numSendDelay
            // 
            this.numSendDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSendDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numSendDelay.Location = new System.Drawing.Point(123, 143);
            this.numSendDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numSendDelay.Name = "numSendDelay";
            this.numSendDelay.Size = new System.Drawing.Size(467, 30);
            this.numSendDelay.TabIndex = 17;
            this.toolTip.SetToolTip(this.numSendDelay, "数据发送的延迟时间，单位毫秒");
            // 
            // grpTriggerConditions
            // 
            this.grpTriggerConditions.Controls.Add(this.tableLayoutPanel2);
            this.grpTriggerConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTriggerConditions.Location = new System.Drawing.Point(3, 411);
            this.grpTriggerConditions.Name = "grpTriggerConditions";
            this.grpTriggerConditions.Size = new System.Drawing.Size(1476, 204);
            this.grpTriggerConditions.TabIndex = 2;
            this.grpTriggerConditions.TabStop = false;
            this.grpTriggerConditions.Text = "触发条件配置";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.Controls.Add(this.lblTriggerCondition, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmbTriggerCondition, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTriggerThreshold, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.numTriggerThreshold, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTriggerHelp, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.labTriggerAddress, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.cmbTriggerAddress, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnRefreshAddress, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblTriggerDelay, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.numTriggerDelay, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtStringThreshold, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkTriggerFallingEdge, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkTriggerRisingEdge, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.numCooldownSeconds, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.numConsecutiveCount, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkResetOnSuccess, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblConsecutiveCount, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblStringThreshold, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblCooldownSeconds, 4, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1470, 175);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblTriggerCondition
            // 
            this.lblTriggerCondition.AutoSize = true;
            this.lblTriggerCondition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTriggerCondition.Location = new System.Drawing.Point(3, 0);
            this.lblTriggerCondition.Name = "lblTriggerCondition";
            this.lblTriggerCondition.Size = new System.Drawing.Size(114, 35);
            this.lblTriggerCondition.TabIndex = 0;
            this.lblTriggerCondition.Text = "触发条件:";
            this.lblTriggerCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbTriggerCondition
            // 
            this.cmbTriggerCondition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbTriggerCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTriggerCondition.FormattingEnabled = true;
            this.cmbTriggerCondition.Items.AddRange(new object[] {
            "无触发条件",
            "大于阈值触发",
            "小于阈值触发",
            "等于阈值触发",
            "大于等于阈值触发",
            "小于等于阈值触发",
            "不等于阈值触发",
            "上升沿触发",
            "下降沿触发",
            "变化百分比触发",
            "包含字符串触发",
            "以...开始触发",
            "以...结束触发"});
            this.cmbTriggerCondition.Location = new System.Drawing.Point(123, 3);
            this.cmbTriggerCondition.Name = "cmbTriggerCondition";
            this.cmbTriggerCondition.Size = new System.Drawing.Size(804, 32);
            this.cmbTriggerCondition.TabIndex = 1;
            this.toolTip.SetToolTip(this.cmbTriggerCondition, "选择数据读取的触发条件");
            this.cmbTriggerCondition.SelectedIndexChanged += new System.EventHandler(this.cmbTriggerCondition_SelectedIndexChanged);
            // 
            // lblTriggerThreshold
            // 
            this.lblTriggerThreshold.AutoSize = true;
            this.lblTriggerThreshold.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTriggerThreshold.Location = new System.Drawing.Point(933, 0);
            this.lblTriggerThreshold.Name = "lblTriggerThreshold";
            this.lblTriggerThreshold.Size = new System.Drawing.Size(114, 35);
            this.lblTriggerThreshold.TabIndex = 2;
            this.lblTriggerThreshold.Text = "触发阈值:";
            this.lblTriggerThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numTriggerThreshold
            // 
            this.numTriggerThreshold.DecimalPlaces = 3;
            this.numTriggerThreshold.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTriggerThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTriggerThreshold.Location = new System.Drawing.Point(1053, 3);
            this.numTriggerThreshold.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTriggerThreshold.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numTriggerThreshold.Name = "numTriggerThreshold";
            this.numTriggerThreshold.Size = new System.Drawing.Size(114, 30);
            this.numTriggerThreshold.TabIndex = 3;
            this.toolTip.SetToolTip(this.numTriggerThreshold, "设置触发条件的阈值");
            // 
            // lblTriggerDelay
            // 
            this.lblTriggerDelay.AutoSize = true;
            this.lblTriggerDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTriggerDelay.Location = new System.Drawing.Point(3, 70);
            this.lblTriggerDelay.Name = "lblTriggerDelay";
            this.lblTriggerDelay.Size = new System.Drawing.Size(114, 35);
            this.lblTriggerDelay.TabIndex = 4;
            this.lblTriggerDelay.Text = "触发延迟(ms):";
            this.lblTriggerDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numTriggerDelay
            // 
            this.numTriggerDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTriggerDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTriggerDelay.Location = new System.Drawing.Point(123, 73);
            this.numTriggerDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numTriggerDelay.Name = "numTriggerDelay";
            this.numTriggerDelay.Size = new System.Drawing.Size(804, 30);
            this.numTriggerDelay.TabIndex = 5;
            this.toolTip.SetToolTip(this.numTriggerDelay, "触发后的延迟时间，防止频繁触发");
            this.numTriggerDelay.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chkTriggerRisingEdge
            // 
            this.chkTriggerRisingEdge.AutoSize = true;
            this.chkTriggerRisingEdge.Location = new System.Drawing.Point(1173, 73);
            this.chkTriggerRisingEdge.Name = "chkTriggerRisingEdge";
            this.chkTriggerRisingEdge.Size = new System.Drawing.Size(126, 28);
            this.chkTriggerRisingEdge.TabIndex = 6;
            this.chkTriggerRisingEdge.Text = "上升沿触发";
            this.toolTip.SetToolTip(this.chkTriggerRisingEdge, "在数值上升时触发");
            this.chkTriggerRisingEdge.UseVisualStyleBackColor = true;
            // 
            // chkTriggerFallingEdge
            // 
            this.chkTriggerFallingEdge.AutoSize = true;
            this.chkTriggerFallingEdge.Location = new System.Drawing.Point(1323, 73);
            this.chkTriggerFallingEdge.Name = "chkTriggerFallingEdge";
            this.chkTriggerFallingEdge.Size = new System.Drawing.Size(126, 28);
            this.chkTriggerFallingEdge.TabIndex = 7;
            this.chkTriggerFallingEdge.Text = "下降沿触发";
            this.toolTip.SetToolTip(this.chkTriggerFallingEdge, "在数值下降时触发");
            this.chkTriggerFallingEdge.UseVisualStyleBackColor = true;
            // 
            // lblTriggerHelp
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.lblTriggerHelp, 6);
            this.lblTriggerHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTriggerHelp.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTriggerHelp.ForeColor = System.Drawing.Color.Gray;
            this.lblTriggerHelp.Location = new System.Drawing.Point(0, 155);
            this.lblTriggerHelp.Margin = new System.Windows.Forms.Padding(0);
            this.lblTriggerHelp.Name = "lblTriggerHelp";
            this.lblTriggerHelp.Size = new System.Drawing.Size(1470, 20);
            this.lblTriggerHelp.TabIndex = 8;
            this.lblTriggerHelp.Text = "提示：触发条件用于控制数据读取的时机。当满足条件时才进行数据采集，提高系统效率。";
            this.lblTriggerHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnDelete);
            this.grpActions.Controls.Add(this.btnEdit);
            this.grpActions.Controls.Add(this.btnAdd);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 351);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(1476, 54);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "地址操作";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this.btnDelete.Location = new System.Drawing.Point(174, 20);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 30);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "删除";
            this.toolTip.SetToolTip(this.btnDelete, "删除选中的地址配置");
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.LightBlue;
            this.btnEdit.Location = new System.Drawing.Point(93, 20);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 30);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "编辑";
            this.toolTip.SetToolTip(this.btnEdit, "编辑选中的地址配置");
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.LightGreen;
            this.btnAdd.Location = new System.Drawing.Point(12, 20);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 30);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "添加";
            this.toolTip.SetToolTip(this.btnAdd, "添加新的地址配置");
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // grpFormActions
            // 
            this.grpFormActions.Controls.Add(this.btnClose);
            this.grpFormActions.Controls.Add(this.btnSaveAll);
            this.grpFormActions.Controls.Add(this.btnTestAddress);
            this.grpFormActions.Controls.Add(this.btnTestTrigger);
            this.grpFormActions.Location = new System.Drawing.Point(3, 831);
            this.grpFormActions.Name = "grpFormActions";
            this.grpFormActions.Size = new System.Drawing.Size(1192, 54);
            this.grpFormActions.TabIndex = 4;
            this.grpFormActions.TabStop = false;
            this.grpFormActions.Text = "窗体操作";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.LightGray;
            this.btnClose.Location = new System.Drawing.Point(1111, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭";
            this.toolTip.SetToolTip(this.btnClose, "关闭配置窗口");
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAll.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveAll.Location = new System.Drawing.Point(1006, 20);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(99, 30);
            this.btnSaveAll.TabIndex = 2;
            this.btnSaveAll.Text = "保存所有";
            this.toolTip.SetToolTip(this.btnSaveAll, "保存所有配置更改");
            this.btnSaveAll.UseVisualStyleBackColor = false;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnTestAddress
            // 
            this.btnTestAddress.BackColor = System.Drawing.Color.LightYellow;
            this.btnTestAddress.Location = new System.Drawing.Point(12, 20);
            this.btnTestAddress.Name = "btnTestAddress";
            this.btnTestAddress.Size = new System.Drawing.Size(100, 30);
            this.btnTestAddress.TabIndex = 0;
            this.btnTestAddress.Text = "测试地址";
            this.toolTip.SetToolTip(this.btnTestAddress, "测试当前地址的读写功能");
            this.btnTestAddress.UseVisualStyleBackColor = false;
            this.btnTestAddress.Click += new System.EventHandler(this.btnTestAddress_Click);
            // 
            // btnTestTrigger
            // 
            this.btnTestTrigger.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnTestTrigger.Location = new System.Drawing.Point(118, 20);
            this.btnTestTrigger.Name = "btnTestTrigger";
            this.btnTestTrigger.Size = new System.Drawing.Size(100, 30);
            this.btnTestTrigger.TabIndex = 1;
            this.btnTestTrigger.Text = "测试触发";
            this.toolTip.SetToolTip(this.btnTestTrigger, "测试触发条件配置");
            this.btnTestTrigger.UseVisualStyleBackColor = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.grpAddressList, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.grpTriggerConditions, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.grpActions, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.grpAddressConfig, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.grpFormActions, 0, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1482, 888);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // labTriggerAddress
            // 
            this.labTriggerAddress.AutoSize = true;
            this.labTriggerAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labTriggerAddress.Location = new System.Drawing.Point(3, 105);
            this.labTriggerAddress.Name = "labTriggerAddress";
            this.labTriggerAddress.Size = new System.Drawing.Size(114, 35);
            this.labTriggerAddress.TabIndex = 9;
            this.labTriggerAddress.Text = "触发地址:";
            this.labTriggerAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbTriggerAddress
            // 
            this.cmbTriggerAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbTriggerAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTriggerAddress.FormattingEnabled = true;
            this.cmbTriggerAddress.Items.AddRange(new object[] {
            "无触发条件",
            "大于阈值触发",
            "小于阈值触发",
            "等于阈值触发",
            "大于等于阈值触发",
            "小于等于阈值触发",
            "不等于阈值触发",
            "上升沿触发",
            "下降沿触发",
            "变化百分比触发",
            "包含字符串触发",
            "以...开始触发",
            "以...结束触发"});
            this.cmbTriggerAddress.Location = new System.Drawing.Point(123, 108);
            this.cmbTriggerAddress.Name = "cmbTriggerAddress";
            this.cmbTriggerAddress.Size = new System.Drawing.Size(804, 32);
            this.cmbTriggerAddress.TabIndex = 10;
            this.toolTip.SetToolTip(this.cmbTriggerAddress, "选择数据读取的触发条件");
            // 
            // btnRefreshAddress
            // 
            this.btnRefreshAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefreshAddress.Location = new System.Drawing.Point(930, 105);
            this.btnRefreshAddress.Margin = new System.Windows.Forms.Padding(0);
            this.btnRefreshAddress.Name = "btnRefreshAddress";
            this.btnRefreshAddress.Size = new System.Drawing.Size(120, 35);
            this.btnRefreshAddress.TabIndex = 11;
            this.btnRefreshAddress.Text = "刷新地址表";
            this.btnRefreshAddress.UseVisualStyleBackColor = true;
            this.btnRefreshAddress.Click += new System.EventHandler(this.btnRefreshAddress_Click);
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "触发地址";
            this.columnHeader13.Width = 205;
            // 
            // txtStringThreshold
            // 
            this.txtStringThreshold.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStringThreshold.Location = new System.Drawing.Point(123, 38);
            this.txtStringThreshold.Name = "txtStringThreshold";
            this.txtStringThreshold.Size = new System.Drawing.Size(804, 30);
            this.txtStringThreshold.TabIndex = 12;
            // 
            // numConsecutiveCount
            // 
            this.numConsecutiveCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numConsecutiveCount.Location = new System.Drawing.Point(1053, 38);
            this.numConsecutiveCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numConsecutiveCount.Name = "numConsecutiveCount";
            this.numConsecutiveCount.Size = new System.Drawing.Size(114, 30);
            this.numConsecutiveCount.TabIndex = 13;
            this.numConsecutiveCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numCooldownSeconds
            // 
            this.numCooldownSeconds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numCooldownSeconds.Location = new System.Drawing.Point(1323, 38);
            this.numCooldownSeconds.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numCooldownSeconds.Name = "numCooldownSeconds";
            this.numCooldownSeconds.Size = new System.Drawing.Size(144, 30);
            this.numCooldownSeconds.TabIndex = 14;
            // 
            // chkResetOnSuccess
            // 
            this.chkResetOnSuccess.AutoSize = true;
            this.chkResetOnSuccess.Checked = true;
            this.chkResetOnSuccess.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel2.SetColumnSpan(this.chkResetOnSuccess, 2);
            this.chkResetOnSuccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkResetOnSuccess.Location = new System.Drawing.Point(933, 73);
            this.chkResetOnSuccess.Name = "chkResetOnSuccess";
            this.chkResetOnSuccess.Size = new System.Drawing.Size(234, 29);
            this.chkResetOnSuccess.TabIndex = 15;
            this.chkResetOnSuccess.Text = "触发成功后重置计数";
            this.chkResetOnSuccess.UseVisualStyleBackColor = true;
            // 
            // lblConsecutiveCount
            // 
            this.lblConsecutiveCount.AutoSize = true;
            this.lblConsecutiveCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConsecutiveCount.Location = new System.Drawing.Point(933, 35);
            this.lblConsecutiveCount.Name = "lblConsecutiveCount";
            this.lblConsecutiveCount.Size = new System.Drawing.Size(114, 35);
            this.lblConsecutiveCount.TabIndex = 16;
            this.lblConsecutiveCount.Text = "连续计数:";
            // 
            // lblStringThreshold
            // 
            this.lblStringThreshold.AutoSize = true;
            this.lblStringThreshold.Location = new System.Drawing.Point(3, 35);
            this.lblStringThreshold.Name = "lblStringThreshold";
            this.lblStringThreshold.Size = new System.Drawing.Size(104, 24);
            this.lblStringThreshold.TabIndex = 17;
            this.lblStringThreshold.Text = "字符串阈值:";
            // 
            // lblCooldownSeconds
            // 
            this.lblCooldownSeconds.AutoSize = true;
            this.lblCooldownSeconds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCooldownSeconds.Location = new System.Drawing.Point(1173, 35);
            this.lblCooldownSeconds.Name = "lblCooldownSeconds";
            this.lblCooldownSeconds.Size = new System.Drawing.Size(144, 35);
            this.lblCooldownSeconds.TabIndex = 18;
            this.lblCooldownSeconds.Text = "冷却时间:";
            // 
            // FormPLCAddressConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1482, 888);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MinimumSize = new System.Drawing.Size(1220, 850);
            this.Name = "FormPLCAddressConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PLC地址配置管理";
            this.Load += new System.EventHandler(this.FormPLCAddressConfig_Load);
            this.grpAddressList.ResumeLayout(false);
            this.grpAddressConfig.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSendDelay)).EndInit();
            this.grpTriggerConditions.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTriggerThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTriggerDelay)).EndInit();
            this.grpActions.ResumeLayout(false);
            this.grpFormActions.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numConsecutiveCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCooldownSeconds)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.GroupBox grpAddressList;
        private System.Windows.Forms.ListView lvAddresses;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.GroupBox grpAddressConfig;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblReadInterval;
        private System.Windows.Forms.NumericUpDown numReadInterval;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.NumericUpDown numPower;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.CheckBox chkAutoSend;
        private System.Windows.Forms.Label lblSendDelay;
        private System.Windows.Forms.NumericUpDown numSendDelay;
        private System.Windows.Forms.GroupBox grpTriggerConditions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblTriggerCondition;
        private System.Windows.Forms.ComboBox cmbTriggerCondition;
        private System.Windows.Forms.Label lblTriggerThreshold;
        private System.Windows.Forms.NumericUpDown numTriggerThreshold;
        private System.Windows.Forms.Label lblTriggerDelay;
        private System.Windows.Forms.NumericUpDown numTriggerDelay;
        private System.Windows.Forms.CheckBox chkTriggerRisingEdge;
        private System.Windows.Forms.CheckBox chkTriggerFallingEdge;
        private System.Windows.Forms.Label lblTriggerHelp;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox grpFormActions;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnTestAddress;
        private System.Windows.Forms.Button btnTestTrigger;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ComboBox cmbTriggerAddress;
        private System.Windows.Forms.Label labTriggerAddress;
        private System.Windows.Forms.Button btnRefreshAddress;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.TextBox txtStringThreshold;
        private System.Windows.Forms.NumericUpDown numCooldownSeconds;
        private System.Windows.Forms.NumericUpDown numConsecutiveCount;
        private System.Windows.Forms.CheckBox chkResetOnSuccess;
        private System.Windows.Forms.Label lblConsecutiveCount;
        private System.Windows.Forms.Label lblStringThreshold;
        private System.Windows.Forms.Label lblCooldownSeconds;
        }
    }
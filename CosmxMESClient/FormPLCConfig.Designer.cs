using System.Drawing;
using System.Windows.Forms;

namespace CosmxMESClient {
    partial class FormPLCConfig {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose( bool disposing ) {
            if (disposing&&( components!=null )) {
                components.Dispose( );
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        private void InitializeComponent( ) {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageAddresses = new System.Windows.Forms.TabPage();
            this.splitContainerAddresses = new System.Windows.Forms.SplitContainer();
            this.grpScanAddresses = new System.Windows.Forms.GroupBox();
            this.lvScanAddresses = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpSendAddresses = new System.Windows.Forms.GroupBox();
            this.lvSendAddresses = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpAddressManagement = new System.Windows.Forms.GroupBox();
            this.btnConfigScanAddresses = new System.Windows.Forms.Button();
            this.btnConfigSendAddresses = new System.Windows.Forms.Button();
            this.btnStartAutoRead = new System.Windows.Forms.Button();
            this.btnStopAutoRead = new System.Windows.Forms.Button();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpPLCList = new System.Windows.Forms.GroupBox();
            this.dgvPLCConfigs = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPLCType = new System.Windows.Forms.Label();
            this.cmbPLCType = new System.Windows.Forms.ComboBox();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.lblSlaveID = new System.Windows.Forms.Label();
            this.numSlaveID = new System.Windows.Forms.NumericUpDown();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblByteOrder = new System.Windows.Forms.Label();
            this.cmbByteOrder = new System.Windows.Forms.ComboBox();
            this.lblStringByteOrder = new System.Windows.Forms.Label();
            this.cmbStringByteOrder = new System.Windows.Forms.ComboBox();
            this.chkHeartbeatEnabled = new System.Windows.Forms.CheckBox();
            this.lblHeartbeatInterval = new System.Windows.Forms.Label();
            this.numHeartbeatInterval = new System.Windows.Forms.NumericUpDown();
            this.lblHeartbeatAddress = new System.Windows.Forms.Label();
            this.txtHeartbeatAddress = new System.Windows.Forms.TextBox();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.grpFileActions = new System.Windows.Forms.GroupBox();
            this.btnLoadConfigs = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageAddresses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAddresses)).BeginInit();
            this.splitContainerAddresses.Panel1.SuspendLayout();
            this.splitContainerAddresses.Panel2.SuspendLayout();
            this.splitContainerAddresses.SuspendLayout();
            this.grpScanAddresses.SuspendLayout();
            this.grpSendAddresses.SuspendLayout();
            this.grpAddressManagement.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpPLCList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLCConfigs)).BeginInit();
            this.grpActions.SuspendLayout();
            this.grpConnection.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlaveID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeartbeatInterval)).BeginInit();
            this.grpFileActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageAddresses);
            this.tabControl.Controls.Add(this.tabPageConfig);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1838, 710);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageAddresses
            // 
            this.tabPageAddresses.Controls.Add(this.splitContainerAddresses);
            this.tabPageAddresses.Controls.Add(this.grpAddressManagement);
            this.tabPageAddresses.Location = new System.Drawing.Point(4, 33);
            this.tabPageAddresses.Name = "tabPageAddresses";
            this.tabPageAddresses.Size = new System.Drawing.Size(1830, 673);
            this.tabPageAddresses.TabIndex = 0;
            this.tabPageAddresses.Text = "地址管理";
            // 
            // splitContainerAddresses
            // 
            this.splitContainerAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAddresses.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAddresses.Name = "splitContainerAddresses";
            this.splitContainerAddresses.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerAddresses.Panel1
            // 
            this.splitContainerAddresses.Panel1.Controls.Add(this.grpScanAddresses);
            // 
            // splitContainerAddresses.Panel2
            // 
            this.splitContainerAddresses.Panel2.Controls.Add(this.grpSendAddresses);
            this.splitContainerAddresses.Size = new System.Drawing.Size(1830, 593);
            this.splitContainerAddresses.SplitterDistance = 297;
            this.splitContainerAddresses.TabIndex = 0;
            // 
            // grpScanAddresses
            // 
            this.grpScanAddresses.Controls.Add(this.lvScanAddresses);
            this.grpScanAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpScanAddresses.Location = new System.Drawing.Point(0, 0);
            this.grpScanAddresses.Name = "grpScanAddresses";
            this.grpScanAddresses.Size = new System.Drawing.Size(1830, 297);
            this.grpScanAddresses.TabIndex = 0;
            this.grpScanAddresses.TabStop = false;
            this.grpScanAddresses.Text = "扫描地址配置";
            // 
            // lvScanAddresses
            // 
            this.lvScanAddresses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvScanAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvScanAddresses.FullRowSelect = true;
            this.lvScanAddresses.GridLines = true;
            this.lvScanAddresses.HideSelection = false;
            this.lvScanAddresses.Location = new System.Drawing.Point(3, 26);
            this.lvScanAddresses.Name = "lvScanAddresses";
            this.lvScanAddresses.Size = new System.Drawing.Size(1824, 268);
            this.lvScanAddresses.TabIndex = 1;
            this.lvScanAddresses.UseCompatibleStateImageBehavior = false;
            this.lvScanAddresses.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "地址";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "数据类型";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "描述";
            this.columnHeader3.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "扫描间隔(ms)";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "启用状态";
            this.columnHeader5.Width = 80;
            // 
            // grpSendAddresses
            // 
            this.grpSendAddresses.Controls.Add(this.lvSendAddresses);
            this.grpSendAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSendAddresses.Location = new System.Drawing.Point(0, 0);
            this.grpSendAddresses.Name = "grpSendAddresses";
            this.grpSendAddresses.Size = new System.Drawing.Size(1830, 292);
            this.grpSendAddresses.TabIndex = 0;
            this.grpSendAddresses.TabStop = false;
            this.grpSendAddresses.Text = "发送地址配置";
            // 
            // lvSendAddresses
            // 
            this.lvSendAddresses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14});
            this.lvSendAddresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSendAddresses.FullRowSelect = true;
            this.lvSendAddresses.GridLines = true;
            this.lvSendAddresses.HideSelection = false;
            this.lvSendAddresses.Location = new System.Drawing.Point(3, 26);
            this.lvSendAddresses.Name = "lvSendAddresses";
            this.lvSendAddresses.Size = new System.Drawing.Size(1824, 263);
            this.lvSendAddresses.TabIndex = 0;
            this.lvSendAddresses.UseCompatibleStateImageBehavior = false;
            this.lvSendAddresses.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "键值";
            this.columnHeader6.Width = 120;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "名称";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "地址";
            this.columnHeader8.Width = 80;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "数据类型";
            this.columnHeader9.Width = 80;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "描述";
            this.columnHeader10.Width = 150;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "发送间隔(ms)";
            this.columnHeader11.Width = 100;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "自动发送";
            this.columnHeader12.Width = 80;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "发送延迟(ms)";
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "状态";
            // 
            // grpAddressManagement
            // 
            this.grpAddressManagement.Controls.Add(this.btnConfigScanAddresses);
            this.grpAddressManagement.Controls.Add(this.btnConfigSendAddresses);
            this.grpAddressManagement.Controls.Add(this.btnStartAutoRead);
            this.grpAddressManagement.Controls.Add(this.btnStopAutoRead);
            this.grpAddressManagement.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpAddressManagement.Location = new System.Drawing.Point(0, 593);
            this.grpAddressManagement.Name = "grpAddressManagement";
            this.grpAddressManagement.Size = new System.Drawing.Size(1830, 80);
            this.grpAddressManagement.TabIndex = 1;
            this.grpAddressManagement.TabStop = false;
            this.grpAddressManagement.Text = "地址管理操作";
            // 
            // btnConfigScanAddresses
            // 
            this.btnConfigScanAddresses.Location = new System.Drawing.Point(20, 29);
            this.btnConfigScanAddresses.Name = "btnConfigScanAddresses";
            this.btnConfigScanAddresses.Size = new System.Drawing.Size(140, 30);
            this.btnConfigScanAddresses.TabIndex = 0;
            this.btnConfigScanAddresses.Text = "配置扫描地址";
            this.btnConfigScanAddresses.Click += new System.EventHandler(this.btnConfigScanAddresses_Click);
            // 
            // btnConfigSendAddresses
            // 
            this.btnConfigSendAddresses.Location = new System.Drawing.Point(166, 29);
            this.btnConfigSendAddresses.Name = "btnConfigSendAddresses";
            this.btnConfigSendAddresses.Size = new System.Drawing.Size(143, 30);
            this.btnConfigSendAddresses.TabIndex = 1;
            this.btnConfigSendAddresses.Text = "配置发送地址";
            this.btnConfigSendAddresses.Click += new System.EventHandler(this.btnConfigSendAddresses_Click);
            // 
            // btnStartAutoRead
            // 
            this.btnStartAutoRead.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartAutoRead.Location = new System.Drawing.Point(315, 29);
            this.btnStartAutoRead.Name = "btnStartAutoRead";
            this.btnStartAutoRead.Size = new System.Drawing.Size(129, 30);
            this.btnStartAutoRead.TabIndex = 2;
            this.btnStartAutoRead.Text = "启动自动读取";
            this.btnStartAutoRead.UseVisualStyleBackColor = false;
            this.btnStartAutoRead.Click += new System.EventHandler(this.btnStartAutoRead_Click);
            // 
            // btnStopAutoRead
            // 
            this.btnStopAutoRead.BackColor = System.Drawing.Color.LightCoral;
            this.btnStopAutoRead.Location = new System.Drawing.Point(450, 29);
            this.btnStopAutoRead.Name = "btnStopAutoRead";
            this.btnStopAutoRead.Size = new System.Drawing.Size(145, 30);
            this.btnStopAutoRead.TabIndex = 3;
            this.btnStopAutoRead.Text = "停止自动读取";
            this.btnStopAutoRead.UseVisualStyleBackColor = false;
            this.btnStopAutoRead.Click += new System.EventHandler(this.btnStopAutoRead_Click);
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.Controls.Add(this.splitContainer1);
            this.tabPageConfig.Location = new System.Drawing.Point(4, 33);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfig.Size = new System.Drawing.Size(1830, 673);
            this.tabPageConfig.TabIndex = 0;
            this.tabPageConfig.Text = "PLC连接配置";
            this.tabPageConfig.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpPLCList);
            this.splitContainer1.Panel1.Controls.Add(this.grpActions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpConnection);
            this.splitContainer1.Size = new System.Drawing.Size(1824, 667);
            this.splitContainer1.SplitterDistance = 1300;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpPLCList
            // 
            this.grpPLCList.Controls.Add(this.dgvPLCConfigs);
            this.grpPLCList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPLCList.Location = new System.Drawing.Point(0, 0);
            this.grpPLCList.Name = "grpPLCList";
            this.grpPLCList.Size = new System.Drawing.Size(1300, 592);
            this.grpPLCList.TabIndex = 0;
            this.grpPLCList.TabStop = false;
            this.grpPLCList.Text = "PLC列表";
            // 
            // dgvPLCConfigs
            // 
            this.dgvPLCConfigs.AllowUserToAddRows = false;
            this.dgvPLCConfigs.AllowUserToDeleteRows = false;
            this.dgvPLCConfigs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPLCConfigs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvPLCConfigs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPLCConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPLCConfigs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colType,
            this.colIP,
            this.colStatus,
            this.colEnabled});
            this.dgvPLCConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPLCConfigs.Location = new System.Drawing.Point(3, 26);
            this.dgvPLCConfigs.MultiSelect = false;
            this.dgvPLCConfigs.Name = "dgvPLCConfigs";
            this.dgvPLCConfigs.ReadOnly = true;
            this.dgvPLCConfigs.RowHeadersVisible = false;
            this.dgvPLCConfigs.RowHeadersWidth = 62;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dgvPLCConfigs.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPLCConfigs.RowTemplate.Height = 25;
            this.dgvPLCConfigs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPLCConfigs.Size = new System.Drawing.Size(1294, 563);
            this.dgvPLCConfigs.TabIndex = 0;
            this.dgvPLCConfigs.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvPLCConfigs_CellFormatting);
            this.dgvPLCConfigs.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DgvPLCConfigs_DataError);
            this.dgvPLCConfigs.SelectionChanged += new System.EventHandler(this.dgvPLCConfigs_SelectionChanged);
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "名称";
            this.colName.MinimumWidth = 8;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "PLCType";
            this.colType.HeaderText = "类型";
            this.colType.MinimumWidth = 8;
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colIP
            // 
            this.colIP.DataPropertyName = "IPAddress";
            this.colIP.HeaderText = "IP地址";
            this.colIP.MinimumWidth = 8;
            this.colIP.Name = "colIP";
            this.colIP.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "ConnectionStatus";
            this.colStatus.HeaderText = "状态";
            this.colStatus.MinimumWidth = 8;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colEnabled
            // 
            this.colEnabled.DataPropertyName = "EnabledStatus";
            this.colEnabled.HeaderText = "启用状态";
            this.colEnabled.MinimumWidth = 8;
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.ReadOnly = true;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnTestConnection);
            this.grpActions.Controls.Add(this.btnSaveConfig);
            this.grpActions.Controls.Add(this.btnDelete);
            this.grpActions.Controls.Add(this.btnAdd);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpActions.Location = new System.Drawing.Point(0, 592);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(1300, 75);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "操作";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.LightGreen;
            this.btnTestConnection.Location = new System.Drawing.Point(183, 26);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(95, 35);
            this.btnTestConnection.TabIndex = 3;
            this.btnTestConnection.Text = "测试连接";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveConfig.Location = new System.Drawing.Point(84, 26);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(93, 35);
            this.btnSaveConfig.TabIndex = 2;
            this.btnSaveConfig.Text = "保存配置";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this.btnDelete.Location = new System.Drawing.Point(284, 26);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 35);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAdd.Location = new System.Drawing.Point(3, 26);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 35);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.tableLayoutPanel1);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConnection.Location = new System.Drawing.Point(0, 0);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(520, 667);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "连接配置";
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
            this.tableLayoutPanel1.Controls.Add(this.lblPLCType, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbPLCType, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblIPAddress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtIPAddress, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPort, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.numPort, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSlaveID, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.numSlaveID, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeout, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.numTimeout, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblByteOrder, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cmbByteOrder, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblStringByteOrder, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.cmbStringByteOrder, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkHeartbeatEnabled, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblHeartbeatInterval, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.numHeartbeatInterval, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblHeartbeatAddress, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtHeartbeatAddress, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkEnabled, 2, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 638);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(3, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(114, 40);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "PLC名称:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(123, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(131, 30);
            this.txtName.TabIndex = 1;
            // 
            // lblPLCType
            // 
            this.lblPLCType.AutoSize = true;
            this.lblPLCType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPLCType.Location = new System.Drawing.Point(260, 0);
            this.lblPLCType.Name = "lblPLCType";
            this.lblPLCType.Size = new System.Drawing.Size(114, 40);
            this.lblPLCType.TabIndex = 2;
            this.lblPLCType.Text = "PLC类型:";
            this.lblPLCType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbPLCType
            // 
            this.cmbPLCType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbPLCType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPLCType.FormattingEnabled = true;
            this.cmbPLCType.Location = new System.Drawing.Point(380, 3);
            this.cmbPLCType.Name = "cmbPLCType";
            this.cmbPLCType.Size = new System.Drawing.Size(131, 32);
            this.cmbPLCType.TabIndex = 3;
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIPAddress.Location = new System.Drawing.Point(3, 40);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(114, 40);
            this.lblIPAddress.TabIndex = 4;
            this.lblIPAddress.Text = "IP地址:";
            this.lblIPAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIPAddress.Location = new System.Drawing.Point(123, 43);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(131, 30);
            this.txtIPAddress.TabIndex = 5;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPort.Location = new System.Drawing.Point(260, 40);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(114, 40);
            this.lblPort.TabIndex = 6;
            this.lblPort.Text = "端口:";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numPort
            // 
            this.numPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPort.Location = new System.Drawing.Point(380, 43);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(131, 30);
            this.numPort.TabIndex = 7;
            this.numPort.Value = new decimal(new int[] {
            502,
            0,
            0,
            0});
            // 
            // lblSlaveID
            // 
            this.lblSlaveID.AutoSize = true;
            this.lblSlaveID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSlaveID.Location = new System.Drawing.Point(3, 80);
            this.lblSlaveID.Name = "lblSlaveID";
            this.lblSlaveID.Size = new System.Drawing.Size(114, 40);
            this.lblSlaveID.TabIndex = 8;
            this.lblSlaveID.Text = "站号:";
            this.lblSlaveID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numSlaveID
            // 
            this.numSlaveID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSlaveID.Location = new System.Drawing.Point(123, 83);
            this.numSlaveID.Maximum = new decimal(new int[] {
            247,
            0,
            0,
            0});
            this.numSlaveID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSlaveID.Name = "numSlaveID";
            this.numSlaveID.Size = new System.Drawing.Size(131, 30);
            this.numSlaveID.TabIndex = 9;
            this.numSlaveID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimeout.Location = new System.Drawing.Point(260, 80);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(114, 40);
            this.lblTimeout.TabIndex = 10;
            this.lblTimeout.Text = "超时(ms):";
            this.lblTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numTimeout
            // 
            this.numTimeout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTimeout.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTimeout.Location = new System.Drawing.Point(380, 83);
            this.numTimeout.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(131, 30);
            this.numTimeout.TabIndex = 11;
            this.numTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblByteOrder
            // 
            this.lblByteOrder.AutoSize = true;
            this.lblByteOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblByteOrder.Location = new System.Drawing.Point(3, 120);
            this.lblByteOrder.Name = "lblByteOrder";
            this.lblByteOrder.Size = new System.Drawing.Size(114, 40);
            this.lblByteOrder.TabIndex = 12;
            this.lblByteOrder.Text = "数据端序:";
            this.lblByteOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbByteOrder
            // 
            this.cmbByteOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbByteOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbByteOrder.FormattingEnabled = true;
            this.cmbByteOrder.Location = new System.Drawing.Point(123, 123);
            this.cmbByteOrder.Name = "cmbByteOrder";
            this.cmbByteOrder.Size = new System.Drawing.Size(131, 32);
            this.cmbByteOrder.TabIndex = 13;
            // 
            // lblStringByteOrder
            // 
            this.lblStringByteOrder.AutoSize = true;
            this.lblStringByteOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStringByteOrder.Location = new System.Drawing.Point(260, 120);
            this.lblStringByteOrder.Name = "lblStringByteOrder";
            this.lblStringByteOrder.Size = new System.Drawing.Size(114, 40);
            this.lblStringByteOrder.TabIndex = 14;
            this.lblStringByteOrder.Text = "字符串端序:";
            this.lblStringByteOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbStringByteOrder
            // 
            this.cmbStringByteOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbStringByteOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStringByteOrder.FormattingEnabled = true;
            this.cmbStringByteOrder.Location = new System.Drawing.Point(380, 123);
            this.cmbStringByteOrder.Name = "cmbStringByteOrder";
            this.cmbStringByteOrder.Size = new System.Drawing.Size(131, 32);
            this.cmbStringByteOrder.TabIndex = 15;
            // 
            // chkHeartbeatEnabled
            // 
            this.chkHeartbeatEnabled.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkHeartbeatEnabled, 2);
            this.chkHeartbeatEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkHeartbeatEnabled.Location = new System.Drawing.Point(3, 163);
            this.chkHeartbeatEnabled.Name = "chkHeartbeatEnabled";
            this.chkHeartbeatEnabled.Size = new System.Drawing.Size(251, 34);
            this.chkHeartbeatEnabled.TabIndex = 16;
            this.chkHeartbeatEnabled.Text = "启用心跳检测";
            this.chkHeartbeatEnabled.UseVisualStyleBackColor = true;
            // 
            // lblHeartbeatInterval
            // 
            this.lblHeartbeatInterval.AutoSize = true;
            this.lblHeartbeatInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeartbeatInterval.Location = new System.Drawing.Point(260, 160);
            this.lblHeartbeatInterval.Name = "lblHeartbeatInterval";
            this.lblHeartbeatInterval.Size = new System.Drawing.Size(114, 40);
            this.lblHeartbeatInterval.TabIndex = 17;
            this.lblHeartbeatInterval.Text = "心跳间隔(ms):";
            this.lblHeartbeatInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numHeartbeatInterval
            // 
            this.numHeartbeatInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numHeartbeatInterval.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numHeartbeatInterval.Location = new System.Drawing.Point(380, 163);
            this.numHeartbeatInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numHeartbeatInterval.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numHeartbeatInterval.Name = "numHeartbeatInterval";
            this.numHeartbeatInterval.Size = new System.Drawing.Size(131, 30);
            this.numHeartbeatInterval.TabIndex = 18;
            this.numHeartbeatInterval.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            // 
            // lblHeartbeatAddress
            // 
            this.lblHeartbeatAddress.AutoSize = true;
            this.lblHeartbeatAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeartbeatAddress.Location = new System.Drawing.Point(3, 200);
            this.lblHeartbeatAddress.Name = "lblHeartbeatAddress";
            this.lblHeartbeatAddress.Size = new System.Drawing.Size(114, 40);
            this.lblHeartbeatAddress.TabIndex = 19;
            this.lblHeartbeatAddress.Text = "心跳地址:";
            this.lblHeartbeatAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHeartbeatAddress
            // 
            this.txtHeartbeatAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHeartbeatAddress.Location = new System.Drawing.Point(123, 203);
            this.txtHeartbeatAddress.Name = "txtHeartbeatAddress";
            this.txtHeartbeatAddress.Size = new System.Drawing.Size(131, 30);
            this.txtHeartbeatAddress.TabIndex = 20;
            this.txtHeartbeatAddress.Text = "100";
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkEnabled.Location = new System.Drawing.Point(260, 203);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(114, 34);
            this.chkEnabled.TabIndex = 21;
            this.chkEnabled.Text = "启用连接";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // grpFileActions
            // 
            this.grpFileActions.Controls.Add(this.btnLoadConfigs);
            this.grpFileActions.Controls.Add(this.btnSaveAll);
            this.grpFileActions.Controls.Add(this.btnClose);
            this.grpFileActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpFileActions.Location = new System.Drawing.Point(0, 710);
            this.grpFileActions.Name = "grpFileActions";
            this.grpFileActions.Size = new System.Drawing.Size(1838, 60);
            this.grpFileActions.TabIndex = 1;
            this.grpFileActions.TabStop = false;
            this.grpFileActions.Text = "文件操作";
            // 
            // btnLoadConfigs
            // 
            this.btnLoadConfigs.BackColor = System.Drawing.Color.LightYellow;
            this.btnLoadConfigs.Location = new System.Drawing.Point(12, 20);
            this.btnLoadConfigs.Name = "btnLoadConfigs";
            this.btnLoadConfigs.Size = new System.Drawing.Size(100, 35);
            this.btnLoadConfigs.TabIndex = 2;
            this.btnLoadConfigs.Text = "加载配置";
            this.btnLoadConfigs.UseVisualStyleBackColor = false;
            this.btnLoadConfigs.Click += new System.EventHandler(this.btnLoadConfigs_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.BackColor = System.Drawing.Color.LightGreen;
            this.btnSaveAll.Location = new System.Drawing.Point(118, 20);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(100, 35);
            this.btnSaveAll.TabIndex = 1;
            this.btnSaveAll.Text = "保存所有";
            this.btnSaveAll.UseVisualStyleBackColor = false;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.LightGray;
            this.btnClose.Location = new System.Drawing.Point(872, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 35);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormPLCConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1838, 770);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.grpFileActions);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MinimumSize = new System.Drawing.Size(1000, 660);
            this.Name = "FormPLCConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PLC连接配置管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPLCConfig_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageAddresses.ResumeLayout(false);
            this.splitContainerAddresses.Panel1.ResumeLayout(false);
            this.splitContainerAddresses.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAddresses)).EndInit();
            this.splitContainerAddresses.ResumeLayout(false);
            this.grpScanAddresses.ResumeLayout(false);
            this.grpSendAddresses.ResumeLayout(false);
            this.grpAddressManagement.ResumeLayout(false);
            this.tabPageConfig.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpPLCList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLCConfigs)).EndInit();
            this.grpActions.ResumeLayout(false);
            this.grpConnection.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlaveID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeartbeatInterval)).EndInit();
            this.grpFileActions.ResumeLayout(false);
            this.ResumeLayout(false);

            }
        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageConfig;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpPLCList;
        private System.Windows.Forms.DataGridView dgvPLCConfigs;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPLCType;
        private System.Windows.Forms.ComboBox cmbPLCType;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label lblSlaveID;
        private System.Windows.Forms.NumericUpDown numSlaveID;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Label lblByteOrder;
        private System.Windows.Forms.ComboBox cmbByteOrder;
        private System.Windows.Forms.Label lblStringByteOrder;
        private System.Windows.Forms.ComboBox cmbStringByteOrder;
        private System.Windows.Forms.CheckBox chkHeartbeatEnabled;
        private System.Windows.Forms.Label lblHeartbeatInterval;
        private System.Windows.Forms.NumericUpDown numHeartbeatInterval;
        private System.Windows.Forms.Label lblHeartbeatAddress;
        private System.Windows.Forms.TextBox txtHeartbeatAddress;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox grpFileActions;
        private System.Windows.Forms.Button btnLoadConfigs;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnClose;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colIP;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colEnabled;

        // 添加TabPage用于地址管理
        private TabPage tabPageAddresses;
        private SplitContainer splitContainerAddresses;
        private GroupBox grpScanAddresses;
        private GroupBox grpSendAddresses;
        private ListView lvSendAddresses;
        private GroupBox grpAddressManagement;
        private Button btnConfigScanAddresses;
        private Button btnConfigSendAddresses;
        private Button btnImportAddresses;
        private Button btnExportAddresses;
        private Button btnStartAutoRead;
        private Button btnStopAutoRead;
        private ListView lvScanAddresses;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private ColumnHeader columnHeader11;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader13;
        private ColumnHeader columnHeader14;
        }
    }
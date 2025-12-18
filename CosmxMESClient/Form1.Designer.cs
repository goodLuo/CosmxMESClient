namespace CosmxMESClient {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose( bool disposing ) {
            if (disposing&&( components!=null )) {
                components.Dispose( );
                }
            base.Dispose(disposing);
            }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent( ) {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPublic = new System.Windows.Forms.TabPage();
            this.grpPublic = new System.Windows.Forms.GroupBox();
            this.lstProductCodes = new System.Windows.Forms.ListBox();
            this.btnGetProductCode = new System.Windows.Forms.Button();
            this.txtMachineNo = new System.Windows.Forms.TextBox();
            this.lblMachineNo = new System.Windows.Forms.Label();
            this.tabBaking = new System.Windows.Forms.TabPage();
            this.grpBaking = new System.Windows.Forms.GroupBox();
            this.btnGetWaterInfo = new System.Windows.Forms.Button();
            this.btnUploadBakeData = new System.Windows.Forms.Button();
            this.btnCheckCell = new System.Windows.Forms.Button();
            this.txtBakeProjCode = new System.Windows.Forms.TextBox();
            this.lblBakeProjCode = new System.Windows.Forms.Label();
            this.txtBakeCellName = new System.Windows.Forms.TextBox();
            this.lblBakeCellName = new System.Windows.Forms.Label();
            this.tabInjection = new System.Windows.Forms.TabPage();
            this.grpInjection = new System.Windows.Forms.GroupBox();
            this.btnCheckCondition = new System.Windows.Forms.Button();
            this.btnGetIJPara = new System.Windows.Forms.Button();
            this.btnElectCheck = new System.Windows.Forms.Button();
            this.cmbIJType = new System.Windows.Forms.ComboBox();
            this.lblIJType = new System.Windows.Forms.Label();
            this.txtIJProjCode = new System.Windows.Forms.TextBox();
            this.lblIJProjCode = new System.Windows.Forms.Label();
            this.txtIJCellName = new System.Windows.Forms.TextBox();
            this.lblIJCellName = new System.Windows.Forms.Label();
            this.txtElectrolyte = new System.Windows.Forms.TextBox();
            this.lblElectrolyte = new System.Windows.Forms.Label();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.lblBaseUrl = new System.Windows.Forms.Label();
            this.grpCardCheck = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCardNo = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.btnCardCheck = new System.Windows.Forms.Button();
            this.lblProcName = new System.Windows.Forms.Label();
            this.cmbProcName = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.pnlPLCStatus = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lstPLCStatus = new System.Windows.Forms.ListView();
            this.colPLCName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPLCStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLastUpdate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRetryCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStartAllConnections = new System.Windows.Forms.Button();
            this.btnStopAllConnections = new System.Windows.Forms.Button();
            this.btnStartAutoRead = new System.Windows.Forms.Button();
            this.btnStopAutoRead = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPublic.SuspendLayout();
            this.grpPublic.SuspendLayout();
            this.tabBaking.SuspendLayout();
            this.grpBaking.SuspendLayout();
            this.tabInjection.SuspendLayout();
            this.grpInjection.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpCardCheck.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlPLCStatus.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPublic);
            this.tabControl1.Controls.Add(this.tabBaking);
            this.tabControl1.Controls.Add(this.tabInjection);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(2, 2);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(582, 169);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPublic
            // 
            this.tabPublic.Controls.Add(this.grpPublic);
            this.tabPublic.Location = new System.Drawing.Point(4, 22);
            this.tabPublic.Margin = new System.Windows.Forms.Padding(2);
            this.tabPublic.Name = "tabPublic";
            this.tabPublic.Padding = new System.Windows.Forms.Padding(2);
            this.tabPublic.Size = new System.Drawing.Size(574, 143);
            this.tabPublic.TabIndex = 0;
            this.tabPublic.Text = "公共接口";
            this.tabPublic.UseVisualStyleBackColor = true;
            // 
            // grpPublic
            // 
            this.grpPublic.Controls.Add(this.lstProductCodes);
            this.grpPublic.Controls.Add(this.btnGetProductCode);
            this.grpPublic.Controls.Add(this.txtMachineNo);
            this.grpPublic.Controls.Add(this.lblMachineNo);
            this.grpPublic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPublic.Location = new System.Drawing.Point(2, 2);
            this.grpPublic.Margin = new System.Windows.Forms.Padding(2);
            this.grpPublic.Name = "grpPublic";
            this.grpPublic.Padding = new System.Windows.Forms.Padding(2);
            this.grpPublic.Size = new System.Drawing.Size(570, 139);
            this.grpPublic.TabIndex = 0;
            this.grpPublic.TabStop = false;
            this.grpPublic.Text = "设备基本信息";
            // 
            // lstProductCodes
            // 
            this.lstProductCodes.FormattingEnabled = true;
            this.lstProductCodes.ItemHeight = 12;
            this.lstProductCodes.Location = new System.Drawing.Point(13, 40);
            this.lstProductCodes.Margin = new System.Windows.Forms.Padding(2);
            this.lstProductCodes.Name = "lstProductCodes";
            this.lstProductCodes.Size = new System.Drawing.Size(303, 100);
            this.lstProductCodes.TabIndex = 5;
            this.lstProductCodes.SelectedIndexChanged += new System.EventHandler(this.lstProductCodes_SelectedIndexChanged);
            // 
            // btnGetProductCode
            // 
            this.btnGetProductCode.Location = new System.Drawing.Point(211, 18);
            this.btnGetProductCode.Margin = new System.Windows.Forms.Padding(2);
            this.btnGetProductCode.Name = "btnGetProductCode";
            this.btnGetProductCode.Size = new System.Drawing.Size(105, 19);
            this.btnGetProductCode.TabIndex = 4;
            this.btnGetProductCode.Text = "获取产品型号";
            this.btnGetProductCode.UseVisualStyleBackColor = true;
            this.btnGetProductCode.Click += new System.EventHandler(this.btnGetProductCode_Click);
            // 
            // txtMachineNo
            // 
            this.txtMachineNo.Location = new System.Drawing.Point(73, 18);
            this.txtMachineNo.Margin = new System.Windows.Forms.Padding(2);
            this.txtMachineNo.Name = "txtMachineNo";
            this.txtMachineNo.Size = new System.Drawing.Size(135, 21);
            this.txtMachineNo.TabIndex = 3;
            this.txtMachineNo.Text = "10MEJQ10-1204";
            // 
            // lblMachineNo
            // 
            this.lblMachineNo.AutoSize = true;
            this.lblMachineNo.Location = new System.Drawing.Point(10, 20);
            this.lblMachineNo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMachineNo.Name = "lblMachineNo";
            this.lblMachineNo.Size = new System.Drawing.Size(59, 12);
            this.lblMachineNo.TabIndex = 2;
            this.lblMachineNo.Text = "设备编号:";
            // 
            // tabBaking
            // 
            this.tabBaking.Controls.Add(this.grpBaking);
            this.tabBaking.Location = new System.Drawing.Point(4, 22);
            this.tabBaking.Margin = new System.Windows.Forms.Padding(2);
            this.tabBaking.Name = "tabBaking";
            this.tabBaking.Padding = new System.Windows.Forms.Padding(2);
            this.tabBaking.Size = new System.Drawing.Size(574, 143);
            this.tabBaking.TabIndex = 1;
            this.tabBaking.Text = "烘烤工序";
            this.tabBaking.UseVisualStyleBackColor = true;
            // 
            // grpBaking
            // 
            this.grpBaking.Controls.Add(this.btnGetWaterInfo);
            this.grpBaking.Controls.Add(this.btnUploadBakeData);
            this.grpBaking.Controls.Add(this.btnCheckCell);
            this.grpBaking.Controls.Add(this.txtBakeProjCode);
            this.grpBaking.Controls.Add(this.lblBakeProjCode);
            this.grpBaking.Controls.Add(this.txtBakeCellName);
            this.grpBaking.Controls.Add(this.lblBakeCellName);
            this.grpBaking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBaking.Location = new System.Drawing.Point(2, 2);
            this.grpBaking.Margin = new System.Windows.Forms.Padding(2);
            this.grpBaking.Name = "grpBaking";
            this.grpBaking.Padding = new System.Windows.Forms.Padding(2);
            this.grpBaking.Size = new System.Drawing.Size(570, 139);
            this.grpBaking.TabIndex = 0;
            this.grpBaking.TabStop = false;
            this.grpBaking.Text = "烘烤工序操作";
            // 
            // btnGetWaterInfo
            // 
            this.btnGetWaterInfo.BackColor = System.Drawing.Color.LightGreen;
            this.btnGetWaterInfo.Location = new System.Drawing.Point(207, 73);
            this.btnGetWaterInfo.Margin = new System.Windows.Forms.Padding(2);
            this.btnGetWaterInfo.Name = "btnGetWaterInfo";
            this.btnGetWaterInfo.Size = new System.Drawing.Size(90, 20);
            this.btnGetWaterInfo.TabIndex = 6;
            this.btnGetWaterInfo.Text = "获取水分数据";
            this.btnGetWaterInfo.UseVisualStyleBackColor = false;
            this.btnGetWaterInfo.Click += new System.EventHandler(this.btnGetWaterInfo_Click);
            // 
            // btnUploadBakeData
            // 
            this.btnUploadBakeData.BackColor = System.Drawing.Color.LightSalmon;
            this.btnUploadBakeData.Location = new System.Drawing.Point(109, 73);
            this.btnUploadBakeData.Margin = new System.Windows.Forms.Padding(2);
            this.btnUploadBakeData.Name = "btnUploadBakeData";
            this.btnUploadBakeData.Size = new System.Drawing.Size(87, 20);
            this.btnUploadBakeData.TabIndex = 5;
            this.btnUploadBakeData.Text = "上传烘烤数据";
            this.btnUploadBakeData.UseVisualStyleBackColor = false;
            this.btnUploadBakeData.Click += new System.EventHandler(this.btnUploadBakeData_Click);
            // 
            // btnCheckCell
            // 
            this.btnCheckCell.BackColor = System.Drawing.Color.LightBlue;
            this.btnCheckCell.Location = new System.Drawing.Point(33, 73);
            this.btnCheckCell.Margin = new System.Windows.Forms.Padding(2);
            this.btnCheckCell.Name = "btnCheckCell";
            this.btnCheckCell.Size = new System.Drawing.Size(67, 20);
            this.btnCheckCell.TabIndex = 4;
            this.btnCheckCell.Text = "电芯校验";
            this.btnCheckCell.UseVisualStyleBackColor = false;
            this.btnCheckCell.Click += new System.EventHandler(this.btnCheckCell_Click);
            // 
            // txtBakeProjCode
            // 
            this.txtBakeProjCode.Location = new System.Drawing.Point(77, 45);
            this.txtBakeProjCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtBakeProjCode.Name = "txtBakeProjCode";
            this.txtBakeProjCode.Size = new System.Drawing.Size(168, 21);
            this.txtBakeProjCode.TabIndex = 3;
            // 
            // lblBakeProjCode
            // 
            this.lblBakeProjCode.AutoSize = true;
            this.lblBakeProjCode.Location = new System.Drawing.Point(13, 47);
            this.lblBakeProjCode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBakeProjCode.Name = "lblBakeProjCode";
            this.lblBakeProjCode.Size = new System.Drawing.Size(59, 12);
            this.lblBakeProjCode.TabIndex = 2;
            this.lblBakeProjCode.Text = "产品型号:";
            // 
            // txtBakeCellName
            // 
            this.txtBakeCellName.Location = new System.Drawing.Point(77, 18);
            this.txtBakeCellName.Margin = new System.Windows.Forms.Padding(2);
            this.txtBakeCellName.Name = "txtBakeCellName";
            this.txtBakeCellName.Size = new System.Drawing.Size(168, 21);
            this.txtBakeCellName.TabIndex = 1;
            this.txtBakeCellName.Text = "请输入电芯条码";
            // 
            // lblBakeCellName
            // 
            this.lblBakeCellName.AutoSize = true;
            this.lblBakeCellName.Location = new System.Drawing.Point(13, 20);
            this.lblBakeCellName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBakeCellName.Name = "lblBakeCellName";
            this.lblBakeCellName.Size = new System.Drawing.Size(59, 12);
            this.lblBakeCellName.TabIndex = 0;
            this.lblBakeCellName.Text = "电芯条码:";
            // 
            // tabInjection
            // 
            this.tabInjection.Controls.Add(this.grpInjection);
            this.tabInjection.Location = new System.Drawing.Point(4, 22);
            this.tabInjection.Margin = new System.Windows.Forms.Padding(2);
            this.tabInjection.Name = "tabInjection";
            this.tabInjection.Size = new System.Drawing.Size(574, 143);
            this.tabInjection.TabIndex = 2;
            this.tabInjection.Text = "注液工序";
            this.tabInjection.UseVisualStyleBackColor = true;
            // 
            // grpInjection
            // 
            this.grpInjection.Controls.Add(this.btnCheckCondition);
            this.grpInjection.Controls.Add(this.btnGetIJPara);
            this.grpInjection.Controls.Add(this.btnElectCheck);
            this.grpInjection.Controls.Add(this.cmbIJType);
            this.grpInjection.Controls.Add(this.lblIJType);
            this.grpInjection.Controls.Add(this.txtIJProjCode);
            this.grpInjection.Controls.Add(this.lblIJProjCode);
            this.grpInjection.Controls.Add(this.txtIJCellName);
            this.grpInjection.Controls.Add(this.lblIJCellName);
            this.grpInjection.Controls.Add(this.txtElectrolyte);
            this.grpInjection.Controls.Add(this.lblElectrolyte);
            this.grpInjection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInjection.Location = new System.Drawing.Point(0, 0);
            this.grpInjection.Margin = new System.Windows.Forms.Padding(2);
            this.grpInjection.Name = "grpInjection";
            this.grpInjection.Padding = new System.Windows.Forms.Padding(2);
            this.grpInjection.Size = new System.Drawing.Size(574, 143);
            this.grpInjection.TabIndex = 0;
            this.grpInjection.TabStop = false;
            this.grpInjection.Text = "注液工序操作";
            // 
            // btnCheckCondition
            // 
            this.btnCheckCondition.BackColor = System.Drawing.Color.LightGreen;
            this.btnCheckCondition.Location = new System.Drawing.Point(203, 125);
            this.btnCheckCondition.Margin = new System.Windows.Forms.Padding(2);
            this.btnCheckCondition.Name = "btnCheckCondition";
            this.btnCheckCondition.Size = new System.Drawing.Size(92, 20);
            this.btnCheckCondition.TabIndex = 10;
            this.btnCheckCondition.Text = "注液前检查";
            this.btnCheckCondition.UseVisualStyleBackColor = false;
            this.btnCheckCondition.Click += new System.EventHandler(this.btnCheckCondition_Click);
            // 
            // btnGetIJPara
            // 
            this.btnGetIJPara.BackColor = System.Drawing.Color.LightYellow;
            this.btnGetIJPara.Location = new System.Drawing.Point(98, 125);
            this.btnGetIJPara.Margin = new System.Windows.Forms.Padding(2);
            this.btnGetIJPara.Name = "btnGetIJPara";
            this.btnGetIJPara.Size = new System.Drawing.Size(101, 20);
            this.btnGetIJPara.TabIndex = 9;
            this.btnGetIJPara.Text = "获取注液参数";
            this.btnGetIJPara.UseVisualStyleBackColor = false;
            this.btnGetIJPara.Click += new System.EventHandler(this.btnGetIJPara_Click);
            // 
            // btnElectCheck
            // 
            this.btnElectCheck.BackColor = System.Drawing.Color.LightBlue;
            this.btnElectCheck.Location = new System.Drawing.Point(14, 125);
            this.btnElectCheck.Margin = new System.Windows.Forms.Padding(2);
            this.btnElectCheck.Name = "btnElectCheck";
            this.btnElectCheck.Size = new System.Drawing.Size(80, 20);
            this.btnElectCheck.TabIndex = 8;
            this.btnElectCheck.Text = "电解液验证";
            this.btnElectCheck.UseVisualStyleBackColor = false;
            this.btnElectCheck.Click += new System.EventHandler(this.btnElectCheck_Click);
            // 
            // cmbIJType
            // 
            this.cmbIJType.AutoCompleteCustomSource.AddRange(new string[] {
            "一次注液",
            "二次注液"});
            this.cmbIJType.FormattingEnabled = true;
            this.cmbIJType.Items.AddRange(new object[] {
            "一次注液",
            "二次注液"});
            this.cmbIJType.Location = new System.Drawing.Point(89, 94);
            this.cmbIJType.Margin = new System.Windows.Forms.Padding(2);
            this.cmbIJType.Name = "cmbIJType";
            this.cmbIJType.Size = new System.Drawing.Size(168, 20);
            this.cmbIJType.TabIndex = 7;
            // 
            // lblIJType
            // 
            this.lblIJType.AutoSize = true;
            this.lblIJType.Location = new System.Drawing.Point(13, 100);
            this.lblIJType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIJType.Name = "lblIJType";
            this.lblIJType.Size = new System.Drawing.Size(59, 12);
            this.lblIJType.TabIndex = 6;
            this.lblIJType.Text = "注液类型:";
            // 
            // txtIJProjCode
            // 
            this.txtIJProjCode.Location = new System.Drawing.Point(89, 63);
            this.txtIJProjCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtIJProjCode.Name = "txtIJProjCode";
            this.txtIJProjCode.Size = new System.Drawing.Size(168, 21);
            this.txtIJProjCode.TabIndex = 5;
            // 
            // lblIJProjCode
            // 
            this.lblIJProjCode.AutoSize = true;
            this.lblIJProjCode.Location = new System.Drawing.Point(13, 73);
            this.lblIJProjCode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIJProjCode.Name = "lblIJProjCode";
            this.lblIJProjCode.Size = new System.Drawing.Size(59, 12);
            this.lblIJProjCode.TabIndex = 4;
            this.lblIJProjCode.Text = "产品型号:";
            // 
            // txtIJCellName
            // 
            this.txtIJCellName.Location = new System.Drawing.Point(89, 41);
            this.txtIJCellName.Margin = new System.Windows.Forms.Padding(2);
            this.txtIJCellName.Name = "txtIJCellName";
            this.txtIJCellName.Size = new System.Drawing.Size(168, 21);
            this.txtIJCellName.TabIndex = 3;
            // 
            // lblIJCellName
            // 
            this.lblIJCellName.AutoSize = true;
            this.lblIJCellName.Location = new System.Drawing.Point(13, 47);
            this.lblIJCellName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIJCellName.Name = "lblIJCellName";
            this.lblIJCellName.Size = new System.Drawing.Size(59, 12);
            this.lblIJCellName.TabIndex = 2;
            this.lblIJCellName.Text = "电芯条码:";
            // 
            // txtElectrolyte
            // 
            this.txtElectrolyte.Location = new System.Drawing.Point(89, 19);
            this.txtElectrolyte.Margin = new System.Windows.Forms.Padding(2);
            this.txtElectrolyte.Name = "txtElectrolyte";
            this.txtElectrolyte.Size = new System.Drawing.Size(168, 21);
            this.txtElectrolyte.TabIndex = 1;
            this.txtElectrolyte.Text = "请输入电解液条码";
            // 
            // lblElectrolyte
            // 
            this.lblElectrolyte.AutoSize = true;
            this.lblElectrolyte.Location = new System.Drawing.Point(13, 20);
            this.lblElectrolyte.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElectrolyte.Name = "lblElectrolyte";
            this.lblElectrolyte.Size = new System.Drawing.Size(71, 12);
            this.lblElectrolyte.TabIndex = 0;
            this.lblElectrolyte.Text = "电解液编号:";
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.grpSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(2);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(574, 143);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "设置";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.btnTestConnection);
            this.grpSettings.Controls.Add(this.btnSaveSettings);
            this.grpSettings.Controls.Add(this.txtTimeout);
            this.grpSettings.Controls.Add(this.lblTimeout);
            this.grpSettings.Controls.Add(this.txtBaseUrl);
            this.grpSettings.Controls.Add(this.lblBaseUrl);
            this.grpSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSettings.Location = new System.Drawing.Point(0, 0);
            this.grpSettings.Margin = new System.Windows.Forms.Padding(2);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Padding = new System.Windows.Forms.Padding(2);
            this.grpSettings.Size = new System.Drawing.Size(574, 143);
            this.grpSettings.TabIndex = 0;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "系统配置";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.LightGreen;
            this.btnTestConnection.Location = new System.Drawing.Point(117, 88);
            this.btnTestConnection.Margin = new System.Windows.Forms.Padding(2);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(66, 20);
            this.btnTestConnection.TabIndex = 5;
            this.btnTestConnection.Text = "测试连接";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveSettings.Location = new System.Drawing.Point(25, 88);
            this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(72, 20);
            this.btnSaveSettings.TabIndex = 4;
            this.btnSaveSettings.Text = "保存设置";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(101, 41);
            this.txtTimeout.Margin = new System.Windows.Forms.Padding(2);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(55, 21);
            this.txtTimeout.TabIndex = 3;
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(13, 47);
            this.lblTimeout.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(83, 12);
            this.lblTimeout.TabIndex = 2;
            this.lblTimeout.Text = "超时时间(秒):";
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(101, 18);
            this.txtBaseUrl.Margin = new System.Windows.Forms.Padding(2);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(188, 21);
            this.txtBaseUrl.TabIndex = 1;
            this.txtBaseUrl.Text = "http://10.0.3.27:88/api";
            // 
            // lblBaseUrl
            // 
            this.lblBaseUrl.AutoSize = true;
            this.lblBaseUrl.Location = new System.Drawing.Point(13, 20);
            this.lblBaseUrl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaseUrl.Name = "lblBaseUrl";
            this.lblBaseUrl.Size = new System.Drawing.Size(53, 12);
            this.lblBaseUrl.TabIndex = 0;
            this.lblBaseUrl.Text = "API地址:";
            // 
            // grpCardCheck
            // 
            this.grpCardCheck.Controls.Add(this.tableLayoutPanel4);
            this.grpCardCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCardCheck.Location = new System.Drawing.Point(2, 175);
            this.grpCardCheck.Margin = new System.Windows.Forms.Padding(2);
            this.grpCardCheck.Name = "grpCardCheck";
            this.grpCardCheck.Padding = new System.Windows.Forms.Padding(2);
            this.grpCardCheck.Size = new System.Drawing.Size(582, 168);
            this.grpCardCheck.TabIndex = 1;
            this.grpCardCheck.TabStop = false;
            this.grpCardCheck.Text = "刷卡开机验证";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 5;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.lblCardNo, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtResult, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.txtCardNo, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnCardCheck, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblProcName, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.cmbProcName, 3, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 16);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(578, 150);
            this.tableLayoutPanel4.TabIndex = 6;
            // 
            // lblCardNo
            // 
            this.lblCardNo.AutoSize = true;
            this.lblCardNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCardNo.Location = new System.Drawing.Point(2, 2);
            this.lblCardNo.Margin = new System.Windows.Forms.Padding(2);
            this.lblCardNo.Name = "lblCardNo";
            this.lblCardNo.Size = new System.Drawing.Size(36, 23);
            this.lblCardNo.TabIndex = 0;
            this.lblCardNo.Text = "卡号:";
            // 
            // txtResult
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.txtResult, 5);
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Location = new System.Drawing.Point(2, 29);
            this.txtResult.Margin = new System.Windows.Forms.Padding(2);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(574, 119);
            this.txtResult.TabIndex = 5;
            // 
            // txtCardNo
            // 
            this.txtCardNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCardNo.Location = new System.Drawing.Point(42, 2);
            this.txtCardNo.Margin = new System.Windows.Forms.Padding(2);
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(129, 21);
            this.txtCardNo.TabIndex = 1;
            this.txtCardNo.Text = "请输入员工卡号";
            // 
            // btnCardCheck
            // 
            this.btnCardCheck.BackColor = System.Drawing.Color.LightGreen;
            this.btnCardCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCardCheck.Location = new System.Drawing.Point(348, 2);
            this.btnCardCheck.Margin = new System.Windows.Forms.Padding(2);
            this.btnCardCheck.Name = "btnCardCheck";
            this.btnCardCheck.Size = new System.Drawing.Size(228, 23);
            this.btnCardCheck.TabIndex = 4;
            this.btnCardCheck.Text = "刷卡验证";
            this.btnCardCheck.UseVisualStyleBackColor = false;
            this.btnCardCheck.Click += new System.EventHandler(this.btnCardCheck_Click);
            // 
            // lblProcName
            // 
            this.lblProcName.AutoSize = true;
            this.lblProcName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProcName.Location = new System.Drawing.Point(175, 0);
            this.lblProcName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProcName.Name = "lblProcName";
            this.lblProcName.Size = new System.Drawing.Size(36, 27);
            this.lblProcName.TabIndex = 2;
            this.lblProcName.Text = "工序:";
            // 
            // cmbProcName
            // 
            this.cmbProcName.AutoCompleteCustomSource.AddRange(new string[] {
            "JR",
            "FZ",
            "ZY",
            "HC",
            "CH",
            "EF",
            "DJ",
            "OCV",
            "FX",
            "BZ"});
            this.cmbProcName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProcName.FormattingEnabled = true;
            this.cmbProcName.Items.AddRange(new object[] {
            "JR",
            "FZ",
            "ZY",
            "HC",
            "CH",
            "EF",
            "DJ",
            "OCV",
            "FX",
            "BZ"});
            this.cmbProcName.Location = new System.Drawing.Point(215, 2);
            this.cmbProcName.Margin = new System.Windows.Forms.Padding(2);
            this.cmbProcName.Name = "cmbProcName";
            this.cmbProcName.Size = new System.Drawing.Size(129, 20);
            this.cmbProcName.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(2, 345);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(29, 12);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "就绪";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.grpCardCheck, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblStatus, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.grpLog, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(586, 578);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(2, 377);
            this.grpLog.Margin = new System.Windows.Forms.Padding(2);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(2);
            this.grpLog.Size = new System.Drawing.Size(582, 168);
            this.grpLog.TabIndex = 7;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "系统日志";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.Color.Black;
            this.txtLog.Location = new System.Drawing.Point(2, 16);
            this.txtLog.Margin = new System.Windows.Forms.Padding(2);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(578, 150);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnSaveLog, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClearLog, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 549);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(230, 27);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveLog.Location = new System.Drawing.Point(117, 2);
            this.btnSaveLog.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(111, 23);
            this.btnSaveLog.TabIndex = 11;
            this.btnSaveLog.Text = "保存日志";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.SaveLog);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearLog.Location = new System.Drawing.Point(2, 2);
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(111, 23);
            this.btnClearLog.TabIndex = 8;
            this.btnClearLog.Text = "清空日志";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.ClearLog);
            // 
            // pnlPLCStatus
            // 
            this.pnlPLCStatus.Controls.Add(this.tableLayoutPanel3);
            this.pnlPLCStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlPLCStatus.Location = new System.Drawing.Point(586, 0);
            this.pnlPLCStatus.Margin = new System.Windows.Forms.Padding(2);
            this.pnlPLCStatus.Name = "pnlPLCStatus";
            this.pnlPLCStatus.Size = new System.Drawing.Size(379, 578);
            this.pnlPLCStatus.TabIndex = 2;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lstPLCStatus, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnStartAllConnections, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnStopAllConnections, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnStartAutoRead, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnStopAutoRead, 3, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(379, 578);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // lstPLCStatus
            // 
            this.lstPLCStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPLCName,
            this.colPLCStatus,
            this.colLastUpdate,
            this.colRetryCount});
            this.tableLayoutPanel3.SetColumnSpan(this.lstPLCStatus, 5);
            this.lstPLCStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPLCStatus.FullRowSelect = true;
            this.lstPLCStatus.GridLines = true;
            this.lstPLCStatus.HideSelection = false;
            this.lstPLCStatus.Location = new System.Drawing.Point(2, 2);
            this.lstPLCStatus.Margin = new System.Windows.Forms.Padding(2);
            this.lstPLCStatus.Name = "lstPLCStatus";
            this.lstPLCStatus.Size = new System.Drawing.Size(375, 547);
            this.lstPLCStatus.TabIndex = 0;
            this.lstPLCStatus.UseCompatibleStateImageBehavior = false;
            this.lstPLCStatus.View = System.Windows.Forms.View.Details;
            // 
            // colPLCName
            // 
            this.colPLCName.Text = "PLC名称";
            this.colPLCName.Width = 80;
            // 
            // colPLCStatus
            // 
            this.colPLCStatus.Text = "状态";
            // 
            // colLastUpdate
            // 
            this.colLastUpdate.Text = "最后更新";
            this.colLastUpdate.Width = 80;
            // 
            // colRetryCount
            // 
            this.colRetryCount.Text = "重试次数";
            // 
            // btnStartAllConnections
            // 
            this.btnStartAllConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartAllConnections.Location = new System.Drawing.Point(2, 553);
            this.btnStartAllConnections.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartAllConnections.Name = "btnStartAllConnections";
            this.btnStartAllConnections.Size = new System.Drawing.Size(76, 23);
            this.btnStartAllConnections.TabIndex = 1;
            this.btnStartAllConnections.Text = "启动连接";
            this.btnStartAllConnections.UseVisualStyleBackColor = true;
            this.btnStartAllConnections.Click += new System.EventHandler(this.btnStartAllConnections_Click);
            // 
            // btnStopAllConnections
            // 
            this.btnStopAllConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStopAllConnections.Location = new System.Drawing.Point(82, 553);
            this.btnStopAllConnections.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopAllConnections.Name = "btnStopAllConnections";
            this.btnStopAllConnections.Size = new System.Drawing.Size(76, 23);
            this.btnStopAllConnections.TabIndex = 2;
            this.btnStopAllConnections.Text = "停止连接";
            this.btnStopAllConnections.UseVisualStyleBackColor = true;
            this.btnStopAllConnections.Click += new System.EventHandler(this.btnStopAllConnections_Click);
            // 
            // btnStartAutoRead
            // 
            this.btnStartAutoRead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartAutoRead.Location = new System.Drawing.Point(162, 553);
            this.btnStartAutoRead.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartAutoRead.Name = "btnStartAutoRead";
            this.btnStartAutoRead.Size = new System.Drawing.Size(103, 23);
            this.btnStartAutoRead.TabIndex = 3;
            this.btnStartAutoRead.Text = "启动自动读取";
            this.btnStartAutoRead.UseVisualStyleBackColor = true;
            // 
            // btnStopAutoRead
            // 
            this.btnStopAutoRead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStopAutoRead.Location = new System.Drawing.Point(269, 553);
            this.btnStopAutoRead.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopAutoRead.Name = "btnStopAutoRead";
            this.btnStopAutoRead.Size = new System.Drawing.Size(103, 23);
            this.btnStopAutoRead.TabIndex = 4;
            this.btnStopAutoRead.Text = "停止自动读取";
            this.btnStopAutoRead.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 578);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.pnlPLCStatus);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPublic.ResumeLayout(false);
            this.grpPublic.ResumeLayout(false);
            this.grpPublic.PerformLayout();
            this.tabBaking.ResumeLayout(false);
            this.grpBaking.ResumeLayout(false);
            this.grpBaking.PerformLayout();
            this.tabInjection.ResumeLayout(false);
            this.grpInjection.ResumeLayout(false);
            this.grpInjection.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpCardCheck.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlPLCStatus.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPublic;
        private System.Windows.Forms.TabPage tabBaking;
        private System.Windows.Forms.GroupBox grpPublic;
        private System.Windows.Forms.TabPage tabInjection;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TextBox txtMachineNo;
        private System.Windows.Forms.Label lblMachineNo;
        private System.Windows.Forms.GroupBox grpCardCheck;
        private System.Windows.Forms.ListBox lstProductCodes;
        private System.Windows.Forms.Button btnGetProductCode;
        private System.Windows.Forms.Label lblProcName;
        private System.Windows.Forms.TextBox txtCardNo;
        private System.Windows.Forms.Label lblCardNo;
        private System.Windows.Forms.Button btnCardCheck;
        private System.Windows.Forms.ComboBox cmbProcName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.GroupBox grpBaking;
        private System.Windows.Forms.TextBox txtBakeCellName;
        private System.Windows.Forms.Label lblBakeCellName;
        private System.Windows.Forms.Label lblBakeProjCode;
        private System.Windows.Forms.Button btnCheckCell;
        private System.Windows.Forms.TextBox txtBakeProjCode;
        private System.Windows.Forms.Button btnGetWaterInfo;
        private System.Windows.Forms.Button btnUploadBakeData;
        private System.Windows.Forms.GroupBox grpInjection;
        private System.Windows.Forms.Label lblIJCellName;
        private System.Windows.Forms.TextBox txtElectrolyte;
        private System.Windows.Forms.Label lblElectrolyte;
        private System.Windows.Forms.Label lblIJType;
        private System.Windows.Forms.TextBox txtIJProjCode;
        private System.Windows.Forms.Label lblIJProjCode;
        private System.Windows.Forms.TextBox txtIJCellName;
        private System.Windows.Forms.ComboBox cmbIJType;
        private System.Windows.Forms.Button btnElectCheck;
        private System.Windows.Forms.Button btnGetIJPara;
        private System.Windows.Forms.Button btnCheckCondition;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.Label lblBaseUrl;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlPLCStatus;
        private System.Windows.Forms.ListView lstPLCStatus;
        private System.Windows.Forms.ColumnHeader colPLCName;
        private System.Windows.Forms.ColumnHeader colPLCStatus;
        private System.Windows.Forms.ColumnHeader colLastUpdate;
        private System.Windows.Forms.ColumnHeader colRetryCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnStartAllConnections;
        private System.Windows.Forms.Button btnStopAllConnections;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnStartAutoRead;
        private System.Windows.Forms.Button btnStopAutoRead;
        }
    }


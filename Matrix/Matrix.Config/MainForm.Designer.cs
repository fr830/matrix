namespace Matrix.Config
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.базаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTags = new System.Windows.Forms.TabPage();
            this.btnTagCopy = new System.Windows.Forms.Button();
            this.dataGridViewTags = new System.Windows.Forms.DataGridView();
            this.columnTagName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTagType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTagDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkTagLogMode = new System.Windows.Forms.CheckBox();
            this.tbTagDelta = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTagStep = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnTagCancel = new System.Windows.Forms.Button();
            this.btnTagEdit = new System.Windows.Forms.Button();
            this.btnTagSave = new System.Windows.Forms.Button();
            this.btnTagDel = new System.Windows.Forms.Button();
            this.btnTagAdd = new System.Windows.Forms.Button();
            this.tbTagDefault = new System.Windows.Forms.TextBox();
            this.cbTagType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTagDesc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTagName = new System.Windows.Forms.TextBox();
            this.tabService = new System.Windows.Forms.TabPage();
            this.btnSrvCopy = new System.Windows.Forms.Button();
            this.btnSrvSave = new System.Windows.Forms.Button();
            this.btnSrvCancel = new System.Windows.Forms.Button();
            this.btnSrvEdit = new System.Windows.Forms.Button();
            this.btnSrvDel = new System.Windows.Forms.Button();
            this.btnSrvAdd = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbSrvInitDesc = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbSrvMaxCycle = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.tbSrvRetryPause = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.tbSrvBeforeRetry = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tbSrvRetry = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.tbSrvParams = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.tbSrvStatus = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbSrvStartIf = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.chkSrvKeepAlive = new System.Windows.Forms.CheckBox();
            this.tbSrvCheckInterval = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.tbSrvInterval = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tbSrvDesc = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tbSrvName = new System.Windows.Forms.TextBox();
            this.cbSrvType = new System.Windows.Forms.ComboBox();
            this.chkSrvEnabled = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbSrvInit = new System.Windows.Forms.TextBox();
            this.dataGridViewServices = new System.Windows.Forms.DataGridView();
            this.columnServiceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnServiceEnabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnServiceDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tbServiceCheckInterval = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbMsgLogMode = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbMsgLogMaxSize = new System.Windows.Forms.TextBox();
            this.tbMsgLogName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbTagCacheType = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbTagCacheFile = new System.Windows.Forms.TextBox();
            this.tbTagCacheInterval = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tbTagStoreConStr = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbTagStoreRowPerTran = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbTagStoreDelimiter = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cbTagStoreMode = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbTagStoreFile = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridViewLogs = new System.Windows.Forms.DataGridView();
            this.columnLogName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLogSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbLogQueueDelimiter = new System.Windows.Forms.TextBox();
            this.tbLogQueue = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cbLogMode = new System.Windows.Forms.ComboBox();
            this.btnLogAdd = new System.Windows.Forms.Button();
            this.btnLogDel = new System.Windows.Forms.Button();
            this.btnLogCancel = new System.Windows.Forms.Button();
            this.btnLogSave = new System.Windows.Forms.Button();
            this.btnLogEdit = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTags)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.tabService.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewServices)).BeginInit();
            this.tabConfig.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.базаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(825, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // базаToolStripMenuItem
            // 
            this.базаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewToolStripMenuItem,
            this.openConfigToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.базаToolStripMenuItem.Name = "базаToolStripMenuItem";
            this.базаToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.базаToolStripMenuItem.Text = "Конфигурация";
            // 
            // createNewToolStripMenuItem
            // 
            this.createNewToolStripMenuItem.Name = "createNewToolStripMenuItem";
            this.createNewToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.createNewToolStripMenuItem.Text = "Создать новую";
            this.createNewToolStripMenuItem.Click += new System.EventHandler(this.createNewToolStripMenuItem_Click);
            // 
            // openConfigToolStripMenuItem
            // 
            this.openConfigToolStripMenuItem.Name = "openConfigToolStripMenuItem";
            this.openConfigToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.openConfigToolStripMenuItem.Text = "Открыть";
            this.openConfigToolStripMenuItem.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveToolStripMenuItem.Text = "Сохранить";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveAsToolStripMenuItem.Text = "Сохранить как";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.closeToolStripMenuItem.Text = "Закрыть";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 693);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(825, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTags);
            this.tabControl1.Controls.Add(this.tabService);
            this.tabControl1.Controls.Add(this.tabConfig);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(825, 669);
            this.tabControl1.TabIndex = 3;
            // 
            // tabTags
            // 
            this.tabTags.Controls.Add(this.btnTagCopy);
            this.tabTags.Controls.Add(this.dataGridViewTags);
            this.tabTags.Controls.Add(this.groupBox6);
            this.tabTags.Controls.Add(this.btnTagCancel);
            this.tabTags.Controls.Add(this.btnTagEdit);
            this.tabTags.Controls.Add(this.btnTagSave);
            this.tabTags.Controls.Add(this.btnTagDel);
            this.tabTags.Controls.Add(this.btnTagAdd);
            this.tabTags.Controls.Add(this.tbTagDefault);
            this.tabTags.Controls.Add(this.cbTagType);
            this.tabTags.Controls.Add(this.label5);
            this.tabTags.Controls.Add(this.label4);
            this.tabTags.Controls.Add(this.label2);
            this.tabTags.Controls.Add(this.tbTagDesc);
            this.tabTags.Controls.Add(this.label1);
            this.tabTags.Controls.Add(this.tbTagName);
            this.tabTags.Location = new System.Drawing.Point(4, 22);
            this.tabTags.Name = "tabTags";
            this.tabTags.Padding = new System.Windows.Forms.Padding(3);
            this.tabTags.Size = new System.Drawing.Size(817, 643);
            this.tabTags.TabIndex = 0;
            this.tabTags.Text = "Теги";
            this.tabTags.UseVisualStyleBackColor = true;
            // 
            // btnTagCopy
            // 
            this.btnTagCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagCopy.Location = new System.Drawing.Point(98, 598);
            this.btnTagCopy.Name = "btnTagCopy";
            this.btnTagCopy.Size = new System.Drawing.Size(75, 23);
            this.btnTagCopy.TabIndex = 42;
            this.btnTagCopy.Text = "Копировать";
            this.btnTagCopy.UseVisualStyleBackColor = true;
            this.btnTagCopy.Click += new System.EventHandler(this.btnTagCopy_Click);
            // 
            // dataGridViewTags
            // 
            this.dataGridViewTags.AllowUserToAddRows = false;
            this.dataGridViewTags.AllowUserToDeleteRows = false;
            this.dataGridViewTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTags.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnTagName,
            this.columnTagType,
            this.columnTagDesc});
            this.dataGridViewTags.Location = new System.Drawing.Point(4, 4);
            this.dataGridViewTags.Name = "dataGridViewTags";
            this.dataGridViewTags.ReadOnly = true;
            this.dataGridViewTags.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTags.Size = new System.Drawing.Size(810, 390);
            this.dataGridViewTags.TabIndex = 41;
            this.dataGridViewTags.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTags_RowEnter);
            // 
            // columnTagName
            // 
            this.columnTagName.FillWeight = 111.9289F;
            this.columnTagName.HeaderText = "Tag";
            this.columnTagName.Name = "columnTagName";
            this.columnTagName.ReadOnly = true;
            this.columnTagName.Width = 51;
            // 
            // columnTagType
            // 
            this.columnTagType.FillWeight = 76.14214F;
            this.columnTagType.HeaderText = "Тип";
            this.columnTagType.Name = "columnTagType";
            this.columnTagType.ReadOnly = true;
            this.columnTagType.Width = 51;
            // 
            // columnTagDesc
            // 
            this.columnTagDesc.FillWeight = 111.9289F;
            this.columnTagDesc.HeaderText = "Описание";
            this.columnTagDesc.Name = "columnTagDesc";
            this.columnTagDesc.ReadOnly = true;
            this.columnTagDesc.Width = 82;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox6.Controls.Add(this.chkTagLogMode);
            this.groupBox6.Controls.Add(this.tbTagDelta);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.tbTagStep);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Location = new System.Drawing.Point(17, 521);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(298, 63);
            this.groupBox6.TabIndex = 40;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Архивирование";
            // 
            // chkTagLogMode
            // 
            this.chkTagLogMode.AutoSize = true;
            this.chkTagLogMode.Enabled = false;
            this.chkTagLogMode.Location = new System.Drawing.Point(21, 28);
            this.chkTagLogMode.Name = "chkTagLogMode";
            this.chkTagLogMode.Size = new System.Drawing.Size(97, 17);
            this.chkTagLogMode.TabIndex = 31;
            this.chkTagLogMode.Text = "Архивировать";
            this.chkTagLogMode.UseVisualStyleBackColor = true;
            this.chkTagLogMode.CheckedChanged += new System.EventHandler(this.chkLogMode_CheckedChanged);
            // 
            // tbTagDelta
            // 
            this.tbTagDelta.Enabled = false;
            this.tbTagDelta.Location = new System.Drawing.Point(146, 26);
            this.tbTagDelta.Name = "tbTagDelta";
            this.tbTagDelta.Size = new System.Drawing.Size(56, 20);
            this.tbTagDelta.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(143, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Delta";
            // 
            // tbTagStep
            // 
            this.tbTagStep.Enabled = false;
            this.tbTagStep.Location = new System.Drawing.Point(223, 26);
            this.tbTagStep.Name = "tbTagStep";
            this.tbTagStep.Size = new System.Drawing.Size(56, 20);
            this.tbTagStep.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(220, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Step";
            // 
            // btnTagCancel
            // 
            this.btnTagCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagCancel.Location = new System.Drawing.Point(534, 598);
            this.btnTagCancel.Name = "btnTagCancel";
            this.btnTagCancel.Size = new System.Drawing.Size(75, 23);
            this.btnTagCancel.TabIndex = 39;
            this.btnTagCancel.Text = "Отменить";
            this.btnTagCancel.UseVisualStyleBackColor = true;
            this.btnTagCancel.Click += new System.EventHandler(this.btnTagCancel_Click);
            // 
            // btnTagEdit
            // 
            this.btnTagEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagEdit.Location = new System.Drawing.Point(349, 598);
            this.btnTagEdit.Name = "btnTagEdit";
            this.btnTagEdit.Size = new System.Drawing.Size(75, 23);
            this.btnTagEdit.TabIndex = 38;
            this.btnTagEdit.Text = "Изменить";
            this.btnTagEdit.UseVisualStyleBackColor = true;
            this.btnTagEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnTagSave
            // 
            this.btnTagSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagSave.Location = new System.Drawing.Point(453, 598);
            this.btnTagSave.Name = "btnTagSave";
            this.btnTagSave.Size = new System.Drawing.Size(75, 23);
            this.btnTagSave.TabIndex = 37;
            this.btnTagSave.Text = "Сохранить";
            this.btnTagSave.UseVisualStyleBackColor = true;
            this.btnTagSave.Click += new System.EventHandler(this.btnTagSave_Click);
            // 
            // btnTagDel
            // 
            this.btnTagDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagDel.Location = new System.Drawing.Point(179, 598);
            this.btnTagDel.Name = "btnTagDel";
            this.btnTagDel.Size = new System.Drawing.Size(75, 23);
            this.btnTagDel.TabIndex = 36;
            this.btnTagDel.Text = "Удалить";
            this.btnTagDel.UseVisualStyleBackColor = true;
            this.btnTagDel.Click += new System.EventHandler(this.btnTagDel_Click);
            // 
            // btnTagAdd
            // 
            this.btnTagAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTagAdd.Location = new System.Drawing.Point(17, 598);
            this.btnTagAdd.Name = "btnTagAdd";
            this.btnTagAdd.Size = new System.Drawing.Size(75, 23);
            this.btnTagAdd.TabIndex = 35;
            this.btnTagAdd.Text = "Добавить";
            this.btnTagAdd.UseVisualStyleBackColor = true;
            this.btnTagAdd.Click += new System.EventHandler(this.btnTagAdd_Click);
            // 
            // tbTagDefault
            // 
            this.tbTagDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbTagDefault.Enabled = false;
            this.tbTagDefault.Location = new System.Drawing.Point(17, 491);
            this.tbTagDefault.Name = "tbTagDefault";
            this.tbTagDefault.Size = new System.Drawing.Size(592, 20);
            this.tbTagDefault.TabIndex = 34;
            // 
            // cbTagType
            // 
            this.cbTagType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTagType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTagType.Enabled = false;
            this.cbTagType.FormattingEnabled = true;
            this.cbTagType.Items.AddRange(new object[] {
            "object",
            "bool",
            "byte",
            "int",
            "float",
            "datetime",
            "string",
            "xml"});
            this.cbTagType.Location = new System.Drawing.Point(488, 413);
            this.cbTagType.Name = "cbTagType";
            this.cbTagType.Size = new System.Drawing.Size(121, 21);
            this.cbTagType.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 475);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Значение по умолчанию";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(485, 398);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Тип";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 436);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Описание";
            // 
            // tbTagDesc
            // 
            this.tbTagDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbTagDesc.Enabled = false;
            this.tbTagDesc.Location = new System.Drawing.Point(17, 452);
            this.tbTagDesc.Name = "tbTagDesc";
            this.tbTagDesc.Size = new System.Drawing.Size(592, 20);
            this.tbTagDesc.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 397);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Имя";
            // 
            // tbTagName
            // 
            this.tbTagName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbTagName.Enabled = false;
            this.tbTagName.Location = new System.Drawing.Point(17, 413);
            this.tbTagName.Name = "tbTagName";
            this.tbTagName.Size = new System.Drawing.Size(438, 20);
            this.tbTagName.TabIndex = 22;
            // 
            // tabService
            // 
            this.tabService.Controls.Add(this.btnSrvCopy);
            this.tabService.Controls.Add(this.btnSrvSave);
            this.tabService.Controls.Add(this.btnSrvCancel);
            this.tabService.Controls.Add(this.btnSrvEdit);
            this.tabService.Controls.Add(this.btnSrvDel);
            this.tabService.Controls.Add(this.btnSrvAdd);
            this.tabService.Controls.Add(this.tabControl2);
            this.tabService.Controls.Add(this.dataGridViewServices);
            this.tabService.Location = new System.Drawing.Point(4, 22);
            this.tabService.Name = "tabService";
            this.tabService.Padding = new System.Windows.Forms.Padding(3);
            this.tabService.Size = new System.Drawing.Size(817, 643);
            this.tabService.TabIndex = 2;
            this.tabService.Text = "Сервисы";
            this.tabService.UseVisualStyleBackColor = true;
            // 
            // btnSrvCopy
            // 
            this.btnSrvCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvCopy.Location = new System.Drawing.Point(93, 606);
            this.btnSrvCopy.Name = "btnSrvCopy";
            this.btnSrvCopy.Size = new System.Drawing.Size(75, 23);
            this.btnSrvCopy.TabIndex = 45;
            this.btnSrvCopy.Text = "Копировать";
            this.btnSrvCopy.UseVisualStyleBackColor = true;
            this.btnSrvCopy.Click += new System.EventHandler(this.btnSrvCopy_Click);
            // 
            // btnSrvSave
            // 
            this.btnSrvSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvSave.Location = new System.Drawing.Point(448, 606);
            this.btnSrvSave.Name = "btnSrvSave";
            this.btnSrvSave.Size = new System.Drawing.Size(75, 23);
            this.btnSrvSave.TabIndex = 44;
            this.btnSrvSave.Text = "Сохранить";
            this.btnSrvSave.UseVisualStyleBackColor = true;
            this.btnSrvSave.Click += new System.EventHandler(this.btnSrvSave_Click);
            // 
            // btnSrvCancel
            // 
            this.btnSrvCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvCancel.Location = new System.Drawing.Point(529, 606);
            this.btnSrvCancel.Name = "btnSrvCancel";
            this.btnSrvCancel.Size = new System.Drawing.Size(75, 23);
            this.btnSrvCancel.TabIndex = 43;
            this.btnSrvCancel.Text = "Отменить";
            this.btnSrvCancel.UseVisualStyleBackColor = true;
            this.btnSrvCancel.Click += new System.EventHandler(this.btnSrvCancel_Click);
            // 
            // btnSrvEdit
            // 
            this.btnSrvEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvEdit.Location = new System.Drawing.Point(354, 606);
            this.btnSrvEdit.Name = "btnSrvEdit";
            this.btnSrvEdit.Size = new System.Drawing.Size(75, 23);
            this.btnSrvEdit.TabIndex = 42;
            this.btnSrvEdit.Text = "Изменить";
            this.btnSrvEdit.UseVisualStyleBackColor = true;
            this.btnSrvEdit.Click += new System.EventHandler(this.btnSrvEdit_Click);
            // 
            // btnSrvDel
            // 
            this.btnSrvDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvDel.Location = new System.Drawing.Point(177, 606);
            this.btnSrvDel.Name = "btnSrvDel";
            this.btnSrvDel.Size = new System.Drawing.Size(75, 23);
            this.btnSrvDel.TabIndex = 41;
            this.btnSrvDel.Text = "Удалить";
            this.btnSrvDel.UseVisualStyleBackColor = true;
            this.btnSrvDel.Click += new System.EventHandler(this.btnSrvDel_Click);
            // 
            // btnSrvAdd
            // 
            this.btnSrvAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSrvAdd.Location = new System.Drawing.Point(10, 606);
            this.btnSrvAdd.Name = "btnSrvAdd";
            this.btnSrvAdd.Size = new System.Drawing.Size(75, 23);
            this.btnSrvAdd.TabIndex = 40;
            this.btnSrvAdd.Text = "Добавить";
            this.btnSrvAdd.UseVisualStyleBackColor = true;
            this.btnSrvAdd.Click += new System.EventHandler(this.btnSrvAdd_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Location = new System.Drawing.Point(6, 232);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(805, 368);
            this.tabControl2.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbSrvInitDesc);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.tbSrvMaxCycle);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.tbSrvRetryPause);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.tbSrvBeforeRetry);
            this.tabPage1.Controls.Add(this.label33);
            this.tabPage1.Controls.Add(this.tbSrvRetry);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.tbSrvParams);
            this.tabPage1.Controls.Add(this.label31);
            this.tabPage1.Controls.Add(this.tbSrvStatus);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.tbSrvStartIf);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.chkSrvKeepAlive);
            this.tabPage1.Controls.Add(this.tbSrvCheckInterval);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.tbSrvInterval);
            this.tabPage1.Controls.Add(this.label27);
            this.tabPage1.Controls.Add(this.tbSrvDesc);
            this.tabPage1.Controls.Add(this.label26);
            this.tabPage1.Controls.Add(this.tbSrvName);
            this.tabPage1.Controls.Add(this.cbSrvType);
            this.tabPage1.Controls.Add(this.chkSrvEnabled);
            this.tabPage1.Controls.Add(this.label23);
            this.tabPage1.Controls.Add(this.label22);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(797, 342);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Сервис";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbSrvInitDesc
            // 
            this.tbSrvInitDesc.Location = new System.Drawing.Point(15, 230);
            this.tbSrvInitDesc.Name = "tbSrvInitDesc";
            this.tbSrvInitDesc.Size = new System.Drawing.Size(567, 20);
            this.tbSrvInitDesc.TabIndex = 49;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(12, 213);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(171, 13);
            this.label24.TabIndex = 48;
            this.label24.Text = "Описание блока инициализации";
            // 
            // tbSrvMaxCycle
            // 
            this.tbSrvMaxCycle.Location = new System.Drawing.Point(436, 101);
            this.tbSrvMaxCycle.Name = "tbSrvMaxCycle";
            this.tbSrvMaxCycle.Size = new System.Drawing.Size(62, 20);
            this.tbSrvMaxCycle.TabIndex = 47;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(433, 83);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(55, 13);
            this.label35.TabIndex = 46;
            this.label35.Text = "Max cycle";
            // 
            // tbSrvRetryPause
            // 
            this.tbSrvRetryPause.Location = new System.Drawing.Point(352, 101);
            this.tbSrvRetryPause.Name = "tbSrvRetryPause";
            this.tbSrvRetryPause.Size = new System.Drawing.Size(62, 20);
            this.tbSrvRetryPause.TabIndex = 45;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(349, 83);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(64, 13);
            this.label34.TabIndex = 44;
            this.label34.Text = "Retry pause";
            // 
            // tbSrvBeforeRetry
            // 
            this.tbSrvBeforeRetry.Location = new System.Drawing.Point(258, 101);
            this.tbSrvBeforeRetry.Name = "tbSrvBeforeRetry";
            this.tbSrvBeforeRetry.Size = new System.Drawing.Size(62, 20);
            this.tbSrvBeforeRetry.TabIndex = 43;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(255, 83);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(90, 13);
            this.label33.TabIndex = 42;
            this.label33.Text = "Delay before retry";
            // 
            // tbSrvRetry
            // 
            this.tbSrvRetry.Location = new System.Drawing.Point(180, 101);
            this.tbSrvRetry.Name = "tbSrvRetry";
            this.tbSrvRetry.Size = new System.Drawing.Size(62, 20);
            this.tbSrvRetry.TabIndex = 41;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(177, 83);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(32, 13);
            this.label32.TabIndex = 40;
            this.label32.Text = "Retry";
            // 
            // tbSrvParams
            // 
            this.tbSrvParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSrvParams.Location = new System.Drawing.Point(15, 269);
            this.tbSrvParams.Multiline = true;
            this.tbSrvParams.Name = "tbSrvParams";
            this.tbSrvParams.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSrvParams.Size = new System.Drawing.Size(766, 63);
            this.tbSrvParams.TabIndex = 39;
            this.tbSrvParams.WordWrap = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(19, 253);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(155, 13);
            this.label31.TabIndex = 38;
            this.label31.Text = "Дополнительные параметры";
            // 
            // tbSrvStatus
            // 
            this.tbSrvStatus.Location = new System.Drawing.Point(15, 188);
            this.tbSrvStatus.Name = "tbSrvStatus";
            this.tbSrvStatus.Size = new System.Drawing.Size(567, 20);
            this.tbSrvStatus.TabIndex = 37;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(12, 171);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(37, 13);
            this.label29.TabIndex = 36;
            this.label29.Text = "Status";
            // 
            // tbSrvStartIf
            // 
            this.tbSrvStartIf.Location = new System.Drawing.Point(15, 147);
            this.tbSrvStartIf.Name = "tbSrvStartIf";
            this.tbSrvStartIf.Size = new System.Drawing.Size(567, 20);
            this.tbSrvStartIf.TabIndex = 35;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(12, 130);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(41, 13);
            this.label30.TabIndex = 34;
            this.label30.Text = "Start IF";
            // 
            // chkSrvKeepAlive
            // 
            this.chkSrvKeepAlive.AutoSize = true;
            this.chkSrvKeepAlive.Location = new System.Drawing.Point(514, 103);
            this.chkSrvKeepAlive.Name = "chkSrvKeepAlive";
            this.chkSrvKeepAlive.Size = new System.Drawing.Size(77, 17);
            this.chkSrvKeepAlive.TabIndex = 33;
            this.chkSrvKeepAlive.Text = "Keep Alive";
            this.chkSrvKeepAlive.UseVisualStyleBackColor = true;
            // 
            // tbSrvCheckInterval
            // 
            this.tbSrvCheckInterval.Location = new System.Drawing.Point(83, 101);
            this.tbSrvCheckInterval.Name = "tbSrvCheckInterval";
            this.tbSrvCheckInterval.Size = new System.Drawing.Size(62, 20);
            this.tbSrvCheckInterval.TabIndex = 32;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(80, 83);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(76, 13);
            this.label28.TabIndex = 31;
            this.label28.Text = "Check Interval";
            // 
            // tbSrvInterval
            // 
            this.tbSrvInterval.Location = new System.Drawing.Point(15, 101);
            this.tbSrvInterval.Name = "tbSrvInterval";
            this.tbSrvInterval.Size = new System.Drawing.Size(62, 20);
            this.tbSrvInterval.TabIndex = 30;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(12, 83);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(42, 13);
            this.label27.TabIndex = 29;
            this.label27.Text = "Interval";
            // 
            // tbSrvDesc
            // 
            this.tbSrvDesc.Location = new System.Drawing.Point(15, 58);
            this.tbSrvDesc.Name = "tbSrvDesc";
            this.tbSrvDesc.Size = new System.Drawing.Size(567, 20);
            this.tbSrvDesc.TabIndex = 25;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(12, 42);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(57, 13);
            this.label26.TabIndex = 24;
            this.label26.Text = "Описание";
            // 
            // tbSrvName
            // 
            this.tbSrvName.Location = new System.Drawing.Point(15, 21);
            this.tbSrvName.Name = "tbSrvName";
            this.tbSrvName.Size = new System.Drawing.Size(343, 20);
            this.tbSrvName.TabIndex = 21;
            // 
            // cbSrvType
            // 
            this.cbSrvType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSrvType.FormattingEnabled = true;
            this.cbSrvType.Items.AddRange(new object[] {
            "alert",
            "delay",
            "dll",
            "exec",
            "snapshot",
            "http",
            "oledb",
            "mssql",
            "oracle",
            "xml2mssql",
            "custom"});
            this.cbSrvType.Location = new System.Drawing.Point(377, 21);
            this.cbSrvType.Name = "cbSrvType";
            this.cbSrvType.Size = new System.Drawing.Size(121, 21);
            this.cbSrvType.TabIndex = 19;
            this.cbSrvType.SelectionChangeCommitted += new System.EventHandler(this.cbSrvType_SelectionChangeCommitted);
            // 
            // chkSrvEnabled
            // 
            this.chkSrvEnabled.AutoSize = true;
            this.chkSrvEnabled.Location = new System.Drawing.Point(514, 23);
            this.chkSrvEnabled.Name = "chkSrvEnabled";
            this.chkSrvEnabled.Size = new System.Drawing.Size(68, 17);
            this.chkSrvEnabled.TabIndex = 18;
            this.chkSrvEnabled.Text = "Активен";
            this.chkSrvEnabled.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(374, 6);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(26, 13);
            this.label23.TabIndex = 16;
            this.label23.Text = "Тип";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 3);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(74, 13);
            this.label22.TabIndex = 15;
            this.label22.Text = "Имя сервиса";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbSrvInit);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(797, 342);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Init";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbSrvInit
            // 
            this.tbSrvInit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSrvInit.Location = new System.Drawing.Point(7, 7);
            this.tbSrvInit.Multiline = true;
            this.tbSrvInit.Name = "tbSrvInit";
            this.tbSrvInit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSrvInit.Size = new System.Drawing.Size(784, 329);
            this.tbSrvInit.TabIndex = 0;
            this.tbSrvInit.WordWrap = false;
            // 
            // dataGridViewServices
            // 
            this.dataGridViewServices.AllowUserToAddRows = false;
            this.dataGridViewServices.AllowUserToDeleteRows = false;
            this.dataGridViewServices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewServices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnServiceName,
            this.columnServiceEnabled,
            this.columnServiceDesc});
            this.dataGridViewServices.Location = new System.Drawing.Point(4, 7);
            this.dataGridViewServices.MultiSelect = false;
            this.dataGridViewServices.Name = "dataGridViewServices";
            this.dataGridViewServices.ReadOnly = true;
            this.dataGridViewServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewServices.Size = new System.Drawing.Size(810, 219);
            this.dataGridViewServices.TabIndex = 0;
            this.dataGridViewServices.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewServices_RowEnter);
            // 
            // columnServiceName
            // 
            this.columnServiceName.HeaderText = "Сервис";
            this.columnServiceName.Name = "columnServiceName";
            this.columnServiceName.ReadOnly = true;
            this.columnServiceName.Width = 69;
            // 
            // columnServiceEnabled
            // 
            this.columnServiceEnabled.HeaderText = "Активен";
            this.columnServiceEnabled.Name = "columnServiceEnabled";
            this.columnServiceEnabled.ReadOnly = true;
            this.columnServiceEnabled.Width = 74;
            // 
            // columnServiceDesc
            // 
            this.columnServiceDesc.HeaderText = "Описание";
            this.columnServiceDesc.Name = "columnServiceDesc";
            this.columnServiceDesc.ReadOnly = true;
            this.columnServiceDesc.Width = 82;
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.groupBox7);
            this.tabConfig.Controls.Add(this.groupBox1);
            this.tabConfig.Controls.Add(this.groupBox4);
            this.tabConfig.Controls.Add(this.groupBox5);
            this.tabConfig.Controls.Add(this.groupBox2);
            this.tabConfig.Controls.Add(this.btnLogCancel);
            this.tabConfig.Controls.Add(this.btnLogSave);
            this.tabConfig.Controls.Add(this.btnLogEdit);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfig.Size = new System.Drawing.Size(817, 643);
            this.tabConfig.TabIndex = 1;
            this.tabConfig.Text = "Настройки";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tbServiceCheckInterval);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Location = new System.Drawing.Point(9, 484);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(257, 51);
            this.groupBox7.TabIndex = 25;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Service Manager";
            // 
            // tbServiceCheckInterval
            // 
            this.tbServiceCheckInterval.AcceptsReturn = true;
            this.tbServiceCheckInterval.Location = new System.Drawing.Point(90, 19);
            this.tbServiceCheckInterval.Name = "tbServiceCheckInterval";
            this.tbServiceCheckInterval.Size = new System.Drawing.Size(80, 20);
            this.tbServiceCheckInterval.TabIndex = 33;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(75, 13);
            this.label21.TabIndex = 32;
            this.label21.Text = "Check interval";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbMsgLogMode);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tbMsgLogMaxSize);
            this.groupBox1.Controls.Add(this.tbMsgLogName);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(302, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 116);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Журнал сообщений";
            // 
            // cbMsgLogMode
            // 
            this.cbMsgLogMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMsgLogMode.FormattingEnabled = true;
            this.cbMsgLogMode.Items.AddRange(new object[] {
            "console",
            "debug",
            "info",
            "warn",
            "error"});
            this.cbMsgLogMode.Location = new System.Drawing.Point(21, 81);
            this.cbMsgLogMode.Name = "cbMsgLogMode";
            this.cbMsgLogMode.Size = new System.Drawing.Size(66, 21);
            this.cbMsgLogMode.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Режим";
            // 
            // tbMsgLogMaxSize
            // 
            this.tbMsgLogMaxSize.Location = new System.Drawing.Point(174, 81);
            this.tbMsgLogMaxSize.Name = "tbMsgLogMaxSize";
            this.tbMsgLogMaxSize.Size = new System.Drawing.Size(111, 20);
            this.tbMsgLogMaxSize.TabIndex = 24;
            // 
            // tbMsgLogName
            // 
            this.tbMsgLogName.Location = new System.Drawing.Point(21, 41);
            this.tbMsgLogName.Name = "tbMsgLogName";
            this.tbMsgLogName.Size = new System.Drawing.Size(264, 20);
            this.tbMsgLogName.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(171, 66);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Макс. размер, кБайт";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Файл журнала";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbTagCacheType);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.tbTagCacheFile);
            this.groupBox4.Controls.Add(this.tbTagCacheInterval);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Location = new System.Drawing.Point(8, 18);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(275, 116);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tag Cache";
            // 
            // cbTagCacheType
            // 
            this.cbTagCacheType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTagCacheType.FormattingEnabled = true;
            this.cbTagCacheType.Items.AddRange(new object[] {
            "disk"});
            this.cbTagCacheType.Location = new System.Drawing.Point(13, 40);
            this.cbTagCacheType.Name = "cbTagCacheType";
            this.cbTagCacheType.Size = new System.Drawing.Size(63, 21);
            this.cbTagCacheType.TabIndex = 27;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(26, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "Тип";
            // 
            // tbTagCacheFile
            // 
            this.tbTagCacheFile.Location = new System.Drawing.Point(14, 82);
            this.tbTagCacheFile.Name = "tbTagCacheFile";
            this.tbTagCacheFile.Size = new System.Drawing.Size(243, 20);
            this.tbTagCacheFile.TabIndex = 18;
            // 
            // tbTagCacheInterval
            // 
            this.tbTagCacheInterval.Location = new System.Drawing.Point(157, 41);
            this.tbTagCacheInterval.Name = "tbTagCacheInterval";
            this.tbTagCacheInterval.Size = new System.Drawing.Size(100, 20);
            this.tbTagCacheInterval.TabIndex = 17;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Файл кеша";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(154, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Интервал, мс";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tbTagStoreConStr);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.tbTagStoreRowPerTran);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.tbTagStoreDelimiter);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.cbTagStoreMode);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.tbTagStoreFile);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Location = new System.Drawing.Point(8, 365);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(598, 113);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Tag Store";
            // 
            // tbTagStoreConStr
            // 
            this.tbTagStoreConStr.Location = new System.Drawing.Point(13, 80);
            this.tbTagStoreConStr.Name = "tbTagStoreConStr";
            this.tbTagStoreConStr.Size = new System.Drawing.Size(475, 20);
            this.tbTagStoreConStr.TabIndex = 35;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 64);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(106, 13);
            this.label19.TabIndex = 34;
            this.label19.Text = "Строка соединения";
            // 
            // tbTagStoreRowPerTran
            // 
            this.tbTagStoreRowPerTran.Location = new System.Drawing.Point(504, 80);
            this.tbTagStoreRowPerTran.Name = "tbTagStoreRowPerTran";
            this.tbTagStoreRowPerTran.Size = new System.Drawing.Size(81, 20);
            this.tbTagStoreRowPerTran.TabIndex = 33;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(501, 64);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(84, 13);
            this.label20.TabIndex = 32;
            this.label20.Text = "Размер пакета";
            // 
            // tbTagStoreDelimiter
            // 
            this.tbTagStoreDelimiter.Location = new System.Drawing.Point(504, 37);
            this.tbTagStoreDelimiter.Name = "tbTagStoreDelimiter";
            this.tbTagStoreDelimiter.Size = new System.Drawing.Size(80, 20);
            this.tbTagStoreDelimiter.TabIndex = 31;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(501, 22);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(73, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "Разделитель";
            // 
            // cbTagStoreMode
            // 
            this.cbTagStoreMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTagStoreMode.FormattingEnabled = true;
            this.cbTagStoreMode.Items.AddRange(new object[] {
            "file",
            "mssql",
            "oracle"});
            this.cbTagStoreMode.Location = new System.Drawing.Point(13, 37);
            this.cbTagStoreMode.Name = "cbTagStoreMode";
            this.cbTagStoreMode.Size = new System.Drawing.Size(66, 21);
            this.cbTagStoreMode.TabIndex = 29;
            this.cbTagStoreMode.SelectionChangeCommitted += new System.EventHandler(this.cbTagStoreMode_SelectionChangeCommitted);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 28;
            this.label17.Text = "Режим";
            // 
            // tbTagStoreFile
            // 
            this.tbTagStoreFile.Location = new System.Drawing.Point(225, 38);
            this.tbTagStoreFile.Name = "tbTagStoreFile";
            this.tbTagStoreFile.Size = new System.Drawing.Size(263, 20);
            this.tbTagStoreFile.TabIndex = 19;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(222, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(36, 13);
            this.label16.TabIndex = 18;
            this.label16.Text = "Файл";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridViewLogs);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.cbLogMode);
            this.groupBox2.Controls.Add(this.btnLogAdd);
            this.groupBox2.Controls.Add(this.btnLogDel);
            this.groupBox2.Location = new System.Drawing.Point(8, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(598, 219);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Журналирование данных";
            // 
            // dataGridViewLogs
            // 
            this.dataGridViewLogs.AllowUserToAddRows = false;
            this.dataGridViewLogs.AllowUserToDeleteRows = false;
            this.dataGridViewLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLogs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnLogName,
            this.columnLogSize});
            this.dataGridViewLogs.Location = new System.Drawing.Point(14, 69);
            this.dataGridViewLogs.Name = "dataGridViewLogs";
            this.dataGridViewLogs.ReadOnly = true;
            this.dataGridViewLogs.Size = new System.Drawing.Size(269, 126);
            this.dataGridViewLogs.TabIndex = 6;
            // 
            // columnLogName
            // 
            this.columnLogName.HeaderText = "Имя";
            this.columnLogName.Name = "columnLogName";
            this.columnLogName.ReadOnly = true;
            // 
            // columnLogSize
            // 
            this.columnLogSize.HeaderText = "Размер, кБайт";
            this.columnLogSize.Name = "columnLogSize";
            this.columnLogSize.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Режим";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbLogQueueDelimiter);
            this.groupBox3.Controls.Add(this.tbLogQueue);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(294, 69);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(290, 104);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MSMQ";
            // 
            // tbLogQueueDelimiter
            // 
            this.tbLogQueueDelimiter.Location = new System.Drawing.Point(9, 71);
            this.tbLogQueueDelimiter.Name = "tbLogQueueDelimiter";
            this.tbLogQueueDelimiter.Size = new System.Drawing.Size(55, 20);
            this.tbLogQueueDelimiter.TabIndex = 14;
            // 
            // tbLogQueue
            // 
            this.tbLogQueue.Location = new System.Drawing.Point(9, 32);
            this.tbLogQueue.Name = "tbLogQueue";
            this.tbLogQueue.Size = new System.Drawing.Size(275, 20);
            this.tbLogQueue.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Разделитель";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(133, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Имя очереди сообщения";
            // 
            // cbLogMode
            // 
            this.cbLogMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLogMode.FormattingEnabled = true;
            this.cbLogMode.Items.AddRange(new object[] {
            "nolog",
            "log",
            "msmq"});
            this.cbLogMode.Location = new System.Drawing.Point(13, 32);
            this.cbLogMode.Name = "cbLogMode";
            this.cbLogMode.Size = new System.Drawing.Size(63, 21);
            this.cbLogMode.TabIndex = 4;
            // 
            // btnLogAdd
            // 
            this.btnLogAdd.Location = new System.Drawing.Point(121, 30);
            this.btnLogAdd.Name = "btnLogAdd";
            this.btnLogAdd.Size = new System.Drawing.Size(75, 23);
            this.btnLogAdd.TabIndex = 10;
            this.btnLogAdd.Text = "Добавить";
            this.btnLogAdd.UseVisualStyleBackColor = true;
            this.btnLogAdd.Click += new System.EventHandler(this.btnLogAdd_Click);
            // 
            // btnLogDel
            // 
            this.btnLogDel.Location = new System.Drawing.Point(202, 30);
            this.btnLogDel.Name = "btnLogDel";
            this.btnLogDel.Size = new System.Drawing.Size(75, 23);
            this.btnLogDel.TabIndex = 11;
            this.btnLogDel.Text = "Удалить";
            this.btnLogDel.UseVisualStyleBackColor = true;
            this.btnLogDel.Click += new System.EventHandler(this.btnLogDel_Click);
            // 
            // btnLogCancel
            // 
            this.btnLogCancel.Location = new System.Drawing.Point(531, 512);
            this.btnLogCancel.Name = "btnLogCancel";
            this.btnLogCancel.Size = new System.Drawing.Size(75, 23);
            this.btnLogCancel.TabIndex = 14;
            this.btnLogCancel.Text = "Отменить";
            this.btnLogCancel.UseVisualStyleBackColor = true;
            this.btnLogCancel.Click += new System.EventHandler(this.btnLogCancel_Click);
            // 
            // btnLogSave
            // 
            this.btnLogSave.Location = new System.Drawing.Point(438, 512);
            this.btnLogSave.Name = "btnLogSave";
            this.btnLogSave.Size = new System.Drawing.Size(75, 23);
            this.btnLogSave.TabIndex = 13;
            this.btnLogSave.Text = "Сохранить";
            this.btnLogSave.UseVisualStyleBackColor = true;
            this.btnLogSave.Click += new System.EventHandler(this.btnLogSave_Click);
            // 
            // btnLogEdit
            // 
            this.btnLogEdit.Location = new System.Drawing.Point(341, 512);
            this.btnLogEdit.Name = "btnLogEdit";
            this.btnLogEdit.Size = new System.Drawing.Size(75, 23);
            this.btnLogEdit.TabIndex = 12;
            this.btnLogEdit.Text = "Изменить";
            this.btnLogEdit.UseVisualStyleBackColor = true;
            this.btnLogEdit.Click += new System.EventHandler(this.btnLogEdit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 715);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Matrix";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabTags.ResumeLayout(false);
            this.tabTags.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTags)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabService.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewServices)).EndInit();
            this.tabConfig.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem базаToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem openConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTags;
        private System.Windows.Forms.DataGridView dataGridViewTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTagName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTagType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTagDesc;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkTagLogMode;
        private System.Windows.Forms.TextBox tbTagDelta;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbTagStep;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnTagCancel;
        private System.Windows.Forms.Button btnTagEdit;
        private System.Windows.Forms.Button btnTagSave;
        private System.Windows.Forms.Button btnTagDel;
        private System.Windows.Forms.Button btnTagAdd;
        private System.Windows.Forms.TextBox tbTagDefault;
        private System.Windows.Forms.ComboBox cbTagType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTagDesc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTagName;
        private System.Windows.Forms.TabPage tabService;
        private System.Windows.Forms.Button btnSrvSave;
        private System.Windows.Forms.Button btnSrvCancel;
        private System.Windows.Forms.Button btnSrvEdit;
        private System.Windows.Forms.Button btnSrvDel;
        private System.Windows.Forms.Button btnSrvAdd;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox tbSrvInitDesc;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbSrvMaxCycle;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox tbSrvRetryPause;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox tbSrvBeforeRetry;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox tbSrvRetry;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox tbSrvParams;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox tbSrvStatus;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox tbSrvStartIf;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox chkSrvKeepAlive;
        private System.Windows.Forms.TextBox tbSrvCheckInterval;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbSrvInterval;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbSrvDesc;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbSrvName;
        private System.Windows.Forms.ComboBox cbSrvType;
        private System.Windows.Forms.CheckBox chkSrvEnabled;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbSrvInit;
        private System.Windows.Forms.DataGridView dataGridViewServices;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnServiceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnServiceEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnServiceDesc;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tbServiceCheckInterval;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbMsgLogMode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbMsgLogMaxSize;
        private System.Windows.Forms.TextBox tbMsgLogName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbTagCacheType;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbTagCacheFile;
        private System.Windows.Forms.TextBox tbTagCacheInterval;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tbTagStoreConStr;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbTagStoreRowPerTran;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbTagStoreDelimiter;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cbTagStoreMode;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbTagStoreFile;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridViewLogs;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLogName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLogSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbLogQueueDelimiter;
        private System.Windows.Forms.TextBox tbLogQueue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbLogMode;
        private System.Windows.Forms.Button btnLogAdd;
        private System.Windows.Forms.Button btnLogDel;
        private System.Windows.Forms.Button btnLogCancel;
        private System.Windows.Forms.Button btnLogSave;
        private System.Windows.Forms.Button btnLogEdit;
        private System.Windows.Forms.Button btnTagCopy;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button btnSrvCopy;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    }
}


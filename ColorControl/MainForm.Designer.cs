namespace ColorControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabNVIDIA = new System.Windows.Forms.TabPage();
            this.tabAMD = new System.Windows.Forms.TabPage();
            this.btnAmdSettings = new System.Windows.Forms.Button();
            this.chkAmdQuickAccess = new System.Windows.Forms.CheckBox();
            this.lblAmdPresetName = new System.Windows.Forms.Label();
            this.edtAmdPresetName = new System.Windows.Forms.TextBox();
            this.btnAddAmd = new System.Windows.Forms.Button();
            this.lblErrorAMD = new System.Windows.Forms.Label();
            this.btnDeleteAmd = new System.Windows.Forms.Button();
            this.btnCloneAmd = new System.Windows.Forms.Button();
            this.btnAmdPresetSave = new System.Windows.Forms.Button();
            this.lblAmdShortcut = new System.Windows.Forms.Label();
            this.edtAmdShortcut = new System.Windows.Forms.TextBox();
            this.btnChangeAmd = new System.Windows.Forms.Button();
            this.mnuAmdPresets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAmdApply = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdPresetApplyOnStartup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAmdDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdPrimaryDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAmdColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdColorSettingsIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAmdRefreshRate = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdRefreshRateIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAmdDithering = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdDitheringIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAmdHDR = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdHDRIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdHDRToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdHDREnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.miAmdCopyId = new System.Windows.Forms.ToolStripMenuItem();
            this.btnApplyAmd = new System.Windows.Forms.Button();
            this.lvAmdPresets = new System.Windows.Forms.ListView();
            this.tabLG = new System.Windows.Forms.TabPage();
            this.scLgController = new System.Windows.Forms.SplitContainer();
            this.btnLgSettings = new System.Windows.Forms.Button();
            this.chkLgQuickAccess = new System.Windows.Forms.CheckBox();
            this.cbxLgPcHdmiPort = new System.Windows.Forms.ComboBox();
            this.lblLgPcHdmiPort = new System.Windows.Forms.Label();
            this.edtLgPresetDescription = new System.Windows.Forms.TextBox();
            this.lblLgPresetDescription = new System.Windows.Forms.Label();
            this.btnLgDeviceOptionsHelp = new System.Windows.Forms.Button();
            this.btnLgGameBar = new System.Windows.Forms.Button();
            this.btnLgPresetEditTriggerConditions = new System.Windows.Forms.Button();
            this.edtLgPresetTriggerConditions = new System.Windows.Forms.TextBox();
            this.lblLgPresetExcludedProcesses = new System.Windows.Forms.Label();
            this.edtLgPresetExcludedProcesses = new System.Windows.Forms.TextBox();
            this.lblLgPresetIncludedProcesses = new System.Windows.Forms.Label();
            this.edtLgPresetIncludedProcesses = new System.Windows.Forms.TextBox();
            this.lblLgPresetTriggerCondition = new System.Windows.Forms.Label();
            this.cbxLgPresetTrigger = new System.Windows.Forms.ComboBox();
            this.lblLgPresetTrigger = new System.Windows.Forms.Label();
            this.btnLgExpert = new System.Windows.Forms.Button();
            this.mnuLgExpert = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuLgOLEDMotionPro = new System.Windows.Forms.ToolStripMenuItem();
            this.miLgEnableMotionPro = new System.Windows.Forms.ToolStripMenuItem();
            this.miLgDisableMotionPro = new System.Windows.Forms.ToolStripMenuItem();
            this.miLgExpertSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.chkLgRemoteControlShow = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxLgDevices = new System.Windows.Forms.ComboBox();
            this.btnLgDeviceConvertToCustom = new System.Windows.Forms.Button();
            this.cbxLgApps = new System.Windows.Forms.ComboBox();
            this.lvLgPresets = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxLgPresetDevice = new System.Windows.Forms.ComboBox();
            this.btnDeleteLg = new System.Windows.Forms.Button();
            this.btnApplyLg = new System.Windows.Forms.Button();
            this.lblDeviceFilter = new System.Windows.Forms.Label();
            this.lblLgPresetDevice = new System.Windows.Forms.Label();
            this.btnAddLg = new System.Windows.Forms.Button();
            this.edtShortcutLg = new System.Windows.Forms.TextBox();
            this.edtLgDeviceFilter = new System.Windows.Forms.TextBox();
            this.btnLgRemoveDevice = new System.Windows.Forms.Button();
            this.btnLgRefreshApps = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLgAddDevice = new System.Windows.Forms.Button();
            this.edtNameLg = new System.Windows.Forms.TextBox();
            this.btnSetShortcutLg = new System.Windows.Forms.Button();
            this.edtStepsLg = new System.Windows.Forms.TextBox();
            this.btnLgAddButton = new System.Windows.Forms.Button();
            this.mnuLgButtons = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuLgRcButtons = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLgActions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLgNvPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLgAmdPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLgProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCloneLg = new System.Windows.Forms.Button();
            this.lblStepsLg = new System.Windows.Forms.Label();
            this.btnLgDeviceFilterRefresh = new System.Windows.Forms.Button();
            this.clbLgPower = new System.Windows.Forms.CheckedListBox();
            this.lblLgError = new System.Windows.Forms.Label();
            this.tabGameLauncher = new System.Windows.Forms.TabPage();
            this.cbxGameStepType = new System.Windows.Forms.ComboBox();
            this.btnGameOptions = new System.Windows.Forms.Button();
            this.mnuGameOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miGameProcessorAffinity = new System.Windows.Forms.ToolStripMenuItem();
            this.miGameProcessPriority = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGameQuickAccess = new System.Windows.Forms.CheckBox();
            this.btnGameSettings = new System.Windows.Forms.Button();
            this.mnuGameActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGameNvInspector = new System.Windows.Forms.ToolStripMenuItem();
            this.miGameSetQuickAccessShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGameRunAsAdmin = new System.Windows.Forms.CheckBox();
            this.lblGameParameters = new System.Windows.Forms.Label();
            this.edtGameParameters = new System.Windows.Forms.TextBox();
            this.btnGameBrowse = new System.Windows.Forms.Button();
            this.lblGameFilePath = new System.Windows.Forms.Label();
            this.edtGamePath = new System.Windows.Forms.TextBox();
            this.edtGamePrelaunchSteps = new System.Windows.Forms.TextBox();
            this.btnGameAddStep = new System.Windows.Forms.Button();
            this.mnuGameAddStep = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGameNvidiaPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameAmdPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameLgPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameStartProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.edtGameName = new System.Windows.Forms.TextBox();
            this.btnGameDelete = new System.Windows.Forms.Button();
            this.btnGameClone = new System.Windows.Forms.Button();
            this.btnGameSave = new System.Windows.Forms.Button();
            this.btnGameAdd = new System.Windows.Forms.Button();
            this.btnGameLaunch = new System.Windows.Forms.Button();
            this.lvGamePresets = new System.Windows.Forms.ListView();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.grpNvidiaOptions = new System.Windows.Forms.GroupBox();
            this.lblDitheringDisplay = new System.Windows.Forms.Label();
            this.cbxDitheringDisplay = new System.Windows.Forms.ComboBox();
            this.lblDitheringMode = new System.Windows.Forms.Label();
            this.cbxDitheringMode = new System.Windows.Forms.ComboBox();
            this.lblDitheringBitDepth = new System.Windows.Forms.Label();
            this.cbxDitheringBitDepth = new System.Windows.Forms.ComboBox();
            this.chkDitheringEnabled = new System.Windows.Forms.CheckBox();
            this.pbGradient = new System.Windows.Forms.PictureBox();
            this.grpMiscellaneousOptions = new System.Windows.Forms.GroupBox();
            this.btnSetShortcutScreenSaver = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.edtBlankScreenSaverShortcut = new System.Windows.Forms.TextBox();
            this.lblFixChromeFontsDescription = new System.Windows.Forms.Label();
            this.chkFixChromeFonts = new System.Windows.Forms.CheckBox();
            this.grpLGOptions = new System.Windows.Forms.GroupBox();
            this.chkLgSetSelectedDeviceByPowerOn = new System.Windows.Forms.CheckBox();
            this.lblGameBarShortcut = new System.Windows.Forms.Label();
            this.edtLgGameBarShortcut = new System.Windows.Forms.TextBox();
            this.lblLgOptionShutdownDelayHelp = new System.Windows.Forms.Label();
            this.edtLgOptionShutdownDelay = new System.Windows.Forms.NumericUpDown();
            this.lblLgOptionShutdownDelay = new System.Windows.Forms.Label();
            this.chkLgShowAdvancedActions = new System.Windows.Forms.CheckBox();
            this.btnLGTestPower = new System.Windows.Forms.Button();
            this.lblLgMaxPowerOnRetriesDescription = new System.Windows.Forms.Label();
            this.edtLgMaxPowerOnRetries = new System.Windows.Forms.NumericUpDown();
            this.lblLgMaxPowerOnRetries = new System.Windows.Forms.Label();
            this.grpGeneralOptions = new System.Windows.Forms.GroupBox();
            this.chkAutoInstallUpdates = new System.Windows.Forms.CheckBox();
            this.btnStartStopService = new System.Windows.Forms.Button();
            this.btnElevationInfo = new System.Windows.Forms.Button();
            this.rbElevationService = new System.Windows.Forms.RadioButton();
            this.rbElevationProcess = new System.Windows.Forms.RadioButton();
            this.rbElevationAdmin = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.rbElevationNone = new System.Windows.Forms.RadioButton();
            this.chkGdiScaling = new System.Windows.Forms.CheckBox();
            this.chkCheckForUpdates = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
            this.chkMinimizeOnClose = new System.Windows.Forms.CheckBox();
            this.chkStartMinimized = new System.Windows.Forms.CheckBox();
            this.chkStartAfterLogin = new System.Windows.Forms.CheckBox();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.lblShowLog = new System.Windows.Forms.Label();
            this.cbxLogType = new System.Windows.Forms.ComboBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.edtLog = new System.Windows.Forms.TextBox();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.grpNVIDIAInfo = new System.Windows.Forms.GroupBox();
            this.btnRefreshNVIDIAInfo = new System.Windows.Forms.Button();
            this.tvNVIDIAInfo = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbPlugins = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lvNvPresetsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tcMain.SuspendLayout();
            this.tabAMD.SuspendLayout();
            this.mnuAmdPresets.SuspendLayout();
            this.tabLG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scLgController)).BeginInit();
            this.scLgController.Panel1.SuspendLayout();
            this.scLgController.SuspendLayout();
            this.mnuLgExpert.SuspendLayout();
            this.mnuLgButtons.SuspendLayout();
            this.tabGameLauncher.SuspendLayout();
            this.mnuGameOptions.SuspendLayout();
            this.mnuGameActions.SuspendLayout();
            this.mnuGameAddStep.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.grpNvidiaOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGradient)).BeginInit();
            this.grpMiscellaneousOptions.SuspendLayout();
            this.grpLGOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtLgOptionShutdownDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtLgMaxPowerOnRetries)).BeginInit();
            this.grpGeneralOptions.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.grpNVIDIAInfo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tabNVIDIA);
            this.tcMain.Controls.Add(this.tabAMD);
            this.tcMain.Controls.Add(this.tabLG);
            this.tcMain.Controls.Add(this.tabGameLauncher);
            this.tcMain.Controls.Add(this.tabOptions);
            this.tcMain.Controls.Add(this.tabLog);
            this.tcMain.Controls.Add(this.tabInfo);
            this.tcMain.Location = new System.Drawing.Point(14, 14);
            this.tcMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1122, 567);
            this.tcMain.TabIndex = 1;
            this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabNVIDIA
            // 
            this.tabNVIDIA.Location = new System.Drawing.Point(4, 24);
            this.tabNVIDIA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabNVIDIA.Name = "tabNVIDIA";
            this.tabNVIDIA.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabNVIDIA.Size = new System.Drawing.Size(1114, 539);
            this.tabNVIDIA.TabIndex = 0;
            this.tabNVIDIA.Text = "NVIDIA controller";
            this.tabNVIDIA.UseVisualStyleBackColor = true;
            // 
            // tabAMD
            // 
            this.tabAMD.Controls.Add(this.btnAmdSettings);
            this.tabAMD.Controls.Add(this.chkAmdQuickAccess);
            this.tabAMD.Controls.Add(this.lblAmdPresetName);
            this.tabAMD.Controls.Add(this.edtAmdPresetName);
            this.tabAMD.Controls.Add(this.btnAddAmd);
            this.tabAMD.Controls.Add(this.lblErrorAMD);
            this.tabAMD.Controls.Add(this.btnDeleteAmd);
            this.tabAMD.Controls.Add(this.btnCloneAmd);
            this.tabAMD.Controls.Add(this.btnAmdPresetSave);
            this.tabAMD.Controls.Add(this.lblAmdShortcut);
            this.tabAMD.Controls.Add(this.edtAmdShortcut);
            this.tabAMD.Controls.Add(this.btnChangeAmd);
            this.tabAMD.Controls.Add(this.btnApplyAmd);
            this.tabAMD.Controls.Add(this.lvAmdPresets);
            this.tabAMD.Location = new System.Drawing.Point(4, 24);
            this.tabAMD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabAMD.Name = "tabAMD";
            this.tabAMD.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabAMD.Size = new System.Drawing.Size(1114, 539);
            this.tabAMD.TabIndex = 5;
            this.tabAMD.Text = "AMD controller";
            this.tabAMD.UseVisualStyleBackColor = true;
            // 
            // btnAmdSettings
            // 
            this.btnAmdSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAmdSettings.Location = new System.Drawing.Point(1017, 429);
            this.btnAmdSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAmdSettings.Name = "btnAmdSettings";
            this.btnAmdSettings.Size = new System.Drawing.Size(88, 27);
            this.btnAmdSettings.TabIndex = 60;
            this.btnAmdSettings.Text = "Settings...";
            this.btnAmdSettings.UseVisualStyleBackColor = true;
            this.btnAmdSettings.Click += new System.EventHandler(this.btnAmdSettings_Click);
            // 
            // chkAmdQuickAccess
            // 
            this.chkAmdQuickAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAmdQuickAccess.AutoSize = true;
            this.chkAmdQuickAccess.Location = new System.Drawing.Point(343, 465);
            this.chkAmdQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkAmdQuickAccess.Name = "chkAmdQuickAccess";
            this.chkAmdQuickAccess.Size = new System.Drawing.Size(141, 19);
            this.chkAmdQuickAccess.TabIndex = 46;
            this.chkAmdQuickAccess.Text = "Show in Quick Access";
            this.chkAmdQuickAccess.UseVisualStyleBackColor = true;
            // 
            // lblAmdPresetName
            // 
            this.lblAmdPresetName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmdPresetName.AutoSize = true;
            this.lblAmdPresetName.Location = new System.Drawing.Point(7, 466);
            this.lblAmdPresetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAmdPresetName.Name = "lblAmdPresetName";
            this.lblAmdPresetName.Size = new System.Drawing.Size(42, 15);
            this.lblAmdPresetName.TabIndex = 24;
            this.lblAmdPresetName.Text = "Name:";
            // 
            // edtAmdPresetName
            // 
            this.edtAmdPresetName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtAmdPresetName.Enabled = false;
            this.edtAmdPresetName.Location = new System.Drawing.Point(102, 463);
            this.edtAmdPresetName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtAmdPresetName.Name = "edtAmdPresetName";
            this.edtAmdPresetName.Size = new System.Drawing.Size(233, 23);
            this.edtAmdPresetName.TabIndex = 23;
            // 
            // btnAddAmd
            // 
            this.btnAddAmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddAmd.Location = new System.Drawing.Point(385, 429);
            this.btnAddAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddAmd.Name = "btnAddAmd";
            this.btnAddAmd.Size = new System.Drawing.Size(88, 27);
            this.btnAddAmd.TabIndex = 18;
            this.btnAddAmd.Text = "Add";
            this.btnAddAmd.UseVisualStyleBackColor = true;
            this.btnAddAmd.Click += new System.EventHandler(this.btnAddAmd_Click);
            // 
            // lblErrorAMD
            // 
            this.lblErrorAMD.AutoSize = true;
            this.lblErrorAMD.Location = new System.Drawing.Point(7, 7);
            this.lblErrorAMD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblErrorAMD.Name = "lblErrorAMD";
            this.lblErrorAMD.Size = new System.Drawing.Size(53, 15);
            this.lblErrorAMD.TabIndex = 8;
            this.lblErrorAMD.Text = "ErrorText";
            this.lblErrorAMD.Visible = false;
            // 
            // btnDeleteAmd
            // 
            this.btnDeleteAmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteAmd.Enabled = false;
            this.btnDeleteAmd.Location = new System.Drawing.Point(290, 429);
            this.btnDeleteAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteAmd.Name = "btnDeleteAmd";
            this.btnDeleteAmd.Size = new System.Drawing.Size(88, 27);
            this.btnDeleteAmd.TabIndex = 17;
            this.btnDeleteAmd.Text = "Delete";
            this.btnDeleteAmd.UseVisualStyleBackColor = true;
            this.btnDeleteAmd.Click += new System.EventHandler(this.btnDeleteAmd_Click);
            // 
            // btnCloneAmd
            // 
            this.btnCloneAmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCloneAmd.Enabled = false;
            this.btnCloneAmd.Location = new System.Drawing.Point(196, 429);
            this.btnCloneAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCloneAmd.Name = "btnCloneAmd";
            this.btnCloneAmd.Size = new System.Drawing.Size(88, 27);
            this.btnCloneAmd.TabIndex = 16;
            this.btnCloneAmd.Text = "Clone";
            this.btnCloneAmd.UseVisualStyleBackColor = true;
            this.btnCloneAmd.Click += new System.EventHandler(this.btnCloneAmd_Click);
            // 
            // btnAmdPresetSave
            // 
            this.btnAmdPresetSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAmdPresetSave.Enabled = false;
            this.btnAmdPresetSave.Location = new System.Drawing.Point(479, 429);
            this.btnAmdPresetSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAmdPresetSave.Name = "btnAmdPresetSave";
            this.btnAmdPresetSave.Size = new System.Drawing.Size(88, 27);
            this.btnAmdPresetSave.TabIndex = 15;
            this.btnAmdPresetSave.Text = "Save";
            this.btnAmdPresetSave.UseVisualStyleBackColor = true;
            this.btnAmdPresetSave.Click += new System.EventHandler(this.btnSetAmdShortcut_Click);
            // 
            // lblAmdShortcut
            // 
            this.lblAmdShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmdShortcut.AutoSize = true;
            this.lblAmdShortcut.Location = new System.Drawing.Point(7, 496);
            this.lblAmdShortcut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAmdShortcut.Name = "lblAmdShortcut";
            this.lblAmdShortcut.Size = new System.Drawing.Size(55, 15);
            this.lblAmdShortcut.TabIndex = 14;
            this.lblAmdShortcut.Text = "Shortcut:";
            // 
            // edtAmdShortcut
            // 
            this.edtAmdShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtAmdShortcut.Enabled = false;
            this.edtAmdShortcut.Location = new System.Drawing.Point(102, 493);
            this.edtAmdShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtAmdShortcut.Name = "edtAmdShortcut";
            this.edtAmdShortcut.ReadOnly = true;
            this.edtAmdShortcut.Size = new System.Drawing.Size(233, 23);
            this.edtAmdShortcut.TabIndex = 13;
            this.edtAmdShortcut.TextChanged += new System.EventHandler(this.edtAmdShortcut_TextChanged);
            this.edtAmdShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyDown);
            this.edtAmdShortcut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyUp);
            // 
            // btnChangeAmd
            // 
            this.btnChangeAmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeAmd.ContextMenuStrip = this.mnuAmdPresets;
            this.btnChangeAmd.Enabled = false;
            this.btnChangeAmd.Location = new System.Drawing.Point(102, 429);
            this.btnChangeAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChangeAmd.Name = "btnChangeAmd";
            this.btnChangeAmd.Size = new System.Drawing.Size(88, 27);
            this.btnChangeAmd.TabIndex = 12;
            this.btnChangeAmd.Text = "Change...";
            this.btnChangeAmd.UseVisualStyleBackColor = true;
            this.btnChangeAmd.Click += new System.EventHandler(this.btnChangeAmd_Click);
            // 
            // mnuAmdPresets
            // 
            this.mnuAmdPresets.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuAmdPresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdApply,
            this.miAmdPresetApplyOnStartup,
            this.toolStripSeparator2,
            this.mnuAmdDisplay,
            this.mnuAmdColorSettings,
            this.mnuAmdRefreshRate,
            this.mnuAmdDithering,
            this.mnuAmdHDR,
            this.miAmdCopyId});
            this.mnuAmdPresets.Name = "mnuNvPresets";
            this.mnuAmdPresets.Size = new System.Drawing.Size(185, 186);
            this.mnuAmdPresets.Opening += new System.ComponentModel.CancelEventHandler(this.mnuAmdPresets_Opening);
            // 
            // miAmdApply
            // 
            this.miAmdApply.Name = "miAmdApply";
            this.miAmdApply.Size = new System.Drawing.Size(184, 22);
            this.miAmdApply.Text = "Apply";
            this.miAmdApply.Click += new System.EventHandler(this.btnApplyAmd_Click);
            // 
            // miAmdPresetApplyOnStartup
            // 
            this.miAmdPresetApplyOnStartup.CheckOnClick = true;
            this.miAmdPresetApplyOnStartup.Name = "miAmdPresetApplyOnStartup";
            this.miAmdPresetApplyOnStartup.Size = new System.Drawing.Size(184, 22);
            this.miAmdPresetApplyOnStartup.Text = "Apply on startup";
            this.miAmdPresetApplyOnStartup.Click += new System.EventHandler(this.miAmdPresetApplyOnStartup_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuAmdDisplay
            // 
            this.mnuAmdDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdPrimaryDisplay});
            this.mnuAmdDisplay.Name = "mnuAmdDisplay";
            this.mnuAmdDisplay.Size = new System.Drawing.Size(184, 22);
            this.mnuAmdDisplay.Text = "Display";
            // 
            // miAmdPrimaryDisplay
            // 
            this.miAmdPrimaryDisplay.Name = "miAmdPrimaryDisplay";
            this.miAmdPrimaryDisplay.Size = new System.Drawing.Size(155, 22);
            this.miAmdPrimaryDisplay.Text = "Primary display";
            this.miAmdPrimaryDisplay.Click += new System.EventHandler(this.displayMenuItemAmd_Click);
            // 
            // mnuAmdColorSettings
            // 
            this.mnuAmdColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdColorSettingsIncluded});
            this.mnuAmdColorSettings.Name = "mnuAmdColorSettings";
            this.mnuAmdColorSettings.Size = new System.Drawing.Size(184, 22);
            this.mnuAmdColorSettings.Text = "Color settings";
            // 
            // miAmdColorSettingsIncluded
            // 
            this.miAmdColorSettingsIncluded.Name = "miAmdColorSettingsIncluded";
            this.miAmdColorSettingsIncluded.Size = new System.Drawing.Size(120, 22);
            this.miAmdColorSettingsIncluded.Text = "Included";
            this.miAmdColorSettingsIncluded.Click += new System.EventHandler(this.miAmdColorSettingsIncluded_Click);
            // 
            // mnuAmdRefreshRate
            // 
            this.mnuAmdRefreshRate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdRefreshRateIncluded});
            this.mnuAmdRefreshRate.Name = "mnuAmdRefreshRate";
            this.mnuAmdRefreshRate.Size = new System.Drawing.Size(184, 22);
            this.mnuAmdRefreshRate.Text = "Refresh Rate";
            // 
            // miAmdRefreshRateIncluded
            // 
            this.miAmdRefreshRateIncluded.Name = "miAmdRefreshRateIncluded";
            this.miAmdRefreshRateIncluded.Size = new System.Drawing.Size(120, 22);
            this.miAmdRefreshRateIncluded.Text = "Included";
            this.miAmdRefreshRateIncluded.Click += new System.EventHandler(this.miAmdRefreshRateIncluded_Click);
            // 
            // mnuAmdDithering
            // 
            this.mnuAmdDithering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdDitheringIncluded});
            this.mnuAmdDithering.Name = "mnuAmdDithering";
            this.mnuAmdDithering.Size = new System.Drawing.Size(184, 22);
            this.mnuAmdDithering.Text = "Dithering";
            // 
            // miAmdDitheringIncluded
            // 
            this.miAmdDitheringIncluded.Name = "miAmdDitheringIncluded";
            this.miAmdDitheringIncluded.Size = new System.Drawing.Size(120, 22);
            this.miAmdDitheringIncluded.Text = "Included";
            this.miAmdDitheringIncluded.Click += new System.EventHandler(this.miAmdDitheringIncluded_Click);
            // 
            // mnuAmdHDR
            // 
            this.mnuAmdHDR.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAmdHDRIncluded,
            this.miAmdHDRToggle,
            this.miAmdHDREnabled});
            this.mnuAmdHDR.Name = "mnuAmdHDR";
            this.mnuAmdHDR.Size = new System.Drawing.Size(184, 22);
            this.mnuAmdHDR.Text = "HDR";
            // 
            // miAmdHDRIncluded
            // 
            this.miAmdHDRIncluded.Name = "miAmdHDRIncluded";
            this.miAmdHDRIncluded.Size = new System.Drawing.Size(136, 22);
            this.miAmdHDRIncluded.Text = "Included";
            this.miAmdHDRIncluded.Click += new System.EventHandler(this.miAmdHDRIncluded_Click);
            // 
            // miAmdHDRToggle
            // 
            this.miAmdHDRToggle.Name = "miAmdHDRToggle";
            this.miAmdHDRToggle.Size = new System.Drawing.Size(136, 22);
            this.miAmdHDRToggle.Text = "Toggle HDR";
            this.miAmdHDRToggle.Click += new System.EventHandler(this.miAmdHDRToggle_Click);
            // 
            // miAmdHDREnabled
            // 
            this.miAmdHDREnabled.Name = "miAmdHDREnabled";
            this.miAmdHDREnabled.Size = new System.Drawing.Size(136, 22);
            this.miAmdHDREnabled.Text = "Enabled";
            this.miAmdHDREnabled.Click += new System.EventHandler(this.miAmdHDREnabled_Click);
            // 
            // miAmdCopyId
            // 
            this.miAmdCopyId.Name = "miAmdCopyId";
            this.miAmdCopyId.Size = new System.Drawing.Size(184, 22);
            this.miAmdCopyId.Text = "Copy Id to Clipboard";
            this.miAmdCopyId.Click += new System.EventHandler(this.miAmdCopyId_Click);
            // 
            // btnApplyAmd
            // 
            this.btnApplyAmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApplyAmd.Enabled = false;
            this.btnApplyAmd.Location = new System.Drawing.Point(7, 429);
            this.btnApplyAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnApplyAmd.Name = "btnApplyAmd";
            this.btnApplyAmd.Size = new System.Drawing.Size(88, 27);
            this.btnApplyAmd.TabIndex = 11;
            this.btnApplyAmd.Text = "Apply";
            this.btnApplyAmd.UseVisualStyleBackColor = true;
            this.btnApplyAmd.Click += new System.EventHandler(this.btnApplyAmd_Click);
            // 
            // lvAmdPresets
            // 
            this.lvAmdPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAmdPresets.CheckBoxes = true;
            this.lvAmdPresets.ContextMenuStrip = this.mnuAmdPresets;
            this.lvAmdPresets.FullRowSelect = true;
            this.lvAmdPresets.Location = new System.Drawing.Point(7, 7);
            this.lvAmdPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvAmdPresets.MultiSelect = false;
            this.lvAmdPresets.Name = "lvAmdPresets";
            this.lvAmdPresets.ShowItemToolTips = true;
            this.lvAmdPresets.Size = new System.Drawing.Size(1098, 415);
            this.lvAmdPresets.TabIndex = 10;
            this.lvAmdPresets.UseCompatibleStateImageBehavior = false;
            this.lvAmdPresets.View = System.Windows.Forms.View.Details;
            this.lvAmdPresets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLgPresets_ColumnClick);
            this.lvAmdPresets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvNvPresets_ItemCheck);
            this.lvAmdPresets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvAmdPresets_ItemChecked);
            this.lvAmdPresets.SelectedIndexChanged += new System.EventHandler(this.lvAmdPresets_SelectedIndexChanged);
            this.lvAmdPresets.DoubleClick += new System.EventHandler(this.btnApplyAmd_Click);
            // 
            // tabLG
            // 
            this.tabLG.Controls.Add(this.scLgController);
            this.tabLG.Location = new System.Drawing.Point(4, 24);
            this.tabLG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLG.Name = "tabLG";
            this.tabLG.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLG.Size = new System.Drawing.Size(1114, 539);
            this.tabLG.TabIndex = 1;
            this.tabLG.Text = "LG controller";
            this.tabLG.UseVisualStyleBackColor = true;
            // 
            // scLgController
            // 
            this.scLgController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scLgController.Location = new System.Drawing.Point(4, 3);
            this.scLgController.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scLgController.Name = "scLgController";
            // 
            // scLgController.Panel1
            // 
            this.scLgController.Panel1.Controls.Add(this.btnLgSettings);
            this.scLgController.Panel1.Controls.Add(this.chkLgQuickAccess);
            this.scLgController.Panel1.Controls.Add(this.cbxLgPcHdmiPort);
            this.scLgController.Panel1.Controls.Add(this.lblLgPcHdmiPort);
            this.scLgController.Panel1.Controls.Add(this.edtLgPresetDescription);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetDescription);
            this.scLgController.Panel1.Controls.Add(this.btnLgDeviceOptionsHelp);
            this.scLgController.Panel1.Controls.Add(this.btnLgGameBar);
            this.scLgController.Panel1.Controls.Add(this.btnLgPresetEditTriggerConditions);
            this.scLgController.Panel1.Controls.Add(this.edtLgPresetTriggerConditions);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetExcludedProcesses);
            this.scLgController.Panel1.Controls.Add(this.edtLgPresetExcludedProcesses);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetIncludedProcesses);
            this.scLgController.Panel1.Controls.Add(this.edtLgPresetIncludedProcesses);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetTriggerCondition);
            this.scLgController.Panel1.Controls.Add(this.cbxLgPresetTrigger);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetTrigger);
            this.scLgController.Panel1.Controls.Add(this.btnLgExpert);
            this.scLgController.Panel1.Controls.Add(this.chkLgRemoteControlShow);
            this.scLgController.Panel1.Controls.Add(this.label3);
            this.scLgController.Panel1.Controls.Add(this.cbxLgDevices);
            this.scLgController.Panel1.Controls.Add(this.btnLgDeviceConvertToCustom);
            this.scLgController.Panel1.Controls.Add(this.cbxLgApps);
            this.scLgController.Panel1.Controls.Add(this.lvLgPresets);
            this.scLgController.Panel1.Controls.Add(this.label5);
            this.scLgController.Panel1.Controls.Add(this.cbxLgPresetDevice);
            this.scLgController.Panel1.Controls.Add(this.btnDeleteLg);
            this.scLgController.Panel1.Controls.Add(this.btnApplyLg);
            this.scLgController.Panel1.Controls.Add(this.lblDeviceFilter);
            this.scLgController.Panel1.Controls.Add(this.lblLgPresetDevice);
            this.scLgController.Panel1.Controls.Add(this.btnAddLg);
            this.scLgController.Panel1.Controls.Add(this.edtShortcutLg);
            this.scLgController.Panel1.Controls.Add(this.edtLgDeviceFilter);
            this.scLgController.Panel1.Controls.Add(this.btnLgRemoveDevice);
            this.scLgController.Panel1.Controls.Add(this.btnLgRefreshApps);
            this.scLgController.Panel1.Controls.Add(this.label2);
            this.scLgController.Panel1.Controls.Add(this.label1);
            this.scLgController.Panel1.Controls.Add(this.btnLgAddDevice);
            this.scLgController.Panel1.Controls.Add(this.edtNameLg);
            this.scLgController.Panel1.Controls.Add(this.btnSetShortcutLg);
            this.scLgController.Panel1.Controls.Add(this.edtStepsLg);
            this.scLgController.Panel1.Controls.Add(this.btnLgAddButton);
            this.scLgController.Panel1.Controls.Add(this.btnCloneLg);
            this.scLgController.Panel1.Controls.Add(this.lblStepsLg);
            this.scLgController.Panel1.Controls.Add(this.btnLgDeviceFilterRefresh);
            this.scLgController.Panel1.Controls.Add(this.clbLgPower);
            this.scLgController.Panel1.Controls.Add(this.lblLgError);
            this.scLgController.Panel1MinSize = 750;
            this.scLgController.Panel2MinSize = 150;
            this.scLgController.Size = new System.Drawing.Size(1106, 533);
            this.scLgController.SplitterDistance = 894;
            this.scLgController.SplitterWidth = 5;
            this.scLgController.TabIndex = 43;
            // 
            // btnLgSettings
            // 
            this.btnLgSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLgSettings.Location = new System.Drawing.Point(804, 278);
            this.btnLgSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgSettings.Name = "btnLgSettings";
            this.btnLgSettings.Size = new System.Drawing.Size(88, 27);
            this.btnLgSettings.TabIndex = 64;
            this.btnLgSettings.Text = "Settings...";
            this.btnLgSettings.UseVisualStyleBackColor = true;
            this.btnLgSettings.Click += new System.EventHandler(this.btnLgSettings_Click);
            // 
            // chkLgQuickAccess
            // 
            this.chkLgQuickAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLgQuickAccess.AutoSize = true;
            this.chkLgQuickAccess.Enabled = false;
            this.chkLgQuickAccess.Location = new System.Drawing.Point(341, 314);
            this.chkLgQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkLgQuickAccess.Name = "chkLgQuickAccess";
            this.chkLgQuickAccess.Size = new System.Drawing.Size(96, 19);
            this.chkLgQuickAccess.TabIndex = 63;
            this.chkLgQuickAccess.Text = "Quick Access";
            this.chkLgQuickAccess.UseVisualStyleBackColor = true;
            // 
            // cbxLgPcHdmiPort
            // 
            this.cbxLgPcHdmiPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLgPcHdmiPort.FormattingEnabled = true;
            this.cbxLgPcHdmiPort.Items.AddRange(new object[] {
            "None",
            "HDMI1",
            "HDMI2",
            "HDMI3",
            "HDMI4"});
            this.cbxLgPcHdmiPort.Location = new System.Drawing.Point(667, 42);
            this.cbxLgPcHdmiPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLgPcHdmiPort.Name = "cbxLgPcHdmiPort";
            this.cbxLgPcHdmiPort.Size = new System.Drawing.Size(86, 23);
            this.cbxLgPcHdmiPort.TabIndex = 62;
            this.cbxLgPcHdmiPort.SelectedIndexChanged += new System.EventHandler(this.cbxLgPcHdmiPort_SelectedIndexChanged);
            // 
            // lblLgPcHdmiPort
            // 
            this.lblLgPcHdmiPort.AutoSize = true;
            this.lblLgPcHdmiPort.Location = new System.Drawing.Point(577, 45);
            this.lblLgPcHdmiPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPcHdmiPort.Name = "lblLgPcHdmiPort";
            this.lblLgPcHdmiPort.Size = new System.Drawing.Size(84, 15);
            this.lblLgPcHdmiPort.TabIndex = 61;
            this.lblLgPcHdmiPort.Text = "PC HDMI port:";
            // 
            // edtLgPresetDescription
            // 
            this.edtLgPresetDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtLgPresetDescription.Enabled = false;
            this.edtLgPresetDescription.Location = new System.Drawing.Point(100, 483);
            this.edtLgPresetDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgPresetDescription.Multiline = true;
            this.edtLgPresetDescription.Name = "edtLgPresetDescription";
            this.edtLgPresetDescription.Size = new System.Drawing.Size(694, 43);
            this.edtLgPresetDescription.TabIndex = 60;
            // 
            // lblLgPresetDescription
            // 
            this.lblLgPresetDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetDescription.AutoSize = true;
            this.lblLgPresetDescription.Location = new System.Drawing.Point(6, 488);
            this.lblLgPresetDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetDescription.Name = "lblLgPresetDescription";
            this.lblLgPresetDescription.Size = new System.Drawing.Size(70, 15);
            this.lblLgPresetDescription.TabIndex = 59;
            this.lblLgPresetDescription.Text = "Description:";
            // 
            // btnLgDeviceOptionsHelp
            // 
            this.btnLgDeviceOptionsHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLgDeviceOptionsHelp.Location = new System.Drawing.Point(867, 70);
            this.btnLgDeviceOptionsHelp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgDeviceOptionsHelp.Name = "btnLgDeviceOptionsHelp";
            this.btnLgDeviceOptionsHelp.Size = new System.Drawing.Size(22, 27);
            this.btnLgDeviceOptionsHelp.TabIndex = 58;
            this.btnLgDeviceOptionsHelp.Text = "?";
            this.btnLgDeviceOptionsHelp.UseVisualStyleBackColor = true;
            this.btnLgDeviceOptionsHelp.Click += new System.EventHandler(this.btnLgDeviceOptionsHelp_Click);
            // 
            // btnLgGameBar
            // 
            this.btnLgGameBar.Location = new System.Drawing.Point(478, 40);
            this.btnLgGameBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgGameBar.Name = "btnLgGameBar";
            this.btnLgGameBar.Size = new System.Drawing.Size(88, 27);
            this.btnLgGameBar.TabIndex = 55;
            this.btnLgGameBar.Text = "Game Bar";
            this.btnLgGameBar.UseVisualStyleBackColor = true;
            this.btnLgGameBar.Click += new System.EventHandler(this.btnLgGameBar_Click);
            // 
            // btnLgPresetEditTriggerConditions
            // 
            this.btnLgPresetEditTriggerConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLgPresetEditTriggerConditions.Enabled = false;
            this.btnLgPresetEditTriggerConditions.Location = new System.Drawing.Point(804, 341);
            this.btnLgPresetEditTriggerConditions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgPresetEditTriggerConditions.Name = "btnLgPresetEditTriggerConditions";
            this.btnLgPresetEditTriggerConditions.Size = new System.Drawing.Size(88, 27);
            this.btnLgPresetEditTriggerConditions.TabIndex = 54;
            this.btnLgPresetEditTriggerConditions.Text = "Edit...";
            this.btnLgPresetEditTriggerConditions.UseVisualStyleBackColor = true;
            this.btnLgPresetEditTriggerConditions.Click += new System.EventHandler(this.btnLgPresetEditTriggerConditions_Click);
            // 
            // edtLgPresetTriggerConditions
            // 
            this.edtLgPresetTriggerConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtLgPresetTriggerConditions.Enabled = false;
            this.edtLgPresetTriggerConditions.Location = new System.Drawing.Point(601, 343);
            this.edtLgPresetTriggerConditions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgPresetTriggerConditions.Name = "edtLgPresetTriggerConditions";
            this.edtLgPresetTriggerConditions.ReadOnly = true;
            this.edtLgPresetTriggerConditions.Size = new System.Drawing.Size(194, 23);
            this.edtLgPresetTriggerConditions.TabIndex = 53;
            // 
            // lblLgPresetExcludedProcesses
            // 
            this.lblLgPresetExcludedProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetExcludedProcesses.AutoSize = true;
            this.lblLgPresetExcludedProcesses.Location = new System.Drawing.Point(475, 408);
            this.lblLgPresetExcludedProcesses.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetExcludedProcesses.Name = "lblLgPresetExcludedProcesses";
            this.lblLgPresetExcludedProcesses.Size = new System.Drawing.Size(112, 15);
            this.lblLgPresetExcludedProcesses.TabIndex = 52;
            this.lblLgPresetExcludedProcesses.Text = "Excluded processes:";
            // 
            // edtLgPresetExcludedProcesses
            // 
            this.edtLgPresetExcludedProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtLgPresetExcludedProcesses.Enabled = false;
            this.edtLgPresetExcludedProcesses.Location = new System.Drawing.Point(601, 405);
            this.edtLgPresetExcludedProcesses.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgPresetExcludedProcesses.Name = "edtLgPresetExcludedProcesses";
            this.edtLgPresetExcludedProcesses.Size = new System.Drawing.Size(289, 23);
            this.edtLgPresetExcludedProcesses.TabIndex = 51;
            // 
            // lblLgPresetIncludedProcesses
            // 
            this.lblLgPresetIncludedProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetIncludedProcesses.AutoSize = true;
            this.lblLgPresetIncludedProcesses.Location = new System.Drawing.Point(475, 378);
            this.lblLgPresetIncludedProcesses.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetIncludedProcesses.Name = "lblLgPresetIncludedProcesses";
            this.lblLgPresetIncludedProcesses.Size = new System.Drawing.Size(110, 15);
            this.lblLgPresetIncludedProcesses.TabIndex = 50;
            this.lblLgPresetIncludedProcesses.Text = "Included processes:";
            // 
            // edtLgPresetIncludedProcesses
            // 
            this.edtLgPresetIncludedProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtLgPresetIncludedProcesses.Enabled = false;
            this.edtLgPresetIncludedProcesses.Location = new System.Drawing.Point(601, 375);
            this.edtLgPresetIncludedProcesses.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgPresetIncludedProcesses.Name = "edtLgPresetIncludedProcesses";
            this.edtLgPresetIncludedProcesses.Size = new System.Drawing.Size(289, 23);
            this.edtLgPresetIncludedProcesses.TabIndex = 49;
            // 
            // lblLgPresetTriggerCondition
            // 
            this.lblLgPresetTriggerCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetTriggerCondition.AutoSize = true;
            this.lblLgPresetTriggerCondition.Location = new System.Drawing.Point(475, 347);
            this.lblLgPresetTriggerCondition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetTriggerCondition.Name = "lblLgPresetTriggerCondition";
            this.lblLgPresetTriggerCondition.Size = new System.Drawing.Size(63, 15);
            this.lblLgPresetTriggerCondition.TabIndex = 47;
            this.lblLgPresetTriggerCondition.Text = "Condition:";
            // 
            // cbxLgPresetTrigger
            // 
            this.cbxLgPresetTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxLgPresetTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLgPresetTrigger.Enabled = false;
            this.cbxLgPresetTrigger.FormattingEnabled = true;
            this.cbxLgPresetTrigger.Location = new System.Drawing.Point(601, 312);
            this.cbxLgPresetTrigger.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLgPresetTrigger.Name = "cbxLgPresetTrigger";
            this.cbxLgPresetTrigger.Size = new System.Drawing.Size(289, 23);
            this.cbxLgPresetTrigger.TabIndex = 46;
            // 
            // lblLgPresetTrigger
            // 
            this.lblLgPresetTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetTrigger.AutoSize = true;
            this.lblLgPresetTrigger.Location = new System.Drawing.Point(475, 315);
            this.lblLgPresetTrigger.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetTrigger.Name = "lblLgPresetTrigger";
            this.lblLgPresetTrigger.Size = new System.Drawing.Size(46, 15);
            this.lblLgPresetTrigger.TabIndex = 45;
            this.lblLgPresetTrigger.Text = "Trigger:";
            // 
            // btnLgExpert
            // 
            this.btnLgExpert.ContextMenuStrip = this.mnuLgExpert;
            this.btnLgExpert.Location = new System.Drawing.Point(368, 39);
            this.btnLgExpert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgExpert.Name = "btnLgExpert";
            this.btnLgExpert.Size = new System.Drawing.Size(88, 27);
            this.btnLgExpert.TabIndex = 44;
            this.btnLgExpert.Text = "Expert...";
            this.btnLgExpert.UseVisualStyleBackColor = true;
            this.btnLgExpert.Click += new System.EventHandler(this.btnLgExpert_Click);
            // 
            // mnuLgExpert
            // 
            this.mnuLgExpert.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuLgExpert.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLgOLEDMotionPro,
            this.miLgExpertSeparator1});
            this.mnuLgExpert.Name = "mnuLgButtons";
            this.mnuLgExpert.Size = new System.Drawing.Size(359, 32);
            this.mnuLgExpert.Opening += new System.ComponentModel.CancelEventHandler(this.mnuLgExpert_Opening);
            // 
            // mnuLgOLEDMotionPro
            // 
            this.mnuLgOLEDMotionPro.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLgEnableMotionPro,
            this.miLgDisableMotionPro});
            this.mnuLgOLEDMotionPro.Name = "mnuLgOLEDMotionPro";
            this.mnuLgOLEDMotionPro.Size = new System.Drawing.Size(358, 22);
            this.mnuLgOLEDMotionPro.Text = "Activate OLED Motion Pro (B9/C9/E9/W9/C2/G2 only)";
            // 
            // miLgEnableMotionPro
            // 
            this.miLgEnableMotionPro.Name = "miLgEnableMotionPro";
            this.miLgEnableMotionPro.Size = new System.Drawing.Size(207, 22);
            this.miLgEnableMotionPro.Text = "Enable OLED Motion Pro";
            this.miLgEnableMotionPro.Click += new System.EventHandler(this.miLgEnableMotionPro_Click);
            // 
            // miLgDisableMotionPro
            // 
            this.miLgDisableMotionPro.Name = "miLgDisableMotionPro";
            this.miLgDisableMotionPro.Size = new System.Drawing.Size(207, 22);
            this.miLgDisableMotionPro.Text = "Disable OLED Motion Pro";
            this.miLgDisableMotionPro.Click += new System.EventHandler(this.miLgDisableMotionPro_Click);
            // 
            // miLgExpertSeparator1
            // 
            this.miLgExpertSeparator1.Name = "miLgExpertSeparator1";
            this.miLgExpertSeparator1.Size = new System.Drawing.Size(355, 6);
            // 
            // chkLgRemoteControlShow
            // 
            this.chkLgRemoteControlShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLgRemoteControlShow.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkLgRemoteControlShow.AutoSize = true;
            this.chkLgRemoteControlShow.Location = new System.Drawing.Point(788, 8);
            this.chkLgRemoteControlShow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkLgRemoteControlShow.Name = "chkLgRemoteControlShow";
            this.chkLgRemoteControlShow.Size = new System.Drawing.Size(101, 25);
            this.chkLgRemoteControlShow.TabIndex = 43;
            this.chkLgRemoteControlShow.Text = "Remote Control";
            this.chkLgRemoteControlShow.UseVisualStyleBackColor = true;
            this.chkLgRemoteControlShow.CheckedChanged += new System.EventHandler(this.chkLgRemoteControlShow_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 31;
            this.label3.Text = "Device:";
            // 
            // cbxLgDevices
            // 
            this.cbxLgDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLgDevices.DropDownWidth = 400;
            this.cbxLgDevices.FormattingEnabled = true;
            this.cbxLgDevices.Location = new System.Drawing.Point(100, 9);
            this.cbxLgDevices.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLgDevices.Name = "cbxLgDevices";
            this.cbxLgDevices.Size = new System.Drawing.Size(370, 23);
            this.cbxLgDevices.TabIndex = 30;
            this.cbxLgDevices.SelectedIndexChanged += new System.EventHandler(this.cbxLgDevices_SelectedIndexChanged);
            // 
            // btnLgDeviceConvertToCustom
            // 
            this.btnLgDeviceConvertToCustom.Location = new System.Drawing.Point(667, 8);
            this.btnLgDeviceConvertToCustom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgDeviceConvertToCustom.Name = "btnLgDeviceConvertToCustom";
            this.btnLgDeviceConvertToCustom.Size = new System.Drawing.Size(88, 27);
            this.btnLgDeviceConvertToCustom.TabIndex = 41;
            this.btnLgDeviceConvertToCustom.Text = "To custom...";
            this.btnLgDeviceConvertToCustom.UseVisualStyleBackColor = true;
            this.btnLgDeviceConvertToCustom.Click += new System.EventHandler(this.btnLgDeviceConvertToCustom_Click);
            // 
            // cbxLgApps
            // 
            this.cbxLgApps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxLgApps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLgApps.Enabled = false;
            this.cbxLgApps.FormattingEnabled = true;
            this.cbxLgApps.Location = new System.Drawing.Point(100, 374);
            this.cbxLgApps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLgApps.Name = "cbxLgApps";
            this.cbxLgApps.Size = new System.Drawing.Size(233, 23);
            this.cbxLgApps.TabIndex = 26;
            this.cbxLgApps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbxLgApps_KeyPress);
            // 
            // lvLgPresets
            // 
            this.lvLgPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLgPresets.CheckBoxes = true;
            this.lvLgPresets.FullRowSelect = true;
            this.lvLgPresets.Location = new System.Drawing.Point(6, 152);
            this.lvLgPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvLgPresets.MultiSelect = false;
            this.lvLgPresets.Name = "lvLgPresets";
            this.lvLgPresets.ShowItemToolTips = true;
            this.lvLgPresets.Size = new System.Drawing.Size(883, 120);
            this.lvLgPresets.TabIndex = 8;
            this.lvLgPresets.UseCompatibleStateImageBehavior = false;
            this.lvLgPresets.View = System.Windows.Forms.View.Details;
            this.lvLgPresets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLgPresets_ColumnClick);
            this.lvLgPresets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvLgPresets_ItemChecked);
            this.lvLgPresets.SelectedIndexChanged += new System.EventHandler(this.lvLgPresets_SelectedIndexChanged);
            this.lvLgPresets.DoubleClick += new System.EventHandler(this.lvLgPresets_DoubleClick);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 378);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 15);
            this.label5.TabIndex = 25;
            this.label5.Text = "App:";
            // 
            // cbxLgPresetDevice
            // 
            this.cbxLgPresetDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxLgPresetDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLgPresetDevice.Enabled = false;
            this.cbxLgPresetDevice.FormattingEnabled = true;
            this.cbxLgPresetDevice.Location = new System.Drawing.Point(100, 342);
            this.cbxLgPresetDevice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLgPresetDevice.Name = "cbxLgPresetDevice";
            this.cbxLgPresetDevice.Size = new System.Drawing.Size(370, 23);
            this.cbxLgPresetDevice.TabIndex = 40;
            // 
            // btnDeleteLg
            // 
            this.btnDeleteLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteLg.Enabled = false;
            this.btnDeleteLg.Location = new System.Drawing.Point(289, 278);
            this.btnDeleteLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteLg.Name = "btnDeleteLg";
            this.btnDeleteLg.Size = new System.Drawing.Size(88, 27);
            this.btnDeleteLg.TabIndex = 27;
            this.btnDeleteLg.Text = "Delete";
            this.btnDeleteLg.UseVisualStyleBackColor = true;
            this.btnDeleteLg.Click += new System.EventHandler(this.btnDeleteLg_Click);
            // 
            // btnApplyLg
            // 
            this.btnApplyLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApplyLg.Enabled = false;
            this.btnApplyLg.Location = new System.Drawing.Point(6, 278);
            this.btnApplyLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnApplyLg.Name = "btnApplyLg";
            this.btnApplyLg.Size = new System.Drawing.Size(88, 27);
            this.btnApplyLg.TabIndex = 9;
            this.btnApplyLg.Text = "Apply";
            this.btnApplyLg.UseVisualStyleBackColor = true;
            this.btnApplyLg.Click += new System.EventHandler(this.btnApplyLg_Click);
            // 
            // lblDeviceFilter
            // 
            this.lblDeviceFilter.AutoSize = true;
            this.lblDeviceFilter.Location = new System.Drawing.Point(6, 45);
            this.lblDeviceFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDeviceFilter.Name = "lblDeviceFilter";
            this.lblDeviceFilter.Size = new System.Drawing.Size(72, 15);
            this.lblDeviceFilter.TabIndex = 24;
            this.lblDeviceFilter.Text = "Device filter:";
            // 
            // lblLgPresetDevice
            // 
            this.lblLgPresetDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLgPresetDevice.AutoSize = true;
            this.lblLgPresetDevice.Location = new System.Drawing.Point(6, 345);
            this.lblLgPresetDevice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgPresetDevice.Name = "lblLgPresetDevice";
            this.lblLgPresetDevice.Size = new System.Drawing.Size(45, 15);
            this.lblLgPresetDevice.TabIndex = 39;
            this.lblLgPresetDevice.Text = "Device:";
            // 
            // btnAddLg
            // 
            this.btnAddLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddLg.Location = new System.Drawing.Point(195, 278);
            this.btnAddLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddLg.Name = "btnAddLg";
            this.btnAddLg.Size = new System.Drawing.Size(88, 27);
            this.btnAddLg.TabIndex = 28;
            this.btnAddLg.Text = "Add";
            this.btnAddLg.UseVisualStyleBackColor = true;
            this.btnAddLg.Click += new System.EventHandler(this.btnAddLg_Click);
            // 
            // edtShortcutLg
            // 
            this.edtShortcutLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtShortcutLg.Enabled = false;
            this.edtShortcutLg.Location = new System.Drawing.Point(100, 405);
            this.edtShortcutLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtShortcutLg.Name = "edtShortcutLg";
            this.edtShortcutLg.ReadOnly = true;
            this.edtShortcutLg.Size = new System.Drawing.Size(233, 23);
            this.edtShortcutLg.TabIndex = 11;
            this.edtShortcutLg.TextChanged += new System.EventHandler(this.edtShortcutLg_TextChanged);
            this.edtShortcutLg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyDown);
            this.edtShortcutLg.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyUp);
            // 
            // edtLgDeviceFilter
            // 
            this.edtLgDeviceFilter.Location = new System.Drawing.Point(100, 40);
            this.edtLgDeviceFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgDeviceFilter.Name = "edtLgDeviceFilter";
            this.edtLgDeviceFilter.Size = new System.Drawing.Size(165, 23);
            this.edtLgDeviceFilter.TabIndex = 23;
            this.edtLgDeviceFilter.TextChanged += new System.EventHandler(this.edtLgDeviceFilter_TextChanged);
            // 
            // btnLgRemoveDevice
            // 
            this.btnLgRemoveDevice.Location = new System.Drawing.Point(573, 8);
            this.btnLgRemoveDevice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgRemoveDevice.Name = "btnLgRemoveDevice";
            this.btnLgRemoveDevice.Size = new System.Drawing.Size(88, 27);
            this.btnLgRemoveDevice.TabIndex = 38;
            this.btnLgRemoveDevice.Text = "Remove";
            this.btnLgRemoveDevice.UseVisualStyleBackColor = true;
            this.btnLgRemoveDevice.Click += new System.EventHandler(this.btnLgRemoveDevice_Click);
            // 
            // btnLgRefreshApps
            // 
            this.btnLgRefreshApps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLgRefreshApps.Enabled = false;
            this.btnLgRefreshApps.Location = new System.Drawing.Point(341, 372);
            this.btnLgRefreshApps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgRefreshApps.Name = "btnLgRefreshApps";
            this.btnLgRefreshApps.Size = new System.Drawing.Size(88, 27);
            this.btnLgRefreshApps.TabIndex = 29;
            this.btnLgRefreshApps.Text = "Refresh";
            this.btnLgRefreshApps.UseVisualStyleBackColor = true;
            this.btnLgRefreshApps.Click += new System.EventHandler(this.btnLgRefreshApps_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 409);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "Shortcut:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 315);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 20;
            this.label1.Text = "Name:";
            // 
            // btnLgAddDevice
            // 
            this.btnLgAddDevice.Location = new System.Drawing.Point(478, 8);
            this.btnLgAddDevice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgAddDevice.Name = "btnLgAddDevice";
            this.btnLgAddDevice.Size = new System.Drawing.Size(88, 27);
            this.btnLgAddDevice.TabIndex = 37;
            this.btnLgAddDevice.Text = "Add";
            this.btnLgAddDevice.UseVisualStyleBackColor = true;
            this.btnLgAddDevice.Click += new System.EventHandler(this.btnLgAddDevice_Click);
            // 
            // edtNameLg
            // 
            this.edtNameLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtNameLg.Enabled = false;
            this.edtNameLg.Location = new System.Drawing.Point(100, 312);
            this.edtNameLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtNameLg.Name = "edtNameLg";
            this.edtNameLg.Size = new System.Drawing.Size(233, 23);
            this.edtNameLg.TabIndex = 19;
            // 
            // btnSetShortcutLg
            // 
            this.btnSetShortcutLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetShortcutLg.Enabled = false;
            this.btnSetShortcutLg.Location = new System.Drawing.Point(384, 278);
            this.btnSetShortcutLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSetShortcutLg.Name = "btnSetShortcutLg";
            this.btnSetShortcutLg.Size = new System.Drawing.Size(88, 27);
            this.btnSetShortcutLg.TabIndex = 13;
            this.btnSetShortcutLg.Text = "Save";
            this.btnSetShortcutLg.UseVisualStyleBackColor = true;
            this.btnSetShortcutLg.Click += new System.EventHandler(this.btnSetShortcutLg_Click);
            // 
            // edtStepsLg
            // 
            this.edtStepsLg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtStepsLg.Enabled = false;
            this.edtStepsLg.Location = new System.Drawing.Point(100, 434);
            this.edtStepsLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtStepsLg.Multiline = true;
            this.edtStepsLg.Name = "edtStepsLg";
            this.edtStepsLg.Size = new System.Drawing.Size(694, 43);
            this.edtStepsLg.TabIndex = 17;
            // 
            // btnLgAddButton
            // 
            this.btnLgAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLgAddButton.ContextMenuStrip = this.mnuLgButtons;
            this.btnLgAddButton.Enabled = false;
            this.btnLgAddButton.Location = new System.Drawing.Point(802, 433);
            this.btnLgAddButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgAddButton.Name = "btnLgAddButton";
            this.btnLgAddButton.Size = new System.Drawing.Size(88, 27);
            this.btnLgAddButton.TabIndex = 32;
            this.btnLgAddButton.Text = "Add step";
            this.btnLgAddButton.UseVisualStyleBackColor = true;
            this.btnLgAddButton.Click += new System.EventHandler(this.btnLgAddButton_Click);
            // 
            // mnuLgButtons
            // 
            this.mnuLgButtons.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuLgButtons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLgRcButtons,
            this.mnuLgActions,
            this.mnuLgNvPresets,
            this.mnuLgAmdPresets,
            this.mnuLgProgram});
            this.mnuLgButtons.Name = "mnuLgButtons";
            this.mnuLgButtons.Size = new System.Drawing.Size(153, 114);
            this.mnuLgButtons.Opening += new System.ComponentModel.CancelEventHandler(this.mnuLgButtons_Opening);
            // 
            // mnuLgRcButtons
            // 
            this.mnuLgRcButtons.Name = "mnuLgRcButtons";
            this.mnuLgRcButtons.Size = new System.Drawing.Size(152, 22);
            this.mnuLgRcButtons.Text = "Buttons";
            // 
            // mnuLgActions
            // 
            this.mnuLgActions.Name = "mnuLgActions";
            this.mnuLgActions.Size = new System.Drawing.Size(152, 22);
            this.mnuLgActions.Text = "Actions";
            // 
            // mnuLgNvPresets
            // 
            this.mnuLgNvPresets.Name = "mnuLgNvPresets";
            this.mnuLgNvPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuLgNvPresets.Text = "NVIDIA presets";
            // 
            // mnuLgAmdPresets
            // 
            this.mnuLgAmdPresets.Name = "mnuLgAmdPresets";
            this.mnuLgAmdPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuLgAmdPresets.Text = "AMD presets";
            // 
            // mnuLgProgram
            // 
            this.mnuLgProgram.Name = "mnuLgProgram";
            this.mnuLgProgram.Size = new System.Drawing.Size(152, 22);
            this.mnuLgProgram.Text = "Start Program";
            this.mnuLgProgram.Click += new System.EventHandler(this.mnuLgProgram_Click);
            // 
            // btnCloneLg
            // 
            this.btnCloneLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCloneLg.Enabled = false;
            this.btnCloneLg.Location = new System.Drawing.Point(100, 278);
            this.btnCloneLg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCloneLg.Name = "btnCloneLg";
            this.btnCloneLg.Size = new System.Drawing.Size(88, 27);
            this.btnCloneLg.TabIndex = 14;
            this.btnCloneLg.Text = "Clone";
            this.btnCloneLg.UseVisualStyleBackColor = true;
            this.btnCloneLg.Click += new System.EventHandler(this.btnCloneLg_Click);
            // 
            // lblStepsLg
            // 
            this.lblStepsLg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStepsLg.AutoSize = true;
            this.lblStepsLg.Location = new System.Drawing.Point(6, 439);
            this.lblStepsLg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStepsLg.Name = "lblStepsLg";
            this.lblStepsLg.Size = new System.Drawing.Size(38, 15);
            this.lblStepsLg.TabIndex = 16;
            this.lblStepsLg.Text = "Steps:";
            // 
            // btnLgDeviceFilterRefresh
            // 
            this.btnLgDeviceFilterRefresh.Location = new System.Drawing.Point(273, 39);
            this.btnLgDeviceFilterRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLgDeviceFilterRefresh.Name = "btnLgDeviceFilterRefresh";
            this.btnLgDeviceFilterRefresh.Size = new System.Drawing.Size(88, 27);
            this.btnLgDeviceFilterRefresh.TabIndex = 35;
            this.btnLgDeviceFilterRefresh.Text = "Refresh";
            this.btnLgDeviceFilterRefresh.UseVisualStyleBackColor = true;
            this.btnLgDeviceFilterRefresh.Click += new System.EventHandler(this.btnLgDeviceFilterRefresh_Click);
            // 
            // clbLgPower
            // 
            this.clbLgPower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbLgPower.CheckOnClick = true;
            this.clbLgPower.ColumnWidth = 240;
            this.clbLgPower.FormattingEnabled = true;
            this.clbLgPower.Items.AddRange(new object[] {
            "Power on after startup",
            "Power on after resume from standby",
            "Power off on shutdown",
            "Power off on standby",
            "Power off when screensaver activates",
            "Power on when screensaver deactivates",
            "Power on even after manual power off",
            "Allow triggers to be fired for this device",
            "Use Windows power settings to power on",
            "Use Windows power settings to power off",
            "Use secure connection"});
            this.clbLgPower.Location = new System.Drawing.Point(6, 70);
            this.clbLgPower.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbLgPower.MultiColumn = true;
            this.clbLgPower.Name = "clbLgPower";
            this.clbLgPower.Size = new System.Drawing.Size(856, 76);
            this.clbLgPower.TabIndex = 34;
            this.clbLgPower.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbLgPower_ItemCheck);
            // 
            // lblLgError
            // 
            this.lblLgError.AutoSize = true;
            this.lblLgError.Location = new System.Drawing.Point(6, 9);
            this.lblLgError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgError.Name = "lblLgError";
            this.lblLgError.Size = new System.Drawing.Size(53, 15);
            this.lblLgError.TabIndex = 15;
            this.lblLgError.Text = "ErrorText";
            this.lblLgError.Visible = false;
            // 
            // tabGameLauncher
            // 
            this.tabGameLauncher.Controls.Add(this.cbxGameStepType);
            this.tabGameLauncher.Controls.Add(this.btnGameOptions);
            this.tabGameLauncher.Controls.Add(this.chkGameQuickAccess);
            this.tabGameLauncher.Controls.Add(this.btnGameSettings);
            this.tabGameLauncher.Controls.Add(this.chkGameRunAsAdmin);
            this.tabGameLauncher.Controls.Add(this.lblGameParameters);
            this.tabGameLauncher.Controls.Add(this.edtGameParameters);
            this.tabGameLauncher.Controls.Add(this.btnGameBrowse);
            this.tabGameLauncher.Controls.Add(this.lblGameFilePath);
            this.tabGameLauncher.Controls.Add(this.edtGamePath);
            this.tabGameLauncher.Controls.Add(this.edtGamePrelaunchSteps);
            this.tabGameLauncher.Controls.Add(this.btnGameAddStep);
            this.tabGameLauncher.Controls.Add(this.label4);
            this.tabGameLauncher.Controls.Add(this.edtGameName);
            this.tabGameLauncher.Controls.Add(this.btnGameDelete);
            this.tabGameLauncher.Controls.Add(this.btnGameClone);
            this.tabGameLauncher.Controls.Add(this.btnGameSave);
            this.tabGameLauncher.Controls.Add(this.btnGameAdd);
            this.tabGameLauncher.Controls.Add(this.btnGameLaunch);
            this.tabGameLauncher.Controls.Add(this.lvGamePresets);
            this.tabGameLauncher.Location = new System.Drawing.Point(4, 24);
            this.tabGameLauncher.Name = "tabGameLauncher";
            this.tabGameLauncher.Padding = new System.Windows.Forms.Padding(3);
            this.tabGameLauncher.Size = new System.Drawing.Size(1114, 539);
            this.tabGameLauncher.TabIndex = 6;
            this.tabGameLauncher.Text = "Game Launcher";
            this.tabGameLauncher.UseVisualStyleBackColor = true;
            // 
            // cbxGameStepType
            // 
            this.cbxGameStepType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxGameStepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGameStepType.Enabled = false;
            this.cbxGameStepType.FormattingEnabled = true;
            this.cbxGameStepType.Location = new System.Drawing.Point(10, 474);
            this.cbxGameStepType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxGameStepType.Name = "cbxGameStepType";
            this.cbxGameStepType.Size = new System.Drawing.Size(136, 23);
            this.cbxGameStepType.TabIndex = 46;
            this.cbxGameStepType.SelectedIndexChanged += new System.EventHandler(this.cbxGameStepType_SelectedIndexChanged);
            // 
            // btnGameOptions
            // 
            this.btnGameOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameOptions.ContextMenuStrip = this.mnuGameOptions;
            this.btnGameOptions.Enabled = false;
            this.btnGameOptions.Location = new System.Drawing.Point(785, 439);
            this.btnGameOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameOptions.Name = "btnGameOptions";
            this.btnGameOptions.Size = new System.Drawing.Size(88, 27);
            this.btnGameOptions.TabIndex = 45;
            this.btnGameOptions.Text = "Options...";
            this.btnGameOptions.UseVisualStyleBackColor = true;
            this.btnGameOptions.Click += new System.EventHandler(this.btnGameOptions_Click);
            // 
            // mnuGameOptions
            // 
            this.mnuGameOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miGameProcessorAffinity,
            this.miGameProcessPriority});
            this.mnuGameOptions.Name = "mnuLgButtons";
            this.mnuGameOptions.Size = new System.Drawing.Size(166, 48);
            // 
            // miGameProcessorAffinity
            // 
            this.miGameProcessorAffinity.Name = "miGameProcessorAffinity";
            this.miGameProcessorAffinity.Size = new System.Drawing.Size(165, 22);
            this.miGameProcessorAffinity.Text = "Processor affinity";
            this.miGameProcessorAffinity.Click += new System.EventHandler(this.btnGameProcessAffinity_Click);
            // 
            // miGameProcessPriority
            // 
            this.miGameProcessPriority.Name = "miGameProcessPriority";
            this.miGameProcessPriority.Size = new System.Drawing.Size(165, 22);
            this.miGameProcessPriority.Text = "Process priority";
            this.miGameProcessPriority.Click += new System.EventHandler(this.miGameProcessPriority_Click);
            // 
            // chkGameQuickAccess
            // 
            this.chkGameQuickAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGameQuickAccess.AutoSize = true;
            this.chkGameQuickAccess.Enabled = false;
            this.chkGameQuickAccess.Location = new System.Drawing.Point(353, 387);
            this.chkGameQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkGameQuickAccess.Name = "chkGameQuickAccess";
            this.chkGameQuickAccess.Size = new System.Drawing.Size(141, 19);
            this.chkGameQuickAccess.TabIndex = 44;
            this.chkGameQuickAccess.Text = "Show in Quick Access";
            this.chkGameQuickAccess.UseVisualStyleBackColor = true;
            // 
            // btnGameSettings
            // 
            this.btnGameSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGameSettings.ContextMenuStrip = this.mnuGameActions;
            this.btnGameSettings.Location = new System.Drawing.Point(1018, 351);
            this.btnGameSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameSettings.Name = "btnGameSettings";
            this.btnGameSettings.Size = new System.Drawing.Size(88, 27);
            this.btnGameSettings.TabIndex = 43;
            this.btnGameSettings.Text = "Settings...";
            this.btnGameSettings.UseVisualStyleBackColor = true;
            this.btnGameSettings.Click += new System.EventHandler(this.btnGameActions_Click);
            // 
            // mnuGameActions
            // 
            this.mnuGameActions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGameNvInspector,
            this.miGameSetQuickAccessShortcut});
            this.mnuGameActions.Name = "mnuLgButtons";
            this.mnuGameActions.Size = new System.Drawing.Size(212, 48);
            this.mnuGameActions.Opening += new System.ComponentModel.CancelEventHandler(this.mnuGameActions_Opening);
            // 
            // mnuGameNvInspector
            // 
            this.mnuGameNvInspector.Name = "mnuGameNvInspector";
            this.mnuGameNvInspector.Size = new System.Drawing.Size(211, 22);
            this.mnuGameNvInspector.Text = "NVIDIA Profile Inspector";
            this.mnuGameNvInspector.Click += new System.EventHandler(this.mnuGameNvInspector_Click);
            // 
            // miGameSetQuickAccessShortcut
            // 
            this.miGameSetQuickAccessShortcut.Name = "miGameSetQuickAccessShortcut";
            this.miGameSetQuickAccessShortcut.Size = new System.Drawing.Size(211, 22);
            this.miGameSetQuickAccessShortcut.Text = "Set Quick Access Shortcut";
            this.miGameSetQuickAccessShortcut.Click += new System.EventHandler(this.miGameSetQuickAccessShortcut_Click);
            // 
            // chkGameRunAsAdmin
            // 
            this.chkGameRunAsAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGameRunAsAdmin.AutoSize = true;
            this.chkGameRunAsAdmin.Enabled = false;
            this.chkGameRunAsAdmin.Location = new System.Drawing.Point(881, 415);
            this.chkGameRunAsAdmin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkGameRunAsAdmin.Name = "chkGameRunAsAdmin";
            this.chkGameRunAsAdmin.Size = new System.Drawing.Size(135, 19);
            this.chkGameRunAsAdmin.TabIndex = 42;
            this.chkGameRunAsAdmin.Text = "Run as administrator";
            this.chkGameRunAsAdmin.UseVisualStyleBackColor = true;
            // 
            // lblGameParameters
            // 
            this.lblGameParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGameParameters.AutoSize = true;
            this.lblGameParameters.Location = new System.Drawing.Point(8, 445);
            this.lblGameParameters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameParameters.Name = "lblGameParameters";
            this.lblGameParameters.Size = new System.Drawing.Size(69, 15);
            this.lblGameParameters.TabIndex = 41;
            this.lblGameParameters.Text = "Parameters:";
            // 
            // edtGameParameters
            // 
            this.edtGameParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGameParameters.Enabled = false;
            this.edtGameParameters.Location = new System.Drawing.Point(154, 442);
            this.edtGameParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGameParameters.Name = "edtGameParameters";
            this.edtGameParameters.Size = new System.Drawing.Size(623, 23);
            this.edtGameParameters.TabIndex = 40;
            // 
            // btnGameBrowse
            // 
            this.btnGameBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameBrowse.ContextMenuStrip = this.mnuLgButtons;
            this.btnGameBrowse.Enabled = false;
            this.btnGameBrowse.Location = new System.Drawing.Point(785, 410);
            this.btnGameBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameBrowse.Name = "btnGameBrowse";
            this.btnGameBrowse.Size = new System.Drawing.Size(88, 27);
            this.btnGameBrowse.TabIndex = 39;
            this.btnGameBrowse.Text = "Browse...";
            this.btnGameBrowse.UseVisualStyleBackColor = true;
            this.btnGameBrowse.Click += new System.EventHandler(this.btnGameBrowse_Click);
            // 
            // lblGameFilePath
            // 
            this.lblGameFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGameFilePath.AutoSize = true;
            this.lblGameFilePath.Location = new System.Drawing.Point(8, 417);
            this.lblGameFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameFilePath.Name = "lblGameFilePath";
            this.lblGameFilePath.Size = new System.Drawing.Size(51, 15);
            this.lblGameFilePath.TabIndex = 38;
            this.lblGameFilePath.Text = "File/URI:";
            // 
            // edtGamePath
            // 
            this.edtGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGamePath.Enabled = false;
            this.edtGamePath.Location = new System.Drawing.Point(154, 413);
            this.edtGamePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGamePath.Name = "edtGamePath";
            this.edtGamePath.Size = new System.Drawing.Size(623, 23);
            this.edtGamePath.TabIndex = 37;
            // 
            // edtGamePrelaunchSteps
            // 
            this.edtGamePrelaunchSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGamePrelaunchSteps.Enabled = false;
            this.edtGamePrelaunchSteps.Location = new System.Drawing.Point(154, 471);
            this.edtGamePrelaunchSteps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGamePrelaunchSteps.Multiline = true;
            this.edtGamePrelaunchSteps.Name = "edtGamePrelaunchSteps";
            this.edtGamePrelaunchSteps.Size = new System.Drawing.Size(623, 48);
            this.edtGamePrelaunchSteps.TabIndex = 35;
            this.edtGamePrelaunchSteps.Leave += new System.EventHandler(this.edtGamePrelaunchSteps_Leave);
            // 
            // btnGameAddStep
            // 
            this.btnGameAddStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameAddStep.ContextMenuStrip = this.mnuGameAddStep;
            this.btnGameAddStep.Enabled = false;
            this.btnGameAddStep.Location = new System.Drawing.Point(785, 470);
            this.btnGameAddStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameAddStep.Name = "btnGameAddStep";
            this.btnGameAddStep.Size = new System.Drawing.Size(88, 27);
            this.btnGameAddStep.TabIndex = 36;
            this.btnGameAddStep.Text = "Add step";
            this.btnGameAddStep.UseVisualStyleBackColor = true;
            this.btnGameAddStep.Click += new System.EventHandler(this.btnGameAddStep_Click);
            // 
            // mnuGameAddStep
            // 
            this.mnuGameAddStep.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameAddStep.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGameNvidiaPresets,
            this.mnuGameAmdPresets,
            this.mnuGameLgPresets,
            this.mnuGameStartProgram});
            this.mnuGameAddStep.Name = "mnuLgButtons";
            this.mnuGameAddStep.Size = new System.Drawing.Size(153, 92);
            this.mnuGameAddStep.Opening += new System.ComponentModel.CancelEventHandler(this.mnuGameAddStep_Opening);
            // 
            // mnuGameNvidiaPresets
            // 
            this.mnuGameNvidiaPresets.Name = "mnuGameNvidiaPresets";
            this.mnuGameNvidiaPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameNvidiaPresets.Text = "NVIDIA presets";
            // 
            // mnuGameAmdPresets
            // 
            this.mnuGameAmdPresets.Name = "mnuGameAmdPresets";
            this.mnuGameAmdPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameAmdPresets.Text = "AMD presets";
            // 
            // mnuGameLgPresets
            // 
            this.mnuGameLgPresets.Name = "mnuGameLgPresets";
            this.mnuGameLgPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameLgPresets.Text = "LG presets";
            // 
            // mnuGameStartProgram
            // 
            this.mnuGameStartProgram.Name = "mnuGameStartProgram";
            this.mnuGameStartProgram.Size = new System.Drawing.Size(152, 22);
            this.mnuGameStartProgram.Text = "Start Program";
            this.mnuGameStartProgram.Click += new System.EventHandler(this.mnuGameStartProgram_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 388);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 33;
            this.label4.Text = "Name:";
            // 
            // edtGameName
            // 
            this.edtGameName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGameName.Enabled = false;
            this.edtGameName.Location = new System.Drawing.Point(154, 384);
            this.edtGameName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGameName.Name = "edtGameName";
            this.edtGameName.Size = new System.Drawing.Size(191, 23);
            this.edtGameName.TabIndex = 32;
            // 
            // btnGameDelete
            // 
            this.btnGameDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameDelete.Enabled = false;
            this.btnGameDelete.Location = new System.Drawing.Point(291, 351);
            this.btnGameDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameDelete.Name = "btnGameDelete";
            this.btnGameDelete.Size = new System.Drawing.Size(88, 27);
            this.btnGameDelete.TabIndex = 30;
            this.btnGameDelete.Text = "Delete";
            this.btnGameDelete.UseVisualStyleBackColor = true;
            this.btnGameDelete.Click += new System.EventHandler(this.btnGameDelete_Click);
            // 
            // btnGameClone
            // 
            this.btnGameClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameClone.Enabled = false;
            this.btnGameClone.Location = new System.Drawing.Point(103, 351);
            this.btnGameClone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameClone.Name = "btnGameClone";
            this.btnGameClone.Size = new System.Drawing.Size(88, 27);
            this.btnGameClone.TabIndex = 29;
            this.btnGameClone.Text = "Clone";
            this.btnGameClone.UseVisualStyleBackColor = true;
            this.btnGameClone.Click += new System.EventHandler(this.btnGameClone_Click);
            // 
            // btnGameSave
            // 
            this.btnGameSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameSave.Enabled = false;
            this.btnGameSave.Location = new System.Drawing.Point(387, 351);
            this.btnGameSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameSave.Name = "btnGameSave";
            this.btnGameSave.Size = new System.Drawing.Size(88, 27);
            this.btnGameSave.TabIndex = 28;
            this.btnGameSave.Text = "Save";
            this.btnGameSave.UseVisualStyleBackColor = true;
            this.btnGameSave.Click += new System.EventHandler(this.btnGameSave_Click);
            // 
            // btnGameAdd
            // 
            this.btnGameAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameAdd.Location = new System.Drawing.Point(199, 351);
            this.btnGameAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameAdd.Name = "btnGameAdd";
            this.btnGameAdd.Size = new System.Drawing.Size(88, 27);
            this.btnGameAdd.TabIndex = 25;
            this.btnGameAdd.Text = "Add";
            this.btnGameAdd.UseVisualStyleBackColor = true;
            this.btnGameAdd.Click += new System.EventHandler(this.btnGameAdd_Click);
            // 
            // btnGameLaunch
            // 
            this.btnGameLaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameLaunch.Enabled = false;
            this.btnGameLaunch.Location = new System.Drawing.Point(8, 351);
            this.btnGameLaunch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameLaunch.Name = "btnGameLaunch";
            this.btnGameLaunch.Size = new System.Drawing.Size(88, 27);
            this.btnGameLaunch.TabIndex = 24;
            this.btnGameLaunch.Text = "Launch";
            this.btnGameLaunch.UseVisualStyleBackColor = true;
            this.btnGameLaunch.Click += new System.EventHandler(this.btnGameLaunch_Click);
            // 
            // lvGamePresets
            // 
            this.lvGamePresets.AllowDrop = true;
            this.lvGamePresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvGamePresets.CheckBoxes = true;
            this.lvGamePresets.FullRowSelect = true;
            this.lvGamePresets.Location = new System.Drawing.Point(8, 8);
            this.lvGamePresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvGamePresets.MultiSelect = false;
            this.lvGamePresets.Name = "lvGamePresets";
            this.lvGamePresets.ShowItemToolTips = true;
            this.lvGamePresets.Size = new System.Drawing.Size(1098, 335);
            this.lvGamePresets.TabIndex = 23;
            this.lvGamePresets.UseCompatibleStateImageBehavior = false;
            this.lvGamePresets.View = System.Windows.Forms.View.Details;
            this.lvGamePresets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLgPresets_ColumnClick);
            this.lvGamePresets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvGamePresets_ItemChecked);
            this.lvGamePresets.SelectedIndexChanged += new System.EventHandler(this.lvGamePresets_SelectedIndexChanged);
            this.lvGamePresets.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvGamePresets_DragDrop);
            this.lvGamePresets.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvGamePresets_DragEnter);
            this.lvGamePresets.DoubleClick += new System.EventHandler(this.lvGamePresets_DoubleClick);
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.grpNvidiaOptions);
            this.tabOptions.Controls.Add(this.grpMiscellaneousOptions);
            this.tabOptions.Controls.Add(this.grpLGOptions);
            this.tabOptions.Controls.Add(this.grpGeneralOptions);
            this.tabOptions.Location = new System.Drawing.Point(4, 24);
            this.tabOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabOptions.Size = new System.Drawing.Size(1114, 539);
            this.tabOptions.TabIndex = 2;
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // grpNvidiaOptions
            // 
            this.grpNvidiaOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNvidiaOptions.Controls.Add(this.lblDitheringDisplay);
            this.grpNvidiaOptions.Controls.Add(this.cbxDitheringDisplay);
            this.grpNvidiaOptions.Controls.Add(this.lblDitheringMode);
            this.grpNvidiaOptions.Controls.Add(this.cbxDitheringMode);
            this.grpNvidiaOptions.Controls.Add(this.lblDitheringBitDepth);
            this.grpNvidiaOptions.Controls.Add(this.cbxDitheringBitDepth);
            this.grpNvidiaOptions.Controls.Add(this.chkDitheringEnabled);
            this.grpNvidiaOptions.Controls.Add(this.pbGradient);
            this.grpNvidiaOptions.Location = new System.Drawing.Point(537, 7);
            this.grpNvidiaOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNvidiaOptions.Name = "grpNvidiaOptions";
            this.grpNvidiaOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNvidiaOptions.Size = new System.Drawing.Size(569, 365);
            this.grpNvidiaOptions.TabIndex = 6;
            this.grpNvidiaOptions.TabStop = false;
            this.grpNvidiaOptions.Text = "NVIDIA options - test dithering";
            // 
            // lblDitheringDisplay
            // 
            this.lblDitheringDisplay.AutoSize = true;
            this.lblDitheringDisplay.Location = new System.Drawing.Point(7, 23);
            this.lblDitheringDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDitheringDisplay.Name = "lblDitheringDisplay";
            this.lblDitheringDisplay.Size = new System.Drawing.Size(48, 15);
            this.lblDitheringDisplay.TabIndex = 9;
            this.lblDitheringDisplay.Text = "Display:";
            // 
            // cbxDitheringDisplay
            // 
            this.cbxDitheringDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDitheringDisplay.FormattingEnabled = true;
            this.cbxDitheringDisplay.Location = new System.Drawing.Point(75, 17);
            this.cbxDitheringDisplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxDitheringDisplay.Name = "cbxDitheringDisplay";
            this.cbxDitheringDisplay.Size = new System.Drawing.Size(212, 23);
            this.cbxDitheringDisplay.TabIndex = 8;
            this.cbxDitheringDisplay.SelectedIndexChanged += new System.EventHandler(this.cbxDitheringDisplay_SelectedIndexChanged);
            // 
            // lblDitheringMode
            // 
            this.lblDitheringMode.AutoSize = true;
            this.lblDitheringMode.Location = new System.Drawing.Point(7, 105);
            this.lblDitheringMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDitheringMode.Name = "lblDitheringMode";
            this.lblDitheringMode.Size = new System.Drawing.Size(41, 15);
            this.lblDitheringMode.TabIndex = 7;
            this.lblDitheringMode.Text = "Mode:";
            // 
            // cbxDitheringMode
            // 
            this.cbxDitheringMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDitheringMode.FormattingEnabled = true;
            this.cbxDitheringMode.Location = new System.Drawing.Point(75, 99);
            this.cbxDitheringMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxDitheringMode.Name = "cbxDitheringMode";
            this.cbxDitheringMode.Size = new System.Drawing.Size(136, 23);
            this.cbxDitheringMode.TabIndex = 6;
            this.cbxDitheringMode.SelectedIndexChanged += new System.EventHandler(this.cbxDitheringMode_SelectedIndexChanged);
            // 
            // lblDitheringBitDepth
            // 
            this.lblDitheringBitDepth.AutoSize = true;
            this.lblDitheringBitDepth.Location = new System.Drawing.Point(7, 74);
            this.lblDitheringBitDepth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDitheringBitDepth.Name = "lblDitheringBitDepth";
            this.lblDitheringBitDepth.Size = new System.Drawing.Size(60, 15);
            this.lblDitheringBitDepth.TabIndex = 5;
            this.lblDitheringBitDepth.Text = "Bit-depth:";
            // 
            // cbxDitheringBitDepth
            // 
            this.cbxDitheringBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDitheringBitDepth.FormattingEnabled = true;
            this.cbxDitheringBitDepth.Location = new System.Drawing.Point(75, 68);
            this.cbxDitheringBitDepth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxDitheringBitDepth.Name = "cbxDitheringBitDepth";
            this.cbxDitheringBitDepth.Size = new System.Drawing.Size(136, 23);
            this.cbxDitheringBitDepth.TabIndex = 4;
            this.cbxDitheringBitDepth.SelectedIndexChanged += new System.EventHandler(this.cbxDitheringBitDepth_SelectedIndexChanged);
            // 
            // chkDitheringEnabled
            // 
            this.chkDitheringEnabled.AutoSize = true;
            this.chkDitheringEnabled.Checked = true;
            this.chkDitheringEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDitheringEnabled.Location = new System.Drawing.Point(7, 46);
            this.chkDitheringEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkDitheringEnabled.Name = "chkDitheringEnabled";
            this.chkDitheringEnabled.Size = new System.Drawing.Size(120, 19);
            this.chkDitheringEnabled.TabIndex = 3;
            this.chkDitheringEnabled.Text = "Dithering enabled";
            this.chkDitheringEnabled.ThreeState = true;
            this.chkDitheringEnabled.UseVisualStyleBackColor = true;
            this.chkDitheringEnabled.CheckStateChanged += new System.EventHandler(this.chkDitheringEnabled_CheckStateChanged);
            // 
            // pbGradient
            // 
            this.pbGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbGradient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbGradient.Location = new System.Drawing.Point(7, 128);
            this.pbGradient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbGradient.Name = "pbGradient";
            this.pbGradient.Size = new System.Drawing.Size(555, 228);
            this.pbGradient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGradient.TabIndex = 0;
            this.pbGradient.TabStop = false;
            // 
            // grpMiscellaneousOptions
            // 
            this.grpMiscellaneousOptions.Controls.Add(this.btnSetShortcutScreenSaver);
            this.grpMiscellaneousOptions.Controls.Add(this.label11);
            this.grpMiscellaneousOptions.Controls.Add(this.edtBlankScreenSaverShortcut);
            this.grpMiscellaneousOptions.Controls.Add(this.lblFixChromeFontsDescription);
            this.grpMiscellaneousOptions.Controls.Add(this.chkFixChromeFonts);
            this.grpMiscellaneousOptions.Location = new System.Drawing.Point(7, 402);
            this.grpMiscellaneousOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpMiscellaneousOptions.Name = "grpMiscellaneousOptions";
            this.grpMiscellaneousOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpMiscellaneousOptions.Size = new System.Drawing.Size(522, 131);
            this.grpMiscellaneousOptions.TabIndex = 5;
            this.grpMiscellaneousOptions.TabStop = false;
            this.grpMiscellaneousOptions.Text = "Miscellaneous";
            // 
            // btnSetShortcutScreenSaver
            // 
            this.btnSetShortcutScreenSaver.Location = new System.Drawing.Point(181, 98);
            this.btnSetShortcutScreenSaver.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSetShortcutScreenSaver.Name = "btnSetShortcutScreenSaver";
            this.btnSetShortcutScreenSaver.Size = new System.Drawing.Size(40, 27);
            this.btnSetShortcutScreenSaver.TabIndex = 10;
            this.btnSetShortcutScreenSaver.Text = "Set";
            this.btnSetShortcutScreenSaver.UseVisualStyleBackColor = true;
            this.btnSetShortcutScreenSaver.Click += new System.EventHandler(this.btnSetShortcutScreenSaver_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 80);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(238, 15);
            this.label11.TabIndex = 9;
            this.label11.Text = "Set a shortcut to start the blank screensaver:";
            // 
            // edtBlankScreenSaverShortcut
            // 
            this.edtBlankScreenSaverShortcut.Location = new System.Drawing.Point(7, 101);
            this.edtBlankScreenSaverShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtBlankScreenSaverShortcut.Name = "edtBlankScreenSaverShortcut";
            this.edtBlankScreenSaverShortcut.ReadOnly = true;
            this.edtBlankScreenSaverShortcut.Size = new System.Drawing.Size(165, 23);
            this.edtBlankScreenSaverShortcut.TabIndex = 7;
            this.edtBlankScreenSaverShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyDown);
            this.edtBlankScreenSaverShortcut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyUp);
            // 
            // lblFixChromeFontsDescription
            // 
            this.lblFixChromeFontsDescription.AutoSize = true;
            this.lblFixChromeFontsDescription.Location = new System.Drawing.Point(4, 45);
            this.lblFixChromeFontsDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFixChromeFontsDescription.Name = "lblFixChromeFontsDescription";
            this.lblFixChromeFontsDescription.Size = new System.Drawing.Size(427, 15);
            this.lblFixChromeFontsDescription.TabIndex = 6;
            this.lblFixChromeFontsDescription.Text = "This will add the parameter --disable-lcd-text to Chrome and requires elevation.";
            // 
            // chkFixChromeFonts
            // 
            this.chkFixChromeFonts.AutoSize = true;
            this.chkFixChromeFonts.Location = new System.Drawing.Point(7, 22);
            this.chkFixChromeFonts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkFixChromeFonts.Name = "chkFixChromeFonts";
            this.chkFixChromeFonts.Size = new System.Drawing.Size(378, 19);
            this.chkFixChromeFonts.TabIndex = 4;
            this.chkFixChromeFonts.Text = "ClearType: fix bad fonts in Chrome (turn on grayscale anti-aliasing)";
            this.chkFixChromeFonts.UseVisualStyleBackColor = true;
            this.chkFixChromeFonts.CheckedChanged += new System.EventHandler(this.chkFixChromeFonts_CheckedChanged);
            // 
            // grpLGOptions
            // 
            this.grpLGOptions.Controls.Add(this.chkLgSetSelectedDeviceByPowerOn);
            this.grpLGOptions.Controls.Add(this.lblGameBarShortcut);
            this.grpLGOptions.Controls.Add(this.edtLgGameBarShortcut);
            this.grpLGOptions.Controls.Add(this.lblLgOptionShutdownDelayHelp);
            this.grpLGOptions.Controls.Add(this.edtLgOptionShutdownDelay);
            this.grpLGOptions.Controls.Add(this.lblLgOptionShutdownDelay);
            this.grpLGOptions.Controls.Add(this.chkLgShowAdvancedActions);
            this.grpLGOptions.Controls.Add(this.btnLGTestPower);
            this.grpLGOptions.Controls.Add(this.lblLgMaxPowerOnRetriesDescription);
            this.grpLGOptions.Controls.Add(this.edtLgMaxPowerOnRetries);
            this.grpLGOptions.Controls.Add(this.lblLgMaxPowerOnRetries);
            this.grpLGOptions.Location = new System.Drawing.Point(8, 187);
            this.grpLGOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpLGOptions.Name = "grpLGOptions";
            this.grpLGOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpLGOptions.Size = new System.Drawing.Size(522, 209);
            this.grpLGOptions.TabIndex = 4;
            this.grpLGOptions.TabStop = false;
            this.grpLGOptions.Text = "LG controller";
            // 
            // chkLgSetSelectedDeviceByPowerOn
            // 
            this.chkLgSetSelectedDeviceByPowerOn.AutoSize = true;
            this.chkLgSetSelectedDeviceByPowerOn.Location = new System.Drawing.Point(8, 180);
            this.chkLgSetSelectedDeviceByPowerOn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkLgSetSelectedDeviceByPowerOn.Name = "chkLgSetSelectedDeviceByPowerOn";
            this.chkLgSetSelectedDeviceByPowerOn.Size = new System.Drawing.Size(302, 19);
            this.chkLgSetSelectedDeviceByPowerOn.TabIndex = 60;
            this.chkLgSetSelectedDeviceByPowerOn.Text = "Automatically set selected device to last powered on";
            this.chkLgSetSelectedDeviceByPowerOn.UseVisualStyleBackColor = true;
            this.chkLgSetSelectedDeviceByPowerOn.CheckedChanged += new System.EventHandler(this.chkLgSetSelectedDeviceByWol_CheckedChanged);
            // 
            // lblGameBarShortcut
            // 
            this.lblGameBarShortcut.AutoSize = true;
            this.lblGameBarShortcut.Location = new System.Drawing.Point(266, 127);
            this.lblGameBarShortcut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameBarShortcut.Name = "lblGameBarShortcut";
            this.lblGameBarShortcut.Size = new System.Drawing.Size(108, 15);
            this.lblGameBarShortcut.TabIndex = 59;
            this.lblGameBarShortcut.Text = "Game Bar shortcut:";
            // 
            // edtLgGameBarShortcut
            // 
            this.edtLgGameBarShortcut.Location = new System.Drawing.Point(386, 121);
            this.edtLgGameBarShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgGameBarShortcut.Name = "edtLgGameBarShortcut";
            this.edtLgGameBarShortcut.ReadOnly = true;
            this.edtLgGameBarShortcut.Size = new System.Drawing.Size(128, 23);
            this.edtLgGameBarShortcut.TabIndex = 58;
            this.edtLgGameBarShortcut.TextChanged += new System.EventHandler(this.edtLgGameBarShortcut_TextChanged);
            this.edtLgGameBarShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtLgGameBarShortcut_KeyDown);
            this.edtLgGameBarShortcut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtLgGameBarShortcut_KeyUp);
            // 
            // lblLgOptionShutdownDelayHelp
            // 
            this.lblLgOptionShutdownDelayHelp.AutoSize = true;
            this.lblLgOptionShutdownDelayHelp.Location = new System.Drawing.Point(7, 94);
            this.lblLgOptionShutdownDelayHelp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgOptionShutdownDelayHelp.Name = "lblLgOptionShutdownDelayHelp";
            this.lblLgOptionShutdownDelayHelp.Size = new System.Drawing.Size(385, 15);
            this.lblLgOptionShutdownDelayHelp.TabIndex = 11;
            this.lblLgOptionShutdownDelayHelp.Text = "This delay may prevent the tv from powering off when restarting the pc.";
            // 
            // edtLgOptionShutdownDelay
            // 
            this.edtLgOptionShutdownDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.edtLgOptionShutdownDelay.Location = new System.Drawing.Point(428, 68);
            this.edtLgOptionShutdownDelay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgOptionShutdownDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.edtLgOptionShutdownDelay.Name = "edtLgOptionShutdownDelay";
            this.edtLgOptionShutdownDelay.ReadOnly = true;
            this.edtLgOptionShutdownDelay.Size = new System.Drawing.Size(86, 23);
            this.edtLgOptionShutdownDelay.TabIndex = 10;
            this.edtLgOptionShutdownDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.edtLgOptionShutdownDelay.ValueChanged += new System.EventHandler(this.edtLgOptionShutdownDelay_ValueChanged);
            // 
            // lblLgOptionShutdownDelay
            // 
            this.lblLgOptionShutdownDelay.AutoSize = true;
            this.lblLgOptionShutdownDelay.Location = new System.Drawing.Point(7, 70);
            this.lblLgOptionShutdownDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgOptionShutdownDelay.Name = "lblLgOptionShutdownDelay";
            this.lblLgOptionShutdownDelay.Size = new System.Drawing.Size(249, 15);
            this.lblLgOptionShutdownDelay.TabIndex = 9;
            this.lblLgOptionShutdownDelay.Text = "Delay when shutting down/restarting pc (ms):";
            // 
            // chkLgShowAdvancedActions
            // 
            this.chkLgShowAdvancedActions.AutoSize = true;
            this.chkLgShowAdvancedActions.Location = new System.Drawing.Point(8, 155);
            this.chkLgShowAdvancedActions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkLgShowAdvancedActions.Name = "chkLgShowAdvancedActions";
            this.chkLgShowAdvancedActions.Size = new System.Drawing.Size(419, 19);
            this.chkLgShowAdvancedActions.TabIndex = 8;
            this.chkLgShowAdvancedActions.Text = "Show advanced actions under the Expert-button (InStart, Software Update)";
            this.chkLgShowAdvancedActions.UseVisualStyleBackColor = true;
            this.chkLgShowAdvancedActions.CheckedChanged += new System.EventHandler(this.chkLgShowAdvancedActions_CheckedChanged);
            // 
            // btnLGTestPower
            // 
            this.btnLGTestPower.Location = new System.Drawing.Point(8, 121);
            this.btnLGTestPower.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLGTestPower.Name = "btnLGTestPower";
            this.btnLGTestPower.Size = new System.Drawing.Size(167, 27);
            this.btnLGTestPower.TabIndex = 6;
            this.btnLGTestPower.Text = "Test power off/on";
            this.btnLGTestPower.UseVisualStyleBackColor = true;
            this.btnLGTestPower.Click += new System.EventHandler(this.btnLGTestPower_Click);
            // 
            // lblLgMaxPowerOnRetriesDescription
            // 
            this.lblLgMaxPowerOnRetriesDescription.AutoSize = true;
            this.lblLgMaxPowerOnRetriesDescription.Location = new System.Drawing.Point(7, 46);
            this.lblLgMaxPowerOnRetriesDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgMaxPowerOnRetriesDescription.Name = "lblLgMaxPowerOnRetriesDescription";
            this.lblLgMaxPowerOnRetriesDescription.Size = new System.Drawing.Size(415, 15);
            this.lblLgMaxPowerOnRetriesDescription.TabIndex = 5;
            this.lblLgMaxPowerOnRetriesDescription.Text = "Retries are necessary to wait for the network link of your pc to be established. " +
    "";
            // 
            // edtLgMaxPowerOnRetries
            // 
            this.edtLgMaxPowerOnRetries.Location = new System.Drawing.Point(428, 20);
            this.edtLgMaxPowerOnRetries.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLgMaxPowerOnRetries.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.edtLgMaxPowerOnRetries.Name = "edtLgMaxPowerOnRetries";
            this.edtLgMaxPowerOnRetries.ReadOnly = true;
            this.edtLgMaxPowerOnRetries.Size = new System.Drawing.Size(86, 23);
            this.edtLgMaxPowerOnRetries.TabIndex = 1;
            this.edtLgMaxPowerOnRetries.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.edtLgMaxPowerOnRetries.ValueChanged += new System.EventHandler(this.edtLgPowerOnAfterResumeDelay_ValueChanged);
            // 
            // lblLgMaxPowerOnRetries
            // 
            this.lblLgMaxPowerOnRetries.AutoSize = true;
            this.lblLgMaxPowerOnRetries.Location = new System.Drawing.Point(7, 22);
            this.lblLgMaxPowerOnRetries.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLgMaxPowerOnRetries.Name = "lblLgMaxPowerOnRetries";
            this.lblLgMaxPowerOnRetries.Size = new System.Drawing.Size(340, 15);
            this.lblLgMaxPowerOnRetries.TabIndex = 0;
            this.lblLgMaxPowerOnRetries.Text = "Maximum number of retries powering on after startup/resume:";
            // 
            // grpGeneralOptions
            // 
            this.grpGeneralOptions.Controls.Add(this.chkAutoInstallUpdates);
            this.grpGeneralOptions.Controls.Add(this.btnStartStopService);
            this.grpGeneralOptions.Controls.Add(this.btnElevationInfo);
            this.grpGeneralOptions.Controls.Add(this.rbElevationService);
            this.grpGeneralOptions.Controls.Add(this.rbElevationProcess);
            this.grpGeneralOptions.Controls.Add(this.rbElevationAdmin);
            this.grpGeneralOptions.Controls.Add(this.label8);
            this.grpGeneralOptions.Controls.Add(this.rbElevationNone);
            this.grpGeneralOptions.Controls.Add(this.chkGdiScaling);
            this.grpGeneralOptions.Controls.Add(this.chkCheckForUpdates);
            this.grpGeneralOptions.Controls.Add(this.chkMinimizeToSystemTray);
            this.grpGeneralOptions.Controls.Add(this.chkMinimizeOnClose);
            this.grpGeneralOptions.Controls.Add(this.chkStartMinimized);
            this.grpGeneralOptions.Controls.Add(this.chkStartAfterLogin);
            this.grpGeneralOptions.Location = new System.Drawing.Point(7, 7);
            this.grpGeneralOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGeneralOptions.Name = "grpGeneralOptions";
            this.grpGeneralOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGeneralOptions.Size = new System.Drawing.Size(522, 174);
            this.grpGeneralOptions.TabIndex = 2;
            this.grpGeneralOptions.TabStop = false;
            this.grpGeneralOptions.Text = "General";
            // 
            // chkAutoInstallUpdates
            // 
            this.chkAutoInstallUpdates.AutoSize = true;
            this.chkAutoInstallUpdates.Location = new System.Drawing.Point(7, 147);
            this.chkAutoInstallUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkAutoInstallUpdates.Name = "chkAutoInstallUpdates";
            this.chkAutoInstallUpdates.Size = new System.Drawing.Size(179, 19);
            this.chkAutoInstallUpdates.TabIndex = 66;
            this.chkAutoInstallUpdates.Text = "Automatically install updates";
            this.chkAutoInstallUpdates.UseVisualStyleBackColor = true;
            this.chkAutoInstallUpdates.CheckedChanged += new System.EventHandler(this.chkAutoInstallUpdates_CheckedChanged);
            // 
            // btnStartStopService
            // 
            this.btnStartStopService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStopService.Location = new System.Drawing.Point(474, 103);
            this.btnStartStopService.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStartStopService.Name = "btnStartStopService";
            this.btnStartStopService.Size = new System.Drawing.Size(40, 27);
            this.btnStartStopService.TabIndex = 65;
            this.btnStartStopService.Text = "Start";
            this.btnStartStopService.UseVisualStyleBackColor = true;
            this.btnStartStopService.Click += new System.EventHandler(this.btnStartStopService_Click);
            // 
            // btnElevationInfo
            // 
            this.btnElevationInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnElevationInfo.Location = new System.Drawing.Point(329, 41);
            this.btnElevationInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnElevationInfo.Name = "btnElevationInfo";
            this.btnElevationInfo.Size = new System.Drawing.Size(22, 27);
            this.btnElevationInfo.TabIndex = 64;
            this.btnElevationInfo.Text = "?";
            this.btnElevationInfo.UseVisualStyleBackColor = true;
            this.btnElevationInfo.Click += new System.EventHandler(this.btnElevationInfo_Click);
            // 
            // rbElevationService
            // 
            this.rbElevationService.AutoSize = true;
            this.rbElevationService.Location = new System.Drawing.Point(216, 107);
            this.rbElevationService.Name = "rbElevationService";
            this.rbElevationService.Size = new System.Drawing.Size(136, 19);
            this.rbElevationService.TabIndex = 63;
            this.rbElevationService.TabStop = true;
            this.rbElevationService.Tag = "2";
            this.rbElevationService.Text = "Use Windows Service";
            this.rbElevationService.UseVisualStyleBackColor = true;
            this.rbElevationService.CheckedChanged += new System.EventHandler(this.rbElevationNone_CheckedChanged);
            // 
            // rbElevationProcess
            // 
            this.rbElevationProcess.AutoSize = true;
            this.rbElevationProcess.Location = new System.Drawing.Point(216, 128);
            this.rbElevationProcess.Name = "rbElevationProcess";
            this.rbElevationProcess.Size = new System.Drawing.Size(189, 19);
            this.rbElevationProcess.TabIndex = 62;
            this.rbElevationProcess.TabStop = true;
            this.rbElevationProcess.Tag = "3";
            this.rbElevationProcess.Text = "Use dedicated elevated process";
            this.rbElevationProcess.UseVisualStyleBackColor = true;
            this.rbElevationProcess.CheckedChanged += new System.EventHandler(this.rbElevationNone_CheckedChanged);
            // 
            // rbElevationAdmin
            // 
            this.rbElevationAdmin.AutoSize = true;
            this.rbElevationAdmin.Location = new System.Drawing.Point(216, 86);
            this.rbElevationAdmin.Name = "rbElevationAdmin";
            this.rbElevationAdmin.Size = new System.Drawing.Size(97, 19);
            this.rbElevationAdmin.TabIndex = 61;
            this.rbElevationAdmin.TabStop = true;
            this.rbElevationAdmin.Tag = "1";
            this.rbElevationAdmin.Text = "Run as admin";
            this.rbElevationAdmin.UseVisualStyleBackColor = true;
            this.rbElevationAdmin.CheckedChanged += new System.EventHandler(this.rbElevationNone_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(216, 47);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 15);
            this.label8.TabIndex = 60;
            this.label8.Text = "Elevation-method:";
            // 
            // rbElevationNone
            // 
            this.rbElevationNone.AutoSize = true;
            this.rbElevationNone.Location = new System.Drawing.Point(216, 65);
            this.rbElevationNone.Name = "rbElevationNone";
            this.rbElevationNone.Size = new System.Drawing.Size(54, 19);
            this.rbElevationNone.TabIndex = 10;
            this.rbElevationNone.TabStop = true;
            this.rbElevationNone.Tag = "0";
            this.rbElevationNone.Text = "None";
            this.rbElevationNone.UseVisualStyleBackColor = true;
            this.rbElevationNone.CheckedChanged += new System.EventHandler(this.rbElevationNone_CheckedChanged);
            // 
            // chkGdiScaling
            // 
            this.chkGdiScaling.AutoSize = true;
            this.chkGdiScaling.Location = new System.Drawing.Point(216, 22);
            this.chkGdiScaling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkGdiScaling.Name = "chkGdiScaling";
            this.chkGdiScaling.Size = new System.Drawing.Size(197, 19);
            this.chkGdiScaling.TabIndex = 7;
            this.chkGdiScaling.Text = "Use GDI Scaling (requires restart)";
            this.chkGdiScaling.UseVisualStyleBackColor = true;
            this.chkGdiScaling.CheckedChanged += new System.EventHandler(this.chkGdiScaling_CheckedChanged);
            // 
            // chkCheckForUpdates
            // 
            this.chkCheckForUpdates.AutoSize = true;
            this.chkCheckForUpdates.Location = new System.Drawing.Point(7, 122);
            this.chkCheckForUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkCheckForUpdates.Name = "chkCheckForUpdates";
            this.chkCheckForUpdates.Size = new System.Drawing.Size(197, 19);
            this.chkCheckForUpdates.TabIndex = 6;
            this.chkCheckForUpdates.Text = "Automatically check for updates";
            this.chkCheckForUpdates.UseVisualStyleBackColor = true;
            this.chkCheckForUpdates.CheckedChanged += new System.EventHandler(this.chkCheckForUpdates_CheckedChanged);
            // 
            // chkMinimizeToSystemTray
            // 
            this.chkMinimizeToSystemTray.AutoSize = true;
            this.chkMinimizeToSystemTray.Location = new System.Drawing.Point(7, 97);
            this.chkMinimizeToSystemTray.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
            this.chkMinimizeToSystemTray.Size = new System.Drawing.Size(152, 19);
            this.chkMinimizeToSystemTray.TabIndex = 5;
            this.chkMinimizeToSystemTray.Text = "Minimize to system tray";
            this.chkMinimizeToSystemTray.UseVisualStyleBackColor = true;
            this.chkMinimizeToSystemTray.CheckedChanged += new System.EventHandler(this.chkMinimizeToSystemTray_CheckedChanged);
            // 
            // chkMinimizeOnClose
            // 
            this.chkMinimizeOnClose.AutoSize = true;
            this.chkMinimizeOnClose.Location = new System.Drawing.Point(7, 72);
            this.chkMinimizeOnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkMinimizeOnClose.Name = "chkMinimizeOnClose";
            this.chkMinimizeOnClose.Size = new System.Drawing.Size(122, 19);
            this.chkMinimizeOnClose.TabIndex = 4;
            this.chkMinimizeOnClose.Text = "Minimize on close";
            this.chkMinimizeOnClose.UseVisualStyleBackColor = true;
            this.chkMinimizeOnClose.CheckedChanged += new System.EventHandler(this.chkMinimizeOnClose_CheckedChanged);
            // 
            // chkStartMinimized
            // 
            this.chkStartMinimized.AutoSize = true;
            this.chkStartMinimized.Location = new System.Drawing.Point(7, 47);
            this.chkStartMinimized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkStartMinimized.Name = "chkStartMinimized";
            this.chkStartMinimized.Size = new System.Drawing.Size(109, 19);
            this.chkStartMinimized.TabIndex = 3;
            this.chkStartMinimized.Text = "Start minimized";
            this.chkStartMinimized.UseVisualStyleBackColor = true;
            // 
            // chkStartAfterLogin
            // 
            this.chkStartAfterLogin.AutoSize = true;
            this.chkStartAfterLogin.Location = new System.Drawing.Point(7, 22);
            this.chkStartAfterLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkStartAfterLogin.Name = "chkStartAfterLogin";
            this.chkStartAfterLogin.Size = new System.Drawing.Size(183, 19);
            this.chkStartAfterLogin.TabIndex = 2;
            this.chkStartAfterLogin.Text = "Automatically start after login";
            this.chkStartAfterLogin.UseVisualStyleBackColor = true;
            this.chkStartAfterLogin.CheckedChanged += new System.EventHandler(this.chkStartAfterLogin_CheckedChanged);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.lblShowLog);
            this.tabLog.Controls.Add(this.cbxLogType);
            this.tabLog.Controls.Add(this.btnClearLog);
            this.tabLog.Controls.Add(this.edtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 24);
            this.tabLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLog.Size = new System.Drawing.Size(1114, 539);
            this.tabLog.TabIndex = 3;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // lblShowLog
            // 
            this.lblShowLog.AutoSize = true;
            this.lblShowLog.Location = new System.Drawing.Point(8, 14);
            this.lblShowLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShowLog.Name = "lblShowLog";
            this.lblShowLog.Size = new System.Drawing.Size(88, 15);
            this.lblShowLog.TabIndex = 33;
            this.lblShowLog.Text = "Show log from:";
            // 
            // cbxLogType
            // 
            this.cbxLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLogType.FormattingEnabled = true;
            this.cbxLogType.Items.AddRange(new object[] {
            "Main application",
            "Service"});
            this.cbxLogType.Location = new System.Drawing.Point(102, 9);
            this.cbxLogType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxLogType.Name = "cbxLogType";
            this.cbxLogType.Size = new System.Drawing.Size(189, 23);
            this.cbxLogType.TabIndex = 32;
            this.cbxLogType.SelectedIndexChanged += new System.EventHandler(this.cbxLogType_SelectedIndexChanged);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(299, 6);
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(88, 27);
            this.btnClearLog.TabIndex = 2;
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // edtLog
            // 
            this.edtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtLog.Location = new System.Drawing.Point(7, 40);
            this.edtLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtLog.MaxLength = 327670;
            this.edtLog.Multiline = true;
            this.edtLog.Name = "edtLog";
            this.edtLog.ReadOnly = true;
            this.edtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.edtLog.Size = new System.Drawing.Size(1098, 478);
            this.edtLog.TabIndex = 0;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.grpNVIDIAInfo);
            this.tabInfo.Controls.Add(this.groupBox3);
            this.tabInfo.Location = new System.Drawing.Point(4, 24);
            this.tabInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabInfo.Size = new System.Drawing.Size(1114, 539);
            this.tabInfo.TabIndex = 4;
            this.tabInfo.Text = "Info";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // grpNVIDIAInfo
            // 
            this.grpNVIDIAInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNVIDIAInfo.Controls.Add(this.btnRefreshNVIDIAInfo);
            this.grpNVIDIAInfo.Controls.Add(this.tvNVIDIAInfo);
            this.grpNVIDIAInfo.Location = new System.Drawing.Point(463, 7);
            this.grpNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNVIDIAInfo.Name = "grpNVIDIAInfo";
            this.grpNVIDIAInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNVIDIAInfo.Size = new System.Drawing.Size(643, 511);
            this.grpNVIDIAInfo.TabIndex = 4;
            this.grpNVIDIAInfo.TabStop = false;
            this.grpNVIDIAInfo.Text = "NVIDIA info";
            // 
            // btnRefreshNVIDIAInfo
            // 
            this.btnRefreshNVIDIAInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshNVIDIAInfo.Location = new System.Drawing.Point(7, 478);
            this.btnRefreshNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRefreshNVIDIAInfo.Name = "btnRefreshNVIDIAInfo";
            this.btnRefreshNVIDIAInfo.Size = new System.Drawing.Size(88, 27);
            this.btnRefreshNVIDIAInfo.TabIndex = 1;
            this.btnRefreshNVIDIAInfo.Text = "Refresh";
            this.btnRefreshNVIDIAInfo.UseVisualStyleBackColor = true;
            this.btnRefreshNVIDIAInfo.Click += new System.EventHandler(this.btnRefreshNVIDIAInfo_Click);
            // 
            // tvNVIDIAInfo
            // 
            this.tvNVIDIAInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvNVIDIAInfo.Location = new System.Drawing.Point(7, 22);
            this.tvNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tvNVIDIAInfo.Name = "tvNVIDIAInfo";
            this.tvNVIDIAInfo.Size = new System.Drawing.Size(628, 448);
            this.tvNVIDIAInfo.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbPlugins);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.lblInfo);
            this.groupBox3.Location = new System.Drawing.Point(7, 7);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(449, 254);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Info";
            // 
            // lbPlugins
            // 
            this.lbPlugins.FormattingEnabled = true;
            this.lbPlugins.HorizontalScrollbar = true;
            this.lbPlugins.ItemHeight = 15;
            this.lbPlugins.Location = new System.Drawing.Point(10, 77);
            this.lbPlugins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbPlugins.Name = "lbPlugins";
            this.lbPlugins.Size = new System.Drawing.Size(432, 169);
            this.lbPlugins.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 59);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(267, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "This app contains the following 3rd party plugins:";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(7, 23);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(28, 15);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Info";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.ForeColor = System.Drawing.Color.Red;
            this.btnUpdate.Location = new System.Drawing.Point(1055, 0);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(77, 26);
            this.btnUpdate.TabIndex = 61;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Visible = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1150, 594);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.tcMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1140, 611);
            this.Name = "MainForm";
            this.Text = "ColorControl";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.Click += new System.EventHandler(this.MainForm_Click);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tcMain.ResumeLayout(false);
            this.tabAMD.ResumeLayout(false);
            this.tabAMD.PerformLayout();
            this.mnuAmdPresets.ResumeLayout(false);
            this.tabLG.ResumeLayout(false);
            this.scLgController.Panel1.ResumeLayout(false);
            this.scLgController.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scLgController)).EndInit();
            this.scLgController.ResumeLayout(false);
            this.mnuLgExpert.ResumeLayout(false);
            this.mnuLgButtons.ResumeLayout(false);
            this.tabGameLauncher.ResumeLayout(false);
            this.tabGameLauncher.PerformLayout();
            this.mnuGameOptions.ResumeLayout(false);
            this.mnuGameActions.ResumeLayout(false);
            this.mnuGameAddStep.ResumeLayout(false);
            this.tabOptions.ResumeLayout(false);
            this.grpNvidiaOptions.ResumeLayout(false);
            this.grpNvidiaOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGradient)).EndInit();
            this.grpMiscellaneousOptions.ResumeLayout(false);
            this.grpMiscellaneousOptions.PerformLayout();
            this.grpLGOptions.ResumeLayout(false);
            this.grpLGOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtLgOptionShutdownDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtLgMaxPowerOnRetries)).EndInit();
            this.grpGeneralOptions.ResumeLayout(false);
            this.grpGeneralOptions.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabInfo.ResumeLayout(false);
            this.grpNVIDIAInfo.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabLG;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.GroupBox grpGeneralOptions;
        private System.Windows.Forms.CheckBox chkStartMinimized;
        private System.Windows.Forms.CheckBox chkStartAfterLogin;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TextBox edtLog;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbPlugins;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.GroupBox grpNVIDIAInfo;
        private System.Windows.Forms.TreeView tvNVIDIAInfo;
        private System.Windows.Forms.Button btnRefreshNVIDIAInfo;
        private System.Windows.Forms.ContextMenuStrip mnuLgButtons;
        private System.Windows.Forms.GroupBox grpLGOptions;
        private System.Windows.Forms.NumericUpDown edtLgMaxPowerOnRetries;
        private System.Windows.Forms.Label lblLgMaxPowerOnRetries;
        private System.Windows.Forms.Button btnLGTestPower;
        private System.Windows.Forms.Label lblLgMaxPowerOnRetriesDescription;
        private System.Windows.Forms.GroupBox grpMiscellaneousOptions;
        private System.Windows.Forms.CheckBox chkFixChromeFonts;
        private System.Windows.Forms.Label lblFixChromeFontsDescription;
        private System.Windows.Forms.Button btnSetShortcutScreenSaver;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox edtBlankScreenSaverShortcut;
        private System.Windows.Forms.CheckBox chkMinimizeOnClose;
        private System.Windows.Forms.TabPage tabNVIDIA;
        private System.Windows.Forms.TabPage tabAMD;
        private System.Windows.Forms.Label lblErrorAMD;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.GroupBox grpNvidiaOptions;
        private System.Windows.Forms.PictureBox pbGradient;
        private System.Windows.Forms.Label lblDitheringMode;
        private System.Windows.Forms.ComboBox cbxDitheringMode;
        private System.Windows.Forms.Label lblDitheringBitDepth;
        private System.Windows.Forms.ComboBox cbxDitheringBitDepth;
        private System.Windows.Forms.CheckBox chkDitheringEnabled;
        private System.Windows.Forms.ToolStripMenuItem mnuLgRcButtons;
        private System.Windows.Forms.ToolStripMenuItem mnuLgActions;
        private System.Windows.Forms.Button btnDeleteAmd;
        private System.Windows.Forms.Button btnCloneAmd;
        private System.Windows.Forms.Button btnAmdPresetSave;
        private System.Windows.Forms.Label lblAmdShortcut;
        private System.Windows.Forms.TextBox edtAmdShortcut;
        private System.Windows.Forms.Button btnChangeAmd;
        private System.Windows.Forms.ContextMenuStrip mnuAmdPresets;
        private System.Windows.Forms.ToolStripMenuItem miAmdApply;
        private System.Windows.Forms.ToolStripMenuItem miAmdPresetApplyOnStartup;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuAmdDisplay;
        private System.Windows.Forms.ToolStripMenuItem miAmdPrimaryDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuAmdColorSettings;
        private System.Windows.Forms.ToolStripMenuItem miAmdColorSettingsIncluded;
        private System.Windows.Forms.ToolStripMenuItem mnuAmdRefreshRate;
        private System.Windows.Forms.ToolStripMenuItem miAmdRefreshRateIncluded;
        private System.Windows.Forms.ToolStripMenuItem mnuAmdDithering;
        private System.Windows.Forms.ToolStripMenuItem miAmdDitheringIncluded;
        private System.Windows.Forms.ToolStripMenuItem mnuAmdHDR;
        private System.Windows.Forms.ToolStripMenuItem miAmdHDRIncluded;
        private System.Windows.Forms.ToolStripMenuItem miAmdHDRToggle;
        private System.Windows.Forms.ToolStripMenuItem miAmdHDREnabled;
        private System.Windows.Forms.Button btnApplyAmd;
        private System.Windows.Forms.ListView lvAmdPresets;
        private System.Windows.Forms.Button btnAddAmd;
        private System.Windows.Forms.CheckBox chkMinimizeToSystemTray;
        private System.Windows.Forms.Label lblAmdPresetName;
        private System.Windows.Forms.TextBox edtAmdPresetName;
        private System.Windows.Forms.ToolStripMenuItem miAmdCopyId;
        private System.Windows.Forms.SplitContainer scLgController;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxLgDevices;
        private System.Windows.Forms.Button btnLgDeviceConvertToCustom;
        private System.Windows.Forms.ComboBox cbxLgApps;
        private System.Windows.Forms.ListView lvLgPresets;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxLgPresetDevice;
        private System.Windows.Forms.Button btnDeleteLg;
        private System.Windows.Forms.Button btnApplyLg;
        private System.Windows.Forms.Label lblDeviceFilter;
        private System.Windows.Forms.Label lblLgPresetDevice;
        private System.Windows.Forms.Button btnAddLg;
        private System.Windows.Forms.TextBox edtShortcutLg;
        private System.Windows.Forms.TextBox edtLgDeviceFilter;
        private System.Windows.Forms.Button btnLgRemoveDevice;
        private System.Windows.Forms.Button btnLgRefreshApps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLgAddDevice;
        private System.Windows.Forms.TextBox edtNameLg;
        private System.Windows.Forms.Button btnSetShortcutLg;
        private System.Windows.Forms.TextBox edtStepsLg;
        private System.Windows.Forms.Button btnLgAddButton;
        private System.Windows.Forms.Button btnCloneLg;
        private System.Windows.Forms.Label lblStepsLg;
        private System.Windows.Forms.Button btnLgDeviceFilterRefresh;
        private System.Windows.Forms.CheckedListBox clbLgPower;
        private System.Windows.Forms.Label lblLgError;
        private System.Windows.Forms.CheckBox chkLgRemoteControlShow;
        private System.Windows.Forms.Button btnLgExpert;
        private System.Windows.Forms.ContextMenuStrip mnuLgExpert;
        private System.Windows.Forms.ToolStripMenuItem mnuLgOLEDMotionPro;
        private System.Windows.Forms.ToolStripMenuItem miLgEnableMotionPro;
        private System.Windows.Forms.ToolStripMenuItem miLgDisableMotionPro;
        private System.Windows.Forms.ToolStripSeparator miLgExpertSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuLgNvPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuLgAmdPresets;
        private System.Windows.Forms.ComboBox cbxLgPresetTrigger;
        private System.Windows.Forms.Label lblLgPresetTrigger;
        private System.Windows.Forms.Label lblLgPresetTriggerCondition;
        private System.Windows.Forms.Label lblLgPresetExcludedProcesses;
        private System.Windows.Forms.TextBox edtLgPresetExcludedProcesses;
        private System.Windows.Forms.Label lblLgPresetIncludedProcesses;
        private System.Windows.Forms.TextBox edtLgPresetIncludedProcesses;
        private System.Windows.Forms.Button btnLgPresetEditTriggerConditions;
        private System.Windows.Forms.TextBox edtLgPresetTriggerConditions;
        private System.Windows.Forms.CheckBox chkCheckForUpdates;
        private System.Windows.Forms.CheckBox chkLgShowAdvancedActions;
        private System.Windows.Forms.Button btnLgGameBar;
        private System.Windows.Forms.CheckBox chkGdiScaling;
        private System.Windows.Forms.Button btnLgDeviceOptionsHelp;
        private System.Windows.Forms.Label lblLgOptionShutdownDelayHelp;
        private System.Windows.Forms.NumericUpDown edtLgOptionShutdownDelay;
        private System.Windows.Forms.Label lblLgOptionShutdownDelay;
        private System.Windows.Forms.TextBox edtLgPresetDescription;
        private System.Windows.Forms.Label lblLgPresetDescription;
        private System.Windows.Forms.Label lblGameBarShortcut;
        private System.Windows.Forms.TextBox edtLgGameBarShortcut;
        private System.Windows.Forms.ComboBox cbxLgPcHdmiPort;
        private System.Windows.Forms.Label lblLgPcHdmiPort;
        private System.Windows.Forms.ToolStripMenuItem mnuLgProgram;
        private System.Windows.Forms.TabPage tabGameLauncher;
        private System.Windows.Forms.Label lblGameParameters;
        private System.Windows.Forms.TextBox edtGameParameters;
        private System.Windows.Forms.Button btnGameBrowse;
        private System.Windows.Forms.Label lblGameFilePath;
        private System.Windows.Forms.TextBox edtGamePath;
        private System.Windows.Forms.TextBox edtGamePrelaunchSteps;
        private System.Windows.Forms.Button btnGameAddStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox edtGameName;
        private System.Windows.Forms.Button btnGameDelete;
        private System.Windows.Forms.Button btnGameClone;
        private System.Windows.Forms.Button btnGameSave;
        private System.Windows.Forms.Button btnGameAdd;
        private System.Windows.Forms.Button btnGameLaunch;
        private System.Windows.Forms.ListView lvGamePresets;
        private System.Windows.Forms.ContextMenuStrip mnuGameAddStep;
        private System.Windows.Forms.ToolStripMenuItem mnuGameNvidiaPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuGameAmdPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuGameLgPresets;
        private System.Windows.Forms.CheckBox chkGameRunAsAdmin;
        private System.Windows.Forms.ToolStripMenuItem mnuGameStartProgram;
        private System.Windows.Forms.Button btnGameSettings;
        private System.Windows.Forms.ContextMenuStrip mnuGameActions;
        private System.Windows.Forms.ToolStripMenuItem mnuGameNvInspector;
        private System.Windows.Forms.CheckBox chkGameQuickAccess;
        private System.Windows.Forms.ToolStripMenuItem miGameSetQuickAccessShortcut;
        private System.Windows.Forms.CheckBox chkLgQuickAccess;
        private System.Windows.Forms.Button btnLgSettings;
        private System.Windows.Forms.CheckBox chkAmdQuickAccess;
        private System.Windows.Forms.Button btnAmdSettings;
        private System.Windows.Forms.Button btnGameOptions;
        private System.Windows.Forms.ContextMenuStrip mnuGameOptions;
        private System.Windows.Forms.ToolStripMenuItem miGameProcessorAffinity;
        private System.Windows.Forms.ToolStripMenuItem miGameProcessPriority;
        private System.Windows.Forms.RadioButton rbElevationService;
        private System.Windows.Forms.RadioButton rbElevationProcess;
        private System.Windows.Forms.RadioButton rbElevationAdmin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rbElevationNone;
        private System.Windows.Forms.Button btnElevationInfo;
        private System.Windows.Forms.Button btnStartStopService;
        private System.Windows.Forms.Label lblShowLog;
        private System.Windows.Forms.ComboBox cbxLogType;
        private System.Windows.Forms.ComboBox cbxGameStepType;
        private System.Windows.Forms.CheckBox chkAutoInstallUpdates;
        private System.Windows.Forms.CheckBox chkLgSetSelectedDeviceByPowerOn;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblDitheringDisplay;
        private System.Windows.Forms.ComboBox cbxDitheringDisplay;
        private System.Windows.Forms.ToolTip lvNvPresetsToolTip;
    }
}


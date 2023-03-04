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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabNVIDIA = new System.Windows.Forms.TabPage();
            this.tabAMD = new System.Windows.Forms.TabPage();
            this.tabLG = new System.Windows.Forms.TabPage();
            this.tabGameLauncher = new System.Windows.Forms.TabPage();
            this.tabOptions = new System.Windows.Forms.TabPage();
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
            this.tcMain.SuspendLayout();
            this.tabOptions.SuspendLayout();
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
            this.tabAMD.Location = new System.Drawing.Point(4, 24);
            this.tabAMD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabAMD.Name = "tabAMD";
            this.tabAMD.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabAMD.Size = new System.Drawing.Size(1114, 539);
            this.tabAMD.TabIndex = 5;
            this.tabAMD.Text = "AMD controller";
            this.tabAMD.UseVisualStyleBackColor = true;
            // 
            // tabLG
            // 
            this.tabLG.Location = new System.Drawing.Point(4, 24);
            this.tabLG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLG.Name = "tabLG";
            this.tabLG.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLG.Size = new System.Drawing.Size(1114, 539);
            this.tabLG.TabIndex = 1;
            this.tabLG.Text = "LG controller";
            this.tabLG.UseVisualStyleBackColor = true;
            // 
            // tabGameLauncher
            // 
            this.tabGameLauncher.Location = new System.Drawing.Point(4, 24);
            this.tabGameLauncher.Name = "tabGameLauncher";
            this.tabGameLauncher.Padding = new System.Windows.Forms.Padding(3);
            this.tabGameLauncher.Size = new System.Drawing.Size(1114, 539);
            this.tabGameLauncher.TabIndex = 6;
            this.tabGameLauncher.Text = "Game Launcher";
            this.tabGameLauncher.UseVisualStyleBackColor = true;
            // 
            // tabOptions
            // 
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
            this.tabOptions.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkMinimizeToSystemTray;
        private System.Windows.Forms.CheckBox chkCheckForUpdates;
        private System.Windows.Forms.CheckBox chkLgShowAdvancedActions;
        private System.Windows.Forms.CheckBox chkGdiScaling;
        private System.Windows.Forms.Label lblLgOptionShutdownDelayHelp;
        private System.Windows.Forms.NumericUpDown edtLgOptionShutdownDelay;
        private System.Windows.Forms.Label lblLgOptionShutdownDelay;
        private System.Windows.Forms.Label lblGameBarShortcut;
        private System.Windows.Forms.TextBox edtLgGameBarShortcut;
        private System.Windows.Forms.TabPage tabGameLauncher;
        private System.Windows.Forms.RadioButton rbElevationService;
        private System.Windows.Forms.RadioButton rbElevationProcess;
        private System.Windows.Forms.RadioButton rbElevationAdmin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rbElevationNone;
        private System.Windows.Forms.Button btnElevationInfo;
        private System.Windows.Forms.Button btnStartStopService;
        private System.Windows.Forms.Label lblShowLog;
        private System.Windows.Forms.ComboBox cbxLogType;
        private System.Windows.Forms.CheckBox chkAutoInstallUpdates;
        private System.Windows.Forms.CheckBox chkLgSetSelectedDeviceByPowerOn;
        private System.Windows.Forms.Button btnUpdate;
    }
}


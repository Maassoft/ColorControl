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
            tcMain = new System.Windows.Forms.TabControl();
            tabOptions = new System.Windows.Forms.TabPage();
            grpOptionsModules = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            chkModules = new System.Windows.Forms.CheckedListBox();
            grpMiscellaneousOptions = new System.Windows.Forms.GroupBox();
            btnSetShortcutScreenSaver = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            edtBlankScreenSaverShortcut = new System.Windows.Forms.TextBox();
            lblFixChromeFontsDescription = new System.Windows.Forms.Label();
            chkFixChromeFonts = new System.Windows.Forms.CheckBox();
            grpGeneralOptions = new System.Windows.Forms.GroupBox();
            chkOptionsUseDarkMode = new System.Windows.Forms.CheckBox();
            chkAutoInstallUpdates = new System.Windows.Forms.CheckBox();
            btnStartStopService = new System.Windows.Forms.Button();
            btnElevationInfo = new System.Windows.Forms.Button();
            rbElevationService = new System.Windows.Forms.RadioButton();
            rbElevationProcess = new System.Windows.Forms.RadioButton();
            rbElevationAdmin = new System.Windows.Forms.RadioButton();
            label8 = new System.Windows.Forms.Label();
            rbElevationNone = new System.Windows.Forms.RadioButton();
            chkGdiScaling = new System.Windows.Forms.CheckBox();
            chkCheckForUpdates = new System.Windows.Forms.CheckBox();
            chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
            chkMinimizeOnClose = new System.Windows.Forms.CheckBox();
            chkStartMinimized = new System.Windows.Forms.CheckBox();
            chkStartAfterLogin = new System.Windows.Forms.CheckBox();
            tabLog = new System.Windows.Forms.TabPage();
            lblShowLog = new System.Windows.Forms.Label();
            cbxLogType = new System.Windows.Forms.ComboBox();
            btnClearLog = new System.Windows.Forms.Button();
            edtLog = new System.Windows.Forms.TextBox();
            tabInfo = new System.Windows.Forms.TabPage();
            grpNVIDIAInfo = new System.Windows.Forms.GroupBox();
            btnRefreshNVIDIAInfo = new System.Windows.Forms.Button();
            tvNVIDIAInfo = new System.Windows.Forms.TreeView();
            groupBox3 = new System.Windows.Forms.GroupBox();
            lbPlugins = new System.Windows.Forms.ListBox();
            label7 = new System.Windows.Forms.Label();
            lblInfo = new System.Windows.Forms.Label();
            btnUpdate = new System.Windows.Forms.Button();
            btnOptionsAdvanced = new System.Windows.Forms.Button();
            tcMain.SuspendLayout();
            tabOptions.SuspendLayout();
            grpOptionsModules.SuspendLayout();
            grpMiscellaneousOptions.SuspendLayout();
            grpGeneralOptions.SuspendLayout();
            tabLog.SuspendLayout();
            tabInfo.SuspendLayout();
            grpNVIDIAInfo.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tcMain
            // 
            tcMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tcMain.Controls.Add(tabOptions);
            tcMain.Controls.Add(tabLog);
            tcMain.Controls.Add(tabInfo);
            tcMain.Location = new System.Drawing.Point(14, 14);
            tcMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new System.Drawing.Size(1122, 567);
            tcMain.TabIndex = 1;
            tcMain.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabOptions
            // 
            tabOptions.Controls.Add(grpOptionsModules);
            tabOptions.Controls.Add(grpMiscellaneousOptions);
            tabOptions.Controls.Add(grpGeneralOptions);
            tabOptions.Location = new System.Drawing.Point(4, 24);
            tabOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabOptions.Size = new System.Drawing.Size(1114, 539);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Options";
            tabOptions.UseVisualStyleBackColor = true;
            // 
            // grpOptionsModules
            // 
            grpOptionsModules.Controls.Add(label1);
            grpOptionsModules.Controls.Add(chkModules);
            grpOptionsModules.Location = new System.Drawing.Point(11, 223);
            grpOptionsModules.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpOptionsModules.Name = "grpOptionsModules";
            grpOptionsModules.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpOptionsModules.Size = new System.Drawing.Size(522, 131);
            grpOptionsModules.TabIndex = 11;
            grpOptionsModules.TabStop = false;
            grpOptionsModules.Text = "Modules";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 19);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(511, 15);
            label1.TabIndex = 7;
            label1.Text = "Here you can disable modules you do not want. A restart is required for a change to have effect.";
            // 
            // chkModules
            // 
            chkModules.ColumnWidth = 200;
            chkModules.FormattingEnabled = true;
            chkModules.Location = new System.Drawing.Point(7, 40);
            chkModules.MultiColumn = true;
            chkModules.Name = "chkModules";
            chkModules.Size = new System.Drawing.Size(507, 76);
            chkModules.TabIndex = 0;
            chkModules.SelectedIndexChanged += chkModules_SelectedIndexChanged;
            // 
            // grpMiscellaneousOptions
            // 
            grpMiscellaneousOptions.Controls.Add(btnSetShortcutScreenSaver);
            grpMiscellaneousOptions.Controls.Add(label11);
            grpMiscellaneousOptions.Controls.Add(edtBlankScreenSaverShortcut);
            grpMiscellaneousOptions.Controls.Add(lblFixChromeFontsDescription);
            grpMiscellaneousOptions.Controls.Add(chkFixChromeFonts);
            grpMiscellaneousOptions.Location = new System.Drawing.Point(11, 360);
            grpMiscellaneousOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMiscellaneousOptions.Name = "grpMiscellaneousOptions";
            grpMiscellaneousOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMiscellaneousOptions.Size = new System.Drawing.Size(522, 131);
            grpMiscellaneousOptions.TabIndex = 5;
            grpMiscellaneousOptions.TabStop = false;
            grpMiscellaneousOptions.Text = "Miscellaneous";
            // 
            // btnSetShortcutScreenSaver
            // 
            btnSetShortcutScreenSaver.Location = new System.Drawing.Point(181, 98);
            btnSetShortcutScreenSaver.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetShortcutScreenSaver.Name = "btnSetShortcutScreenSaver";
            btnSetShortcutScreenSaver.Size = new System.Drawing.Size(40, 27);
            btnSetShortcutScreenSaver.TabIndex = 10;
            btnSetShortcutScreenSaver.Text = "Set";
            btnSetShortcutScreenSaver.UseVisualStyleBackColor = true;
            btnSetShortcutScreenSaver.Click += btnSetShortcutScreenSaver_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(7, 80);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(238, 15);
            label11.TabIndex = 9;
            label11.Text = "Set a shortcut to start the blank screensaver:";
            // 
            // edtBlankScreenSaverShortcut
            // 
            edtBlankScreenSaverShortcut.Location = new System.Drawing.Point(7, 101);
            edtBlankScreenSaverShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtBlankScreenSaverShortcut.Name = "edtBlankScreenSaverShortcut";
            edtBlankScreenSaverShortcut.ReadOnly = true;
            edtBlankScreenSaverShortcut.Size = new System.Drawing.Size(165, 23);
            edtBlankScreenSaverShortcut.TabIndex = 7;
            edtBlankScreenSaverShortcut.KeyDown += edtShortcut_KeyDown;
            edtBlankScreenSaverShortcut.KeyUp += edtShortcut_KeyUp;
            // 
            // lblFixChromeFontsDescription
            // 
            lblFixChromeFontsDescription.AutoSize = true;
            lblFixChromeFontsDescription.Location = new System.Drawing.Point(4, 45);
            lblFixChromeFontsDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFixChromeFontsDescription.Name = "lblFixChromeFontsDescription";
            lblFixChromeFontsDescription.Size = new System.Drawing.Size(427, 15);
            lblFixChromeFontsDescription.TabIndex = 6;
            lblFixChromeFontsDescription.Text = "This will add the parameter --disable-lcd-text to Chrome and requires elevation.";
            // 
            // chkFixChromeFonts
            // 
            chkFixChromeFonts.AutoSize = true;
            chkFixChromeFonts.Location = new System.Drawing.Point(7, 22);
            chkFixChromeFonts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkFixChromeFonts.Name = "chkFixChromeFonts";
            chkFixChromeFonts.Size = new System.Drawing.Size(378, 19);
            chkFixChromeFonts.TabIndex = 4;
            chkFixChromeFonts.Text = "ClearType: fix bad fonts in Chrome (turn on grayscale anti-aliasing)";
            chkFixChromeFonts.UseVisualStyleBackColor = true;
            chkFixChromeFonts.CheckedChanged += chkFixChromeFonts_CheckedChanged;
            // 
            // grpGeneralOptions
            // 
            grpGeneralOptions.Controls.Add(btnOptionsAdvanced);
            grpGeneralOptions.Controls.Add(chkOptionsUseDarkMode);
            grpGeneralOptions.Controls.Add(chkAutoInstallUpdates);
            grpGeneralOptions.Controls.Add(btnStartStopService);
            grpGeneralOptions.Controls.Add(btnElevationInfo);
            grpGeneralOptions.Controls.Add(rbElevationService);
            grpGeneralOptions.Controls.Add(rbElevationProcess);
            grpGeneralOptions.Controls.Add(rbElevationAdmin);
            grpGeneralOptions.Controls.Add(label8);
            grpGeneralOptions.Controls.Add(rbElevationNone);
            grpGeneralOptions.Controls.Add(chkGdiScaling);
            grpGeneralOptions.Controls.Add(chkCheckForUpdates);
            grpGeneralOptions.Controls.Add(chkMinimizeToSystemTray);
            grpGeneralOptions.Controls.Add(chkMinimizeOnClose);
            grpGeneralOptions.Controls.Add(chkStartMinimized);
            grpGeneralOptions.Controls.Add(chkStartAfterLogin);
            grpGeneralOptions.Location = new System.Drawing.Point(7, 7);
            grpGeneralOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGeneralOptions.Name = "grpGeneralOptions";
            grpGeneralOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGeneralOptions.Size = new System.Drawing.Size(522, 210);
            grpGeneralOptions.TabIndex = 2;
            grpGeneralOptions.TabStop = false;
            grpGeneralOptions.Text = "General";
            // 
            // chkOptionsUseDarkMode
            // 
            chkOptionsUseDarkMode.AutoSize = true;
            chkOptionsUseDarkMode.Location = new System.Drawing.Point(216, 47);
            chkOptionsUseDarkMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkOptionsUseDarkMode.Name = "chkOptionsUseDarkMode";
            chkOptionsUseDarkMode.Size = new System.Drawing.Size(164, 19);
            chkOptionsUseDarkMode.TabIndex = 67;
            chkOptionsUseDarkMode.Text = "Dark Mode (experimental)";
            chkOptionsUseDarkMode.UseVisualStyleBackColor = true;
            chkOptionsUseDarkMode.CheckedChanged += chkOptionsUseDarkMode_CheckedChanged;
            // 
            // chkAutoInstallUpdates
            // 
            chkAutoInstallUpdates.AutoSize = true;
            chkAutoInstallUpdates.Location = new System.Drawing.Point(7, 147);
            chkAutoInstallUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAutoInstallUpdates.Name = "chkAutoInstallUpdates";
            chkAutoInstallUpdates.Size = new System.Drawing.Size(179, 19);
            chkAutoInstallUpdates.TabIndex = 66;
            chkAutoInstallUpdates.Text = "Automatically install updates";
            chkAutoInstallUpdates.UseVisualStyleBackColor = true;
            chkAutoInstallUpdates.CheckedChanged += chkAutoInstallUpdates_CheckedChanged;
            // 
            // btnStartStopService
            // 
            btnStartStopService.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStartStopService.Location = new System.Drawing.Point(474, 129);
            btnStartStopService.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartStopService.Name = "btnStartStopService";
            btnStartStopService.Size = new System.Drawing.Size(40, 27);
            btnStartStopService.TabIndex = 65;
            btnStartStopService.Text = "Start";
            btnStartStopService.UseVisualStyleBackColor = true;
            btnStartStopService.Click += btnStartStopService_Click;
            // 
            // btnElevationInfo
            // 
            btnElevationInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnElevationInfo.Location = new System.Drawing.Point(329, 67);
            btnElevationInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnElevationInfo.Name = "btnElevationInfo";
            btnElevationInfo.Size = new System.Drawing.Size(22, 27);
            btnElevationInfo.TabIndex = 64;
            btnElevationInfo.Text = "?";
            btnElevationInfo.UseVisualStyleBackColor = true;
            btnElevationInfo.Click += btnElevationInfo_Click;
            // 
            // rbElevationService
            // 
            rbElevationService.AutoSize = true;
            rbElevationService.Location = new System.Drawing.Point(216, 133);
            rbElevationService.Name = "rbElevationService";
            rbElevationService.Size = new System.Drawing.Size(136, 19);
            rbElevationService.TabIndex = 63;
            rbElevationService.TabStop = true;
            rbElevationService.Tag = "2";
            rbElevationService.Text = "Use Windows Service";
            rbElevationService.UseVisualStyleBackColor = true;
            rbElevationService.CheckedChanged += rbElevationNone_CheckedChanged;
            // 
            // rbElevationProcess
            // 
            rbElevationProcess.AutoSize = true;
            rbElevationProcess.Location = new System.Drawing.Point(216, 154);
            rbElevationProcess.Name = "rbElevationProcess";
            rbElevationProcess.Size = new System.Drawing.Size(189, 19);
            rbElevationProcess.TabIndex = 62;
            rbElevationProcess.TabStop = true;
            rbElevationProcess.Tag = "3";
            rbElevationProcess.Text = "Use dedicated elevated process";
            rbElevationProcess.UseVisualStyleBackColor = true;
            rbElevationProcess.CheckedChanged += rbElevationNone_CheckedChanged;
            // 
            // rbElevationAdmin
            // 
            rbElevationAdmin.AutoSize = true;
            rbElevationAdmin.Location = new System.Drawing.Point(216, 112);
            rbElevationAdmin.Name = "rbElevationAdmin";
            rbElevationAdmin.Size = new System.Drawing.Size(97, 19);
            rbElevationAdmin.TabIndex = 61;
            rbElevationAdmin.TabStop = true;
            rbElevationAdmin.Tag = "1";
            rbElevationAdmin.Text = "Run as admin";
            rbElevationAdmin.UseVisualStyleBackColor = true;
            rbElevationAdmin.CheckedChanged += rbElevationNone_CheckedChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(216, 73);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(105, 15);
            label8.TabIndex = 60;
            label8.Text = "Elevation-method:";
            // 
            // rbElevationNone
            // 
            rbElevationNone.AutoSize = true;
            rbElevationNone.Location = new System.Drawing.Point(216, 91);
            rbElevationNone.Name = "rbElevationNone";
            rbElevationNone.Size = new System.Drawing.Size(54, 19);
            rbElevationNone.TabIndex = 10;
            rbElevationNone.TabStop = true;
            rbElevationNone.Tag = "0";
            rbElevationNone.Text = "None";
            rbElevationNone.UseVisualStyleBackColor = true;
            rbElevationNone.CheckedChanged += rbElevationNone_CheckedChanged;
            // 
            // chkGdiScaling
            // 
            chkGdiScaling.AutoSize = true;
            chkGdiScaling.Location = new System.Drawing.Point(216, 22);
            chkGdiScaling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkGdiScaling.Name = "chkGdiScaling";
            chkGdiScaling.Size = new System.Drawing.Size(197, 19);
            chkGdiScaling.TabIndex = 7;
            chkGdiScaling.Text = "Use GDI Scaling (requires restart)";
            chkGdiScaling.UseVisualStyleBackColor = true;
            chkGdiScaling.CheckedChanged += chkGdiScaling_CheckedChanged;
            // 
            // chkCheckForUpdates
            // 
            chkCheckForUpdates.AutoSize = true;
            chkCheckForUpdates.Location = new System.Drawing.Point(7, 122);
            chkCheckForUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkCheckForUpdates.Name = "chkCheckForUpdates";
            chkCheckForUpdates.Size = new System.Drawing.Size(197, 19);
            chkCheckForUpdates.TabIndex = 6;
            chkCheckForUpdates.Text = "Automatically check for updates";
            chkCheckForUpdates.UseVisualStyleBackColor = true;
            chkCheckForUpdates.CheckedChanged += chkCheckForUpdates_CheckedChanged;
            // 
            // chkMinimizeToSystemTray
            // 
            chkMinimizeToSystemTray.AutoSize = true;
            chkMinimizeToSystemTray.Location = new System.Drawing.Point(7, 97);
            chkMinimizeToSystemTray.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
            chkMinimizeToSystemTray.Size = new System.Drawing.Size(152, 19);
            chkMinimizeToSystemTray.TabIndex = 5;
            chkMinimizeToSystemTray.Text = "Minimize to system tray";
            chkMinimizeToSystemTray.UseVisualStyleBackColor = true;
            chkMinimizeToSystemTray.CheckedChanged += chkMinimizeToSystemTray_CheckedChanged;
            // 
            // chkMinimizeOnClose
            // 
            chkMinimizeOnClose.AutoSize = true;
            chkMinimizeOnClose.Location = new System.Drawing.Point(7, 72);
            chkMinimizeOnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMinimizeOnClose.Name = "chkMinimizeOnClose";
            chkMinimizeOnClose.Size = new System.Drawing.Size(122, 19);
            chkMinimizeOnClose.TabIndex = 4;
            chkMinimizeOnClose.Text = "Minimize on close";
            chkMinimizeOnClose.UseVisualStyleBackColor = true;
            chkMinimizeOnClose.CheckedChanged += chkMinimizeOnClose_CheckedChanged;
            // 
            // chkStartMinimized
            // 
            chkStartMinimized.AutoSize = true;
            chkStartMinimized.Location = new System.Drawing.Point(7, 47);
            chkStartMinimized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartMinimized.Name = "chkStartMinimized";
            chkStartMinimized.Size = new System.Drawing.Size(109, 19);
            chkStartMinimized.TabIndex = 3;
            chkStartMinimized.Text = "Start minimized";
            chkStartMinimized.UseVisualStyleBackColor = true;
            // 
            // chkStartAfterLogin
            // 
            chkStartAfterLogin.AutoSize = true;
            chkStartAfterLogin.Location = new System.Drawing.Point(7, 22);
            chkStartAfterLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartAfterLogin.Name = "chkStartAfterLogin";
            chkStartAfterLogin.Size = new System.Drawing.Size(183, 19);
            chkStartAfterLogin.TabIndex = 2;
            chkStartAfterLogin.Text = "Automatically start after login";
            chkStartAfterLogin.UseVisualStyleBackColor = true;
            chkStartAfterLogin.CheckedChanged += chkStartAfterLogin_CheckedChanged;
            // 
            // tabLog
            // 
            tabLog.Controls.Add(lblShowLog);
            tabLog.Controls.Add(cbxLogType);
            tabLog.Controls.Add(btnClearLog);
            tabLog.Controls.Add(edtLog);
            tabLog.Location = new System.Drawing.Point(4, 24);
            tabLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabLog.Name = "tabLog";
            tabLog.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabLog.Size = new System.Drawing.Size(1114, 539);
            tabLog.TabIndex = 3;
            tabLog.Text = "Log";
            tabLog.UseVisualStyleBackColor = true;
            // 
            // lblShowLog
            // 
            lblShowLog.AutoSize = true;
            lblShowLog.Location = new System.Drawing.Point(8, 14);
            lblShowLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblShowLog.Name = "lblShowLog";
            lblShowLog.Size = new System.Drawing.Size(88, 15);
            lblShowLog.TabIndex = 33;
            lblShowLog.Text = "Show log from:";
            // 
            // cbxLogType
            // 
            cbxLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxLogType.FormattingEnabled = true;
            cbxLogType.Items.AddRange(new object[] { "Main application", "Service" });
            cbxLogType.Location = new System.Drawing.Point(102, 9);
            cbxLogType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxLogType.Name = "cbxLogType";
            cbxLogType.Size = new System.Drawing.Size(189, 23);
            cbxLogType.TabIndex = 32;
            cbxLogType.SelectedIndexChanged += cbxLogType_SelectedIndexChanged;
            // 
            // btnClearLog
            // 
            btnClearLog.Location = new System.Drawing.Point(299, 6);
            btnClearLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new System.Drawing.Size(88, 27);
            btnClearLog.TabIndex = 2;
            btnClearLog.Text = "Clear log";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += btnClearLog_Click;
            // 
            // edtLog
            // 
            edtLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            edtLog.Location = new System.Drawing.Point(7, 40);
            edtLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtLog.MaxLength = 327670;
            edtLog.Multiline = true;
            edtLog.Name = "edtLog";
            edtLog.ReadOnly = true;
            edtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            edtLog.Size = new System.Drawing.Size(1098, 478);
            edtLog.TabIndex = 0;
            // 
            // tabInfo
            // 
            tabInfo.Controls.Add(grpNVIDIAInfo);
            tabInfo.Controls.Add(groupBox3);
            tabInfo.Location = new System.Drawing.Point(4, 24);
            tabInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabInfo.Name = "tabInfo";
            tabInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabInfo.Size = new System.Drawing.Size(1114, 539);
            tabInfo.TabIndex = 4;
            tabInfo.Text = "Info";
            tabInfo.UseVisualStyleBackColor = true;
            // 
            // grpNVIDIAInfo
            // 
            grpNVIDIAInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpNVIDIAInfo.Controls.Add(btnRefreshNVIDIAInfo);
            grpNVIDIAInfo.Controls.Add(tvNVIDIAInfo);
            grpNVIDIAInfo.Location = new System.Drawing.Point(463, 7);
            grpNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNVIDIAInfo.Name = "grpNVIDIAInfo";
            grpNVIDIAInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNVIDIAInfo.Size = new System.Drawing.Size(643, 511);
            grpNVIDIAInfo.TabIndex = 4;
            grpNVIDIAInfo.TabStop = false;
            grpNVIDIAInfo.Text = "NVIDIA info";
            // 
            // btnRefreshNVIDIAInfo
            // 
            btnRefreshNVIDIAInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnRefreshNVIDIAInfo.Location = new System.Drawing.Point(7, 478);
            btnRefreshNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefreshNVIDIAInfo.Name = "btnRefreshNVIDIAInfo";
            btnRefreshNVIDIAInfo.Size = new System.Drawing.Size(88, 27);
            btnRefreshNVIDIAInfo.TabIndex = 1;
            btnRefreshNVIDIAInfo.Text = "Refresh";
            btnRefreshNVIDIAInfo.UseVisualStyleBackColor = true;
            btnRefreshNVIDIAInfo.Click += btnRefreshNVIDIAInfo_Click;
            // 
            // tvNVIDIAInfo
            // 
            tvNVIDIAInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tvNVIDIAInfo.Location = new System.Drawing.Point(7, 22);
            tvNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tvNVIDIAInfo.Name = "tvNVIDIAInfo";
            tvNVIDIAInfo.Size = new System.Drawing.Size(628, 448);
            tvNVIDIAInfo.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lbPlugins);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(lblInfo);
            groupBox3.Location = new System.Drawing.Point(7, 7);
            groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Size = new System.Drawing.Size(449, 254);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Info";
            // 
            // lbPlugins
            // 
            lbPlugins.FormattingEnabled = true;
            lbPlugins.HorizontalScrollbar = true;
            lbPlugins.ItemHeight = 15;
            lbPlugins.Location = new System.Drawing.Point(10, 77);
            lbPlugins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lbPlugins.Name = "lbPlugins";
            lbPlugins.Size = new System.Drawing.Size(432, 169);
            lbPlugins.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(7, 59);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(267, 15);
            label7.TabIndex = 2;
            label7.Text = "This app contains the following 3rd party plugins:";
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new System.Drawing.Point(7, 23);
            lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new System.Drawing.Size(28, 15);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "Info";
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnUpdate.ForeColor = System.Drawing.Color.Red;
            btnUpdate.Location = new System.Drawing.Point(1055, 0);
            btnUpdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new System.Drawing.Size(77, 26);
            btnUpdate.TabIndex = 61;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Visible = false;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnOptionsAdvanced
            // 
            btnOptionsAdvanced.Location = new System.Drawing.Point(7, 172);
            btnOptionsAdvanced.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOptionsAdvanced.Name = "btnOptionsAdvanced";
            btnOptionsAdvanced.Size = new System.Drawing.Size(88, 27);
            btnOptionsAdvanced.TabIndex = 11;
            btnOptionsAdvanced.Text = "Advanced...";
            btnOptionsAdvanced.UseVisualStyleBackColor = true;
            btnOptionsAdvanced.Click += btnOptionsAdvanced_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(1150, 594);
            Controls.Add(btnUpdate);
            Controls.Add(tcMain);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(1140, 611);
            Name = "MainForm";
            Text = "ColorControl";
            Activated += MainForm_Activated;
            Deactivate += MainForm_Deactivate;
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            ResizeBegin += MainForm_ResizeBegin;
            ResizeEnd += MainForm_ResizeEnd;
            Click += MainForm_Click;
            Resize += MainForm_Resize;
            tcMain.ResumeLayout(false);
            tabOptions.ResumeLayout(false);
            grpOptionsModules.ResumeLayout(false);
            grpOptionsModules.PerformLayout();
            grpMiscellaneousOptions.ResumeLayout(false);
            grpMiscellaneousOptions.PerformLayout();
            grpGeneralOptions.ResumeLayout(false);
            grpGeneralOptions.PerformLayout();
            tabLog.ResumeLayout(false);
            tabLog.PerformLayout();
            tabInfo.ResumeLayout(false);
            grpNVIDIAInfo.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
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
        private System.Windows.Forms.GroupBox grpMiscellaneousOptions;
        private System.Windows.Forms.CheckBox chkFixChromeFonts;
        private System.Windows.Forms.Label lblFixChromeFontsDescription;
        private System.Windows.Forms.Button btnSetShortcutScreenSaver;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox edtBlankScreenSaverShortcut;
        private System.Windows.Forms.CheckBox chkMinimizeOnClose;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkMinimizeToSystemTray;
        private System.Windows.Forms.CheckBox chkCheckForUpdates;
        private System.Windows.Forms.CheckBox chkGdiScaling;
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
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.GroupBox grpOptionsModules;
        private System.Windows.Forms.CheckedListBox chkModules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOptionsUseDarkMode;
        private System.Windows.Forms.Button btnOptionsAdvanced;
    }
}


using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System;

namespace ColorControl.Forms
{
    partial class OptionsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grpOptionsModules = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            chkModules = new System.Windows.Forms.CheckedListBox();
            grpMiscellaneousOptions = new System.Windows.Forms.GroupBox();
            btnOptionsColorProfiles = new System.Windows.Forms.Button();
            btnSetShortcutScreenSaver = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            edtBlankScreenSaverShortcut = new System.Windows.Forms.TextBox();
            lblFixChromeFontsDescription = new System.Windows.Forms.Label();
            chkFixChromeFonts = new System.Windows.Forms.CheckBox();
            grpGeneralOptions = new System.Windows.Forms.GroupBox();
            btnOptionsLog = new System.Windows.Forms.Button();
            btnOptionsAdvanced = new System.Windows.Forms.Button();
            chkAutoInstallUpdates = new System.Windows.Forms.CheckBox();
            btnStartStopService = new System.Windows.Forms.Button();
            btnElevationInfo = new System.Windows.Forms.Button();
            rbElevationService = new System.Windows.Forms.RadioButton();
            rbElevationProcess = new System.Windows.Forms.RadioButton();
            rbElevationAdmin = new System.Windows.Forms.RadioButton();
            label8 = new System.Windows.Forms.Label();
            rbElevationNone = new System.Windows.Forms.RadioButton();
            chkCheckForUpdates = new System.Windows.Forms.CheckBox();
            chkStartAfterLogin = new System.Windows.Forms.CheckBox();
            chkOptionsUseDarkMode = new System.Windows.Forms.CheckBox();
            chkGdiScaling = new System.Windows.Forms.CheckBox();
            chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
            chkMinimizeOnClose = new System.Windows.Forms.CheckBox();
            chkStartMinimized = new System.Windows.Forms.CheckBox();
            mnuColorProfiles = new System.Windows.Forms.ContextMenuStrip(components);
            miCreateHDRColorProfile = new System.Windows.Forms.ToolStripMenuItem();
            miCreateSDRColorProfile = new System.Windows.Forms.ToolStripMenuItem();
            lblUiType = new System.Windows.Forms.Label();
            cbxUiType = new System.Windows.Forms.ComboBox();
            grpUi = new System.Windows.Forms.GroupBox();
            grpOptionsModules.SuspendLayout();
            grpMiscellaneousOptions.SuspendLayout();
            grpGeneralOptions.SuspendLayout();
            mnuColorProfiles.SuspendLayout();
            grpUi.SuspendLayout();
            SuspendLayout();
            // 
            // grpOptionsModules
            // 
            grpOptionsModules.Controls.Add(label1);
            grpOptionsModules.Controls.Add(chkModules);
            grpOptionsModules.Location = new Point(14, 344);
            grpOptionsModules.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpOptionsModules.Name = "grpOptionsModules";
            grpOptionsModules.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpOptionsModules.Size = new Size(658, 131);
            grpOptionsModules.TabIndex = 11;
            grpOptionsModules.TabStop = false;
            grpOptionsModules.Text = "Modules";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 19);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(511, 15);
            label1.TabIndex = 7;
            label1.Text = "Here you can disable modules you do not want. A restart is required for a change to have effect.";
            // 
            // chkModules
            // 
            chkModules.ColumnWidth = 200;
            chkModules.FormattingEnabled = true;
            chkModules.Location = new Point(7, 40);
            chkModules.MultiColumn = true;
            chkModules.Name = "chkModules";
            chkModules.Size = new Size(507, 76);
            chkModules.TabIndex = 0;
            chkModules.ItemCheck += chkModules_ItemCheck;
            // 
            // grpMiscellaneousOptions
            // 
            grpMiscellaneousOptions.Controls.Add(btnOptionsColorProfiles);
            grpMiscellaneousOptions.Controls.Add(btnSetShortcutScreenSaver);
            grpMiscellaneousOptions.Controls.Add(label11);
            grpMiscellaneousOptions.Controls.Add(edtBlankScreenSaverShortcut);
            grpMiscellaneousOptions.Controls.Add(lblFixChromeFontsDescription);
            grpMiscellaneousOptions.Controls.Add(chkFixChromeFonts);
            grpMiscellaneousOptions.Location = new Point(14, 481);
            grpMiscellaneousOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMiscellaneousOptions.Name = "grpMiscellaneousOptions";
            grpMiscellaneousOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMiscellaneousOptions.Size = new Size(658, 131);
            grpMiscellaneousOptions.TabIndex = 5;
            grpMiscellaneousOptions.TabStop = false;
            grpMiscellaneousOptions.Text = "Miscellaneous";
            // 
            // btnOptionsColorProfiles
            // 
            btnOptionsColorProfiles.Location = new Point(538, 17);
            btnOptionsColorProfiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOptionsColorProfiles.Name = "btnOptionsColorProfiles";
            btnOptionsColorProfiles.Size = new Size(112, 27);
            btnOptionsColorProfiles.TabIndex = 12;
            btnOptionsColorProfiles.Text = "Color Profiles...";
            btnOptionsColorProfiles.UseVisualStyleBackColor = true;
            btnOptionsColorProfiles.Click += btnOptionsColorProfiles_Click;
            // 
            // btnSetShortcutScreenSaver
            // 
            btnSetShortcutScreenSaver.Location = new Point(181, 98);
            btnSetShortcutScreenSaver.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetShortcutScreenSaver.Name = "btnSetShortcutScreenSaver";
            btnSetShortcutScreenSaver.Size = new Size(40, 27);
            btnSetShortcutScreenSaver.TabIndex = 10;
            btnSetShortcutScreenSaver.Text = "Set";
            btnSetShortcutScreenSaver.UseVisualStyleBackColor = true;
            btnSetShortcutScreenSaver.Click += btnSetShortcutScreenSaver_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(7, 80);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(238, 15);
            label11.TabIndex = 9;
            label11.Text = "Set a shortcut to start the blank screensaver:";
            // 
            // edtBlankScreenSaverShortcut
            // 
            edtBlankScreenSaverShortcut.Location = new Point(7, 101);
            edtBlankScreenSaverShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtBlankScreenSaverShortcut.Name = "edtBlankScreenSaverShortcut";
            edtBlankScreenSaverShortcut.ReadOnly = true;
            edtBlankScreenSaverShortcut.Size = new Size(165, 23);
            edtBlankScreenSaverShortcut.TabIndex = 7;
            edtBlankScreenSaverShortcut.KeyDown += edtShortcut_KeyDown;
            edtBlankScreenSaverShortcut.KeyUp += edtShortcut_KeyUp;
            // 
            // lblFixChromeFontsDescription
            // 
            lblFixChromeFontsDescription.AutoSize = true;
            lblFixChromeFontsDescription.Location = new Point(4, 45);
            lblFixChromeFontsDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFixChromeFontsDescription.Name = "lblFixChromeFontsDescription";
            lblFixChromeFontsDescription.Size = new Size(427, 15);
            lblFixChromeFontsDescription.TabIndex = 6;
            lblFixChromeFontsDescription.Text = "This will add the parameter --disable-lcd-text to Chrome and requires elevation.";
            // 
            // chkFixChromeFonts
            // 
            chkFixChromeFonts.AutoSize = true;
            chkFixChromeFonts.Location = new Point(7, 22);
            chkFixChromeFonts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkFixChromeFonts.Name = "chkFixChromeFonts";
            chkFixChromeFonts.Size = new Size(378, 19);
            chkFixChromeFonts.TabIndex = 4;
            chkFixChromeFonts.Text = "ClearType: fix bad fonts in Chrome (turn on grayscale anti-aliasing)";
            chkFixChromeFonts.UseVisualStyleBackColor = true;
            chkFixChromeFonts.CheckedChanged += chkFixChromeFonts_CheckedChanged;
            // 
            // grpGeneralOptions
            // 
            grpGeneralOptions.Controls.Add(btnOptionsLog);
            grpGeneralOptions.Controls.Add(btnOptionsAdvanced);
            grpGeneralOptions.Controls.Add(chkAutoInstallUpdates);
            grpGeneralOptions.Controls.Add(btnStartStopService);
            grpGeneralOptions.Controls.Add(btnElevationInfo);
            grpGeneralOptions.Controls.Add(rbElevationService);
            grpGeneralOptions.Controls.Add(rbElevationProcess);
            grpGeneralOptions.Controls.Add(rbElevationAdmin);
            grpGeneralOptions.Controls.Add(label8);
            grpGeneralOptions.Controls.Add(rbElevationNone);
            grpGeneralOptions.Controls.Add(chkCheckForUpdates);
            grpGeneralOptions.Controls.Add(chkStartAfterLogin);
            grpGeneralOptions.Location = new Point(7, 6);
            grpGeneralOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGeneralOptions.Name = "grpGeneralOptions";
            grpGeneralOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGeneralOptions.Size = new Size(662, 202);
            grpGeneralOptions.TabIndex = 2;
            grpGeneralOptions.TabStop = false;
            grpGeneralOptions.Text = "General";
            // 
            // btnOptionsLog
            // 
            btnOptionsLog.Location = new Point(405, 47);
            btnOptionsLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOptionsLog.Name = "btnOptionsLog";
            btnOptionsLog.Size = new Size(88, 27);
            btnOptionsLog.TabIndex = 68;
            btnOptionsLog.Text = "Logging...";
            btnOptionsLog.UseVisualStyleBackColor = true;
            btnOptionsLog.Click += btnOptionsLog_Click;
            // 
            // btnOptionsAdvanced
            // 
            btnOptionsAdvanced.Location = new Point(405, 17);
            btnOptionsAdvanced.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOptionsAdvanced.Name = "btnOptionsAdvanced";
            btnOptionsAdvanced.Size = new Size(88, 27);
            btnOptionsAdvanced.TabIndex = 11;
            btnOptionsAdvanced.Text = "Advanced...";
            btnOptionsAdvanced.UseVisualStyleBackColor = true;
            btnOptionsAdvanced.Click += btnOptionsAdvanced_Click;
            // 
            // chkAutoInstallUpdates
            // 
            chkAutoInstallUpdates.AutoSize = true;
            chkAutoInstallUpdates.Location = new Point(7, 72);
            chkAutoInstallUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAutoInstallUpdates.Name = "chkAutoInstallUpdates";
            chkAutoInstallUpdates.Size = new Size(179, 19);
            chkAutoInstallUpdates.TabIndex = 66;
            chkAutoInstallUpdates.Text = "Automatically install updates";
            chkAutoInstallUpdates.UseVisualStyleBackColor = true;
            chkAutoInstallUpdates.CheckedChanged += chkAutoInstallUpdates_CheckedChanged;
            // 
            // btnStartStopService
            // 
            btnStartStopService.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStartStopService.Location = new Point(405, 150);
            btnStartStopService.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartStopService.Name = "btnStartStopService";
            btnStartStopService.Size = new Size(40, 27);
            btnStartStopService.TabIndex = 65;
            btnStartStopService.Text = "Start";
            btnStartStopService.UseVisualStyleBackColor = true;
            btnStartStopService.Click += btnStartStopService_Click;
            // 
            // btnElevationInfo
            // 
            btnElevationInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnElevationInfo.Location = new Point(120, 88);
            btnElevationInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnElevationInfo.Name = "btnElevationInfo";
            btnElevationInfo.Size = new Size(22, 27);
            btnElevationInfo.TabIndex = 64;
            btnElevationInfo.Text = "?";
            btnElevationInfo.UseVisualStyleBackColor = true;
            btnElevationInfo.Click += btnElevationInfo_Click;
            // 
            // rbElevationService
            // 
            rbElevationService.AutoSize = true;
            rbElevationService.Location = new Point(7, 154);
            rbElevationService.Name = "rbElevationService";
            rbElevationService.Size = new Size(136, 19);
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
            rbElevationProcess.Location = new Point(7, 175);
            rbElevationProcess.Name = "rbElevationProcess";
            rbElevationProcess.Size = new Size(189, 19);
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
            rbElevationAdmin.Location = new Point(7, 133);
            rbElevationAdmin.Name = "rbElevationAdmin";
            rbElevationAdmin.Size = new Size(97, 19);
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
            label8.Location = new Point(7, 94);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(105, 15);
            label8.TabIndex = 60;
            label8.Text = "Elevation-method:";
            // 
            // rbElevationNone
            // 
            rbElevationNone.AutoSize = true;
            rbElevationNone.Location = new Point(7, 112);
            rbElevationNone.Name = "rbElevationNone";
            rbElevationNone.Size = new Size(54, 19);
            rbElevationNone.TabIndex = 10;
            rbElevationNone.TabStop = true;
            rbElevationNone.Tag = "0";
            rbElevationNone.Text = "None";
            rbElevationNone.UseVisualStyleBackColor = true;
            rbElevationNone.CheckedChanged += rbElevationNone_CheckedChanged;
            // 
            // chkCheckForUpdates
            // 
            chkCheckForUpdates.AutoSize = true;
            chkCheckForUpdates.Location = new Point(7, 47);
            chkCheckForUpdates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkCheckForUpdates.Name = "chkCheckForUpdates";
            chkCheckForUpdates.Size = new Size(197, 19);
            chkCheckForUpdates.TabIndex = 6;
            chkCheckForUpdates.Text = "Automatically check for updates";
            chkCheckForUpdates.UseVisualStyleBackColor = true;
            chkCheckForUpdates.CheckedChanged += chkCheckForUpdates_CheckedChanged;
            // 
            // chkStartAfterLogin
            // 
            chkStartAfterLogin.AutoSize = true;
            chkStartAfterLogin.Location = new Point(7, 22);
            chkStartAfterLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartAfterLogin.Name = "chkStartAfterLogin";
            chkStartAfterLogin.Size = new Size(183, 19);
            chkStartAfterLogin.TabIndex = 2;
            chkStartAfterLogin.Text = "Automatically start after login";
            chkStartAfterLogin.UseVisualStyleBackColor = true;
            chkStartAfterLogin.CheckedChanged += chkStartAfterLogin_CheckedChanged;
            // 
            // chkOptionsUseDarkMode
            // 
            chkOptionsUseDarkMode.AutoSize = true;
            chkOptionsUseDarkMode.Location = new Point(329, 22);
            chkOptionsUseDarkMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkOptionsUseDarkMode.Name = "chkOptionsUseDarkMode";
            chkOptionsUseDarkMode.Size = new Size(224, 19);
            chkOptionsUseDarkMode.TabIndex = 67;
            chkOptionsUseDarkMode.Text = "Dark Mode (WinForms: experimental)";
            chkOptionsUseDarkMode.UseVisualStyleBackColor = true;
            chkOptionsUseDarkMode.CheckedChanged += chkOptionsUseDarkMode_CheckedChanged;
            // 
            // chkGdiScaling
            // 
            chkGdiScaling.AutoSize = true;
            chkGdiScaling.Location = new Point(329, 47);
            chkGdiScaling.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkGdiScaling.Name = "chkGdiScaling";
            chkGdiScaling.Size = new Size(257, 19);
            chkGdiScaling.TabIndex = 7;
            chkGdiScaling.Text = "WinForms: Use GDI Scaling (requires restart)";
            chkGdiScaling.UseVisualStyleBackColor = true;
            chkGdiScaling.CheckedChanged += chkGdiScaling_CheckedChanged;
            // 
            // chkMinimizeToSystemTray
            // 
            chkMinimizeToSystemTray.AutoSize = true;
            chkMinimizeToSystemTray.Location = new Point(7, 97);
            chkMinimizeToSystemTray.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
            chkMinimizeToSystemTray.Size = new Size(152, 19);
            chkMinimizeToSystemTray.TabIndex = 5;
            chkMinimizeToSystemTray.Text = "Minimize to system tray";
            chkMinimizeToSystemTray.UseVisualStyleBackColor = true;
            chkMinimizeToSystemTray.CheckedChanged += chkMinimizeToSystemTray_CheckedChanged;
            // 
            // chkMinimizeOnClose
            // 
            chkMinimizeOnClose.AutoSize = true;
            chkMinimizeOnClose.Location = new Point(7, 72);
            chkMinimizeOnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMinimizeOnClose.Name = "chkMinimizeOnClose";
            chkMinimizeOnClose.Size = new Size(122, 19);
            chkMinimizeOnClose.TabIndex = 4;
            chkMinimizeOnClose.Text = "Minimize on close";
            chkMinimizeOnClose.UseVisualStyleBackColor = true;
            chkMinimizeOnClose.CheckedChanged += chkMinimizeOnClose_CheckedChanged;
            // 
            // chkStartMinimized
            // 
            chkStartMinimized.AutoSize = true;
            chkStartMinimized.Location = new Point(7, 47);
            chkStartMinimized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartMinimized.Name = "chkStartMinimized";
            chkStartMinimized.Size = new Size(109, 19);
            chkStartMinimized.TabIndex = 3;
            chkStartMinimized.Text = "Start minimized";
            chkStartMinimized.UseVisualStyleBackColor = true;
            chkStartMinimized.CheckedChanged += chkStartMinimized_CheckedChanged;
            // 
            // mnuColorProfiles
            // 
            mnuColorProfiles.ImageScalingSize = new Size(20, 20);
            mnuColorProfiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miCreateHDRColorProfile, miCreateSDRColorProfile });
            mnuColorProfiles.Name = "mnuLgButtons";
            mnuColorProfiles.Size = new Size(214, 48);
            // 
            // miCreateHDRColorProfile
            // 
            miCreateHDRColorProfile.Name = "miCreateHDRColorProfile";
            miCreateHDRColorProfile.Size = new Size(213, 22);
            miCreateHDRColorProfile.Text = "Create HDR Color Profile...";
            miCreateHDRColorProfile.Click += miCreateHDRColorProfile_Click;
            // 
            // miCreateSDRColorProfile
            // 
            miCreateSDRColorProfile.Name = "miCreateSDRColorProfile";
            miCreateSDRColorProfile.Size = new Size(213, 22);
            miCreateSDRColorProfile.Text = "Create SDR Color Profile...";
            miCreateSDRColorProfile.Click += miCreateSDRColorProfile_Click;
            // 
            // lblUiType
            // 
            lblUiType.AutoSize = true;
            lblUiType.Location = new Point(6, 21);
            lblUiType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUiType.Name = "lblUiType";
            lblUiType.Size = new Size(62, 15);
            lblUiType.TabIndex = 70;
            lblUiType.Text = "Default UI:";
            // 
            // cbxUiType
            // 
            cbxUiType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxUiType.FormattingEnabled = true;
            cbxUiType.Items.AddRange(new object[] { "WinForms", "Web (default browser)", "Web (embedded browser)" });
            cbxUiType.Location = new Point(105, 18);
            cbxUiType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxUiType.Name = "cbxUiType";
            cbxUiType.Size = new Size(208, 23);
            cbxUiType.TabIndex = 69;
            cbxUiType.SelectedIndexChanged += cbxUiType_SelectedIndexChanged;
            // 
            // grpUi
            // 
            grpUi.Controls.Add(lblUiType);
            grpUi.Controls.Add(chkStartMinimized);
            grpUi.Controls.Add(chkOptionsUseDarkMode);
            grpUi.Controls.Add(cbxUiType);
            grpUi.Controls.Add(chkMinimizeOnClose);
            grpUi.Controls.Add(chkMinimizeToSystemTray);
            grpUi.Controls.Add(chkGdiScaling);
            grpUi.Location = new Point(7, 214);
            grpUi.Name = "grpUi";
            grpUi.Size = new Size(662, 124);
            grpUi.TabIndex = 8;
            grpUi.TabStop = false;
            grpUi.Text = "UI-settings";
            // 
            // OptionsPanel
            // 
            Controls.Add(grpUi);
            Controls.Add(grpOptionsModules);
            Controls.Add(grpMiscellaneousOptions);
            Controls.Add(grpGeneralOptions);
            Name = "OptionsPanel";
            Size = new Size(732, 615);
            grpOptionsModules.ResumeLayout(false);
            grpOptionsModules.PerformLayout();
            grpMiscellaneousOptions.ResumeLayout(false);
            grpMiscellaneousOptions.PerformLayout();
            grpGeneralOptions.ResumeLayout(false);
            grpGeneralOptions.PerformLayout();
            mnuColorProfiles.ResumeLayout(false);
            grpUi.ResumeLayout(false);
            grpUi.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpGeneralOptions;
        private System.Windows.Forms.CheckBox chkStartMinimized;
        private System.Windows.Forms.CheckBox chkStartAfterLogin;
        private System.Windows.Forms.GroupBox grpMiscellaneousOptions;
        private System.Windows.Forms.CheckBox chkFixChromeFonts;
        private System.Windows.Forms.Label lblFixChromeFontsDescription;
        private System.Windows.Forms.Button btnSetShortcutScreenSaver;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox edtBlankScreenSaverShortcut;
        private System.Windows.Forms.CheckBox chkMinimizeOnClose;
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
        private System.Windows.Forms.CheckBox chkAutoInstallUpdates;
        private System.Windows.Forms.GroupBox grpOptionsModules;
        private System.Windows.Forms.CheckedListBox chkModules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOptionsUseDarkMode;
        private System.Windows.Forms.Button btnOptionsAdvanced;
        private System.Windows.Forms.Button btnOptionsLog;
        private System.Windows.Forms.Button btnOptionsColorProfiles;
        private System.Windows.Forms.ContextMenuStrip mnuColorProfiles;
        private System.Windows.Forms.ToolStripMenuItem miCreateHDRColorProfile;
        private System.Windows.Forms.ToolStripMenuItem miCreateSDRColorProfile;

        #endregion

        private System.Windows.Forms.Label lblUiType;
        private System.Windows.Forms.ComboBox cbxUiType;
        private System.Windows.Forms.GroupBox grpUi;
    }
}

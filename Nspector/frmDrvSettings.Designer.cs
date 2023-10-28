namespace nspector
{
    partial class frmDrvSettings
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDrvSettings));
            ilListView = new ImageList(components);
            pbMain = new ProgressBar();
            tsMain = new ToolStrip();
            tslProfiles = new ToolStripLabel();
            cbProfiles = new ToolStripComboBox();
            tsbModifiedProfiles = new ToolStripSplitButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbRefreshProfile = new ToolStripButton();
            tsbRestoreProfile = new ToolStripButton();
            tsbCreateProfile = new ToolStripButton();
            tsbDeleteProfile = new ToolStripButton();
            tsSep2 = new ToolStripSeparator();
            tsbAddApplication = new ToolStripButton();
            tssbRemoveApplication = new ToolStripSplitButton();
            tsSep3 = new ToolStripSeparator();
            tsbExportProfiles = new ToolStripSplitButton();
            exportCurrentProfileOnlyToolStripMenuItem = new ToolStripMenuItem();
            exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem = new ToolStripMenuItem();
            exportUserdefinedProfilesToolStripMenuItem = new ToolStripMenuItem();
            exportAllProfilesNVIDIATextFormatToolStripMenuItem = new ToolStripMenuItem();
            tsbImportProfiles = new ToolStripSplitButton();
            importProfilesToolStripMenuItem = new ToolStripMenuItem();
            importAllProfilesNVIDIATextFormatToolStripMenuItem = new ToolStripMenuItem();
            tsSep4 = new ToolStripSeparator();
            tscbShowCustomSettingNamesOnly = new ToolStripButton();
            tsSep5 = new ToolStripSeparator();
            tscbShowScannedUnknownSettings = new ToolStripButton();
            tsbBitValueEditor = new ToolStripButton();
            tsSep6 = new ToolStripSeparator();
            tsbApplyProfile = new ToolStripButton();
            btnResetValue = new Button();
            lblApplications = new Label();
            toolStripButton5 = new ToolStripButton();
            toolStripLabel2 = new ToolStripLabel();
            toolStripButton6 = new ToolStripButton();
            ilCombo = new ImageList(components);
            cbValues = new ComboBox();
            lblWidth96 = new Label();
            lblWidth330 = new Label();
            lblWidth16 = new Label();
            lblWidth30 = new Label();
            lvSettings = new ListViewEx();
            chSettingID = new ColumnHeader();
            chSettingValue = new ColumnHeader();
            chSettingValueHex = new ColumnHeader();
            tbSettingDescription = new TextBox();
            pnlListview = new Panel();
            tsMain.SuspendLayout();
            pnlListview.SuspendLayout();
            SuspendLayout();
            // 
            // ilListView
            // 
            ilListView.ColorDepth = ColorDepth.Depth24Bit;
            ilListView.ImageStream = (ImageListStreamer)resources.GetObject("ilListView.ImageStream");
            ilListView.TransparentColor = Color.Transparent;
            ilListView.Images.SetKeyName(0, "0_gear2.png");
            ilListView.Images.SetKeyName(1, "1_gear2_2.png");
            ilListView.Images.SetKeyName(2, "4_gear_nv2.png");
            ilListView.Images.SetKeyName(3, "6_gear_inherit.png");
            // 
            // pbMain
            // 
            pbMain.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbMain.Location = new Point(14, 548);
            pbMain.Margin = new Padding(5, 5, 5, 5);
            pbMain.Name = "pbMain";
            pbMain.Size = new Size(980, 10);
            pbMain.TabIndex = 19;
            // 
            // tsMain
            // 
            tsMain.AllowMerge = false;
            tsMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tsMain.AutoSize = false;
            tsMain.BackgroundImage = (Image)resources.GetObject("tsMain.BackgroundImage");
            tsMain.CanOverflow = false;
            tsMain.Dock = DockStyle.None;
            tsMain.GripMargin = new Padding(0);
            tsMain.GripStyle = ToolStripGripStyle.Hidden;
            tsMain.ImageScalingSize = new Size(20, 20);
            tsMain.Items.AddRange(new ToolStripItem[] { tslProfiles, cbProfiles, tsbModifiedProfiles, toolStripSeparator1, tsbRefreshProfile, tsbRestoreProfile, tsbCreateProfile, tsbDeleteProfile, tsSep2, tsbAddApplication, tssbRemoveApplication, tsSep3, tsbExportProfiles, tsbImportProfiles, tsSep4, tscbShowCustomSettingNamesOnly, tsSep5, tscbShowScannedUnknownSettings, tsbBitValueEditor, tsSep6, tsbApplyProfile });
            tsMain.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            tsMain.Location = new Point(14, 5);
            tsMain.Name = "tsMain";
            tsMain.RenderMode = ToolStripRenderMode.Professional;
            tsMain.Size = new Size(980, 29);
            tsMain.TabIndex = 24;
            tsMain.Text = "toolStrip1";
            // 
            // tslProfiles
            // 
            tslProfiles.ImageScaling = ToolStripItemImageScaling.None;
            tslProfiles.Margin = new Padding(0, 5, 10, 2);
            tslProfiles.Name = "tslProfiles";
            tslProfiles.Size = new Size(49, 22);
            tslProfiles.Text = "Profiles:";
            // 
            // cbProfiles
            // 
            cbProfiles.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbProfiles.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbProfiles.AutoSize = false;
            cbProfiles.DropDownWidth = 290;
            cbProfiles.Margin = new Padding(1);
            cbProfiles.MaxDropDownItems = 50;
            cbProfiles.Name = "cbProfiles";
            cbProfiles.Size = new Size(338, 23);
            cbProfiles.SelectedIndexChanged += cbProfiles_SelectedIndexChanged;
            cbProfiles.TextChanged += cbProfiles_TextChanged;
            // 
            // tsbModifiedProfiles
            // 
            tsbModifiedProfiles.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbModifiedProfiles.Enabled = false;
            tsbModifiedProfiles.Image = (Image)resources.GetObject("tsbModifiedProfiles.Image");
            tsbModifiedProfiles.ImageTransparentColor = Color.Magenta;
            tsbModifiedProfiles.Name = "tsbModifiedProfiles";
            tsbModifiedProfiles.Size = new Size(36, 26);
            tsbModifiedProfiles.TextImageRelation = TextImageRelation.Overlay;
            tsbModifiedProfiles.ToolTipText = "Back to global profile (Home) / User modified profiles";
            tsbModifiedProfiles.ButtonClick += tsbModifiedProfiles_ButtonClick;
            tsbModifiedProfiles.DropDownItemClicked += tsbModifiedProfiles_DropDownItemClicked;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 29);
            // 
            // tsbRefreshProfile
            // 
            tsbRefreshProfile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRefreshProfile.Image = (Image)resources.GetObject("tsbRefreshProfile.Image");
            tsbRefreshProfile.ImageTransparentColor = Color.Magenta;
            tsbRefreshProfile.Name = "tsbRefreshProfile";
            tsbRefreshProfile.Size = new Size(24, 26);
            tsbRefreshProfile.Text = "Refresh current profile.";
            tsbRefreshProfile.Click += tsbRefreshProfile_Click;
            // 
            // tsbRestoreProfile
            // 
            tsbRestoreProfile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRestoreProfile.Image = (Image)resources.GetObject("tsbRestoreProfile.Image");
            tsbRestoreProfile.ImageTransparentColor = Color.Magenta;
            tsbRestoreProfile.Name = "tsbRestoreProfile";
            tsbRestoreProfile.Size = new Size(24, 26);
            tsbRestoreProfile.Text = "Restore current profile to NVIDIA defaults.";
            tsbRestoreProfile.Click += tsbRestoreProfile_Click;
            // 
            // tsbCreateProfile
            // 
            tsbCreateProfile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCreateProfile.Image = (Image)resources.GetObject("tsbCreateProfile.Image");
            tsbCreateProfile.ImageTransparentColor = Color.Magenta;
            tsbCreateProfile.Name = "tsbCreateProfile";
            tsbCreateProfile.Size = new Size(24, 26);
            tsbCreateProfile.Text = "Create new profile";
            tsbCreateProfile.Click += tsbCreateProfile_Click;
            // 
            // tsbDeleteProfile
            // 
            tsbDeleteProfile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDeleteProfile.Image = (Image)resources.GetObject("tsbDeleteProfile.Image");
            tsbDeleteProfile.ImageTransparentColor = Color.Magenta;
            tsbDeleteProfile.Name = "tsbDeleteProfile";
            tsbDeleteProfile.Size = new Size(24, 26);
            tsbDeleteProfile.Text = "Delete current Profile";
            tsbDeleteProfile.Click += tsbDeleteProfile_Click;
            // 
            // tsSep2
            // 
            tsSep2.Name = "tsSep2";
            tsSep2.Size = new Size(6, 29);
            // 
            // tsbAddApplication
            // 
            tsbAddApplication.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAddApplication.Image = (Image)resources.GetObject("tsbAddApplication.Image");
            tsbAddApplication.ImageTransparentColor = Color.Magenta;
            tsbAddApplication.Name = "tsbAddApplication";
            tsbAddApplication.Size = new Size(24, 26);
            tsbAddApplication.Text = "Add application to current profile.";
            tsbAddApplication.Click += tsbAddApplication_Click;
            // 
            // tssbRemoveApplication
            // 
            tssbRemoveApplication.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tssbRemoveApplication.Image = (Image)resources.GetObject("tssbRemoveApplication.Image");
            tssbRemoveApplication.ImageTransparentColor = Color.Magenta;
            tssbRemoveApplication.Name = "tssbRemoveApplication";
            tssbRemoveApplication.Size = new Size(36, 26);
            tssbRemoveApplication.Text = "Remove application from current profile";
            tssbRemoveApplication.DropDownItemClicked += tssbRemoveApplication_DropDownItemClicked;
            tssbRemoveApplication.Click += tssbRemoveApplication_Click;
            // 
            // tsSep3
            // 
            tsSep3.Name = "tsSep3";
            tsSep3.Size = new Size(6, 29);
            // 
            // tsbExportProfiles
            // 
            tsbExportProfiles.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbExportProfiles.DropDownItems.AddRange(new ToolStripItem[] { exportCurrentProfileOnlyToolStripMenuItem, exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem, exportUserdefinedProfilesToolStripMenuItem, exportAllProfilesNVIDIATextFormatToolStripMenuItem });
            tsbExportProfiles.Image = (Image)resources.GetObject("tsbExportProfiles.Image");
            tsbExportProfiles.ImageTransparentColor = Color.Magenta;
            tsbExportProfiles.Name = "tsbExportProfiles";
            tsbExportProfiles.Size = new Size(36, 26);
            tsbExportProfiles.Text = "Export user defined profiles";
            tsbExportProfiles.Click += tsbExportProfiles_Click;
            // 
            // exportCurrentProfileOnlyToolStripMenuItem
            // 
            exportCurrentProfileOnlyToolStripMenuItem.Name = "exportCurrentProfileOnlyToolStripMenuItem";
            exportCurrentProfileOnlyToolStripMenuItem.Size = new Size(343, 22);
            exportCurrentProfileOnlyToolStripMenuItem.Text = "Export current profile only";
            exportCurrentProfileOnlyToolStripMenuItem.Click += exportCurrentProfileOnlyToolStripMenuItem_Click;
            // 
            // exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem
            // 
            exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem.Name = "exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem";
            exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem.Size = new Size(343, 22);
            exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem.Text = "Export current profile including predefined settings";
            exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem.Click += exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem_Click;
            // 
            // exportUserdefinedProfilesToolStripMenuItem
            // 
            exportUserdefinedProfilesToolStripMenuItem.Name = "exportUserdefinedProfilesToolStripMenuItem";
            exportUserdefinedProfilesToolStripMenuItem.Size = new Size(343, 22);
            exportUserdefinedProfilesToolStripMenuItem.Text = "Export all customized profiles";
            exportUserdefinedProfilesToolStripMenuItem.Click += exportUserdefinedProfilesToolStripMenuItem_Click;
            // 
            // exportAllProfilesNVIDIATextFormatToolStripMenuItem
            // 
            exportAllProfilesNVIDIATextFormatToolStripMenuItem.Name = "exportAllProfilesNVIDIATextFormatToolStripMenuItem";
            exportAllProfilesNVIDIATextFormatToolStripMenuItem.Size = new Size(343, 22);
            exportAllProfilesNVIDIATextFormatToolStripMenuItem.Text = "Export all driver profiles (NVIDIA Text Format)";
            exportAllProfilesNVIDIATextFormatToolStripMenuItem.Click += exportAllProfilesNVIDIATextFormatToolStripMenuItem_Click;
            // 
            // tsbImportProfiles
            // 
            tsbImportProfiles.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbImportProfiles.DropDownItems.AddRange(new ToolStripItem[] { importProfilesToolStripMenuItem, importAllProfilesNVIDIATextFormatToolStripMenuItem });
            tsbImportProfiles.Image = (Image)resources.GetObject("tsbImportProfiles.Image");
            tsbImportProfiles.ImageTransparentColor = Color.Magenta;
            tsbImportProfiles.Name = "tsbImportProfiles";
            tsbImportProfiles.Size = new Size(36, 26);
            tsbImportProfiles.Text = "Import user defined profiles";
            tsbImportProfiles.Click += tsbImportProfiles_Click;
            // 
            // importProfilesToolStripMenuItem
            // 
            importProfilesToolStripMenuItem.Name = "importProfilesToolStripMenuItem";
            importProfilesToolStripMenuItem.Size = new Size(363, 22);
            importProfilesToolStripMenuItem.Text = "Import profile(s)";
            importProfilesToolStripMenuItem.Click += importProfilesToolStripMenuItem_Click;
            // 
            // importAllProfilesNVIDIATextFormatToolStripMenuItem
            // 
            importAllProfilesNVIDIATextFormatToolStripMenuItem.Name = "importAllProfilesNVIDIATextFormatToolStripMenuItem";
            importAllProfilesNVIDIATextFormatToolStripMenuItem.Size = new Size(363, 22);
            importAllProfilesNVIDIATextFormatToolStripMenuItem.Text = "Import (replace) all driver profiles (NVIDIA Text Format)";
            importAllProfilesNVIDIATextFormatToolStripMenuItem.Click += importAllProfilesNVIDIATextFormatToolStripMenuItem_Click;
            // 
            // tsSep4
            // 
            tsSep4.Name = "tsSep4";
            tsSep4.Size = new Size(6, 29);
            // 
            // tscbShowCustomSettingNamesOnly
            // 
            tscbShowCustomSettingNamesOnly.CheckOnClick = true;
            tscbShowCustomSettingNamesOnly.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tscbShowCustomSettingNamesOnly.Image = (Image)resources.GetObject("tscbShowCustomSettingNamesOnly.Image");
            tscbShowCustomSettingNamesOnly.ImageTransparentColor = Color.Magenta;
            tscbShowCustomSettingNamesOnly.Name = "tscbShowCustomSettingNamesOnly";
            tscbShowCustomSettingNamesOnly.Size = new Size(24, 26);
            tscbShowCustomSettingNamesOnly.Text = "Show the settings and values from CustomSettingNames file only.";
            tscbShowCustomSettingNamesOnly.CheckedChanged += cbCustomSettingsOnly_CheckedChanged;
            // 
            // tsSep5
            // 
            tsSep5.Name = "tsSep5";
            tsSep5.Size = new Size(6, 29);
            // 
            // tscbShowScannedUnknownSettings
            // 
            tscbShowScannedUnknownSettings.CheckOnClick = true;
            tscbShowScannedUnknownSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tscbShowScannedUnknownSettings.Enabled = false;
            tscbShowScannedUnknownSettings.Image = (Image)resources.GetObject("tscbShowScannedUnknownSettings.Image");
            tscbShowScannedUnknownSettings.ImageTransparentColor = Color.Magenta;
            tscbShowScannedUnknownSettings.Name = "tscbShowScannedUnknownSettings";
            tscbShowScannedUnknownSettings.Size = new Size(24, 26);
            tscbShowScannedUnknownSettings.Text = "Show unknown settings from NVIDIA predefined profiles";
            tscbShowScannedUnknownSettings.Click += tscbShowScannedUnknownSettings_Click;
            // 
            // tsbBitValueEditor
            // 
            tsbBitValueEditor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbBitValueEditor.Image = (Image)resources.GetObject("tsbBitValueEditor.Image");
            tsbBitValueEditor.ImageTransparentColor = Color.Magenta;
            tsbBitValueEditor.Name = "tsbBitValueEditor";
            tsbBitValueEditor.Size = new Size(24, 26);
            tsbBitValueEditor.Text = "Show bit value editor.";
            tsbBitValueEditor.Click += tsbBitValueEditor_Click;
            // 
            // tsSep6
            // 
            tsSep6.Name = "tsSep6";
            tsSep6.Size = new Size(6, 29);
            // 
            // tsbApplyProfile
            // 
            tsbApplyProfile.Alignment = ToolStripItemAlignment.Right;
            tsbApplyProfile.Image = (Image)resources.GetObject("tsbApplyProfile.Image");
            tsbApplyProfile.ImageTransparentColor = Color.Magenta;
            tsbApplyProfile.Name = "tsbApplyProfile";
            tsbApplyProfile.Overflow = ToolStripItemOverflow.Never;
            tsbApplyProfile.Size = new Size(109, 26);
            tsbApplyProfile.Text = "Apply changes";
            tsbApplyProfile.TextAlign = ContentAlignment.MiddleRight;
            tsbApplyProfile.Click += tsbApplyProfile_Click;
            // 
            // btnResetValue
            // 
            btnResetValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResetValue.Enabled = false;
            btnResetValue.Image = (Image)resources.GetObject("btnResetValue.Image");
            btnResetValue.Location = new Point(854, 202);
            btnResetValue.Margin = new Padding(0, 1, 0, 0);
            btnResetValue.Name = "btnResetValue";
            btnResetValue.Size = new Size(29, 22);
            btnResetValue.TabIndex = 7;
            btnResetValue.UseVisualStyleBackColor = true;
            btnResetValue.Click += btnResetValue_Click;
            // 
            // lblApplications
            // 
            lblApplications.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblApplications.BackColor = Color.FromArgb(118, 185, 0);
            lblApplications.BorderStyle = BorderStyle.Fixed3D;
            lblApplications.ForeColor = Color.White;
            lblApplications.Location = new Point(14, 37);
            lblApplications.Margin = new Padding(5, 0, 5, 0);
            lblApplications.Name = "lblApplications";
            lblApplications.Size = new Size(980, 20);
            lblApplications.TabIndex = 25;
            lblApplications.Text = "fsagame.exe, bond.exe, herozero.exe";
            // 
            // toolStripButton5
            // 
            toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton5.Image = (Image)resources.GetObject("toolStripButton5.Image");
            toolStripButton5.ImageTransparentColor = Color.Magenta;
            toolStripButton5.Name = "toolStripButton5";
            toolStripButton5.Size = new Size(23, 22);
            toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(86, 22);
            toolStripLabel2.Text = "toolStripLabel2";
            // 
            // toolStripButton6
            // 
            toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton6.Image = (Image)resources.GetObject("toolStripButton6.Image");
            toolStripButton6.ImageTransparentColor = Color.Magenta;
            toolStripButton6.Name = "toolStripButton6";
            toolStripButton6.Size = new Size(23, 22);
            toolStripButton6.Text = "toolStripButton6";
            // 
            // ilCombo
            // 
            ilCombo.ColorDepth = ColorDepth.Depth8Bit;
            ilCombo.ImageSize = new Size(16, 16);
            ilCombo.TransparentColor = Color.Transparent;
            // 
            // cbValues
            // 
            cbValues.BackColor = SystemColors.Window;
            cbValues.FormattingEnabled = true;
            cbValues.Location = new Point(611, 202);
            cbValues.Margin = new Padding(5, 0, 5, 0);
            cbValues.Name = "cbValues";
            cbValues.Size = new Size(83, 23);
            cbValues.TabIndex = 5;
            cbValues.Visible = false;
            cbValues.SelectedValueChanged += cbValues_SelectedValueChanged;
            cbValues.Leave += cbValues_Leave;
            // 
            // lblWidth96
            // 
            lblWidth96.Location = new Point(90, 269);
            lblWidth96.Margin = new Padding(5, 0, 5, 0);
            lblWidth96.Name = "lblWidth96";
            lblWidth96.Size = new Size(112, 21);
            lblWidth96.TabIndex = 77;
            lblWidth96.Text = "96";
            lblWidth96.Visible = false;
            // 
            // lblWidth330
            // 
            lblWidth330.Location = new Point(90, 242);
            lblWidth330.Margin = new Padding(5, 0, 5, 0);
            lblWidth330.Name = "lblWidth330";
            lblWidth330.Size = new Size(385, 25);
            lblWidth330.TabIndex = 78;
            lblWidth330.Text = "330 (Helper Labels for DPI Scaling)";
            lblWidth330.Visible = false;
            // 
            // lblWidth16
            // 
            lblWidth16.Location = new Point(90, 310);
            lblWidth16.Margin = new Padding(5, 0, 5, 0);
            lblWidth16.Name = "lblWidth16";
            lblWidth16.Size = new Size(19, 21);
            lblWidth16.TabIndex = 79;
            lblWidth16.Text = "16";
            lblWidth16.Visible = false;
            // 
            // lblWidth30
            // 
            lblWidth30.Location = new Point(90, 290);
            lblWidth30.Margin = new Padding(5, 0, 5, 0);
            lblWidth30.Name = "lblWidth30";
            lblWidth30.Size = new Size(35, 21);
            lblWidth30.TabIndex = 80;
            lblWidth30.Text = "30";
            lblWidth30.Visible = false;
            // 
            // lvSettings
            // 
            lvSettings.Columns.AddRange(new ColumnHeader[] { chSettingID, chSettingValue, chSettingValueHex });
            lvSettings.Dock = DockStyle.Fill;
            lvSettings.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lvSettings.FullRowSelect = true;
            lvSettings.GridLines = true;
            lvSettings.HeaderStyle = ColumnHeaderStyle.None;
            lvSettings.Location = new Point(0, 0);
            lvSettings.Margin = new Padding(5, 5, 5, 5);
            lvSettings.MultiSelect = false;
            lvSettings.Name = "lvSettings";
            lvSettings.ShowItemToolTips = true;
            lvSettings.Size = new Size(980, 430);
            lvSettings.SmallImageList = ilListView;
            lvSettings.TabIndex = 2;
            lvSettings.UseCompatibleStateImageBehavior = false;
            lvSettings.View = View.Details;
            lvSettings.ColumnWidthChanging += lvSettings_ColumnWidthChanging;
            lvSettings.SelectedIndexChanged += lvSettings_SelectedIndexChanged;
            lvSettings.DoubleClick += lvSettings_DoubleClick;
            lvSettings.KeyDown += lvSettings_KeyDown;
            lvSettings.Resize += lvSettings_Resize;
            // 
            // chSettingID
            // 
            chSettingID.Text = "SettingID";
            chSettingID.Width = 330;
            // 
            // chSettingValue
            // 
            chSettingValue.Text = "SettingValue";
            chSettingValue.Width = 340;
            // 
            // chSettingValueHex
            // 
            chSettingValueHex.Text = "SettingValueHex";
            chSettingValueHex.Width = 96;
            // 
            // tbSettingDescription
            // 
            tbSettingDescription.Dock = DockStyle.Bottom;
            tbSettingDescription.Location = new Point(0, 430);
            tbSettingDescription.Margin = new Padding(4, 3, 4, 3);
            tbSettingDescription.Multiline = true;
            tbSettingDescription.Name = "tbSettingDescription";
            tbSettingDescription.ReadOnly = true;
            tbSettingDescription.ScrollBars = ScrollBars.Vertical;
            tbSettingDescription.Size = new Size(980, 50);
            tbSettingDescription.TabIndex = 81;
            tbSettingDescription.Visible = false;
            // 
            // pnlListview
            // 
            pnlListview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlListview.Controls.Add(lvSettings);
            pnlListview.Controls.Add(tbSettingDescription);
            pnlListview.Location = new Point(14, 60);
            pnlListview.Margin = new Padding(4, 3, 4, 3);
            pnlListview.Name = "pnlListview";
            pnlListview.Size = new Size(980, 480);
            pnlListview.TabIndex = 82;
            // 
            // frmDrvSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 568);
            Controls.Add(pnlListview);
            Controls.Add(lblWidth30);
            Controls.Add(lblWidth16);
            Controls.Add(lblWidth330);
            Controls.Add(lblWidth96);
            Controls.Add(lblApplications);
            Controls.Add(tsMain);
            Controls.Add(pbMain);
            Controls.Add(btnResetValue);
            Controls.Add(cbValues);
            Margin = new Padding(5, 5, 5, 5);
            MinimumSize = new Size(1023, 393);
            Name = "frmDrvSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "nSpector - Driver Profile Settings";
            Activated += frmDrvSettings_Activated;
            FormClosed += frmDrvSettings_FormClosed;
            Load += frmDrvSettings_Load;
            Shown += frmDrvSettings_Shown;
            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            pnlListview.ResumeLayout(false);
            pnlListview.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ListViewEx lvSettings;
        private ColumnHeader chSettingID;
        private ColumnHeader chSettingValue;
        private ColumnHeader chSettingValueHex;
        private ImageList ilListView;
        private ComboBox cbValues;
        private Button btnResetValue;
        private ProgressBar pbMain;
        private ToolStrip tsMain;
        private ToolStripButton tsbRestoreProfile;
        private ToolStripButton tsbApplyProfile;
        private ToolStripButton tsbRefreshProfile;
        private ToolStripSeparator tsSep3;
        private ToolStripButton tsbBitValueEditor;
        private ToolStripSeparator tsSep6;
        private ToolStripButton tscbShowCustomSettingNamesOnly;
        private ToolStripSeparator tsSep5;
        private ToolStripButton tscbShowScannedUnknownSettings;
        private ToolStripLabel tslProfiles;
        private Label lblApplications;
        private ToolStripButton toolStripButton5;
        private ToolStripLabel toolStripLabel2;
        private ToolStripButton toolStripButton6;
        private ToolStripSeparator tsSep2;
        private ToolStripButton tsbDeleteProfile;
        private ToolStripButton tsbCreateProfile;
        private ToolStripButton tsbAddApplication;
        private ToolStripSplitButton tssbRemoveApplication;
        private ToolStripSeparator tsSep4;
        private ToolStripSplitButton tsbExportProfiles;
        private ToolStripMenuItem exportCurrentProfileOnlyToolStripMenuItem;
        private ToolStripMenuItem exportUserdefinedProfilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        public ToolStripComboBox cbProfiles;
        private ToolStripSplitButton tsbModifiedProfiles;
        private ImageList ilCombo;
        private ToolStripMenuItem exportAllProfilesNVIDIATextFormatToolStripMenuItem;
        private ToolStripSplitButton tsbImportProfiles;
        private ToolStripMenuItem importProfilesToolStripMenuItem;
        private ToolStripMenuItem importAllProfilesNVIDIATextFormatToolStripMenuItem;
        private Label lblWidth96;
        private Label lblWidth330;
        private Label lblWidth16;
        private Label lblWidth30;
        private ToolStripMenuItem exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem;
        private TextBox tbSettingDescription;
        private Panel pnlListview;
    }
}
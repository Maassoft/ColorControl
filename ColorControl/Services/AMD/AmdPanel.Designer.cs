namespace ColorControl.Services.AMD
{
    partial class AmdPanel
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
            this.components = new System.ComponentModel.Container();
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
            this.mnuAmdPresets.SuspendLayout();
            this.SuspendLayout();
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
            this.chkAmdQuickAccess.Enabled = false;
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
            // AmdPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAmdSettings);
            this.Controls.Add(this.chkAmdQuickAccess);
            this.Controls.Add(this.lblAmdPresetName);
            this.Controls.Add(this.edtAmdPresetName);
            this.Controls.Add(this.btnAddAmd);
            this.Controls.Add(this.lblErrorAMD);
            this.Controls.Add(this.btnDeleteAmd);
            this.Controls.Add(this.btnCloneAmd);
            this.Controls.Add(this.btnAmdPresetSave);
            this.Controls.Add(this.lblAmdShortcut);
            this.Controls.Add(this.edtAmdShortcut);
            this.Controls.Add(this.btnChangeAmd);
            this.Controls.Add(this.btnApplyAmd);
            this.Controls.Add(this.lvAmdPresets);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AmdPanel";
            this.Size = new System.Drawing.Size(1114, 539);
            this.mnuAmdPresets.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblErrorAMD;
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
        private System.Windows.Forms.Label lblAmdPresetName;
        private System.Windows.Forms.TextBox edtAmdPresetName;
        private System.Windows.Forms.ToolStripMenuItem miAmdCopyId;
        private System.Windows.Forms.CheckBox chkAmdQuickAccess;
        private System.Windows.Forms.Button btnAmdSettings;

    }
}

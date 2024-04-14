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
            components = new System.ComponentModel.Container();
            btnAmdSettings = new System.Windows.Forms.Button();
            chkAmdQuickAccess = new System.Windows.Forms.CheckBox();
            lblAmdPresetName = new System.Windows.Forms.Label();
            edtAmdPresetName = new System.Windows.Forms.TextBox();
            btnAddAmd = new System.Windows.Forms.Button();
            lblErrorAMD = new System.Windows.Forms.Label();
            btnDeleteAmd = new System.Windows.Forms.Button();
            btnCloneAmd = new System.Windows.Forms.Button();
            btnAmdPresetSave = new System.Windows.Forms.Button();
            lblAmdShortcut = new System.Windows.Forms.Label();
            edtAmdShortcut = new System.Windows.Forms.TextBox();
            btnChangeAmd = new System.Windows.Forms.Button();
            mnuAmdPresets = new System.Windows.Forms.ContextMenuStrip(components);
            miAmdApply = new System.Windows.Forms.ToolStripMenuItem();
            miAmdPresetApplyOnStartup = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            mnuAmdDisplay = new System.Windows.Forms.ToolStripMenuItem();
            miAmdPrimaryDisplay = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            miAmdColorSettingsIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdRefreshRate = new System.Windows.Forms.ToolStripMenuItem();
            miAmdRefreshRateIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdResolution = new System.Windows.Forms.ToolStripMenuItem();
            miAmdResolutionIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdDithering = new System.Windows.Forms.ToolStripMenuItem();
            miAmdDitheringIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdHDR = new System.Windows.Forms.ToolStripMenuItem();
            miAmdHDRIncluded = new System.Windows.Forms.ToolStripMenuItem();
            miAmdHDRToggle = new System.Windows.Forms.ToolStripMenuItem();
            miAmdHDREnabled = new System.Windows.Forms.ToolStripMenuItem();
            miAmdCopyId = new System.Windows.Forms.ToolStripMenuItem();
            btnApplyAmd = new System.Windows.Forms.Button();
            lvAmdPresets = new System.Windows.Forms.ListView();
            miAmdActiveResolution = new System.Windows.Forms.ToolStripMenuItem();
            miAmdVirtualResolution = new System.Windows.Forms.ToolStripMenuItem();
            mnuAmdPresets.SuspendLayout();
            SuspendLayout();
            // 
            // btnAmdSettings
            // 
            btnAmdSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnAmdSettings.Location = new System.Drawing.Point(1017, 429);
            btnAmdSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAmdSettings.Name = "btnAmdSettings";
            btnAmdSettings.Size = new System.Drawing.Size(88, 27);
            btnAmdSettings.TabIndex = 60;
            btnAmdSettings.Text = "Settings...";
            btnAmdSettings.UseVisualStyleBackColor = true;
            btnAmdSettings.Click += btnAmdSettings_Click;
            // 
            // chkAmdQuickAccess
            // 
            chkAmdQuickAccess.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkAmdQuickAccess.AutoSize = true;
            chkAmdQuickAccess.Enabled = false;
            chkAmdQuickAccess.Location = new System.Drawing.Point(343, 465);
            chkAmdQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkAmdQuickAccess.Name = "chkAmdQuickAccess";
            chkAmdQuickAccess.Size = new System.Drawing.Size(141, 19);
            chkAmdQuickAccess.TabIndex = 46;
            chkAmdQuickAccess.Text = "Show in Quick Access";
            chkAmdQuickAccess.UseVisualStyleBackColor = true;
            // 
            // lblAmdPresetName
            // 
            lblAmdPresetName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblAmdPresetName.AutoSize = true;
            lblAmdPresetName.Location = new System.Drawing.Point(7, 466);
            lblAmdPresetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAmdPresetName.Name = "lblAmdPresetName";
            lblAmdPresetName.Size = new System.Drawing.Size(42, 15);
            lblAmdPresetName.TabIndex = 24;
            lblAmdPresetName.Text = "Name:";
            // 
            // edtAmdPresetName
            // 
            edtAmdPresetName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtAmdPresetName.Enabled = false;
            edtAmdPresetName.Location = new System.Drawing.Point(102, 463);
            edtAmdPresetName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtAmdPresetName.Name = "edtAmdPresetName";
            edtAmdPresetName.Size = new System.Drawing.Size(233, 23);
            edtAmdPresetName.TabIndex = 23;
            // 
            // btnAddAmd
            // 
            btnAddAmd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAddAmd.Location = new System.Drawing.Point(385, 429);
            btnAddAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddAmd.Name = "btnAddAmd";
            btnAddAmd.Size = new System.Drawing.Size(88, 27);
            btnAddAmd.TabIndex = 18;
            btnAddAmd.Text = "Add";
            btnAddAmd.UseVisualStyleBackColor = true;
            btnAddAmd.Click += btnAddAmd_Click;
            // 
            // lblErrorAMD
            // 
            lblErrorAMD.AutoSize = true;
            lblErrorAMD.Location = new System.Drawing.Point(7, 7);
            lblErrorAMD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblErrorAMD.Name = "lblErrorAMD";
            lblErrorAMD.Size = new System.Drawing.Size(53, 15);
            lblErrorAMD.TabIndex = 8;
            lblErrorAMD.Text = "ErrorText";
            lblErrorAMD.Visible = false;
            // 
            // btnDeleteAmd
            // 
            btnDeleteAmd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnDeleteAmd.Enabled = false;
            btnDeleteAmd.Location = new System.Drawing.Point(290, 429);
            btnDeleteAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDeleteAmd.Name = "btnDeleteAmd";
            btnDeleteAmd.Size = new System.Drawing.Size(88, 27);
            btnDeleteAmd.TabIndex = 17;
            btnDeleteAmd.Text = "Delete";
            btnDeleteAmd.UseVisualStyleBackColor = true;
            btnDeleteAmd.Click += btnDeleteAmd_Click;
            // 
            // btnCloneAmd
            // 
            btnCloneAmd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnCloneAmd.Enabled = false;
            btnCloneAmd.Location = new System.Drawing.Point(196, 429);
            btnCloneAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCloneAmd.Name = "btnCloneAmd";
            btnCloneAmd.Size = new System.Drawing.Size(88, 27);
            btnCloneAmd.TabIndex = 16;
            btnCloneAmd.Text = "Clone";
            btnCloneAmd.UseVisualStyleBackColor = true;
            btnCloneAmd.Click += btnCloneAmd_Click;
            // 
            // btnAmdPresetSave
            // 
            btnAmdPresetSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAmdPresetSave.Enabled = false;
            btnAmdPresetSave.Location = new System.Drawing.Point(479, 429);
            btnAmdPresetSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAmdPresetSave.Name = "btnAmdPresetSave";
            btnAmdPresetSave.Size = new System.Drawing.Size(88, 27);
            btnAmdPresetSave.TabIndex = 15;
            btnAmdPresetSave.Text = "Save";
            btnAmdPresetSave.UseVisualStyleBackColor = true;
            btnAmdPresetSave.Click += btnSetAmdShortcut_Click;
            // 
            // lblAmdShortcut
            // 
            lblAmdShortcut.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblAmdShortcut.AutoSize = true;
            lblAmdShortcut.Location = new System.Drawing.Point(7, 496);
            lblAmdShortcut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAmdShortcut.Name = "lblAmdShortcut";
            lblAmdShortcut.Size = new System.Drawing.Size(55, 15);
            lblAmdShortcut.TabIndex = 14;
            lblAmdShortcut.Text = "Shortcut:";
            // 
            // edtAmdShortcut
            // 
            edtAmdShortcut.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtAmdShortcut.Enabled = false;
            edtAmdShortcut.Location = new System.Drawing.Point(102, 493);
            edtAmdShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtAmdShortcut.Name = "edtAmdShortcut";
            edtAmdShortcut.ReadOnly = true;
            edtAmdShortcut.Size = new System.Drawing.Size(233, 23);
            edtAmdShortcut.TabIndex = 13;
            edtAmdShortcut.TextChanged += edtAmdShortcut_TextChanged;
            edtAmdShortcut.KeyDown += edtShortcut_KeyDown;
            edtAmdShortcut.KeyUp += edtShortcut_KeyUp;
            // 
            // btnChangeAmd
            // 
            btnChangeAmd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnChangeAmd.ContextMenuStrip = mnuAmdPresets;
            btnChangeAmd.Enabled = false;
            btnChangeAmd.Location = new System.Drawing.Point(102, 429);
            btnChangeAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnChangeAmd.Name = "btnChangeAmd";
            btnChangeAmd.Size = new System.Drawing.Size(88, 27);
            btnChangeAmd.TabIndex = 12;
            btnChangeAmd.Text = "Change...";
            btnChangeAmd.UseVisualStyleBackColor = true;
            btnChangeAmd.Click += btnChangeAmd_Click;
            // 
            // mnuAmdPresets
            // 
            mnuAmdPresets.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuAmdPresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdApply, miAmdPresetApplyOnStartup, toolStripSeparator2, mnuAmdDisplay, mnuAmdColorSettings, mnuAmdRefreshRate, mnuAmdResolution, mnuAmdDithering, mnuAmdHDR, miAmdCopyId });
            mnuAmdPresets.Name = "mnuNvPresets";
            mnuAmdPresets.Size = new System.Drawing.Size(185, 208);
            mnuAmdPresets.Opening += mnuAmdPresets_Opening;
            // 
            // miAmdApply
            // 
            miAmdApply.Name = "miAmdApply";
            miAmdApply.Size = new System.Drawing.Size(184, 22);
            miAmdApply.Text = "Apply";
            miAmdApply.Click += btnApplyAmd_Click;
            // 
            // miAmdPresetApplyOnStartup
            // 
            miAmdPresetApplyOnStartup.CheckOnClick = true;
            miAmdPresetApplyOnStartup.Name = "miAmdPresetApplyOnStartup";
            miAmdPresetApplyOnStartup.Size = new System.Drawing.Size(184, 22);
            miAmdPresetApplyOnStartup.Text = "Apply on startup";
            miAmdPresetApplyOnStartup.Click += miAmdPresetApplyOnStartup_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuAmdDisplay
            // 
            mnuAmdDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdPrimaryDisplay });
            mnuAmdDisplay.Name = "mnuAmdDisplay";
            mnuAmdDisplay.Size = new System.Drawing.Size(184, 22);
            mnuAmdDisplay.Text = "Display";
            // 
            // miAmdPrimaryDisplay
            // 
            miAmdPrimaryDisplay.Name = "miAmdPrimaryDisplay";
            miAmdPrimaryDisplay.Size = new System.Drawing.Size(155, 22);
            miAmdPrimaryDisplay.Text = "Primary display";
            miAmdPrimaryDisplay.Click += displayMenuItemAmd_Click;
            // 
            // mnuAmdColorSettings
            // 
            mnuAmdColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdColorSettingsIncluded });
            mnuAmdColorSettings.Name = "mnuAmdColorSettings";
            mnuAmdColorSettings.Size = new System.Drawing.Size(184, 22);
            mnuAmdColorSettings.Text = "Color settings";
            // 
            // miAmdColorSettingsIncluded
            // 
            miAmdColorSettingsIncluded.Name = "miAmdColorSettingsIncluded";
            miAmdColorSettingsIncluded.Size = new System.Drawing.Size(120, 22);
            miAmdColorSettingsIncluded.Text = "Included";
            miAmdColorSettingsIncluded.Click += miAmdColorSettingsIncluded_Click;
            // 
            // mnuAmdRefreshRate
            // 
            mnuAmdRefreshRate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdRefreshRateIncluded });
            mnuAmdRefreshRate.Name = "mnuAmdRefreshRate";
            mnuAmdRefreshRate.Size = new System.Drawing.Size(184, 22);
            mnuAmdRefreshRate.Text = "Refresh Rate";
            // 
            // miAmdRefreshRateIncluded
            // 
            miAmdRefreshRateIncluded.Name = "miAmdRefreshRateIncluded";
            miAmdRefreshRateIncluded.Size = new System.Drawing.Size(180, 22);
            miAmdRefreshRateIncluded.Text = "Included";
            miAmdRefreshRateIncluded.Click += miAmdRefreshRateIncluded_Click;
            // 
            // mnuAmdResolution
            // 
            mnuAmdResolution.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdResolutionIncluded, miAmdActiveResolution, miAmdVirtualResolution });
            mnuAmdResolution.Name = "mnuAmdResolution";
            mnuAmdResolution.Size = new System.Drawing.Size(184, 22);
            mnuAmdResolution.Text = "Resolution";
            // 
            // miAmdResolutionIncluded
            // 
            miAmdResolutionIncluded.Name = "miAmdResolutionIncluded";
            miAmdResolutionIncluded.Size = new System.Drawing.Size(180, 22);
            miAmdResolutionIncluded.Text = "Included";
            miAmdResolutionIncluded.Click += miAmdResolutionIncluded_Click;
            // 
            // mnuAmdDithering
            // 
            mnuAmdDithering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdDitheringIncluded });
            mnuAmdDithering.Name = "mnuAmdDithering";
            mnuAmdDithering.Size = new System.Drawing.Size(184, 22);
            mnuAmdDithering.Text = "Dithering";
            // 
            // miAmdDitheringIncluded
            // 
            miAmdDitheringIncluded.Name = "miAmdDitheringIncluded";
            miAmdDitheringIncluded.Size = new System.Drawing.Size(120, 22);
            miAmdDitheringIncluded.Text = "Included";
            miAmdDitheringIncluded.Click += miAmdDitheringIncluded_Click;
            // 
            // mnuAmdHDR
            // 
            mnuAmdHDR.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miAmdHDRIncluded, miAmdHDRToggle, miAmdHDREnabled });
            mnuAmdHDR.Name = "mnuAmdHDR";
            mnuAmdHDR.Size = new System.Drawing.Size(184, 22);
            mnuAmdHDR.Text = "HDR";
            // 
            // miAmdHDRIncluded
            // 
            miAmdHDRIncluded.Name = "miAmdHDRIncluded";
            miAmdHDRIncluded.Size = new System.Drawing.Size(136, 22);
            miAmdHDRIncluded.Text = "Included";
            miAmdHDRIncluded.Click += miAmdHDRIncluded_Click;
            // 
            // miAmdHDRToggle
            // 
            miAmdHDRToggle.Name = "miAmdHDRToggle";
            miAmdHDRToggle.Size = new System.Drawing.Size(136, 22);
            miAmdHDRToggle.Text = "Toggle HDR";
            miAmdHDRToggle.Click += miAmdHDRToggle_Click;
            // 
            // miAmdHDREnabled
            // 
            miAmdHDREnabled.Name = "miAmdHDREnabled";
            miAmdHDREnabled.Size = new System.Drawing.Size(136, 22);
            miAmdHDREnabled.Text = "Enabled";
            miAmdHDREnabled.Click += miAmdHDREnabled_Click;
            // 
            // miAmdCopyId
            // 
            miAmdCopyId.Name = "miAmdCopyId";
            miAmdCopyId.Size = new System.Drawing.Size(184, 22);
            miAmdCopyId.Text = "Copy Id to Clipboard";
            miAmdCopyId.Click += miAmdCopyId_Click;
            // 
            // btnApplyAmd
            // 
            btnApplyAmd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnApplyAmd.Enabled = false;
            btnApplyAmd.Location = new System.Drawing.Point(7, 429);
            btnApplyAmd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnApplyAmd.Name = "btnApplyAmd";
            btnApplyAmd.Size = new System.Drawing.Size(88, 27);
            btnApplyAmd.TabIndex = 11;
            btnApplyAmd.Text = "Apply";
            btnApplyAmd.UseVisualStyleBackColor = true;
            btnApplyAmd.Click += btnApplyAmd_Click;
            // 
            // lvAmdPresets
            // 
            lvAmdPresets.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvAmdPresets.CheckBoxes = true;
            lvAmdPresets.ContextMenuStrip = mnuAmdPresets;
            lvAmdPresets.FullRowSelect = true;
            lvAmdPresets.Location = new System.Drawing.Point(7, 7);
            lvAmdPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvAmdPresets.MultiSelect = false;
            lvAmdPresets.Name = "lvAmdPresets";
            lvAmdPresets.ShowItemToolTips = true;
            lvAmdPresets.Size = new System.Drawing.Size(1098, 415);
            lvAmdPresets.TabIndex = 10;
            lvAmdPresets.UseCompatibleStateImageBehavior = false;
            lvAmdPresets.View = System.Windows.Forms.View.Details;
            lvAmdPresets.ColumnClick += lvLgPresets_ColumnClick;
            lvAmdPresets.ItemCheck += lvNvPresets_ItemCheck;
            lvAmdPresets.ItemChecked += lvAmdPresets_ItemChecked;
            lvAmdPresets.SelectedIndexChanged += lvAmdPresets_SelectedIndexChanged;
            lvAmdPresets.DoubleClick += btnApplyAmd_Click;
            // 
            // miAmdActiveResolution
            // 
            miAmdActiveResolution.Name = "miAmdActiveResolution";
            miAmdActiveResolution.Size = new System.Drawing.Size(180, 22);
            miAmdActiveResolution.Text = "Active";
            // 
            // miAmdVirtualResolution
            // 
            miAmdVirtualResolution.Name = "miAmdVirtualResolution";
            miAmdVirtualResolution.Size = new System.Drawing.Size(180, 22);
            miAmdVirtualResolution.Text = "Virtual";
            // 
            // AmdPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(btnAmdSettings);
            Controls.Add(chkAmdQuickAccess);
            Controls.Add(lblAmdPresetName);
            Controls.Add(edtAmdPresetName);
            Controls.Add(btnAddAmd);
            Controls.Add(lblErrorAMD);
            Controls.Add(btnDeleteAmd);
            Controls.Add(btnCloneAmd);
            Controls.Add(btnAmdPresetSave);
            Controls.Add(lblAmdShortcut);
            Controls.Add(edtAmdShortcut);
            Controls.Add(btnChangeAmd);
            Controls.Add(btnApplyAmd);
            Controls.Add(lvAmdPresets);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "AmdPanel";
            Size = new System.Drawing.Size(1114, 539);
            mnuAmdPresets.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem mnuAmdResolution;
        private System.Windows.Forms.ToolStripMenuItem miAmdResolutionIncluded;
        private System.Windows.Forms.ToolStripMenuItem miAmdActiveResolution;
        private System.Windows.Forms.ToolStripMenuItem miAmdVirtualResolution;
    }
}


namespace ColorControl.Services.NVIDIA
{
    partial class NvPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblNvOverclock = new System.Windows.Forms.Label();
            this.edtNvOverclock = new System.Windows.Forms.TextBox();
            this.btnNvSetClocks = new System.Windows.Forms.Button();
            this.lblNvGpuInfo = new System.Windows.Forms.Label();
            this.edtNvGpuInfo = new System.Windows.Forms.TextBox();
            this.lblNvGPU = new System.Windows.Forms.Label();
            this.cbxNvGPU = new System.Windows.Forms.ComboBox();
            this.btnNvSettings = new System.Windows.Forms.Button();
            this.chkNvShowInQuickAccess = new System.Windows.Forms.CheckBox();
            this.lblNvPresetName = new System.Windows.Forms.Label();
            this.edtNvPresetName = new System.Windows.Forms.TextBox();
            this.btnAddModesNv = new System.Windows.Forms.Button();
            this.btnNvPresetDelete = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.btnClone = new System.Windows.Forms.Button();
            this.btnNvPresetSave = new System.Windows.Forms.Button();
            this.lblShortcut = new System.Windows.Forms.Label();
            this.edtShortcut = new System.Windows.Forms.TextBox();
            this.btnChange = new System.Windows.Forms.Button();
            this.mnuNvPresets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miNvApply = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPresetApplyOnStartup = new System.Windows.Forms.ToolStripMenuItem();
            this.tssNvPresetMenu = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNvDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPrimaryDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvPresetsColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPresetColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefreshRate = new System.Windows.Forms.ToolStripMenuItem();
            this.miRefreshRateIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvResolution = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvResolutionIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPresetDithering = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPresetApplyDithering = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvPresetDitheringEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvDitheringBitDepth = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvDithering6bit = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvDithering8bit = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvDithering10bit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvDitheringMode = new System.Windows.Forms.ToolStripMenuItem();
            this.spatial1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spatial2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialDynamic2x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialStatic2x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temporalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvHDR = new System.Windows.Forms.ToolStripMenuItem();
            this.miHDRIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.miToggleHDR = new System.Windows.Forms.ToolStripMenuItem();
            this.miHDREnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvDriverSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvDriverSettingsIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNvOverclocking = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvOverclockingIncluded = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvCopyId = new System.Windows.Forms.ToolStripMenuItem();
            this.btnApply = new System.Windows.Forms.Button();
            this.lvNvPresets = new System.Windows.Forms.ListView();
            this.mnuNvSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miNvProfileInspector = new System.Windows.Forms.ToolStripMenuItem();
            this.miNvSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.lvNvPresetsToolTip = new System.Windows.Forms.ToolTip(this.components);

            this.mnuNvPresets.SuspendLayout();
            this.mnuNvSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // NvPanel
            // 

            this.Controls.Add(this.lblNvOverclock);
            this.Controls.Add(this.edtNvOverclock);
            this.Controls.Add(this.btnNvSetClocks);
            this.Controls.Add(this.lblNvGpuInfo);
            this.Controls.Add(this.edtNvGpuInfo);
            this.Controls.Add(this.lblNvGPU);
            this.Controls.Add(this.cbxNvGPU);
            this.Controls.Add(this.btnNvSettings);
            this.Controls.Add(this.chkNvShowInQuickAccess);
            this.Controls.Add(this.lblNvPresetName);
            this.Controls.Add(this.edtNvPresetName);
            this.Controls.Add(this.btnAddModesNv);
            this.Controls.Add(this.btnNvPresetDelete);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnClone);
            this.Controls.Add(this.btnNvPresetSave);
            this.Controls.Add(this.lblShortcut);
            this.Controls.Add(this.edtShortcut);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lvNvPresets);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "NvPanel";
            this.Size = new System.Drawing.Size(1114, 539);
            this.Load += new System.EventHandler(this.RemoteControlPanel_Load);

            // 
            // lblNvOverclock
            // 
            this.lblNvOverclock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNvOverclock.AutoSize = true;
            this.lblNvOverclock.Location = new System.Drawing.Point(578, 496);
            this.lblNvOverclock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNvOverclock.Name = "lblNvOverclock";
            this.lblNvOverclock.Size = new System.Drawing.Size(51, 15);
            this.lblNvOverclock.TabIndex = 66;
            this.lblNvOverclock.Text = "OC info:";
            // 
            // edtNvOverclock
            // 
            this.edtNvOverclock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtNvOverclock.Enabled = false;
            this.edtNvOverclock.Location = new System.Drawing.Point(648, 493);
            this.edtNvOverclock.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtNvOverclock.Name = "edtNvOverclock";
            this.edtNvOverclock.Size = new System.Drawing.Size(457, 23);
            this.edtNvOverclock.TabIndex = 65;
            // 
            // btnNvSetClocks
            // 
            this.btnNvSetClocks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNvSetClocks.Location = new System.Drawing.Point(868, 433);
            this.btnNvSetClocks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNvSetClocks.Name = "btnNvSetClocks";
            this.btnNvSetClocks.Size = new System.Drawing.Size(88, 27);
            this.btnNvSetClocks.TabIndex = 64;
            this.btnNvSetClocks.Text = "Change...";
            this.btnNvSetClocks.UseVisualStyleBackColor = true;
            this.btnNvSetClocks.Click += new System.EventHandler(this.btnNvSetClocks_Click);
            // 
            // lblNvGpuInfo
            // 
            this.lblNvGpuInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNvGpuInfo.AutoSize = true;
            this.lblNvGpuInfo.Location = new System.Drawing.Point(578, 469);
            this.lblNvGpuInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNvGpuInfo.Name = "lblNvGpuInfo";
            this.lblNvGpuInfo.Size = new System.Drawing.Size(31, 15);
            this.lblNvGpuInfo.TabIndex = 63;
            this.lblNvGpuInfo.Text = "Info:";
            // 
            // edtNvGpuInfo
            // 
            this.edtNvGpuInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtNvGpuInfo.Enabled = false;
            this.edtNvGpuInfo.Location = new System.Drawing.Point(648, 466);
            this.edtNvGpuInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtNvGpuInfo.Name = "edtNvGpuInfo";
            this.edtNvGpuInfo.Size = new System.Drawing.Size(457, 23);
            this.edtNvGpuInfo.TabIndex = 62;
            // 
            // lblNvGPU
            // 
            this.lblNvGPU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNvGPU.AutoSize = true;
            this.lblNvGPU.Location = new System.Drawing.Point(578, 439);
            this.lblNvGPU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNvGPU.Name = "lblNvGPU";
            this.lblNvGPU.Size = new System.Drawing.Size(33, 15);
            this.lblNvGPU.TabIndex = 61;
            this.lblNvGPU.Text = "GPU:";
            // 
            // cbxNvGPU
            // 
            this.cbxNvGPU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxNvGPU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNvGPU.FormattingEnabled = true;
            this.cbxNvGPU.Location = new System.Drawing.Point(648, 436);
            this.cbxNvGPU.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxNvGPU.Name = "cbxNvGPU";
            this.cbxNvGPU.Size = new System.Drawing.Size(212, 23);
            this.cbxNvGPU.TabIndex = 60;
            this.cbxNvGPU.SelectedIndexChanged += new System.EventHandler(this.cbxNvGPU_SelectedIndexChanged);
            // 
            // btnNvSettings
            // 
            this.btnNvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNvSettings.Location = new System.Drawing.Point(1017, 428);
            this.btnNvSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNvSettings.Name = "btnNvSettings";
            this.btnNvSettings.Size = new System.Drawing.Size(88, 27);
            this.btnNvSettings.TabIndex = 59;
            this.btnNvSettings.Text = "Settings...";
            this.btnNvSettings.UseVisualStyleBackColor = true;
            this.btnNvSettings.Click += new System.EventHandler(this.btnNvSettings_Click);
            // 
            // chkNvShowInQuickAccess
            // 
            this.chkNvShowInQuickAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkNvShowInQuickAccess.AutoSize = true;
            this.chkNvShowInQuickAccess.Enabled = false;
            this.chkNvShowInQuickAccess.Location = new System.Drawing.Point(343, 465);
            this.chkNvShowInQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkNvShowInQuickAccess.Name = "chkNvShowInQuickAccess";
            this.chkNvShowInQuickAccess.Size = new System.Drawing.Size(141, 19);
            this.chkNvShowInQuickAccess.TabIndex = 45;
            this.chkNvShowInQuickAccess.Text = "Show in Quick Access";
            this.chkNvShowInQuickAccess.UseVisualStyleBackColor = true;
            // 
            // lblNvPresetName
            // 
            this.lblNvPresetName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNvPresetName.AutoSize = true;
            this.lblNvPresetName.Location = new System.Drawing.Point(7, 466);
            this.lblNvPresetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNvPresetName.Name = "lblNvPresetName";
            this.lblNvPresetName.Size = new System.Drawing.Size(42, 15);
            this.lblNvPresetName.TabIndex = 22;
            this.lblNvPresetName.Text = "Name:";
            // 
            // edtNvPresetName
            // 
            this.edtNvPresetName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtNvPresetName.Enabled = false;
            this.edtNvPresetName.Location = new System.Drawing.Point(102, 463);
            this.edtNvPresetName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtNvPresetName.Name = "edtNvPresetName";
            this.edtNvPresetName.Size = new System.Drawing.Size(233, 23);
            this.edtNvPresetName.TabIndex = 21;
            // 
            // btnAddModesNv
            // 
            this.btnAddModesNv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddModesNv.Location = new System.Drawing.Point(385, 429);
            this.btnAddModesNv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddModesNv.Name = "btnAddModesNv";
            this.btnAddModesNv.Size = new System.Drawing.Size(88, 27);
            this.btnAddModesNv.TabIndex = 9;
            this.btnAddModesNv.Text = "Add modes";
            this.btnAddModesNv.UseVisualStyleBackColor = true;
            this.btnAddModesNv.Click += new System.EventHandler(this.btnAddModesNv_Click);
            // 
            // btnNvPresetDelete
            // 
            this.btnNvPresetDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNvPresetDelete.Enabled = false;
            this.btnNvPresetDelete.Location = new System.Drawing.Point(290, 429);
            this.btnNvPresetDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNvPresetDelete.Name = "btnNvPresetDelete";
            this.btnNvPresetDelete.Size = new System.Drawing.Size(88, 27);
            this.btnNvPresetDelete.TabIndex = 8;
            this.btnNvPresetDelete.Text = "Delete";
            this.btnNvPresetDelete.UseVisualStyleBackColor = true;
            this.btnNvPresetDelete.Click += new System.EventHandler(this.btnNvPresetDelete_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(7, 7);
            this.lblError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(53, 15);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "ErrorText";
            this.lblError.Visible = false;
            // 
            // btnClone
            // 
            this.btnClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClone.Enabled = false;
            this.btnClone.Location = new System.Drawing.Point(196, 429);
            this.btnClone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(88, 27);
            this.btnClone.TabIndex = 6;
            this.btnClone.Text = "Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // btnNvPresetSave
            // 
            this.btnNvPresetSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNvPresetSave.Enabled = false;
            this.btnNvPresetSave.Location = new System.Drawing.Point(479, 429);
            this.btnNvPresetSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNvPresetSave.Name = "btnNvPresetSave";
            this.btnNvPresetSave.Size = new System.Drawing.Size(88, 27);
            this.btnNvPresetSave.TabIndex = 5;
            this.btnNvPresetSave.Text = "Save";
            this.btnNvPresetSave.UseVisualStyleBackColor = true;
            this.btnNvPresetSave.Click += new System.EventHandler(this.btnSetShortcut_Click);
            // 
            // lblShortcut
            // 
            this.lblShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblShortcut.AutoSize = true;
            this.lblShortcut.Location = new System.Drawing.Point(7, 496);
            this.lblShortcut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShortcut.Name = "lblShortcut";
            this.lblShortcut.Size = new System.Drawing.Size(55, 15);
            this.lblShortcut.TabIndex = 4;
            this.lblShortcut.Text = "Shortcut:";
            // 
            // edtShortcut
            // 
            this.edtShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtShortcut.Enabled = false;
            this.edtShortcut.Location = new System.Drawing.Point(102, 493);
            this.edtShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtShortcut.Name = "edtShortcut";
            this.edtShortcut.ReadOnly = true;
            this.edtShortcut.Size = new System.Drawing.Size(233, 23);
            this.edtShortcut.TabIndex = 3;
            this.edtShortcut.TextChanged += new System.EventHandler(this.edtShortcut_TextChanged);
            this.edtShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyDown);
            this.edtShortcut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtShortcut_KeyUp);
            // 
            // btnChange
            // 
            this.btnChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChange.ContextMenuStrip = this.mnuNvPresets;
            this.btnChange.Location = new System.Drawing.Point(102, 429);
            this.btnChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(88, 27);
            this.btnChange.TabIndex = 2;
            this.btnChange.Text = "Change...";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // mnuNvPresets
            // 
            this.mnuNvPresets.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuNvPresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvApply,
            this.miNvPresetApplyOnStartup,
            this.tssNvPresetMenu,
            this.mnuNvDisplay,
            this.mnuNvPresetsColorSettings,
            this.mnuRefreshRate,
            this.mnuNvResolution,
            this.miNvPresetDithering,
            this.miNvHDR,
            this.mnuNvDriverSettings,
            this.mnuNvOverclocking,
            this.miNvCopyId});
            this.mnuNvPresets.Name = "mnuNvPresets";
            this.mnuNvPresets.Size = new System.Drawing.Size(185, 252);
            this.mnuNvPresets.Opening += new System.ComponentModel.CancelEventHandler(this.mnuNvPresets_Opening);
            // 
            // miNvApply
            // 
            this.miNvApply.Name = "miNvApply";
            this.miNvApply.Size = new System.Drawing.Size(184, 22);
            this.miNvApply.Text = "Apply";
            this.miNvApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // miNvPresetApplyOnStartup
            // 
            this.miNvPresetApplyOnStartup.CheckOnClick = true;
            this.miNvPresetApplyOnStartup.Name = "miNvPresetApplyOnStartup";
            this.miNvPresetApplyOnStartup.Size = new System.Drawing.Size(184, 22);
            this.miNvPresetApplyOnStartup.Text = "Apply on startup";
            this.miNvPresetApplyOnStartup.Click += new System.EventHandler(this.miNvPresetApplyOnStartup_Click);
            // 
            // tssNvPresetMenu
            // 
            this.tssNvPresetMenu.Name = "tssNvPresetMenu";
            this.tssNvPresetMenu.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuNvDisplay
            // 
            this.mnuNvDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvPrimaryDisplay});
            this.mnuNvDisplay.Name = "mnuNvDisplay";
            this.mnuNvDisplay.Size = new System.Drawing.Size(184, 22);
            this.mnuNvDisplay.Text = "Display";
            // 
            // miNvPrimaryDisplay
            // 
            this.miNvPrimaryDisplay.Name = "miNvPrimaryDisplay";
            this.miNvPrimaryDisplay.Size = new System.Drawing.Size(155, 22);
            this.miNvPrimaryDisplay.Text = "Primary display";
            this.miNvPrimaryDisplay.Click += new System.EventHandler(this.displayMenuItem_Click);
            // 
            // mnuNvPresetsColorSettings
            // 
            this.mnuNvPresetsColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvPresetColorSettings});
            this.mnuNvPresetsColorSettings.Name = "mnuNvPresetsColorSettings";
            this.mnuNvPresetsColorSettings.Size = new System.Drawing.Size(184, 22);
            this.mnuNvPresetsColorSettings.Text = "Color Settings";
            // 
            // miNvPresetColorSettings
            // 
            this.miNvPresetColorSettings.Name = "miNvPresetColorSettings";
            this.miNvPresetColorSettings.Size = new System.Drawing.Size(120, 22);
            this.miNvPresetColorSettings.Text = "Included";
            this.miNvPresetColorSettings.Click += new System.EventHandler(this.miNvPresetColorSettings_Click);
            // 
            // mnuRefreshRate
            // 
            this.mnuRefreshRate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRefreshRateIncluded});
            this.mnuRefreshRate.Name = "mnuRefreshRate";
            this.mnuRefreshRate.Size = new System.Drawing.Size(184, 22);
            this.mnuRefreshRate.Text = "Refresh Rate";
            // 
            // miRefreshRateIncluded
            // 
            this.miRefreshRateIncluded.Name = "miRefreshRateIncluded";
            this.miRefreshRateIncluded.Size = new System.Drawing.Size(120, 22);
            this.miRefreshRateIncluded.Text = "Included";
            this.miRefreshRateIncluded.Click += new System.EventHandler(this.includedToolStripMenuItem_Click);
            // 
            // mnuNvResolution
            // 
            this.mnuNvResolution.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvResolutionIncluded});
            this.mnuNvResolution.Name = "mnuNvResolution";
            this.mnuNvResolution.Size = new System.Drawing.Size(184, 22);
            this.mnuNvResolution.Text = "Resolution";
            // 
            // miNvResolutionIncluded
            // 
            this.miNvResolutionIncluded.CheckOnClick = true;
            this.miNvResolutionIncluded.Name = "miNvResolutionIncluded";
            this.miNvResolutionIncluded.Size = new System.Drawing.Size(120, 22);
            this.miNvResolutionIncluded.Text = "Included";
            this.miNvResolutionIncluded.Click += new System.EventHandler(this.miNvResolutionIncluded_Click);
            // 
            // miNvPresetDithering
            // 
            this.miNvPresetDithering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvPresetApplyDithering,
            this.miNvPresetDitheringEnabled,
            this.mnuNvDitheringBitDepth,
            this.mnuNvDitheringMode});
            this.miNvPresetDithering.Name = "miNvPresetDithering";
            this.miNvPresetDithering.Size = new System.Drawing.Size(184, 22);
            this.miNvPresetDithering.Text = "Dithering";
            // 
            // miNvPresetApplyDithering
            // 
            this.miNvPresetApplyDithering.Name = "miNvPresetApplyDithering";
            this.miNvPresetApplyDithering.Size = new System.Drawing.Size(122, 22);
            this.miNvPresetApplyDithering.Text = "Included";
            this.miNvPresetApplyDithering.Click += new System.EventHandler(this.miNvPresetApplyDithering_Click);
            // 
            // miNvPresetDitheringEnabled
            // 
            this.miNvPresetDitheringEnabled.Name = "miNvPresetDitheringEnabled";
            this.miNvPresetDitheringEnabled.Size = new System.Drawing.Size(122, 22);
            this.miNvPresetDitheringEnabled.Text = "Enabled";
            this.miNvPresetDitheringEnabled.Click += new System.EventHandler(this.miNvPresetDitheringEnabled_Click);
            // 
            // mnuNvDitheringBitDepth
            // 
            this.mnuNvDitheringBitDepth.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvDithering6bit,
            this.miNvDithering8bit,
            this.miNvDithering10bit});
            this.mnuNvDitheringBitDepth.Name = "mnuNvDitheringBitDepth";
            this.mnuNvDitheringBitDepth.Size = new System.Drawing.Size(122, 22);
            this.mnuNvDitheringBitDepth.Text = "Bit depth";
            // 
            // miNvDithering6bit
            // 
            this.miNvDithering6bit.Name = "miNvDithering6bit";
            this.miNvDithering6bit.Size = new System.Drawing.Size(105, 22);
            this.miNvDithering6bit.Tag = "0";
            this.miNvDithering6bit.Text = "6-bit";
            this.miNvDithering6bit.Click += new System.EventHandler(this.miNvDithering6bit_Click);
            // 
            // miNvDithering8bit
            // 
            this.miNvDithering8bit.Name = "miNvDithering8bit";
            this.miNvDithering8bit.Size = new System.Drawing.Size(105, 22);
            this.miNvDithering8bit.Tag = "1";
            this.miNvDithering8bit.Text = "8-bit";
            this.miNvDithering8bit.Click += new System.EventHandler(this.miNvDithering6bit_Click);
            // 
            // miNvDithering10bit
            // 
            this.miNvDithering10bit.Name = "miNvDithering10bit";
            this.miNvDithering10bit.Size = new System.Drawing.Size(105, 22);
            this.miNvDithering10bit.Tag = "2";
            this.miNvDithering10bit.Text = "10-bit";
            this.miNvDithering10bit.Click += new System.EventHandler(this.miNvDithering6bit_Click);
            // 
            // mnuNvDitheringMode
            // 
            this.mnuNvDitheringMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spatial1ToolStripMenuItem,
            this.spatial2ToolStripMenuItem,
            this.spatialDynamic2x2ToolStripMenuItem,
            this.spatialStatic2x2ToolStripMenuItem,
            this.temporalToolStripMenuItem});
            this.mnuNvDitheringMode.Name = "mnuNvDitheringMode";
            this.mnuNvDitheringMode.Size = new System.Drawing.Size(122, 22);
            this.mnuNvDitheringMode.Text = "Mode";
            // 
            // spatial1ToolStripMenuItem
            // 
            this.spatial1ToolStripMenuItem.Name = "spatial1ToolStripMenuItem";
            this.spatial1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.spatial1ToolStripMenuItem.Tag = "0";
            this.spatial1ToolStripMenuItem.Text = "Spatial Dynamic";
            this.spatial1ToolStripMenuItem.Click += new System.EventHandler(this.spatial1ToolStripMenuItem_Click);
            // 
            // spatial2ToolStripMenuItem
            // 
            this.spatial2ToolStripMenuItem.Name = "spatial2ToolStripMenuItem";
            this.spatial2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.spatial2ToolStripMenuItem.Tag = "1";
            this.spatial2ToolStripMenuItem.Text = "Spatial Static";
            this.spatial2ToolStripMenuItem.Click += new System.EventHandler(this.spatial1ToolStripMenuItem_Click);
            // 
            // spatialDynamic2x2ToolStripMenuItem
            // 
            this.spatialDynamic2x2ToolStripMenuItem.Name = "spatialDynamic2x2ToolStripMenuItem";
            this.spatialDynamic2x2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.spatialDynamic2x2ToolStripMenuItem.Tag = "2";
            this.spatialDynamic2x2ToolStripMenuItem.Text = "Spatial Dynamic 2x2";
            this.spatialDynamic2x2ToolStripMenuItem.Click += new System.EventHandler(this.spatial1ToolStripMenuItem_Click);
            // 
            // spatialStatic2x2ToolStripMenuItem
            // 
            this.spatialStatic2x2ToolStripMenuItem.Name = "spatialStatic2x2ToolStripMenuItem";
            this.spatialStatic2x2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.spatialStatic2x2ToolStripMenuItem.Tag = "3";
            this.spatialStatic2x2ToolStripMenuItem.Text = "Spatial Static 2x2";
            this.spatialStatic2x2ToolStripMenuItem.Click += new System.EventHandler(this.spatial1ToolStripMenuItem_Click);
            // 
            // temporalToolStripMenuItem
            // 
            this.temporalToolStripMenuItem.Name = "temporalToolStripMenuItem";
            this.temporalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.temporalToolStripMenuItem.Tag = "4";
            this.temporalToolStripMenuItem.Text = "Temporal";
            this.temporalToolStripMenuItem.Click += new System.EventHandler(this.spatial1ToolStripMenuItem_Click);
            // 
            // miNvHDR
            // 
            this.miNvHDR.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHDRIncluded,
            this.miToggleHDR,
            this.miHDREnabled});
            this.miNvHDR.Name = "miNvHDR";
            this.miNvHDR.Size = new System.Drawing.Size(184, 22);
            this.miNvHDR.Text = "HDR";
            // 
            // miHDRIncluded
            // 
            this.miHDRIncluded.Name = "miHDRIncluded";
            this.miHDRIncluded.Size = new System.Drawing.Size(136, 22);
            this.miHDRIncluded.Text = "Included";
            this.miHDRIncluded.Click += new System.EventHandler(this.miHDRIncluded_Click);
            // 
            // miToggleHDR
            // 
            this.miToggleHDR.Name = "miToggleHDR";
            this.miToggleHDR.Size = new System.Drawing.Size(136, 22);
            this.miToggleHDR.Text = "Toggle HDR";
            this.miToggleHDR.Click += new System.EventHandler(this.miToggleHDR_Click);
            // 
            // miHDREnabled
            // 
            this.miHDREnabled.Name = "miHDREnabled";
            this.miHDREnabled.Size = new System.Drawing.Size(136, 22);
            this.miHDREnabled.Text = "Enabled";
            this.miHDREnabled.Click += new System.EventHandler(this.miHDREnabled_Click);
            // 
            // mnuNvDriverSettings
            // 
            this.mnuNvDriverSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvDriverSettingsIncluded});
            this.mnuNvDriverSettings.Name = "mnuNvDriverSettings";
            this.mnuNvDriverSettings.Size = new System.Drawing.Size(184, 22);
            this.mnuNvDriverSettings.Text = "Driver Settings";
            // 
            // miNvDriverSettingsIncluded
            // 
            this.miNvDriverSettingsIncluded.Name = "miNvDriverSettingsIncluded";
            this.miNvDriverSettingsIncluded.Size = new System.Drawing.Size(120, 22);
            this.miNvDriverSettingsIncluded.Text = "Included";
            this.miNvDriverSettingsIncluded.Click += new System.EventHandler(this.miNvDriverSettingsIncluded_Click);
            // 
            // mnuNvOverclocking
            // 
            this.mnuNvOverclocking.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvOverclockingIncluded});
            this.mnuNvOverclocking.Name = "mnuNvOverclocking";
            this.mnuNvOverclocking.Size = new System.Drawing.Size(184, 22);
            this.mnuNvOverclocking.Text = "Overclocking";
            // 
            // miNvOverclockingIncluded
            // 
            this.miNvOverclockingIncluded.Name = "miNvOverclockingIncluded";
            this.miNvOverclockingIncluded.Size = new System.Drawing.Size(120, 22);
            this.miNvOverclockingIncluded.Text = "Included";
            this.miNvOverclockingIncluded.Click += new System.EventHandler(this.miNvOverclockingIncluded_Click);
            // 
            // miNvCopyId
            // 
            this.miNvCopyId.Name = "miNvCopyId";
            this.miNvCopyId.Size = new System.Drawing.Size(184, 22);
            this.miNvCopyId.Text = "Copy Id to Clipboard";
            this.miNvCopyId.Click += new System.EventHandler(this.miNvCopyId_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(7, 429);
            this.btnApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(88, 27);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lvNvPresets
            // 
            this.lvNvPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvNvPresets.CheckBoxes = true;
            this.lvNvPresets.ContextMenuStrip = this.mnuNvPresets;
            this.lvNvPresets.FullRowSelect = true;
            this.lvNvPresets.Location = new System.Drawing.Point(7, 7);
            this.lvNvPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvNvPresets.MultiSelect = false;
            this.lvNvPresets.Name = "lvNvPresets";
            this.lvNvPresets.Size = new System.Drawing.Size(1098, 415);
            this.lvNvPresets.TabIndex = 0;
            this.lvNvPresets.UseCompatibleStateImageBehavior = false;
            this.lvNvPresets.View = System.Windows.Forms.View.Details;
            this.lvNvPresets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLgPresets_ColumnClick);
            this.lvNvPresets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvNvPresets_ItemCheck);
            this.lvNvPresets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvNvPresets_ItemChecked);
            this.lvNvPresets.SelectedIndexChanged += new System.EventHandler(this.lvNvPresets_SelectedIndexChanged);
            this.lvNvPresets.DoubleClick += new System.EventHandler(this.btnApply_Click);
            this.lvNvPresets.MouseLeave += new System.EventHandler(this.lvNvPresets_MouseLeave);
            this.lvNvPresets.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvNvPresets_MouseMove);

            // 
            // mnuNvSettings
            // 
            this.mnuNvSettings.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuNvSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNvProfileInspector,
            this.miNvSettings});
            this.mnuNvSettings.Name = "mnuLgButtons";
            this.mnuNvSettings.Size = new System.Drawing.Size(202, 48);
            // 
            // miNvProfileInspector
            // 
            this.miNvProfileInspector.Name = "miNvProfileInspector";
            this.miNvProfileInspector.Size = new System.Drawing.Size(201, 22);
            this.miNvProfileInspector.Text = "NVIDIA Profile Inspector";
            this.miNvProfileInspector.Click += new System.EventHandler(this.miNvProfileInspector_Click);
            // 
            // miNvSettings
            // 
            this.miNvSettings.Name = "miNvSettings";
            this.miNvSettings.Size = new System.Drawing.Size(201, 22);
            this.miNvSettings.Text = "Settings";
            this.miNvSettings.Click += new System.EventHandler(this.miNvSettings_Click);

            this.mnuNvPresets.ResumeLayout(false);
            this.mnuNvSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvNvPresets;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnNvPresetSave;
        private System.Windows.Forms.Label lblShortcut;
        private System.Windows.Forms.TextBox edtShortcut;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.Button btnClone;
        private System.Windows.Forms.ContextMenuStrip mnuNvPresets;
        private System.Windows.Forms.ToolStripMenuItem miNvPresetDithering;
        private System.Windows.Forms.ToolStripMenuItem miNvPresetApplyDithering;
        private System.Windows.Forms.ToolStripMenuItem miNvPresetDitheringEnabled;
        private System.Windows.Forms.ToolStripMenuItem mnuRefreshRate;
        private System.Windows.Forms.ToolStripMenuItem miRefreshRateIncluded;
        private System.Windows.Forms.ToolStripMenuItem miNvHDR;
        private System.Windows.Forms.ToolStripMenuItem miHDRIncluded;
        private System.Windows.Forms.ToolStripMenuItem miToggleHDR;
        private System.Windows.Forms.ToolStripMenuItem miHDREnabled;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnNvPresetDelete;
        private System.Windows.Forms.Button btnAddModesNv;
        private System.Windows.Forms.ToolStripMenuItem miNvApply;
        private System.Windows.Forms.ToolStripMenuItem mnuNvDisplay;
        private System.Windows.Forms.ToolStripMenuItem miNvPrimaryDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuNvPresetsColorSettings;
        private System.Windows.Forms.ToolStripMenuItem miNvPresetColorSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuNvDitheringBitDepth;
        private System.Windows.Forms.ToolStripMenuItem miNvDithering6bit;
        private System.Windows.Forms.ToolStripMenuItem miNvDithering8bit;
        private System.Windows.Forms.ToolStripMenuItem miNvDithering10bit;
        private System.Windows.Forms.ToolStripMenuItem mnuNvDitheringMode;
        private System.Windows.Forms.ToolStripMenuItem spatial1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spatial2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spatialDynamic2x2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spatialStatic2x2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temporalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miNvPresetApplyOnStartup;
        private System.Windows.Forms.ToolStripMenuItem miNvCopyId;
        private System.Windows.Forms.Label lblNvPresetName;
        private System.Windows.Forms.TextBox edtNvPresetName;
        private System.Windows.Forms.ToolStripMenuItem mnuNvResolution;
        private System.Windows.Forms.ToolStripMenuItem miNvResolutionIncluded;
        private System.Windows.Forms.ToolStripMenuItem mnuNvDriverSettings;
        private System.Windows.Forms.ToolStripMenuItem miNvDriverSettingsIncluded;
        private System.Windows.Forms.CheckBox chkNvShowInQuickAccess;
        private System.Windows.Forms.Button btnNvSettings;
        private System.Windows.Forms.ContextMenuStrip mnuNvSettings;
        private System.Windows.Forms.ToolStripMenuItem miNvProfileInspector;
        private System.Windows.Forms.ToolStripMenuItem miNvSettings;
        private System.Windows.Forms.ToolTip lvNvPresetsToolTip;
        private System.Windows.Forms.ToolStripSeparator tssNvPresetMenu;
        private System.Windows.Forms.Label lblNvGPU;
        private System.Windows.Forms.ComboBox cbxNvGPU;
        private System.Windows.Forms.Label lblNvGpuInfo;
        private System.Windows.Forms.TextBox edtNvGpuInfo;
        private System.Windows.Forms.Button btnNvSetClocks;
        private System.Windows.Forms.Label lblNvOverclock;
        private System.Windows.Forms.TextBox edtNvOverclock;
        private System.Windows.Forms.ToolStripMenuItem mnuNvOverclocking;
        private System.Windows.Forms.ToolStripMenuItem miNvOverclockingIncluded;
    }
}

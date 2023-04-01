
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
            components = new System.ComponentModel.Container();
            lblNvOverclock = new System.Windows.Forms.Label();
            edtNvOverclock = new System.Windows.Forms.TextBox();
            btnNvSetClocks = new System.Windows.Forms.Button();
            lblNvGpuInfo = new System.Windows.Forms.Label();
            edtNvGpuInfo = new System.Windows.Forms.TextBox();
            lblNvGPU = new System.Windows.Forms.Label();
            cbxNvGPU = new System.Windows.Forms.ComboBox();
            btnNvSettings = new System.Windows.Forms.Button();
            chkNvShowInQuickAccess = new System.Windows.Forms.CheckBox();
            lblNvPresetName = new System.Windows.Forms.Label();
            edtNvPresetName = new System.Windows.Forms.TextBox();
            btnAddModesNv = new System.Windows.Forms.Button();
            btnNvPresetDelete = new System.Windows.Forms.Button();
            lblError = new System.Windows.Forms.Label();
            btnClone = new System.Windows.Forms.Button();
            btnNvPresetSave = new System.Windows.Forms.Button();
            lblShortcut = new System.Windows.Forms.Label();
            edtShortcut = new System.Windows.Forms.TextBox();
            btnChange = new System.Windows.Forms.Button();
            mnuNvPresets = new System.Windows.Forms.ContextMenuStrip(components);
            miNvApply = new System.Windows.Forms.ToolStripMenuItem();
            miNvPresetApplyOnStartup = new System.Windows.Forms.ToolStripMenuItem();
            tssNvPresetMenu = new System.Windows.Forms.ToolStripSeparator();
            mnuNvDisplay = new System.Windows.Forms.ToolStripMenuItem();
            miNvPrimaryDisplay = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvPresetsColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            miNvPresetColorSettings = new System.Windows.Forms.ToolStripMenuItem();
            mnuRefreshRate = new System.Windows.Forms.ToolStripMenuItem();
            miRefreshRateIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvResolution = new System.Windows.Forms.ToolStripMenuItem();
            miNvResolutionIncluded = new System.Windows.Forms.ToolStripMenuItem();
            miNvPresetDithering = new System.Windows.Forms.ToolStripMenuItem();
            miNvPresetApplyDithering = new System.Windows.Forms.ToolStripMenuItem();
            miNvPresetDitheringEnabled = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvDitheringBitDepth = new System.Windows.Forms.ToolStripMenuItem();
            miNvDithering6bit = new System.Windows.Forms.ToolStripMenuItem();
            miNvDithering8bit = new System.Windows.Forms.ToolStripMenuItem();
            miNvDithering10bit = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvDitheringMode = new System.Windows.Forms.ToolStripMenuItem();
            spatial1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            spatial2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            spatialDynamic2x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            spatialStatic2x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            temporalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            miNvHDR = new System.Windows.Forms.ToolStripMenuItem();
            miHDRIncluded = new System.Windows.Forms.ToolStripMenuItem();
            miToggleHDR = new System.Windows.Forms.ToolStripMenuItem();
            miHDREnabled = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvDriverSettings = new System.Windows.Forms.ToolStripMenuItem();
            miNvDriverSettingsIncluded = new System.Windows.Forms.ToolStripMenuItem();
            mnuNvOverclocking = new System.Windows.Forms.ToolStripMenuItem();
            miNvOverclockingIncluded = new System.Windows.Forms.ToolStripMenuItem();
            miNvCopyId = new System.Windows.Forms.ToolStripMenuItem();
            btnApply = new System.Windows.Forms.Button();
            lvNvPresets = new System.Windows.Forms.ListView();
            mnuNvSettings = new System.Windows.Forms.ContextMenuStrip(components);
            miNvProfileInspector = new System.Windows.Forms.ToolStripMenuItem();
            miNvSettings = new System.Windows.Forms.ToolStripMenuItem();
            lvNvPresetsToolTip = new System.Windows.Forms.ToolTip(components);
            mnuNvPresets.SuspendLayout();
            mnuNvSettings.SuspendLayout();
            SuspendLayout();
            // 
            // lblNvOverclock
            // 
            lblNvOverclock.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblNvOverclock.AutoSize = true;
            lblNvOverclock.Location = new System.Drawing.Point(578, 496);
            lblNvOverclock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNvOverclock.Name = "lblNvOverclock";
            lblNvOverclock.Size = new System.Drawing.Size(51, 15);
            lblNvOverclock.TabIndex = 66;
            lblNvOverclock.Text = "OC info:";
            // 
            // edtNvOverclock
            // 
            edtNvOverclock.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            edtNvOverclock.Enabled = false;
            edtNvOverclock.Location = new System.Drawing.Point(648, 493);
            edtNvOverclock.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtNvOverclock.Name = "edtNvOverclock";
            edtNvOverclock.Size = new System.Drawing.Size(457, 23);
            edtNvOverclock.TabIndex = 65;
            // 
            // btnNvSetClocks
            // 
            btnNvSetClocks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnNvSetClocks.Location = new System.Drawing.Point(868, 433);
            btnNvSetClocks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNvSetClocks.Name = "btnNvSetClocks";
            btnNvSetClocks.Size = new System.Drawing.Size(88, 27);
            btnNvSetClocks.TabIndex = 64;
            btnNvSetClocks.Text = "Change OC...";
            btnNvSetClocks.UseVisualStyleBackColor = true;
            btnNvSetClocks.Click += btnNvSetClocks_Click;
            // 
            // lblNvGpuInfo
            // 
            lblNvGpuInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblNvGpuInfo.AutoSize = true;
            lblNvGpuInfo.Location = new System.Drawing.Point(578, 469);
            lblNvGpuInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNvGpuInfo.Name = "lblNvGpuInfo";
            lblNvGpuInfo.Size = new System.Drawing.Size(31, 15);
            lblNvGpuInfo.TabIndex = 63;
            lblNvGpuInfo.Text = "Info:";
            // 
            // edtNvGpuInfo
            // 
            edtNvGpuInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            edtNvGpuInfo.Enabled = false;
            edtNvGpuInfo.Location = new System.Drawing.Point(648, 466);
            edtNvGpuInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtNvGpuInfo.Name = "edtNvGpuInfo";
            edtNvGpuInfo.Size = new System.Drawing.Size(457, 23);
            edtNvGpuInfo.TabIndex = 62;
            // 
            // lblNvGPU
            // 
            lblNvGPU.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblNvGPU.AutoSize = true;
            lblNvGPU.Location = new System.Drawing.Point(578, 439);
            lblNvGPU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNvGPU.Name = "lblNvGPU";
            lblNvGPU.Size = new System.Drawing.Size(33, 15);
            lblNvGPU.TabIndex = 61;
            lblNvGPU.Text = "GPU:";
            // 
            // cbxNvGPU
            // 
            cbxNvGPU.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            cbxNvGPU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxNvGPU.FormattingEnabled = true;
            cbxNvGPU.Location = new System.Drawing.Point(648, 436);
            cbxNvGPU.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxNvGPU.Name = "cbxNvGPU";
            cbxNvGPU.Size = new System.Drawing.Size(212, 23);
            cbxNvGPU.TabIndex = 60;
            cbxNvGPU.SelectedIndexChanged += cbxNvGPU_SelectedIndexChanged;
            // 
            // btnNvSettings
            // 
            btnNvSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnNvSettings.Location = new System.Drawing.Point(1017, 428);
            btnNvSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNvSettings.Name = "btnNvSettings";
            btnNvSettings.Size = new System.Drawing.Size(88, 27);
            btnNvSettings.TabIndex = 59;
            btnNvSettings.Text = "Settings...";
            btnNvSettings.UseVisualStyleBackColor = true;
            btnNvSettings.Click += btnNvSettings_Click;
            // 
            // chkNvShowInQuickAccess
            // 
            chkNvShowInQuickAccess.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkNvShowInQuickAccess.AutoSize = true;
            chkNvShowInQuickAccess.Enabled = false;
            chkNvShowInQuickAccess.Location = new System.Drawing.Point(343, 465);
            chkNvShowInQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkNvShowInQuickAccess.Name = "chkNvShowInQuickAccess";
            chkNvShowInQuickAccess.Size = new System.Drawing.Size(141, 19);
            chkNvShowInQuickAccess.TabIndex = 45;
            chkNvShowInQuickAccess.Text = "Show in Quick Access";
            chkNvShowInQuickAccess.UseVisualStyleBackColor = true;
            // 
            // lblNvPresetName
            // 
            lblNvPresetName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblNvPresetName.AutoSize = true;
            lblNvPresetName.Location = new System.Drawing.Point(7, 466);
            lblNvPresetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNvPresetName.Name = "lblNvPresetName";
            lblNvPresetName.Size = new System.Drawing.Size(42, 15);
            lblNvPresetName.TabIndex = 22;
            lblNvPresetName.Text = "Name:";
            // 
            // edtNvPresetName
            // 
            edtNvPresetName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtNvPresetName.Enabled = false;
            edtNvPresetName.Location = new System.Drawing.Point(102, 463);
            edtNvPresetName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtNvPresetName.Name = "edtNvPresetName";
            edtNvPresetName.Size = new System.Drawing.Size(233, 23);
            edtNvPresetName.TabIndex = 21;
            // 
            // btnAddModesNv
            // 
            btnAddModesNv.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAddModesNv.Location = new System.Drawing.Point(385, 429);
            btnAddModesNv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddModesNv.Name = "btnAddModesNv";
            btnAddModesNv.Size = new System.Drawing.Size(88, 27);
            btnAddModesNv.TabIndex = 9;
            btnAddModesNv.Text = "Add modes";
            btnAddModesNv.UseVisualStyleBackColor = true;
            btnAddModesNv.Click += btnAddModesNv_Click;
            // 
            // btnNvPresetDelete
            // 
            btnNvPresetDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnNvPresetDelete.Enabled = false;
            btnNvPresetDelete.Location = new System.Drawing.Point(290, 429);
            btnNvPresetDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNvPresetDelete.Name = "btnNvPresetDelete";
            btnNvPresetDelete.Size = new System.Drawing.Size(88, 27);
            btnNvPresetDelete.TabIndex = 8;
            btnNvPresetDelete.Text = "Delete";
            btnNvPresetDelete.UseVisualStyleBackColor = true;
            btnNvPresetDelete.Click += btnNvPresetDelete_Click;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new System.Drawing.Point(7, 7);
            lblError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblError.Name = "lblError";
            lblError.Size = new System.Drawing.Size(53, 15);
            lblError.TabIndex = 7;
            lblError.Text = "ErrorText";
            lblError.Visible = false;
            // 
            // btnClone
            // 
            btnClone.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClone.Enabled = false;
            btnClone.Location = new System.Drawing.Point(196, 429);
            btnClone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClone.Name = "btnClone";
            btnClone.Size = new System.Drawing.Size(88, 27);
            btnClone.TabIndex = 6;
            btnClone.Text = "Clone";
            btnClone.UseVisualStyleBackColor = true;
            btnClone.Click += btnClone_Click;
            // 
            // btnNvPresetSave
            // 
            btnNvPresetSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnNvPresetSave.Enabled = false;
            btnNvPresetSave.Location = new System.Drawing.Point(479, 429);
            btnNvPresetSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNvPresetSave.Name = "btnNvPresetSave";
            btnNvPresetSave.Size = new System.Drawing.Size(88, 27);
            btnNvPresetSave.TabIndex = 5;
            btnNvPresetSave.Text = "Save";
            btnNvPresetSave.UseVisualStyleBackColor = true;
            btnNvPresetSave.Click += btnSetShortcut_Click;
            // 
            // lblShortcut
            // 
            lblShortcut.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblShortcut.AutoSize = true;
            lblShortcut.Location = new System.Drawing.Point(7, 496);
            lblShortcut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblShortcut.Name = "lblShortcut";
            lblShortcut.Size = new System.Drawing.Size(55, 15);
            lblShortcut.TabIndex = 4;
            lblShortcut.Text = "Shortcut:";
            // 
            // edtShortcut
            // 
            edtShortcut.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtShortcut.Enabled = false;
            edtShortcut.Location = new System.Drawing.Point(102, 493);
            edtShortcut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtShortcut.Name = "edtShortcut";
            edtShortcut.ReadOnly = true;
            edtShortcut.Size = new System.Drawing.Size(233, 23);
            edtShortcut.TabIndex = 3;
            edtShortcut.TextChanged += edtShortcut_TextChanged;
            edtShortcut.KeyDown += edtShortcut_KeyDown;
            edtShortcut.KeyUp += edtShortcut_KeyUp;
            // 
            // btnChange
            // 
            btnChange.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnChange.ContextMenuStrip = mnuNvPresets;
            btnChange.Location = new System.Drawing.Point(102, 429);
            btnChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnChange.Name = "btnChange";
            btnChange.Size = new System.Drawing.Size(88, 27);
            btnChange.TabIndex = 2;
            btnChange.Text = "Change...";
            btnChange.UseVisualStyleBackColor = true;
            btnChange.Click += btnChange_Click;
            // 
            // mnuNvPresets
            // 
            mnuNvPresets.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuNvPresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvApply, miNvPresetApplyOnStartup, tssNvPresetMenu, mnuNvDisplay, mnuNvPresetsColorSettings, mnuRefreshRate, mnuNvResolution, miNvPresetDithering, miNvHDR, mnuNvDriverSettings, mnuNvOverclocking, miNvCopyId });
            mnuNvPresets.Name = "mnuNvPresets";
            mnuNvPresets.Size = new System.Drawing.Size(185, 252);
            mnuNvPresets.Opening += mnuNvPresets_Opening;
            // 
            // miNvApply
            // 
            miNvApply.Name = "miNvApply";
            miNvApply.Size = new System.Drawing.Size(184, 22);
            miNvApply.Text = "Apply";
            miNvApply.Click += btnApply_Click;
            // 
            // miNvPresetApplyOnStartup
            // 
            miNvPresetApplyOnStartup.CheckOnClick = true;
            miNvPresetApplyOnStartup.Name = "miNvPresetApplyOnStartup";
            miNvPresetApplyOnStartup.Size = new System.Drawing.Size(184, 22);
            miNvPresetApplyOnStartup.Text = "Apply on startup";
            miNvPresetApplyOnStartup.Click += miNvPresetApplyOnStartup_Click;
            // 
            // tssNvPresetMenu
            // 
            tssNvPresetMenu.Name = "tssNvPresetMenu";
            tssNvPresetMenu.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuNvDisplay
            // 
            mnuNvDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvPrimaryDisplay });
            mnuNvDisplay.Name = "mnuNvDisplay";
            mnuNvDisplay.Size = new System.Drawing.Size(184, 22);
            mnuNvDisplay.Text = "Display";
            // 
            // miNvPrimaryDisplay
            // 
            miNvPrimaryDisplay.Name = "miNvPrimaryDisplay";
            miNvPrimaryDisplay.Size = new System.Drawing.Size(155, 22);
            miNvPrimaryDisplay.Text = "Primary display";
            miNvPrimaryDisplay.Click += displayMenuItem_Click;
            // 
            // mnuNvPresetsColorSettings
            // 
            mnuNvPresetsColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvPresetColorSettings });
            mnuNvPresetsColorSettings.Name = "mnuNvPresetsColorSettings";
            mnuNvPresetsColorSettings.Size = new System.Drawing.Size(184, 22);
            mnuNvPresetsColorSettings.Text = "Color Settings";
            // 
            // miNvPresetColorSettings
            // 
            miNvPresetColorSettings.Name = "miNvPresetColorSettings";
            miNvPresetColorSettings.Size = new System.Drawing.Size(120, 22);
            miNvPresetColorSettings.Text = "Included";
            miNvPresetColorSettings.Click += miNvPresetColorSettings_Click;
            // 
            // mnuRefreshRate
            // 
            mnuRefreshRate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miRefreshRateIncluded });
            mnuRefreshRate.Name = "mnuRefreshRate";
            mnuRefreshRate.Size = new System.Drawing.Size(184, 22);
            mnuRefreshRate.Text = "Refresh Rate";
            // 
            // miRefreshRateIncluded
            // 
            miRefreshRateIncluded.Name = "miRefreshRateIncluded";
            miRefreshRateIncluded.Size = new System.Drawing.Size(120, 22);
            miRefreshRateIncluded.Text = "Included";
            miRefreshRateIncluded.Click += includedToolStripMenuItem_Click;
            // 
            // mnuNvResolution
            // 
            mnuNvResolution.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvResolutionIncluded });
            mnuNvResolution.Name = "mnuNvResolution";
            mnuNvResolution.Size = new System.Drawing.Size(184, 22);
            mnuNvResolution.Text = "Resolution";
            // 
            // miNvResolutionIncluded
            // 
            miNvResolutionIncluded.CheckOnClick = true;
            miNvResolutionIncluded.Name = "miNvResolutionIncluded";
            miNvResolutionIncluded.Size = new System.Drawing.Size(120, 22);
            miNvResolutionIncluded.Text = "Included";
            miNvResolutionIncluded.Click += miNvResolutionIncluded_Click;
            // 
            // miNvPresetDithering
            // 
            miNvPresetDithering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvPresetApplyDithering, miNvPresetDitheringEnabled, mnuNvDitheringBitDepth, mnuNvDitheringMode });
            miNvPresetDithering.Name = "miNvPresetDithering";
            miNvPresetDithering.Size = new System.Drawing.Size(184, 22);
            miNvPresetDithering.Text = "Dithering";
            // 
            // miNvPresetApplyDithering
            // 
            miNvPresetApplyDithering.Name = "miNvPresetApplyDithering";
            miNvPresetApplyDithering.Size = new System.Drawing.Size(122, 22);
            miNvPresetApplyDithering.Text = "Included";
            miNvPresetApplyDithering.Click += miNvPresetApplyDithering_Click;
            // 
            // miNvPresetDitheringEnabled
            // 
            miNvPresetDitheringEnabled.Name = "miNvPresetDitheringEnabled";
            miNvPresetDitheringEnabled.Size = new System.Drawing.Size(122, 22);
            miNvPresetDitheringEnabled.Text = "Enabled";
            miNvPresetDitheringEnabled.Click += miNvPresetDitheringEnabled_Click;
            // 
            // mnuNvDitheringBitDepth
            // 
            mnuNvDitheringBitDepth.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvDithering6bit, miNvDithering8bit, miNvDithering10bit });
            mnuNvDitheringBitDepth.Name = "mnuNvDitheringBitDepth";
            mnuNvDitheringBitDepth.Size = new System.Drawing.Size(122, 22);
            mnuNvDitheringBitDepth.Text = "Bit depth";
            // 
            // miNvDithering6bit
            // 
            miNvDithering6bit.Name = "miNvDithering6bit";
            miNvDithering6bit.Size = new System.Drawing.Size(105, 22);
            miNvDithering6bit.Tag = "0";
            miNvDithering6bit.Text = "6-bit";
            miNvDithering6bit.Click += miNvDithering6bit_Click;
            // 
            // miNvDithering8bit
            // 
            miNvDithering8bit.Name = "miNvDithering8bit";
            miNvDithering8bit.Size = new System.Drawing.Size(105, 22);
            miNvDithering8bit.Tag = "1";
            miNvDithering8bit.Text = "8-bit";
            miNvDithering8bit.Click += miNvDithering6bit_Click;
            // 
            // miNvDithering10bit
            // 
            miNvDithering10bit.Name = "miNvDithering10bit";
            miNvDithering10bit.Size = new System.Drawing.Size(105, 22);
            miNvDithering10bit.Tag = "2";
            miNvDithering10bit.Text = "10-bit";
            miNvDithering10bit.Click += miNvDithering6bit_Click;
            // 
            // mnuNvDitheringMode
            // 
            mnuNvDitheringMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { spatial1ToolStripMenuItem, spatial2ToolStripMenuItem, spatialDynamic2x2ToolStripMenuItem, spatialStatic2x2ToolStripMenuItem, temporalToolStripMenuItem });
            mnuNvDitheringMode.Name = "mnuNvDitheringMode";
            mnuNvDitheringMode.Size = new System.Drawing.Size(122, 22);
            mnuNvDitheringMode.Text = "Mode";
            // 
            // spatial1ToolStripMenuItem
            // 
            spatial1ToolStripMenuItem.Name = "spatial1ToolStripMenuItem";
            spatial1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            spatial1ToolStripMenuItem.Tag = "0";
            spatial1ToolStripMenuItem.Text = "Spatial Dynamic";
            spatial1ToolStripMenuItem.Click += spatial1ToolStripMenuItem_Click;
            // 
            // spatial2ToolStripMenuItem
            // 
            spatial2ToolStripMenuItem.Name = "spatial2ToolStripMenuItem";
            spatial2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            spatial2ToolStripMenuItem.Tag = "1";
            spatial2ToolStripMenuItem.Text = "Spatial Static";
            spatial2ToolStripMenuItem.Click += spatial1ToolStripMenuItem_Click;
            // 
            // spatialDynamic2x2ToolStripMenuItem
            // 
            spatialDynamic2x2ToolStripMenuItem.Name = "spatialDynamic2x2ToolStripMenuItem";
            spatialDynamic2x2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            spatialDynamic2x2ToolStripMenuItem.Tag = "2";
            spatialDynamic2x2ToolStripMenuItem.Text = "Spatial Dynamic 2x2";
            spatialDynamic2x2ToolStripMenuItem.Click += spatial1ToolStripMenuItem_Click;
            // 
            // spatialStatic2x2ToolStripMenuItem
            // 
            spatialStatic2x2ToolStripMenuItem.Name = "spatialStatic2x2ToolStripMenuItem";
            spatialStatic2x2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            spatialStatic2x2ToolStripMenuItem.Tag = "3";
            spatialStatic2x2ToolStripMenuItem.Text = "Spatial Static 2x2";
            spatialStatic2x2ToolStripMenuItem.Click += spatial1ToolStripMenuItem_Click;
            // 
            // temporalToolStripMenuItem
            // 
            temporalToolStripMenuItem.Name = "temporalToolStripMenuItem";
            temporalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            temporalToolStripMenuItem.Tag = "4";
            temporalToolStripMenuItem.Text = "Temporal";
            temporalToolStripMenuItem.Click += spatial1ToolStripMenuItem_Click;
            // 
            // miNvHDR
            // 
            miNvHDR.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miHDRIncluded, miToggleHDR, miHDREnabled });
            miNvHDR.Name = "miNvHDR";
            miNvHDR.Size = new System.Drawing.Size(184, 22);
            miNvHDR.Text = "HDR";
            // 
            // miHDRIncluded
            // 
            miHDRIncluded.Name = "miHDRIncluded";
            miHDRIncluded.Size = new System.Drawing.Size(136, 22);
            miHDRIncluded.Text = "Included";
            miHDRIncluded.Click += miHDRIncluded_Click;
            // 
            // miToggleHDR
            // 
            miToggleHDR.Name = "miToggleHDR";
            miToggleHDR.Size = new System.Drawing.Size(136, 22);
            miToggleHDR.Text = "Toggle HDR";
            miToggleHDR.Click += miToggleHDR_Click;
            // 
            // miHDREnabled
            // 
            miHDREnabled.Name = "miHDREnabled";
            miHDREnabled.Size = new System.Drawing.Size(136, 22);
            miHDREnabled.Text = "Enabled";
            miHDREnabled.Click += miHDREnabled_Click;
            // 
            // mnuNvDriverSettings
            // 
            mnuNvDriverSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvDriverSettingsIncluded });
            mnuNvDriverSettings.Name = "mnuNvDriverSettings";
            mnuNvDriverSettings.Size = new System.Drawing.Size(184, 22);
            mnuNvDriverSettings.Text = "Driver Settings";
            // 
            // miNvDriverSettingsIncluded
            // 
            miNvDriverSettingsIncluded.Name = "miNvDriverSettingsIncluded";
            miNvDriverSettingsIncluded.Size = new System.Drawing.Size(120, 22);
            miNvDriverSettingsIncluded.Text = "Included";
            miNvDriverSettingsIncluded.Click += miNvDriverSettingsIncluded_Click;
            // 
            // mnuNvOverclocking
            // 
            mnuNvOverclocking.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvOverclockingIncluded });
            mnuNvOverclocking.Name = "mnuNvOverclocking";
            mnuNvOverclocking.Size = new System.Drawing.Size(184, 22);
            mnuNvOverclocking.Text = "Overclocking";
            // 
            // miNvOverclockingIncluded
            // 
            miNvOverclockingIncluded.Name = "miNvOverclockingIncluded";
            miNvOverclockingIncluded.Size = new System.Drawing.Size(120, 22);
            miNvOverclockingIncluded.Text = "Included";
            miNvOverclockingIncluded.Click += miNvOverclockingIncluded_Click;
            // 
            // miNvCopyId
            // 
            miNvCopyId.Name = "miNvCopyId";
            miNvCopyId.Size = new System.Drawing.Size(184, 22);
            miNvCopyId.Text = "Copy Id to Clipboard";
            miNvCopyId.Click += miNvCopyId_Click;
            // 
            // btnApply
            // 
            btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnApply.Enabled = false;
            btnApply.Location = new System.Drawing.Point(7, 429);
            btnApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnApply.Name = "btnApply";
            btnApply.Size = new System.Drawing.Size(88, 27);
            btnApply.TabIndex = 1;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // lvNvPresets
            // 
            lvNvPresets.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvNvPresets.CheckBoxes = true;
            lvNvPresets.ContextMenuStrip = mnuNvPresets;
            lvNvPresets.FullRowSelect = true;
            lvNvPresets.Location = new System.Drawing.Point(7, 7);
            lvNvPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvNvPresets.MultiSelect = false;
            lvNvPresets.Name = "lvNvPresets";
            lvNvPresets.Size = new System.Drawing.Size(1098, 415);
            lvNvPresets.TabIndex = 0;
            lvNvPresets.UseCompatibleStateImageBehavior = false;
            lvNvPresets.View = System.Windows.Forms.View.Details;
            lvNvPresets.ColumnClick += lvLgPresets_ColumnClick;
            lvNvPresets.ItemCheck += lvNvPresets_ItemCheck;
            lvNvPresets.ItemChecked += lvNvPresets_ItemChecked;
            lvNvPresets.SelectedIndexChanged += lvNvPresets_SelectedIndexChanged;
            lvNvPresets.DoubleClick += btnApply_Click;
            lvNvPresets.MouseLeave += lvNvPresets_MouseLeave;
            lvNvPresets.MouseMove += lvNvPresets_MouseMove;
            // 
            // mnuNvSettings
            // 
            mnuNvSettings.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuNvSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miNvProfileInspector, miNvSettings });
            mnuNvSettings.Name = "mnuLgButtons";
            mnuNvSettings.Size = new System.Drawing.Size(202, 48);
            // 
            // miNvProfileInspector
            // 
            miNvProfileInspector.Name = "miNvProfileInspector";
            miNvProfileInspector.Size = new System.Drawing.Size(201, 22);
            miNvProfileInspector.Text = "NVIDIA Profile Inspector";
            miNvProfileInspector.Click += miNvProfileInspector_Click;
            // 
            // miNvSettings
            // 
            miNvSettings.Name = "miNvSettings";
            miNvSettings.Size = new System.Drawing.Size(201, 22);
            miNvSettings.Text = "Settings";
            miNvSettings.Click += miNvSettings_Click;
            // 
            // NvPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(lblNvOverclock);
            Controls.Add(edtNvOverclock);
            Controls.Add(btnNvSetClocks);
            Controls.Add(lblNvGpuInfo);
            Controls.Add(edtNvGpuInfo);
            Controls.Add(lblNvGPU);
            Controls.Add(cbxNvGPU);
            Controls.Add(btnNvSettings);
            Controls.Add(chkNvShowInQuickAccess);
            Controls.Add(lblNvPresetName);
            Controls.Add(edtNvPresetName);
            Controls.Add(btnAddModesNv);
            Controls.Add(btnNvPresetDelete);
            Controls.Add(lblError);
            Controls.Add(btnClone);
            Controls.Add(btnNvPresetSave);
            Controls.Add(lblShortcut);
            Controls.Add(edtShortcut);
            Controls.Add(btnChange);
            Controls.Add(btnApply);
            Controls.Add(lvNvPresets);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "NvPanel";
            Size = new System.Drawing.Size(1114, 539);
            Load += RemoteControlPanel_Load;
            mnuNvPresets.ResumeLayout(false);
            mnuNvSettings.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView lvNvPresets;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnNvPresetSave;
        private System.Windows.Forms.Label lblShortcut;
        private System.Windows.Forms.TextBox edtShortcut;
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

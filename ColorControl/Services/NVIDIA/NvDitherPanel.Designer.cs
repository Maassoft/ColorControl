namespace ColorControl.Services.NVIDIA
{
    partial class NvDitherPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NvDitherPanel));
            grpNvidiaOptions = new System.Windows.Forms.GroupBox();
            btnSetRegistryKey = new System.Windows.Forms.Button();
            btnRestartDriver = new System.Windows.Forms.Button();
            lblDitherRegistry = new System.Windows.Forms.Label();
            lblDitheringDisplay = new System.Windows.Forms.Label();
            cbxDitheringDisplay = new System.Windows.Forms.ComboBox();
            lblDitheringMode = new System.Windows.Forms.Label();
            cbxDitheringMode = new System.Windows.Forms.ComboBox();
            lblDitheringBitDepth = new System.Windows.Forms.Label();
            cbxDitheringBitDepth = new System.Windows.Forms.ComboBox();
            chkDitheringEnabled = new System.Windows.Forms.CheckBox();
            pbGradient = new System.Windows.Forms.PictureBox();
            grpNvidiaOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbGradient).BeginInit();
            SuspendLayout();
            // 
            // grpNvidiaOptions
            // 
            grpNvidiaOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpNvidiaOptions.Controls.Add(btnSetRegistryKey);
            grpNvidiaOptions.Controls.Add(btnRestartDriver);
            grpNvidiaOptions.Controls.Add(lblDitherRegistry);
            grpNvidiaOptions.Controls.Add(lblDitheringDisplay);
            grpNvidiaOptions.Controls.Add(cbxDitheringDisplay);
            grpNvidiaOptions.Controls.Add(lblDitheringMode);
            grpNvidiaOptions.Controls.Add(cbxDitheringMode);
            grpNvidiaOptions.Controls.Add(lblDitheringBitDepth);
            grpNvidiaOptions.Controls.Add(cbxDitheringBitDepth);
            grpNvidiaOptions.Controls.Add(chkDitheringEnabled);
            grpNvidiaOptions.Controls.Add(pbGradient);
            grpNvidiaOptions.Location = new System.Drawing.Point(4, 3);
            grpNvidiaOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNvidiaOptions.MinimumSize = new System.Drawing.Size(300, 100);
            grpNvidiaOptions.Name = "grpNvidiaOptions";
            grpNvidiaOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNvidiaOptions.Size = new System.Drawing.Size(856, 479);
            grpNvidiaOptions.TabIndex = 6;
            grpNvidiaOptions.TabStop = false;
            grpNvidiaOptions.Text = "NVIDIA options - test dithering";
            // 
            // btnSetRegistryKey
            // 
            btnSetRegistryKey.Location = new System.Drawing.Point(381, 17);
            btnSetRegistryKey.Name = "btnSetRegistryKey";
            btnSetRegistryKey.Size = new System.Drawing.Size(150, 23);
            btnSetRegistryKey.TabIndex = 13;
            btnSetRegistryKey.Text = "Set dither registry key";
            btnSetRegistryKey.UseVisualStyleBackColor = true;
            btnSetRegistryKey.Click += btnSetRegistryKey_Click;
            // 
            // btnRestartDriver
            // 
            btnRestartDriver.Location = new System.Drawing.Point(539, 16);
            btnRestartDriver.Name = "btnRestartDriver";
            btnRestartDriver.Size = new System.Drawing.Size(150, 23);
            btnRestartDriver.TabIndex = 12;
            btnRestartDriver.Text = "Restart driver";
            btnRestartDriver.UseVisualStyleBackColor = true;
            btnRestartDriver.Click += btnRestartDriver_Click;
            // 
            // lblDitherRegistry
            // 
            lblDitherRegistry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblDitherRegistry.Location = new System.Drawing.Point(381, 47);
            lblDitherRegistry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDitherRegistry.Name = "lblDitherRegistry";
            lblDitherRegistry.Size = new System.Drawing.Size(467, 78);
            lblDitherRegistry.TabIndex = 11;
            lblDitherRegistry.Text = resources.GetString("lblDitherRegistry.Text");
            // 
            // lblDitheringDisplay
            // 
            lblDitheringDisplay.AutoSize = true;
            lblDitheringDisplay.Location = new System.Drawing.Point(8, 23);
            lblDitheringDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDitheringDisplay.Name = "lblDitheringDisplay";
            lblDitheringDisplay.Size = new System.Drawing.Size(48, 15);
            lblDitheringDisplay.TabIndex = 9;
            lblDitheringDisplay.Text = "Display:";
            // 
            // cbxDitheringDisplay
            // 
            cbxDitheringDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxDitheringDisplay.DropDownWidth = 300;
            cbxDitheringDisplay.FormattingEnabled = true;
            cbxDitheringDisplay.Location = new System.Drawing.Point(75, 17);
            cbxDitheringDisplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxDitheringDisplay.Name = "cbxDitheringDisplay";
            cbxDitheringDisplay.Size = new System.Drawing.Size(298, 23);
            cbxDitheringDisplay.TabIndex = 8;
            cbxDitheringDisplay.SelectedIndexChanged += cbxDitheringDisplay_SelectedIndexChanged;
            // 
            // lblDitheringMode
            // 
            lblDitheringMode.AutoSize = true;
            lblDitheringMode.Location = new System.Drawing.Point(8, 105);
            lblDitheringMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDitheringMode.Name = "lblDitheringMode";
            lblDitheringMode.Size = new System.Drawing.Size(41, 15);
            lblDitheringMode.TabIndex = 7;
            lblDitheringMode.Text = "Mode:";
            // 
            // cbxDitheringMode
            // 
            cbxDitheringMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxDitheringMode.FormattingEnabled = true;
            cbxDitheringMode.Location = new System.Drawing.Point(75, 99);
            cbxDitheringMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxDitheringMode.Name = "cbxDitheringMode";
            cbxDitheringMode.Size = new System.Drawing.Size(136, 23);
            cbxDitheringMode.TabIndex = 6;
            cbxDitheringMode.SelectedIndexChanged += cbxDitheringMode_SelectedIndexChanged;
            // 
            // lblDitheringBitDepth
            // 
            lblDitheringBitDepth.AutoSize = true;
            lblDitheringBitDepth.Location = new System.Drawing.Point(8, 74);
            lblDitheringBitDepth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDitheringBitDepth.Name = "lblDitheringBitDepth";
            lblDitheringBitDepth.Size = new System.Drawing.Size(60, 15);
            lblDitheringBitDepth.TabIndex = 5;
            lblDitheringBitDepth.Text = "Bit-depth:";
            // 
            // cbxDitheringBitDepth
            // 
            cbxDitheringBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxDitheringBitDepth.FormattingEnabled = true;
            cbxDitheringBitDepth.Location = new System.Drawing.Point(75, 68);
            cbxDitheringBitDepth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxDitheringBitDepth.Name = "cbxDitheringBitDepth";
            cbxDitheringBitDepth.Size = new System.Drawing.Size(136, 23);
            cbxDitheringBitDepth.TabIndex = 4;
            cbxDitheringBitDepth.SelectedIndexChanged += cbxDitheringBitDepth_SelectedIndexChanged;
            // 
            // chkDitheringEnabled
            // 
            chkDitheringEnabled.AutoSize = true;
            chkDitheringEnabled.Checked = true;
            chkDitheringEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            chkDitheringEnabled.Location = new System.Drawing.Point(8, 46);
            chkDitheringEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkDitheringEnabled.Name = "chkDitheringEnabled";
            chkDitheringEnabled.Size = new System.Drawing.Size(120, 19);
            chkDitheringEnabled.TabIndex = 3;
            chkDitheringEnabled.Text = "Dithering enabled";
            chkDitheringEnabled.ThreeState = true;
            chkDitheringEnabled.UseVisualStyleBackColor = true;
            chkDitheringEnabled.CheckStateChanged += chkDitheringEnabled_CheckStateChanged;
            // 
            // pbGradient
            // 
            pbGradient.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbGradient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbGradient.Location = new System.Drawing.Point(8, 128);
            pbGradient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbGradient.Name = "pbGradient";
            pbGradient.Size = new System.Drawing.Size(840, 345);
            pbGradient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pbGradient.TabIndex = 0;
            pbGradient.TabStop = false;
            // 
            // NvDitherPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grpNvidiaOptions);
            Name = "NvDitherPanel";
            Size = new System.Drawing.Size(864, 485);
            grpNvidiaOptions.ResumeLayout(false);
            grpNvidiaOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbGradient).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpNvidiaOptions;
        private System.Windows.Forms.PictureBox pbGradient;
        private System.Windows.Forms.Label lblDitheringMode;
        private System.Windows.Forms.ComboBox cbxDitheringMode;
        private System.Windows.Forms.Label lblDitheringBitDepth;
        private System.Windows.Forms.ComboBox cbxDitheringBitDepth;
        private System.Windows.Forms.CheckBox chkDitheringEnabled;
        private System.Windows.Forms.Label lblDitheringDisplay;
        private System.Windows.Forms.ComboBox cbxDitheringDisplay;
        private System.Windows.Forms.Label lblDitherRegistry;
        private System.Windows.Forms.Button btnRestartDriver;
        private System.Windows.Forms.Button btnSetRegistryKey;
    }
}

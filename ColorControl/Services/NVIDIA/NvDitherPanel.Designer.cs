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
            this.grpNvidiaOptions = new System.Windows.Forms.GroupBox();
            this.lblDitheringDisplay = new System.Windows.Forms.Label();
            this.cbxDitheringDisplay = new System.Windows.Forms.ComboBox();
            this.lblDitheringMode = new System.Windows.Forms.Label();
            this.cbxDitheringMode = new System.Windows.Forms.ComboBox();
            this.lblDitheringBitDepth = new System.Windows.Forms.Label();
            this.cbxDitheringBitDepth = new System.Windows.Forms.ComboBox();
            this.chkDitheringEnabled = new System.Windows.Forms.CheckBox();
            this.pbGradient = new System.Windows.Forms.PictureBox();
            this.grpNvidiaOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGradient)).BeginInit();
            this.SuspendLayout();
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
            this.grpNvidiaOptions.Location = new System.Drawing.Point(4, 3);
            this.grpNvidiaOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNvidiaOptions.Name = "grpNvidiaOptions";
            this.grpNvidiaOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpNvidiaOptions.Size = new System.Drawing.Size(856, 479);
            this.grpNvidiaOptions.TabIndex = 6;
            this.grpNvidiaOptions.TabStop = false;
            this.grpNvidiaOptions.Text = "NVIDIA options - test dithering";
            // 
            // lblDitheringDisplay
            // 
            this.lblDitheringDisplay.AutoSize = true;
            this.lblDitheringDisplay.Location = new System.Drawing.Point(8, 23);
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
            this.lblDitheringMode.Location = new System.Drawing.Point(8, 105);
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
            this.lblDitheringBitDepth.Location = new System.Drawing.Point(8, 74);
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
            this.chkDitheringEnabled.Location = new System.Drawing.Point(8, 46);
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
            this.pbGradient.Location = new System.Drawing.Point(8, 128);
            this.pbGradient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbGradient.Name = "pbGradient";
            this.pbGradient.Size = new System.Drawing.Size(840, 345);
            this.pbGradient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGradient.TabIndex = 0;
            this.pbGradient.TabStop = false;
            // 
            // NvDitherPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpNvidiaOptions);
            this.Name = "NvDitherPanel";
            this.Size = new System.Drawing.Size(864, 485);
            this.grpNvidiaOptions.ResumeLayout(false);
            this.grpNvidiaOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGradient)).EndInit();
            this.ResumeLayout(false);

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
    }
}

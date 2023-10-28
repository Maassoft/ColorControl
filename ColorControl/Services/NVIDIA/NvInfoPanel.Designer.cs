namespace ColorControl.Services.NVIDIA
{
    partial class NvInfoPanel
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
            grpNVIDIAInfo = new System.Windows.Forms.GroupBox();
            btnRefreshNVIDIAInfo = new System.Windows.Forms.Button();
            tvNVIDIAInfo = new System.Windows.Forms.TreeView();
            grpNVIDIAInfo.SuspendLayout();
            SuspendLayout();
            // 
            // grpNVIDIAInfo
            // 
            grpNVIDIAInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpNVIDIAInfo.Controls.Add(btnRefreshNVIDIAInfo);
            grpNVIDIAInfo.Controls.Add(tvNVIDIAInfo);
            grpNVIDIAInfo.Location = new System.Drawing.Point(4, 3);
            grpNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNVIDIAInfo.Name = "grpNVIDIAInfo";
            grpNVIDIAInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpNVIDIAInfo.Size = new System.Drawing.Size(643, 547);
            grpNVIDIAInfo.TabIndex = 5;
            grpNVIDIAInfo.TabStop = false;
            grpNVIDIAInfo.Text = "NVIDIA info";
            // 
            // btnRefreshNVIDIAInfo
            // 
            btnRefreshNVIDIAInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnRefreshNVIDIAInfo.Location = new System.Drawing.Point(8, 514);
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
            tvNVIDIAInfo.Location = new System.Drawing.Point(8, 22);
            tvNVIDIAInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tvNVIDIAInfo.Name = "tvNVIDIAInfo";
            tvNVIDIAInfo.Size = new System.Drawing.Size(627, 486);
            tvNVIDIAInfo.TabIndex = 0;
            // 
            // NvInfoPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grpNVIDIAInfo);
            Name = "NvInfoPanel";
            Size = new System.Drawing.Size(654, 556);
            grpNVIDIAInfo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpNVIDIAInfo;
        private System.Windows.Forms.Button btnRefreshNVIDIAInfo;
        private System.Windows.Forms.TreeView tvNVIDIAInfo;
    }
}

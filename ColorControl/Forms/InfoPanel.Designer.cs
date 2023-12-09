namespace ColorControl.Forms
{
    partial class InfoPanel
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
            wvInfo = new Microsoft.Web.WebView2.WinForms.WebView2();
            grpInfo = new System.Windows.Forms.GroupBox();
            lbPlugins = new System.Windows.Forms.ListBox();
            label7 = new System.Windows.Forms.Label();
            lblInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)wvInfo).BeginInit();
            grpInfo.SuspendLayout();
            SuspendLayout();
            // 
            // wvInfo
            // 
            wvInfo.AllowExternalDrop = true;
            wvInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            wvInfo.CreationProperties = null;
            wvInfo.DefaultBackgroundColor = System.Drawing.Color.White;
            wvInfo.Location = new System.Drawing.Point(433, 3);
            wvInfo.Name = "wvInfo";
            wvInfo.Size = new System.Drawing.Size(671, 526);
            wvInfo.Source = new System.Uri("https://github.com/Maassoft/ColorControl/releases", System.UriKind.Absolute);
            wvInfo.TabIndex = 8;
            wvInfo.ZoomFactor = 1D;
            // 
            // grpInfo
            // 
            grpInfo.Controls.Add(lbPlugins);
            grpInfo.Controls.Add(label7);
            grpInfo.Controls.Add(lblInfo);
            grpInfo.Location = new System.Drawing.Point(4, 3);
            grpInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpInfo.Name = "grpInfo";
            grpInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpInfo.Size = new System.Drawing.Size(422, 336);
            grpInfo.TabIndex = 7;
            grpInfo.TabStop = false;
            grpInfo.Text = "Info";
            // 
            // lbPlugins
            // 
            lbPlugins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbPlugins.FormattingEnabled = true;
            lbPlugins.HorizontalScrollbar = true;
            lbPlugins.ItemHeight = 15;
            lbPlugins.Location = new System.Drawing.Point(11, 77);
            lbPlugins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lbPlugins.Name = "lbPlugins";
            lbPlugins.Size = new System.Drawing.Size(403, 244);
            lbPlugins.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(8, 59);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(267, 15);
            label7.TabIndex = 2;
            label7.Text = "This app contains the following 3rd party plugins:";
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new System.Drawing.Point(8, 23);
            lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new System.Drawing.Size(28, 15);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "Info";
            // 
            // InfoPanel
            // 
            Controls.Add(wvInfo);
            Controls.Add(grpInfo);
            Name = "InfoPanel";
            Size = new System.Drawing.Size(1110, 534);
            ((System.ComponentModel.ISupportInitialize)wvInfo).EndInit();
            grpInfo.ResumeLayout(false);
            grpInfo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 wvInfo;
        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.ListBox lbPlugins;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblInfo;
    }
}

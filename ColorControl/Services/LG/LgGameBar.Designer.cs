
namespace ColorControl.Services.LG
{
    partial class LgGameBar
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
            this.components = new System.ComponentModel.Container();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTvName = new System.Windows.Forms.Label();
            this.tmrHide = new System.Windows.Forms.Timer(this.components);
            this.lblProcessName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Location = new System.Drawing.Point(14, 47);
            this.flowPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(270, 325);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoteControlForm_MouseDown);
            this.flowPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.flowPanel_MouseMove);
            this.flowPanel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.flowPanel_PreviewKeyDown);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(280, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(16, 27);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "x";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTvName
            // 
            this.lblTvName.AutoSize = true;
            this.lblTvName.ForeColor = System.Drawing.Color.White;
            this.lblTvName.Location = new System.Drawing.Point(14, 0);
            this.lblTvName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTvName.Name = "lblTvName";
            this.lblTvName.Size = new System.Drawing.Size(25, 15);
            this.lblTvName.TabIndex = 0;
            this.lblTvName.Text = "      ";
            // 
            // tmrHide
            // 
            this.tmrHide.Interval = 5000;
            this.tmrHide.Tick += new System.EventHandler(this.tmrHide_Tick);
            // 
            // lblProcessName
            // 
            this.lblProcessName.AutoSize = true;
            this.lblProcessName.ForeColor = System.Drawing.Color.White;
            this.lblProcessName.Location = new System.Drawing.Point(14, 23);
            this.lblProcessName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(25, 15);
            this.lblProcessName.TabIndex = 2;
            this.lblProcessName.Text = "      ";
            // 
            // LgGameBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(298, 387);
            this.Controls.Add(this.lblProcessName);
            this.Controls.Add(this.lblTvName);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flowPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LgGameBar";
            this.Opacity = 0.7D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RemoteControlForm";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.LgGameBar_Activated);
            this.Deactivate += new System.EventHandler(this.LgGameBar_Deactivate);
            this.Load += new System.EventHandler(this.RemoteControlForm_Load);
            this.Shown += new System.EventHandler(this.RemoteControlForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RemoteControlForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RemoteControlForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoteControlForm_MouseDown);
            this.Move += new System.EventHandler(this.LgGameBar_Move);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.RemoteControlForm_PreviewKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTvName;
        private System.Windows.Forms.Timer tmrHide;
        private System.Windows.Forms.Label lblProcessName;
    }
}
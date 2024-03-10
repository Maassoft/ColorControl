
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
            components = new System.ComponentModel.Container();
            flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            btnClose = new System.Windows.Forms.Button();
            lblTvName = new System.Windows.Forms.Label();
            tmrHide = new System.Windows.Forms.Timer(components);
            lblProcessName = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // flowPanel
            // 
            flowPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            flowPanel.Location = new System.Drawing.Point(14, 47);
            flowPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowPanel.Name = "flowPanel";
            flowPanel.Size = new System.Drawing.Size(270, 325);
            flowPanel.TabIndex = 0;
            flowPanel.MouseDown += RemoteControlForm_MouseDown;
            flowPanel.MouseEnter += flowPanel_MouseEnter;
            flowPanel.MouseMove += flowPanel_MouseMove;
            flowPanel.PreviewKeyDown += flowPanel_PreviewKeyDown;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnClose.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.Location = new System.Drawing.Point(280, 0);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(16, 27);
            btnClose.TabIndex = 1;
            btnClose.TabStop = false;
            btnClose.Text = "x";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTvName
            // 
            lblTvName.AutoSize = true;
            lblTvName.ForeColor = System.Drawing.Color.White;
            lblTvName.Location = new System.Drawing.Point(14, 0);
            lblTvName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTvName.Name = "lblTvName";
            lblTvName.Size = new System.Drawing.Size(25, 15);
            lblTvName.TabIndex = 0;
            lblTvName.Text = "      ";
            // 
            // tmrHide
            // 
            tmrHide.Interval = 5000;
            tmrHide.Tick += tmrHide_Tick;
            // 
            // lblProcessName
            // 
            lblProcessName.AutoSize = true;
            lblProcessName.ForeColor = System.Drawing.Color.White;
            lblProcessName.Location = new System.Drawing.Point(14, 23);
            lblProcessName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblProcessName.Name = "lblProcessName";
            lblProcessName.Size = new System.Drawing.Size(25, 15);
            lblProcessName.TabIndex = 2;
            lblProcessName.Text = "      ";
            // 
            // LgGameBar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(298, 387);
            Controls.Add(lblProcessName);
            Controls.Add(lblTvName);
            Controls.Add(btnClose);
            Controls.Add(flowPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LgGameBar";
            Opacity = 0.7D;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "RemoteControlForm";
            TopMost = true;
            Activated += LgGameBar_Activated;
            Deactivate += LgGameBar_Deactivate;
            Load += RemoteControlForm_Load;
            Shown += RemoteControlForm_Shown;
            KeyDown += RemoteControlForm_KeyDown;
            KeyUp += RemoteControlForm_KeyUp;
            MouseDown += RemoteControlForm_MouseDown;
            MouseEnter += LgGameBar_MouseEnter;
            MouseLeave += LgGameBar_MouseLeave;
            Move += LgGameBar_Move;
            PreviewKeyDown += RemoteControlForm_PreviewKeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTvName;
        private System.Windows.Forms.Timer tmrHide;
        private System.Windows.Forms.Label lblProcessName;
    }
}
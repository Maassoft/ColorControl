
namespace ColorControl
{
    partial class RemoteControlForm
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
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblUC = new System.Windows.Forms.Label();
            this.chkDirectMode = new System.Windows.Forms.CheckBox();
            this.pnlMouse = new System.Windows.Forms.Panel();
            this.lblPos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Location = new System.Drawing.Point(12, 16);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(787, 164);
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
            this.btnClose.Location = new System.Drawing.Point(796, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(14, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "x";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblUC
            // 
            this.lblUC.AutoSize = true;
            this.lblUC.ForeColor = System.Drawing.Color.Maroon;
            this.lblUC.Location = new System.Drawing.Point(130, 0);
            this.lblUC.Name = "lblUC";
            this.lblUC.Size = new System.Drawing.Size(100, 13);
            this.lblUC.TabIndex = 0;
            this.lblUC.Text = "Under construction!";
            // 
            // chkDirectMode
            // 
            this.chkDirectMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDirectMode.AutoSize = true;
            this.chkDirectMode.ForeColor = System.Drawing.Color.DarkGray;
            this.chkDirectMode.Location = new System.Drawing.Point(12, 643);
            this.chkDirectMode.Name = "chkDirectMode";
            this.chkDirectMode.Size = new System.Drawing.Size(105, 17);
            this.chkDirectMode.TabIndex = 2;
            this.chkDirectMode.Text = "Direct Mode (F1)";
            this.chkDirectMode.UseVisualStyleBackColor = true;
            this.chkDirectMode.CheckedChanged += new System.EventHandler(this.chkDirectMode_CheckedChanged);
            // 
            // pnlMouse
            // 
            this.pnlMouse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMouse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMouse.Location = new System.Drawing.Point(12, 191);
            this.pnlMouse.Name = "pnlMouse";
            this.pnlMouse.Size = new System.Drawing.Size(787, 446);
            this.pnlMouse.TabIndex = 3;
            this.pnlMouse.MouseEnter += new System.EventHandler(this.pnlMouse_MouseEnter);
            this.pnlMouse.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouse_MouseMove);
            this.pnlMouse.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMouse_MouseUp);
            // 
            // lblPos
            // 
            this.lblPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPos.AutoSize = true;
            this.lblPos.ForeColor = System.Drawing.Color.Maroon;
            this.lblPos.Location = new System.Drawing.Point(719, 643);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(0, 13);
            this.lblPos.TabIndex = 1;
            // 
            // RemoteControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(811, 660);
            this.Controls.Add(this.lblPos);
            this.Controls.Add(this.pnlMouse);
            this.Controls.Add(this.chkDirectMode);
            this.Controls.Add(this.lblUC);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flowPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteControlForm";
            this.Opacity = 0.8D;
            this.Text = "RemoteControlForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RemoteControlForm_FormClosed);
            this.Load += new System.EventHandler(this.RemoteControlForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RemoteControlForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RemoteControlForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoteControlForm_MouseDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.RemoteControlForm_PreviewKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblUC;
        private System.Windows.Forms.CheckBox chkDirectMode;
        private System.Windows.Forms.Panel pnlMouse;
        private System.Windows.Forms.Label lblPos;
    }
}
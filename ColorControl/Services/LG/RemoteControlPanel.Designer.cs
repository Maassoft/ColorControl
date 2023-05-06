
namespace ColorControl.Services.LG
{
    partial class RemoteControlPanel
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
            pbRemote = new System.Windows.Forms.PictureBox();
            btnRcDirect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pbRemote).BeginInit();
            SuspendLayout();
            // 
            // pbRemote
            // 
            pbRemote.BackColor = System.Drawing.Color.Transparent;
            pbRemote.Dock = System.Windows.Forms.DockStyle.Fill;
            pbRemote.Image = Properties.Resources.LG_remote;
            pbRemote.Location = new System.Drawing.Point(0, 0);
            pbRemote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbRemote.Name = "pbRemote";
            pbRemote.Size = new System.Drawing.Size(150, 540);
            pbRemote.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbRemote.TabIndex = 0;
            pbRemote.TabStop = false;
            pbRemote.Click += pbRemote_Click;
            pbRemote.MouseMove += pbRemote_MouseMove;
            // 
            // btnRcDirect
            // 
            btnRcDirect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRcDirect.Location = new System.Drawing.Point(83, 8);
            btnRcDirect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRcDirect.Name = "btnRcDirect";
            btnRcDirect.Size = new System.Drawing.Size(64, 27);
            btnRcDirect.TabIndex = 3;
            btnRcDirect.Text = "Direct";
            btnRcDirect.UseVisualStyleBackColor = true;
            btnRcDirect.Click += button1_Click;
            // 
            // RemoteControlPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(btnRcDirect);
            Controls.Add(pbRemote);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "RemoteControlPanel";
            Size = new System.Drawing.Size(150, 540);
            Load += RemoteControlPanel_Load;
            ((System.ComponentModel.ISupportInitialize)pbRemote).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pbRemote;
        private System.Windows.Forms.Button btnRcDirect;
    }
}

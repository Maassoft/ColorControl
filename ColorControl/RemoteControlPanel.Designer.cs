
namespace ColorControl
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
            this.pbRemote = new System.Windows.Forms.PictureBox();
            this.btnRcDirect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbRemote)).BeginInit();
            this.SuspendLayout();
            // 
            // pbRemote
            // 
            this.pbRemote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbRemote.Image = global::ColorControl.Properties.Resources.LG_remote;
            this.pbRemote.Location = new System.Drawing.Point(0, 0);
            this.pbRemote.Name = "pbRemote";
            this.pbRemote.Size = new System.Drawing.Size(129, 468);
            this.pbRemote.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRemote.TabIndex = 0;
            this.pbRemote.TabStop = false;
            this.pbRemote.Click += new System.EventHandler(this.pbRemote_Click);
            this.pbRemote.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbRemote_MouseMove);
            // 
            // btnRcDirect
            // 
            this.btnRcDirect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRcDirect.Location = new System.Drawing.Point(71, 7);
            this.btnRcDirect.Name = "btnRcDirect";
            this.btnRcDirect.Size = new System.Drawing.Size(55, 23);
            this.btnRcDirect.TabIndex = 3;
            this.btnRcDirect.Text = "Direct";
            this.btnRcDirect.UseVisualStyleBackColor = true;
            this.btnRcDirect.Click += new System.EventHandler(this.button1_Click);
            // 
            // RemoteControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnRcDirect);
            this.Controls.Add(this.pbRemote);
            this.Name = "RemoteControlPanel";
            this.Size = new System.Drawing.Size(129, 468);
            this.Load += new System.EventHandler(this.RemoteControlPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbRemote)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbRemote;
        private System.Windows.Forms.Button btnRcDirect;
    }
}

namespace ColorControl
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tcMain = new System.Windows.Forms.TabControl();
            tabInfo = new System.Windows.Forms.TabPage();
            tabOptions = new System.Windows.Forms.TabPage();
            btnUpdate = new System.Windows.Forms.Button();
            tcMain.SuspendLayout();
            tabOptions.SuspendLayout();
            SuspendLayout();
            // 
            // tcMain
            // 
            tcMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tcMain.Controls.Add(tabOptions);
            tcMain.Controls.Add(tabInfo);
            tcMain.Location = new System.Drawing.Point(14, 14);
            tcMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new System.Drawing.Size(1122, 567);
            tcMain.TabIndex = 1;
            tcMain.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabOptions
            // 
            tabOptions.Location = new System.Drawing.Point(4, 24);
            tabOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabOptions.Size = new System.Drawing.Size(1114, 539);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Options";
            tabOptions.UseVisualStyleBackColor = true;
            // 
            // tabInfo
            // 
            tabInfo.Location = new System.Drawing.Point(4, 24);
            tabInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabInfo.Name = "tabInfo";
            tabInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabInfo.Size = new System.Drawing.Size(1114, 539);
            tabInfo.TabIndex = 4;
            tabInfo.Text = "Info";
            tabInfo.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnUpdate.ForeColor = System.Drawing.Color.Red;
            btnUpdate.Location = new System.Drawing.Point(1055, 0);
            btnUpdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new System.Drawing.Size(77, 26);
            btnUpdate.TabIndex = 61;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Visible = false;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(1150, 594);
            Controls.Add(btnUpdate);
            Controls.Add(tcMain);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(1140, 611);
            Name = "MainForm";
            Text = "ColorControl";
            Activated += MainForm_Activated;
            Deactivate += MainForm_Deactivate;
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Shown += MainForm_Shown;
            ResizeBegin += MainForm_ResizeBegin;
            ResizeEnd += MainForm_ResizeEnd;
            Click += MainForm_Click;
            Resize += MainForm_Resize;
            tcMain.ResumeLayout(false);
            tabOptions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.Button btnUpdate;
    }
}


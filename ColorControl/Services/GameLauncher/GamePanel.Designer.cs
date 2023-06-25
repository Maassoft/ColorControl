namespace ColorControl.Services.GameLauncher
{
    partial class GamePanel
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
            components = new System.ComponentModel.Container();
            cbxGameStepType = new System.Windows.Forms.ComboBox();
            btnGameOptions = new System.Windows.Forms.Button();
            mnuGameOptions = new System.Windows.Forms.ContextMenuStrip(components);
            miGameProcessorAffinity = new System.Windows.Forms.ToolStripMenuItem();
            miGameProcessPriority = new System.Windows.Forms.ToolStripMenuItem();
            chkGameQuickAccess = new System.Windows.Forms.CheckBox();
            btnGameSettings = new System.Windows.Forms.Button();
            mnuGameActions = new System.Windows.Forms.ContextMenuStrip(components);
            mnuGameNvInspector = new System.Windows.Forms.ToolStripMenuItem();
            miGameSetQuickAccessShortcut = new System.Windows.Forms.ToolStripMenuItem();
            chkGameRunAsAdmin = new System.Windows.Forms.CheckBox();
            lblGameParameters = new System.Windows.Forms.Label();
            edtGameParameters = new System.Windows.Forms.TextBox();
            btnGameBrowse = new System.Windows.Forms.Button();
            lblGameFilePath = new System.Windows.Forms.Label();
            edtGamePath = new System.Windows.Forms.TextBox();
            edtGamePrelaunchSteps = new System.Windows.Forms.TextBox();
            btnGameAddStep = new System.Windows.Forms.Button();
            mnuGameAddStep = new System.Windows.Forms.ContextMenuStrip(components);
            mnuGameNvidiaPresets = new System.Windows.Forms.ToolStripMenuItem();
            mnuGameAmdPresets = new System.Windows.Forms.ToolStripMenuItem();
            mnuGameLgPresets = new System.Windows.Forms.ToolStripMenuItem();
            mnuGameStartProgram = new System.Windows.Forms.ToolStripMenuItem();
            label4 = new System.Windows.Forms.Label();
            edtGameName = new System.Windows.Forms.TextBox();
            btnGameDelete = new System.Windows.Forms.Button();
            btnGameClone = new System.Windows.Forms.Button();
            btnGameSave = new System.Windows.Forms.Button();
            btnGameAdd = new System.Windows.Forms.Button();
            btnGameLaunch = new System.Windows.Forms.Button();
            lvGamePresets = new System.Windows.Forms.ListView();
            mnuGameSamsungPresets = new System.Windows.Forms.ToolStripMenuItem();
            mnuGameOptions.SuspendLayout();
            mnuGameActions.SuspendLayout();
            mnuGameAddStep.SuspendLayout();
            SuspendLayout();
            // 
            // cbxGameStepType
            // 
            cbxGameStepType.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            cbxGameStepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxGameStepType.Enabled = false;
            cbxGameStepType.FormattingEnabled = true;
            cbxGameStepType.Location = new System.Drawing.Point(10, 474);
            cbxGameStepType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxGameStepType.Name = "cbxGameStepType";
            cbxGameStepType.Size = new System.Drawing.Size(136, 23);
            cbxGameStepType.TabIndex = 46;
            cbxGameStepType.SelectedIndexChanged += cbxGameStepType_SelectedIndexChanged;
            // 
            // btnGameOptions
            // 
            btnGameOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameOptions.ContextMenuStrip = mnuGameOptions;
            btnGameOptions.Enabled = false;
            btnGameOptions.Location = new System.Drawing.Point(785, 439);
            btnGameOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameOptions.Name = "btnGameOptions";
            btnGameOptions.Size = new System.Drawing.Size(88, 27);
            btnGameOptions.TabIndex = 45;
            btnGameOptions.Text = "Options...";
            btnGameOptions.UseVisualStyleBackColor = true;
            btnGameOptions.Click += btnGameOptions_Click;
            // 
            // mnuGameOptions
            // 
            mnuGameOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuGameOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miGameProcessorAffinity, miGameProcessPriority });
            mnuGameOptions.Name = "mnuLgButtons";
            mnuGameOptions.Size = new System.Drawing.Size(166, 48);
            // 
            // miGameProcessorAffinity
            // 
            miGameProcessorAffinity.Name = "miGameProcessorAffinity";
            miGameProcessorAffinity.Size = new System.Drawing.Size(165, 22);
            miGameProcessorAffinity.Text = "Processor affinity";
            miGameProcessorAffinity.Click += btnGameProcessAffinity_Click;
            // 
            // miGameProcessPriority
            // 
            miGameProcessPriority.Name = "miGameProcessPriority";
            miGameProcessPriority.Size = new System.Drawing.Size(165, 22);
            miGameProcessPriority.Text = "Process priority";
            miGameProcessPriority.Click += miGameProcessPriority_Click;
            // 
            // chkGameQuickAccess
            // 
            chkGameQuickAccess.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkGameQuickAccess.AutoSize = true;
            chkGameQuickAccess.Enabled = false;
            chkGameQuickAccess.Location = new System.Drawing.Point(353, 387);
            chkGameQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkGameQuickAccess.Name = "chkGameQuickAccess";
            chkGameQuickAccess.Size = new System.Drawing.Size(141, 19);
            chkGameQuickAccess.TabIndex = 44;
            chkGameQuickAccess.Text = "Show in Quick Access";
            chkGameQuickAccess.UseVisualStyleBackColor = true;
            // 
            // btnGameSettings
            // 
            btnGameSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnGameSettings.ContextMenuStrip = mnuGameActions;
            btnGameSettings.Location = new System.Drawing.Point(1018, 351);
            btnGameSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameSettings.Name = "btnGameSettings";
            btnGameSettings.Size = new System.Drawing.Size(88, 27);
            btnGameSettings.TabIndex = 43;
            btnGameSettings.Text = "Settings...";
            btnGameSettings.UseVisualStyleBackColor = true;
            btnGameSettings.Click += btnGameActions_Click;
            // 
            // mnuGameActions
            // 
            mnuGameActions.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuGameActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuGameNvInspector, miGameSetQuickAccessShortcut });
            mnuGameActions.Name = "mnuLgButtons";
            mnuGameActions.Size = new System.Drawing.Size(212, 48);
            mnuGameActions.Opening += mnuGameActions_Opening;
            // 
            // mnuGameNvInspector
            // 
            mnuGameNvInspector.Name = "mnuGameNvInspector";
            mnuGameNvInspector.Size = new System.Drawing.Size(211, 22);
            mnuGameNvInspector.Text = "NVIDIA Profile Inspector";
            mnuGameNvInspector.Click += mnuGameNvInspector_Click;
            // 
            // miGameSetQuickAccessShortcut
            // 
            miGameSetQuickAccessShortcut.Name = "miGameSetQuickAccessShortcut";
            miGameSetQuickAccessShortcut.Size = new System.Drawing.Size(211, 22);
            miGameSetQuickAccessShortcut.Text = "Set Quick Access Shortcut";
            miGameSetQuickAccessShortcut.Click += miGameSetQuickAccessShortcut_Click;
            // 
            // chkGameRunAsAdmin
            // 
            chkGameRunAsAdmin.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkGameRunAsAdmin.AutoSize = true;
            chkGameRunAsAdmin.Enabled = false;
            chkGameRunAsAdmin.Location = new System.Drawing.Point(881, 415);
            chkGameRunAsAdmin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkGameRunAsAdmin.Name = "chkGameRunAsAdmin";
            chkGameRunAsAdmin.Size = new System.Drawing.Size(135, 19);
            chkGameRunAsAdmin.TabIndex = 42;
            chkGameRunAsAdmin.Text = "Run as administrator";
            chkGameRunAsAdmin.UseVisualStyleBackColor = true;
            // 
            // lblGameParameters
            // 
            lblGameParameters.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblGameParameters.AutoSize = true;
            lblGameParameters.Location = new System.Drawing.Point(8, 445);
            lblGameParameters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblGameParameters.Name = "lblGameParameters";
            lblGameParameters.Size = new System.Drawing.Size(69, 15);
            lblGameParameters.TabIndex = 41;
            lblGameParameters.Text = "Parameters:";
            // 
            // edtGameParameters
            // 
            edtGameParameters.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtGameParameters.Enabled = false;
            edtGameParameters.Location = new System.Drawing.Point(154, 442);
            edtGameParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtGameParameters.Name = "edtGameParameters";
            edtGameParameters.Size = new System.Drawing.Size(623, 23);
            edtGameParameters.TabIndex = 40;
            // 
            // btnGameBrowse
            // 
            btnGameBrowse.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameBrowse.Enabled = false;
            btnGameBrowse.Location = new System.Drawing.Point(785, 410);
            btnGameBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameBrowse.Name = "btnGameBrowse";
            btnGameBrowse.Size = new System.Drawing.Size(88, 27);
            btnGameBrowse.TabIndex = 39;
            btnGameBrowse.Text = "Browse...";
            btnGameBrowse.UseVisualStyleBackColor = true;
            btnGameBrowse.Click += btnGameBrowse_Click;
            // 
            // lblGameFilePath
            // 
            lblGameFilePath.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblGameFilePath.AutoSize = true;
            lblGameFilePath.Location = new System.Drawing.Point(8, 417);
            lblGameFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblGameFilePath.Name = "lblGameFilePath";
            lblGameFilePath.Size = new System.Drawing.Size(51, 15);
            lblGameFilePath.TabIndex = 38;
            lblGameFilePath.Text = "File/URI:";
            // 
            // edtGamePath
            // 
            edtGamePath.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtGamePath.Enabled = false;
            edtGamePath.Location = new System.Drawing.Point(154, 413);
            edtGamePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtGamePath.Name = "edtGamePath";
            edtGamePath.Size = new System.Drawing.Size(623, 23);
            edtGamePath.TabIndex = 37;
            // 
            // edtGamePrelaunchSteps
            // 
            edtGamePrelaunchSteps.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtGamePrelaunchSteps.Enabled = false;
            edtGamePrelaunchSteps.Location = new System.Drawing.Point(154, 471);
            edtGamePrelaunchSteps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtGamePrelaunchSteps.Multiline = true;
            edtGamePrelaunchSteps.Name = "edtGamePrelaunchSteps";
            edtGamePrelaunchSteps.Size = new System.Drawing.Size(623, 48);
            edtGamePrelaunchSteps.TabIndex = 35;
            edtGamePrelaunchSteps.Leave += edtGamePrelaunchSteps_Leave;
            // 
            // btnGameAddStep
            // 
            btnGameAddStep.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameAddStep.ContextMenuStrip = mnuGameAddStep;
            btnGameAddStep.Enabled = false;
            btnGameAddStep.Location = new System.Drawing.Point(785, 470);
            btnGameAddStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameAddStep.Name = "btnGameAddStep";
            btnGameAddStep.Size = new System.Drawing.Size(88, 27);
            btnGameAddStep.TabIndex = 36;
            btnGameAddStep.Text = "Add step";
            btnGameAddStep.UseVisualStyleBackColor = true;
            btnGameAddStep.Click += btnGameAddStep_Click;
            // 
            // mnuGameAddStep
            // 
            mnuGameAddStep.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuGameAddStep.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuGameNvidiaPresets, mnuGameAmdPresets, mnuGameLgPresets, mnuGameSamsungPresets, mnuGameStartProgram });
            mnuGameAddStep.Name = "mnuLgButtons";
            mnuGameAddStep.Size = new System.Drawing.Size(181, 136);
            mnuGameAddStep.Opening += mnuGameAddStep_Opening;
            // 
            // mnuGameNvidiaPresets
            // 
            mnuGameNvidiaPresets.Name = "mnuGameNvidiaPresets";
            mnuGameNvidiaPresets.Size = new System.Drawing.Size(180, 22);
            mnuGameNvidiaPresets.Text = "NVIDIA presets";
            // 
            // mnuGameAmdPresets
            // 
            mnuGameAmdPresets.Name = "mnuGameAmdPresets";
            mnuGameAmdPresets.Size = new System.Drawing.Size(180, 22);
            mnuGameAmdPresets.Text = "AMD presets";
            // 
            // mnuGameLgPresets
            // 
            mnuGameLgPresets.Name = "mnuGameLgPresets";
            mnuGameLgPresets.Size = new System.Drawing.Size(180, 22);
            mnuGameLgPresets.Text = "LG presets";
            // 
            // mnuGameStartProgram
            // 
            mnuGameStartProgram.Name = "mnuGameStartProgram";
            mnuGameStartProgram.Size = new System.Drawing.Size(180, 22);
            mnuGameStartProgram.Text = "Start Program";
            mnuGameStartProgram.Click += mnuGameStartProgram_Click;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(8, 388);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(42, 15);
            label4.TabIndex = 33;
            label4.Text = "Name:";
            // 
            // edtGameName
            // 
            edtGameName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            edtGameName.Enabled = false;
            edtGameName.Location = new System.Drawing.Point(154, 384);
            edtGameName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            edtGameName.Name = "edtGameName";
            edtGameName.Size = new System.Drawing.Size(191, 23);
            edtGameName.TabIndex = 32;
            // 
            // btnGameDelete
            // 
            btnGameDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameDelete.Enabled = false;
            btnGameDelete.Location = new System.Drawing.Point(291, 351);
            btnGameDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameDelete.Name = "btnGameDelete";
            btnGameDelete.Size = new System.Drawing.Size(88, 27);
            btnGameDelete.TabIndex = 30;
            btnGameDelete.Text = "Delete";
            btnGameDelete.UseVisualStyleBackColor = true;
            btnGameDelete.Click += btnGameDelete_Click;
            // 
            // btnGameClone
            // 
            btnGameClone.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameClone.Enabled = false;
            btnGameClone.Location = new System.Drawing.Point(103, 351);
            btnGameClone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameClone.Name = "btnGameClone";
            btnGameClone.Size = new System.Drawing.Size(88, 27);
            btnGameClone.TabIndex = 29;
            btnGameClone.Text = "Clone";
            btnGameClone.UseVisualStyleBackColor = true;
            btnGameClone.Click += btnGameClone_Click;
            // 
            // btnGameSave
            // 
            btnGameSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameSave.Enabled = false;
            btnGameSave.Location = new System.Drawing.Point(387, 351);
            btnGameSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameSave.Name = "btnGameSave";
            btnGameSave.Size = new System.Drawing.Size(88, 27);
            btnGameSave.TabIndex = 28;
            btnGameSave.Text = "Save";
            btnGameSave.UseVisualStyleBackColor = true;
            btnGameSave.Click += btnGameSave_Click;
            // 
            // btnGameAdd
            // 
            btnGameAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameAdd.Location = new System.Drawing.Point(199, 351);
            btnGameAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameAdd.Name = "btnGameAdd";
            btnGameAdd.Size = new System.Drawing.Size(88, 27);
            btnGameAdd.TabIndex = 25;
            btnGameAdd.Text = "Add";
            btnGameAdd.UseVisualStyleBackColor = true;
            btnGameAdd.Click += btnGameAdd_Click;
            // 
            // btnGameLaunch
            // 
            btnGameLaunch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGameLaunch.Enabled = false;
            btnGameLaunch.Location = new System.Drawing.Point(8, 351);
            btnGameLaunch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGameLaunch.Name = "btnGameLaunch";
            btnGameLaunch.Size = new System.Drawing.Size(88, 27);
            btnGameLaunch.TabIndex = 24;
            btnGameLaunch.Text = "Launch";
            btnGameLaunch.UseVisualStyleBackColor = true;
            btnGameLaunch.Click += btnGameLaunch_Click;
            // 
            // lvGamePresets
            // 
            lvGamePresets.AllowDrop = true;
            lvGamePresets.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvGamePresets.CheckBoxes = true;
            lvGamePresets.FullRowSelect = true;
            lvGamePresets.Location = new System.Drawing.Point(8, 8);
            lvGamePresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvGamePresets.MultiSelect = false;
            lvGamePresets.Name = "lvGamePresets";
            lvGamePresets.ShowItemToolTips = true;
            lvGamePresets.Size = new System.Drawing.Size(1098, 335);
            lvGamePresets.TabIndex = 23;
            lvGamePresets.UseCompatibleStateImageBehavior = false;
            lvGamePresets.View = System.Windows.Forms.View.Details;
            lvGamePresets.ColumnClick += lvLgPresets_ColumnClick;
            lvGamePresets.ItemChecked += lvGamePresets_ItemChecked;
            lvGamePresets.SelectedIndexChanged += lvGamePresets_SelectedIndexChanged;
            lvGamePresets.DragDrop += lvGamePresets_DragDrop;
            lvGamePresets.DragEnter += lvGamePresets_DragEnter;
            lvGamePresets.DoubleClick += lvGamePresets_DoubleClick;
            // 
            // mnuGameSamsungPresets
            // 
            mnuGameSamsungPresets.Name = "mnuGameSamsungPresets";
            mnuGameSamsungPresets.Size = new System.Drawing.Size(180, 22);
            mnuGameSamsungPresets.Text = "Samsung presets";
            // 
            // GamePanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(cbxGameStepType);
            Controls.Add(btnGameOptions);
            Controls.Add(chkGameQuickAccess);
            Controls.Add(btnGameSettings);
            Controls.Add(chkGameRunAsAdmin);
            Controls.Add(lblGameParameters);
            Controls.Add(edtGameParameters);
            Controls.Add(btnGameBrowse);
            Controls.Add(lblGameFilePath);
            Controls.Add(edtGamePath);
            Controls.Add(edtGamePrelaunchSteps);
            Controls.Add(btnGameAddStep);
            Controls.Add(label4);
            Controls.Add(edtGameName);
            Controls.Add(btnGameDelete);
            Controls.Add(btnGameClone);
            Controls.Add(btnGameSave);
            Controls.Add(btnGameAdd);
            Controls.Add(btnGameLaunch);
            Controls.Add(lvGamePresets);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "GamePanel";
            Size = new System.Drawing.Size(1114, 539);
            mnuGameOptions.ResumeLayout(false);
            mnuGameActions.ResumeLayout(false);
            mnuGameAddStep.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblGameParameters;
        private System.Windows.Forms.TextBox edtGameParameters;
        private System.Windows.Forms.Button btnGameBrowse;
        private System.Windows.Forms.Label lblGameFilePath;
        private System.Windows.Forms.TextBox edtGamePath;
        private System.Windows.Forms.TextBox edtGamePrelaunchSteps;
        private System.Windows.Forms.Button btnGameAddStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox edtGameName;
        private System.Windows.Forms.Button btnGameDelete;
        private System.Windows.Forms.Button btnGameClone;
        private System.Windows.Forms.Button btnGameSave;
        private System.Windows.Forms.Button btnGameAdd;
        private System.Windows.Forms.Button btnGameLaunch;
        private System.Windows.Forms.ListView lvGamePresets;
        private System.Windows.Forms.ContextMenuStrip mnuGameAddStep;
        private System.Windows.Forms.ToolStripMenuItem mnuGameNvidiaPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuGameAmdPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuGameLgPresets;
        private System.Windows.Forms.CheckBox chkGameRunAsAdmin;
        private System.Windows.Forms.ToolStripMenuItem mnuGameStartProgram;
        private System.Windows.Forms.Button btnGameSettings;
        private System.Windows.Forms.ContextMenuStrip mnuGameActions;
        private System.Windows.Forms.ToolStripMenuItem mnuGameNvInspector;
        private System.Windows.Forms.CheckBox chkGameQuickAccess;
        private System.Windows.Forms.ToolStripMenuItem miGameSetQuickAccessShortcut;
        private System.Windows.Forms.Button btnGameOptions;
        private System.Windows.Forms.ContextMenuStrip mnuGameOptions;
        private System.Windows.Forms.ToolStripMenuItem miGameProcessorAffinity;
        private System.Windows.Forms.ToolStripMenuItem miGameProcessPriority;
        private System.Windows.Forms.ComboBox cbxGameStepType;
        private System.Windows.Forms.ToolStripMenuItem mnuGameSamsungPresets;
    }
}

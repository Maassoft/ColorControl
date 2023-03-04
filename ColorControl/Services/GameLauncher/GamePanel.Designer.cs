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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            this.cbxGameStepType = new System.Windows.Forms.ComboBox();
            this.btnGameOptions = new System.Windows.Forms.Button();
            this.mnuGameOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miGameProcessorAffinity = new System.Windows.Forms.ToolStripMenuItem();
            this.miGameProcessPriority = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGameQuickAccess = new System.Windows.Forms.CheckBox();
            this.btnGameSettings = new System.Windows.Forms.Button();
            this.mnuGameActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGameNvInspector = new System.Windows.Forms.ToolStripMenuItem();
            this.miGameSetQuickAccessShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGameRunAsAdmin = new System.Windows.Forms.CheckBox();
            this.lblGameParameters = new System.Windows.Forms.Label();
            this.edtGameParameters = new System.Windows.Forms.TextBox();
            this.btnGameBrowse = new System.Windows.Forms.Button();
            this.lblGameFilePath = new System.Windows.Forms.Label();
            this.edtGamePath = new System.Windows.Forms.TextBox();
            this.edtGamePrelaunchSteps = new System.Windows.Forms.TextBox();
            this.btnGameAddStep = new System.Windows.Forms.Button();
            this.mnuGameAddStep = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGameNvidiaPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameAmdPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameLgPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGameStartProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.edtGameName = new System.Windows.Forms.TextBox();
            this.btnGameDelete = new System.Windows.Forms.Button();
            this.btnGameClone = new System.Windows.Forms.Button();
            this.btnGameSave = new System.Windows.Forms.Button();
            this.btnGameAdd = new System.Windows.Forms.Button();
            this.btnGameLaunch = new System.Windows.Forms.Button();
            this.lvGamePresets = new System.Windows.Forms.ListView();

            this.mnuGameOptions.SuspendLayout();
            this.mnuGameActions.SuspendLayout();
            this.mnuGameAddStep.SuspendLayout();

            this.SuspendLayout();
            // 
            // GamePanel
            // 

            this.Controls.Add(this.cbxGameStepType);
            this.Controls.Add(this.btnGameOptions);
            this.Controls.Add(this.chkGameQuickAccess);
            this.Controls.Add(this.btnGameSettings);
            this.Controls.Add(this.chkGameRunAsAdmin);
            this.Controls.Add(this.lblGameParameters);
            this.Controls.Add(this.edtGameParameters);
            this.Controls.Add(this.btnGameBrowse);
            this.Controls.Add(this.lblGameFilePath);
            this.Controls.Add(this.edtGamePath);
            this.Controls.Add(this.edtGamePrelaunchSteps);
            this.Controls.Add(this.btnGameAddStep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.edtGameName);
            this.Controls.Add(this.btnGameDelete);
            this.Controls.Add(this.btnGameClone);
            this.Controls.Add(this.btnGameSave);
            this.Controls.Add(this.btnGameAdd);
            this.Controls.Add(this.btnGameLaunch);
            this.Controls.Add(this.lvGamePresets);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GamePanel";
            this.Size = new System.Drawing.Size(1114, 539);

            // 
            // cbxGameStepType
            // 
            this.cbxGameStepType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxGameStepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGameStepType.Enabled = false;
            this.cbxGameStepType.FormattingEnabled = true;
            this.cbxGameStepType.Location = new System.Drawing.Point(10, 474);
            this.cbxGameStepType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxGameStepType.Name = "cbxGameStepType";
            this.cbxGameStepType.Size = new System.Drawing.Size(136, 23);
            this.cbxGameStepType.TabIndex = 46;
            this.cbxGameStepType.SelectedIndexChanged += new System.EventHandler(this.cbxGameStepType_SelectedIndexChanged);
            // 
            // btnGameOptions
            // 
            this.btnGameOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameOptions.ContextMenuStrip = this.mnuGameOptions;
            this.btnGameOptions.Enabled = false;
            this.btnGameOptions.Location = new System.Drawing.Point(785, 439);
            this.btnGameOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameOptions.Name = "btnGameOptions";
            this.btnGameOptions.Size = new System.Drawing.Size(88, 27);
            this.btnGameOptions.TabIndex = 45;
            this.btnGameOptions.Text = "Options...";
            this.btnGameOptions.UseVisualStyleBackColor = true;
            this.btnGameOptions.Click += new System.EventHandler(this.btnGameOptions_Click);
            // 
            // mnuGameOptions
            // 
            this.mnuGameOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miGameProcessorAffinity,
            this.miGameProcessPriority});
            this.mnuGameOptions.Name = "mnuLgButtons";
            this.mnuGameOptions.Size = new System.Drawing.Size(166, 48);
            // 
            // miGameProcessorAffinity
            // 
            this.miGameProcessorAffinity.Name = "miGameProcessorAffinity";
            this.miGameProcessorAffinity.Size = new System.Drawing.Size(165, 22);
            this.miGameProcessorAffinity.Text = "Processor affinity";
            this.miGameProcessorAffinity.Click += new System.EventHandler(this.btnGameProcessAffinity_Click);
            // 
            // miGameProcessPriority
            // 
            this.miGameProcessPriority.Name = "miGameProcessPriority";
            this.miGameProcessPriority.Size = new System.Drawing.Size(165, 22);
            this.miGameProcessPriority.Text = "Process priority";
            this.miGameProcessPriority.Click += new System.EventHandler(this.miGameProcessPriority_Click);
            // 
            // chkGameQuickAccess
            // 
            this.chkGameQuickAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGameQuickAccess.AutoSize = true;
            this.chkGameQuickAccess.Enabled = false;
            this.chkGameQuickAccess.Location = new System.Drawing.Point(353, 387);
            this.chkGameQuickAccess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkGameQuickAccess.Name = "chkGameQuickAccess";
            this.chkGameQuickAccess.Size = new System.Drawing.Size(141, 19);
            this.chkGameQuickAccess.TabIndex = 44;
            this.chkGameQuickAccess.Text = "Show in Quick Access";
            this.chkGameQuickAccess.UseVisualStyleBackColor = true;
            // 
            // btnGameSettings
            // 
            this.btnGameSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGameSettings.ContextMenuStrip = this.mnuGameActions;
            this.btnGameSettings.Location = new System.Drawing.Point(1018, 351);
            this.btnGameSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameSettings.Name = "btnGameSettings";
            this.btnGameSettings.Size = new System.Drawing.Size(88, 27);
            this.btnGameSettings.TabIndex = 43;
            this.btnGameSettings.Text = "Settings...";
            this.btnGameSettings.UseVisualStyleBackColor = true;
            this.btnGameSettings.Click += new System.EventHandler(this.btnGameActions_Click);
            // 
            // mnuGameActions
            // 
            this.mnuGameActions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGameNvInspector,
            this.miGameSetQuickAccessShortcut});
            this.mnuGameActions.Name = "mnuLgButtons";
            this.mnuGameActions.Size = new System.Drawing.Size(212, 48);
            this.mnuGameActions.Opening += new System.ComponentModel.CancelEventHandler(this.mnuGameActions_Opening);
            // 
            // mnuGameNvInspector
            // 
            this.mnuGameNvInspector.Name = "mnuGameNvInspector";
            this.mnuGameNvInspector.Size = new System.Drawing.Size(211, 22);
            this.mnuGameNvInspector.Text = "NVIDIA Profile Inspector";
            this.mnuGameNvInspector.Click += new System.EventHandler(this.mnuGameNvInspector_Click);
            // 
            // miGameSetQuickAccessShortcut
            // 
            this.miGameSetQuickAccessShortcut.Name = "miGameSetQuickAccessShortcut";
            this.miGameSetQuickAccessShortcut.Size = new System.Drawing.Size(211, 22);
            this.miGameSetQuickAccessShortcut.Text = "Set Quick Access Shortcut";
            this.miGameSetQuickAccessShortcut.Click += new System.EventHandler(this.miGameSetQuickAccessShortcut_Click);
            // 
            // chkGameRunAsAdmin
            // 
            this.chkGameRunAsAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGameRunAsAdmin.AutoSize = true;
            this.chkGameRunAsAdmin.Enabled = false;
            this.chkGameRunAsAdmin.Location = new System.Drawing.Point(881, 415);
            this.chkGameRunAsAdmin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkGameRunAsAdmin.Name = "chkGameRunAsAdmin";
            this.chkGameRunAsAdmin.Size = new System.Drawing.Size(135, 19);
            this.chkGameRunAsAdmin.TabIndex = 42;
            this.chkGameRunAsAdmin.Text = "Run as administrator";
            this.chkGameRunAsAdmin.UseVisualStyleBackColor = true;
            // 
            // lblGameParameters
            // 
            this.lblGameParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGameParameters.AutoSize = true;
            this.lblGameParameters.Location = new System.Drawing.Point(8, 445);
            this.lblGameParameters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameParameters.Name = "lblGameParameters";
            this.lblGameParameters.Size = new System.Drawing.Size(69, 15);
            this.lblGameParameters.TabIndex = 41;
            this.lblGameParameters.Text = "Parameters:";
            // 
            // edtGameParameters
            // 
            this.edtGameParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGameParameters.Enabled = false;
            this.edtGameParameters.Location = new System.Drawing.Point(154, 442);
            this.edtGameParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGameParameters.Name = "edtGameParameters";
            this.edtGameParameters.Size = new System.Drawing.Size(623, 23);
            this.edtGameParameters.TabIndex = 40;
            // 
            // btnGameBrowse
            // 
            this.btnGameBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameBrowse.Enabled = false;
            this.btnGameBrowse.Location = new System.Drawing.Point(785, 410);
            this.btnGameBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameBrowse.Name = "btnGameBrowse";
            this.btnGameBrowse.Size = new System.Drawing.Size(88, 27);
            this.btnGameBrowse.TabIndex = 39;
            this.btnGameBrowse.Text = "Browse...";
            this.btnGameBrowse.UseVisualStyleBackColor = true;
            this.btnGameBrowse.Click += new System.EventHandler(this.btnGameBrowse_Click);
            // 
            // lblGameFilePath
            // 
            this.lblGameFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGameFilePath.AutoSize = true;
            this.lblGameFilePath.Location = new System.Drawing.Point(8, 417);
            this.lblGameFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameFilePath.Name = "lblGameFilePath";
            this.lblGameFilePath.Size = new System.Drawing.Size(51, 15);
            this.lblGameFilePath.TabIndex = 38;
            this.lblGameFilePath.Text = "File/URI:";
            // 
            // edtGamePath
            // 
            this.edtGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGamePath.Enabled = false;
            this.edtGamePath.Location = new System.Drawing.Point(154, 413);
            this.edtGamePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGamePath.Name = "edtGamePath";
            this.edtGamePath.Size = new System.Drawing.Size(623, 23);
            this.edtGamePath.TabIndex = 37;
            // 
            // edtGamePrelaunchSteps
            // 
            this.edtGamePrelaunchSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGamePrelaunchSteps.Enabled = false;
            this.edtGamePrelaunchSteps.Location = new System.Drawing.Point(154, 471);
            this.edtGamePrelaunchSteps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGamePrelaunchSteps.Multiline = true;
            this.edtGamePrelaunchSteps.Name = "edtGamePrelaunchSteps";
            this.edtGamePrelaunchSteps.Size = new System.Drawing.Size(623, 48);
            this.edtGamePrelaunchSteps.TabIndex = 35;
            this.edtGamePrelaunchSteps.Leave += new System.EventHandler(this.edtGamePrelaunchSteps_Leave);
            // 
            // btnGameAddStep
            // 
            this.btnGameAddStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameAddStep.ContextMenuStrip = this.mnuGameAddStep;
            this.btnGameAddStep.Enabled = false;
            this.btnGameAddStep.Location = new System.Drawing.Point(785, 470);
            this.btnGameAddStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameAddStep.Name = "btnGameAddStep";
            this.btnGameAddStep.Size = new System.Drawing.Size(88, 27);
            this.btnGameAddStep.TabIndex = 36;
            this.btnGameAddStep.Text = "Add step";
            this.btnGameAddStep.UseVisualStyleBackColor = true;
            this.btnGameAddStep.Click += new System.EventHandler(this.btnGameAddStep_Click);
            // 
            // mnuGameAddStep
            // 
            this.mnuGameAddStep.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuGameAddStep.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGameNvidiaPresets,
            this.mnuGameAmdPresets,
            this.mnuGameLgPresets,
            this.mnuGameStartProgram});
            this.mnuGameAddStep.Name = "mnuLgButtons";
            this.mnuGameAddStep.Size = new System.Drawing.Size(153, 92);
            this.mnuGameAddStep.Opening += new System.ComponentModel.CancelEventHandler(this.mnuGameAddStep_Opening);
            // 
            // mnuGameNvidiaPresets
            // 
            this.mnuGameNvidiaPresets.Name = "mnuGameNvidiaPresets";
            this.mnuGameNvidiaPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameNvidiaPresets.Text = "NVIDIA presets";
            // 
            // mnuGameAmdPresets
            // 
            this.mnuGameAmdPresets.Name = "mnuGameAmdPresets";
            this.mnuGameAmdPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameAmdPresets.Text = "AMD presets";
            // 
            // mnuGameLgPresets
            // 
            this.mnuGameLgPresets.Name = "mnuGameLgPresets";
            this.mnuGameLgPresets.Size = new System.Drawing.Size(152, 22);
            this.mnuGameLgPresets.Text = "LG presets";
            // 
            // mnuGameStartProgram
            // 
            this.mnuGameStartProgram.Name = "mnuGameStartProgram";
            this.mnuGameStartProgram.Size = new System.Drawing.Size(152, 22);
            this.mnuGameStartProgram.Text = "Start Program";
            this.mnuGameStartProgram.Click += new System.EventHandler(this.mnuGameStartProgram_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 388);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 33;
            this.label4.Text = "Name:";
            // 
            // edtGameName
            // 
            this.edtGameName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edtGameName.Enabled = false;
            this.edtGameName.Location = new System.Drawing.Point(154, 384);
            this.edtGameName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.edtGameName.Name = "edtGameName";
            this.edtGameName.Size = new System.Drawing.Size(191, 23);
            this.edtGameName.TabIndex = 32;
            // 
            // btnGameDelete
            // 
            this.btnGameDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameDelete.Enabled = false;
            this.btnGameDelete.Location = new System.Drawing.Point(291, 351);
            this.btnGameDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameDelete.Name = "btnGameDelete";
            this.btnGameDelete.Size = new System.Drawing.Size(88, 27);
            this.btnGameDelete.TabIndex = 30;
            this.btnGameDelete.Text = "Delete";
            this.btnGameDelete.UseVisualStyleBackColor = true;
            this.btnGameDelete.Click += new System.EventHandler(this.btnGameDelete_Click);
            // 
            // btnGameClone
            // 
            this.btnGameClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameClone.Enabled = false;
            this.btnGameClone.Location = new System.Drawing.Point(103, 351);
            this.btnGameClone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameClone.Name = "btnGameClone";
            this.btnGameClone.Size = new System.Drawing.Size(88, 27);
            this.btnGameClone.TabIndex = 29;
            this.btnGameClone.Text = "Clone";
            this.btnGameClone.UseVisualStyleBackColor = true;
            this.btnGameClone.Click += new System.EventHandler(this.btnGameClone_Click);
            // 
            // btnGameSave
            // 
            this.btnGameSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameSave.Enabled = false;
            this.btnGameSave.Location = new System.Drawing.Point(387, 351);
            this.btnGameSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameSave.Name = "btnGameSave";
            this.btnGameSave.Size = new System.Drawing.Size(88, 27);
            this.btnGameSave.TabIndex = 28;
            this.btnGameSave.Text = "Save";
            this.btnGameSave.UseVisualStyleBackColor = true;
            this.btnGameSave.Click += new System.EventHandler(this.btnGameSave_Click);
            // 
            // btnGameAdd
            // 
            this.btnGameAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameAdd.Location = new System.Drawing.Point(199, 351);
            this.btnGameAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameAdd.Name = "btnGameAdd";
            this.btnGameAdd.Size = new System.Drawing.Size(88, 27);
            this.btnGameAdd.TabIndex = 25;
            this.btnGameAdd.Text = "Add";
            this.btnGameAdd.UseVisualStyleBackColor = true;
            this.btnGameAdd.Click += new System.EventHandler(this.btnGameAdd_Click);
            // 
            // btnGameLaunch
            // 
            this.btnGameLaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGameLaunch.Enabled = false;
            this.btnGameLaunch.Location = new System.Drawing.Point(8, 351);
            this.btnGameLaunch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGameLaunch.Name = "btnGameLaunch";
            this.btnGameLaunch.Size = new System.Drawing.Size(88, 27);
            this.btnGameLaunch.TabIndex = 24;
            this.btnGameLaunch.Text = "Launch";
            this.btnGameLaunch.UseVisualStyleBackColor = true;
            this.btnGameLaunch.Click += new System.EventHandler(this.btnGameLaunch_Click);
            // 
            // lvGamePresets
            // 
            this.lvGamePresets.AllowDrop = true;
            this.lvGamePresets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvGamePresets.CheckBoxes = true;
            this.lvGamePresets.FullRowSelect = true;
            this.lvGamePresets.Location = new System.Drawing.Point(8, 8);
            this.lvGamePresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvGamePresets.MultiSelect = false;
            this.lvGamePresets.Name = "lvGamePresets";
            this.lvGamePresets.ShowItemToolTips = true;
            this.lvGamePresets.Size = new System.Drawing.Size(1098, 335);
            this.lvGamePresets.TabIndex = 23;
            this.lvGamePresets.UseCompatibleStateImageBehavior = false;
            this.lvGamePresets.View = System.Windows.Forms.View.Details;
            this.lvGamePresets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLgPresets_ColumnClick);
            this.lvGamePresets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvGamePresets_ItemChecked);
            this.lvGamePresets.SelectedIndexChanged += new System.EventHandler(this.lvGamePresets_SelectedIndexChanged);
            this.lvGamePresets.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvGamePresets_DragDrop);
            this.lvGamePresets.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvGamePresets_DragEnter);
            this.lvGamePresets.DoubleClick += new System.EventHandler(this.lvGamePresets_DoubleClick);

            this.mnuGameOptions.ResumeLayout(false);
            this.mnuGameActions.ResumeLayout(false);
            this.mnuGameAddStep.ResumeLayout(false);

            this.ResumeLayout(false);

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
    }
}

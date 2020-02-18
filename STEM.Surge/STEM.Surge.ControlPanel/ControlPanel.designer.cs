namespace STEM.Surge.ControlPanel
{
    partial class ControlPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanel));
            this.branchListGrid = new System.Windows.Forms.DataGridView();
            this.branchStateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OnlineImg = new System.Windows.Forms.DataGridViewImageColumn();
            this.BranchIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.threadsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rAMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.branchMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.takeOffline = new System.Windows.Forms.ToolStripMenuItem();
            this.bringOnline = new System.Windows.Forms.ToolStripMenuItem();
            this.clearErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.viewErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSurge = new System.Windows.Forms.ToolStripMenuItem();
            this.updateConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.branchDetailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.controlPanelMainTools = new System.Windows.Forms.ToolStrip();
            this.switchboardConfig = new System.Windows.Forms.ToolStripButton();
            this.about = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.licenseAccess = new System.Windows.Forms.ToolStripButton();
            this.systemTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.deploymentControllerEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionSetEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editManagerStaticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploreConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadExtensions = new System.Windows.Forms.ToolStripButton();
            this.viewLocks = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.activity = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextHelp = new System.Windows.Forms.ToolStripButton();
            this.curTime = new System.Windows.Forms.ToolStripLabel();
            this.pollerStats = new System.Windows.Forms.ToolStripButton();
            this.grafana = new System.Windows.Forms.ToolStripButton();
            this.adHocInstructionSet = new System.Windows.Forms.ToolStripButton();
            this.connectedDM = new System.Windows.Forms.ToolStripButton();
            this.openControllerDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.startStopService = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelActive = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelBranches = new System.Windows.Forms.Panel();
            this.openExtensions = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sandboxConfigurationUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.branchListGrid)).BeginInit();
            this.branchMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.branchDetailsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).BeginInit();
            this.controlPanelMainTools.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelBranches.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // branchListGrid
            // 
            this.branchListGrid.AllowUserToAddRows = false;
            this.branchListGrid.AllowUserToDeleteRows = false;
            this.branchListGrid.AutoGenerateColumns = false;
            this.branchListGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.branchListGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.branchListGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.branchListGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.branchStateDataGridViewTextBoxColumn,
            this.OnlineImg,
            this.BranchIP,
            this.BranchName,
            this.assignedDataGridViewTextBoxColumn,
            this.processingDataGridViewTextBoxColumn,
            this.errorsDataGridViewTextBoxColumn,
            this.threadsDataGridViewTextBoxColumn,
            this.rAMDataGridViewTextBoxColumn});
            this.branchListGrid.ContextMenuStrip = this.branchMenu;
            this.branchListGrid.DataSource = this.branchDetailsBindingSource;
            this.branchListGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.branchListGrid.GridColor = System.Drawing.Color.LightGray;
            this.branchListGrid.Location = new System.Drawing.Point(0, 0);
            this.branchListGrid.Name = "branchListGrid";
            this.branchListGrid.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.branchListGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.branchListGrid.RowHeadersWidth = 15;
            this.branchListGrid.Size = new System.Drawing.Size(643, 461);
            this.branchListGrid.TabIndex = 0;
            // 
            // branchStateDataGridViewTextBoxColumn
            // 
            this.branchStateDataGridViewTextBoxColumn.DataPropertyName = "BranchState";
            this.branchStateDataGridViewTextBoxColumn.HeaderText = "BranchState";
            this.branchStateDataGridViewTextBoxColumn.Name = "branchStateDataGridViewTextBoxColumn";
            this.branchStateDataGridViewTextBoxColumn.ReadOnly = true;
            this.branchStateDataGridViewTextBoxColumn.Visible = false;
            // 
            // OnlineImg
            // 
            this.OnlineImg.HeaderText = "";
            this.OnlineImg.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.OnlineImg.Name = "OnlineImg";
            this.OnlineImg.ReadOnly = true;
            this.OnlineImg.Width = 43;
            // 
            // BranchIP
            // 
            this.BranchIP.DataPropertyName = "BranchIP";
            this.BranchIP.HeaderText = "IP";
            this.BranchIP.Name = "BranchIP";
            this.BranchIP.ReadOnly = true;
            // 
            // BranchName
            // 
            this.BranchName.DataPropertyName = "BranchName";
            this.BranchName.HeaderText = "Name";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            // 
            // assignedDataGridViewTextBoxColumn
            // 
            this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
            this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
            this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
            this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
            this.assignedDataGridViewTextBoxColumn.Width = 80;
            // 
            // processingDataGridViewTextBoxColumn
            // 
            this.processingDataGridViewTextBoxColumn.DataPropertyName = "Processing";
            this.processingDataGridViewTextBoxColumn.HeaderText = "Processing";
            this.processingDataGridViewTextBoxColumn.Name = "processingDataGridViewTextBoxColumn";
            this.processingDataGridViewTextBoxColumn.ReadOnly = true;
            this.processingDataGridViewTextBoxColumn.Width = 80;
            // 
            // errorsDataGridViewTextBoxColumn
            // 
            this.errorsDataGridViewTextBoxColumn.DataPropertyName = "Errors";
            this.errorsDataGridViewTextBoxColumn.HeaderText = "Errors";
            this.errorsDataGridViewTextBoxColumn.Name = "errorsDataGridViewTextBoxColumn";
            this.errorsDataGridViewTextBoxColumn.ReadOnly = true;
            this.errorsDataGridViewTextBoxColumn.Width = 68;
            // 
            // threadsDataGridViewTextBoxColumn
            // 
            this.threadsDataGridViewTextBoxColumn.DataPropertyName = "Threads";
            this.threadsDataGridViewTextBoxColumn.HeaderText = "Threads";
            this.threadsDataGridViewTextBoxColumn.Name = "threadsDataGridViewTextBoxColumn";
            this.threadsDataGridViewTextBoxColumn.ReadOnly = true;
            this.threadsDataGridViewTextBoxColumn.Width = 68;
            // 
            // rAMDataGridViewTextBoxColumn
            // 
            this.rAMDataGridViewTextBoxColumn.DataPropertyName = "RAM";
            this.rAMDataGridViewTextBoxColumn.HeaderText = "MB RAM";
            this.rAMDataGridViewTextBoxColumn.Name = "rAMDataGridViewTextBoxColumn";
            this.rAMDataGridViewTextBoxColumn.ReadOnly = true;
            this.rAMDataGridViewTextBoxColumn.Width = 72;
            // 
            // branchMenu
            // 
            this.branchMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.branchMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.takeOffline,
            this.bringOnline,
            this.clearErrors,
            this.viewErrors,
            this.updateSurge,
            this.updateConfiguration});
            this.branchMenu.Name = "branchMenu";
            this.branchMenu.Size = new System.Drawing.Size(190, 136);
            // 
            // takeOffline
            // 
            this.takeOffline.Name = "takeOffline";
            this.takeOffline.Size = new System.Drawing.Size(189, 22);
            this.takeOffline.Text = "Take Offline";
            // 
            // bringOnline
            // 
            this.bringOnline.Name = "bringOnline";
            this.bringOnline.Size = new System.Drawing.Size(189, 22);
            this.bringOnline.Text = "Bring Online";
            // 
            // clearErrors
            // 
            this.clearErrors.Name = "clearErrors";
            this.clearErrors.Size = new System.Drawing.Size(189, 22);
            this.clearErrors.Text = "Clear Errors";
            // 
            // viewErrors
            // 
            this.viewErrors.Name = "viewErrors";
            this.viewErrors.Size = new System.Drawing.Size(189, 22);
            this.viewErrors.Text = "View Errors";
            // 
            // updateSurge
            // 
            this.updateSurge.Name = "updateSurge";
            this.updateSurge.Size = new System.Drawing.Size(189, 22);
            this.updateSurge.Text = "Update Surge";
            // 
            // updateConfiguration
            // 
            this.updateConfiguration.Name = "updateConfiguration";
            this.updateConfiguration.Size = new System.Drawing.Size(189, 22);
            this.updateConfiguration.Text = "Update Configuration";
            // 
            // branchDetailsBindingSource
            // 
            this.branchDetailsBindingSource.DataMember = "BranchDetails";
            this.branchDetailsBindingSource.DataSource = this.tableDataSources;
            // 
            // tableDataSources
            // 
            this.tableDataSources.DataSetName = "TableDataSources";
            this.tableDataSources.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // controlPanelMainTools
            // 
            this.controlPanelMainTools.AllowDrop = true;
            this.controlPanelMainTools.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.controlPanelMainTools.GripMargin = new System.Windows.Forms.Padding(0);
            this.controlPanelMainTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.controlPanelMainTools.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.controlPanelMainTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.switchboardConfig,
            this.about,
            this.toolStripSeparator2,
            this.licenseAccess,
            this.systemTools,
            this.uploadExtensions,
            this.viewLocks,
            this.toolStripLabel3,
            this.toolStripSeparator3,
            this.activity,
            this.toolStripSeparator1,
            this.contextHelp,
            this.curTime,
            this.pollerStats,
            this.grafana,
            this.adHocInstructionSet});
            this.controlPanelMainTools.Location = new System.Drawing.Point(0, 0);
            this.controlPanelMainTools.Margin = new System.Windows.Forms.Padding(3);
            this.controlPanelMainTools.Name = "controlPanelMainTools";
            this.controlPanelMainTools.Padding = new System.Windows.Forms.Padding(0);
            this.controlPanelMainTools.Size = new System.Drawing.Size(782, 35);
            this.controlPanelMainTools.TabIndex = 0;
            this.controlPanelMainTools.Text = "toolStrip1";
            // 
            // switchboardConfig
            // 
            this.switchboardConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.switchboardConfig.Image = global::STEM.Surge.ControlPanel.Properties.Resources.edit;
            this.switchboardConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.switchboardConfig.Name = "switchboardConfig";
            this.switchboardConfig.Size = new System.Drawing.Size(28, 32);
            this.switchboardConfig.Text = "Configure Switchboard";
            this.switchboardConfig.Click += new System.EventHandler(this.switchboardConfig_Click);
            // 
            // about
            // 
            this.about.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.about.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.about.Image = global::STEM.Surge.ControlPanel.Properties.Resources.info;
            this.about.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(28, 32);
            this.about.Text = "About";
            this.about.Click += new System.EventHandler(this.about_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // licenseAccess
            // 
            this.licenseAccess.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.licenseAccess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.licenseAccess.Image = global::STEM.Surge.ControlPanel.Properties.Resources.License;
            this.licenseAccess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.licenseAccess.Name = "licenseAccess";
            this.licenseAccess.Size = new System.Drawing.Size(28, 32);
            this.licenseAccess.Text = "License";
            this.licenseAccess.Click += new System.EventHandler(this.licenseAccess_Click);
            // 
            // systemTools
            // 
            this.systemTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.systemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deploymentControllerEditorToolStripMenuItem,
            this.instructionSetEditorToolStripMenuItem,
            this.editManagerStaticsToolStripMenuItem,
            this.changeVersionsToolStripMenuItem,
            this.manageExtensionsToolStripMenuItem,
            this.exploreConfigurationToolStripMenuItem,
            this.sandboxConfigurationUpdateToolStripMenuItem});
            this.systemTools.Image = global::STEM.Surge.ControlPanel.Properties.Resources.tools;
            this.systemTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.systemTools.Name = "systemTools";
            this.systemTools.Size = new System.Drawing.Size(37, 32);
            this.systemTools.Text = "System Tools";
            // 
            // deploymentControllerEditorToolStripMenuItem
            // 
            this.deploymentControllerEditorToolStripMenuItem.Name = "deploymentControllerEditorToolStripMenuItem";
            this.deploymentControllerEditorToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.deploymentControllerEditorToolStripMenuItem.Text = "Deployment Controller Editor";
            this.deploymentControllerEditorToolStripMenuItem.Click += new System.EventHandler(this.deploymentControllerEditorToolStripMenuItem_Click);
            // 
            // instructionSetEditorToolStripMenuItem
            // 
            this.instructionSetEditorToolStripMenuItem.Name = "instructionSetEditorToolStripMenuItem";
            this.instructionSetEditorToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.instructionSetEditorToolStripMenuItem.Text = "Instruction Set Editor";
            this.instructionSetEditorToolStripMenuItem.Click += new System.EventHandler(this.instructionSetEditorToolStripMenuItem_Click);
            // 
            // editManagerStaticsToolStripMenuItem
            // 
            this.editManagerStaticsToolStripMenuItem.Name = "editManagerStaticsToolStripMenuItem";
            this.editManagerStaticsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.editManagerStaticsToolStripMenuItem.Text = "Statics Editor";
            this.editManagerStaticsToolStripMenuItem.Click += new System.EventHandler(this.editManagerStaticsToolStripMenuItem_Click);
            // 
            // changeVersionsToolStripMenuItem
            // 
            this.changeVersionsToolStripMenuItem.Name = "changeVersionsToolStripMenuItem";
            this.changeVersionsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.changeVersionsToolStripMenuItem.Text = "Change Versions";
            this.changeVersionsToolStripMenuItem.Click += new System.EventHandler(this.changeVersionsToolStripMenuItem_Click);
            // 
            // manageExtensionsToolStripMenuItem
            // 
            this.manageExtensionsToolStripMenuItem.Name = "manageExtensionsToolStripMenuItem";
            this.manageExtensionsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.manageExtensionsToolStripMenuItem.Text = "Manage Extensions";
            this.manageExtensionsToolStripMenuItem.Click += new System.EventHandler(this.manageExtensionsToolStripMenuItem_Click);
            // 
            // exploreConfigurationToolStripMenuItem
            // 
            this.exploreConfigurationToolStripMenuItem.Name = "exploreConfigurationToolStripMenuItem";
            this.exploreConfigurationToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.exploreConfigurationToolStripMenuItem.Text = "Explore Configuration";
            this.exploreConfigurationToolStripMenuItem.Click += new System.EventHandler(this.exploreConfigurationToolStripMenuItem_Click);
            // 
            // uploadExtensions
            // 
            this.uploadExtensions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadExtensions.Image = global::STEM.Surge.ControlPanel.Properties.Resources.Import;
            this.uploadExtensions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uploadExtensions.Name = "uploadExtensions";
            this.uploadExtensions.Size = new System.Drawing.Size(28, 32);
            this.uploadExtensions.Text = "Upload Extensions";
            this.uploadExtensions.Click += new System.EventHandler(this.uploadExtensions_Click);
            // 
            // viewLocks
            // 
            this.viewLocks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewLocks.Image = global::STEM.Surge.ControlPanel.Properties.Resources._lock;
            this.viewLocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewLocks.Name = "viewLocks";
            this.viewLocks.Size = new System.Drawing.Size(28, 32);
            this.viewLocks.Text = "View Locks";
            this.viewLocks.Click += new System.EventHandler(this.viewLocks_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.AutoSize = false;
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(60, 22);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 35);
            // 
            // activity
            // 
            this.activity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.activity.Image = global::STEM.Surge.ControlPanel.Properties.Resources.view;
            this.activity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.activity.Name = "activity";
            this.activity.Size = new System.Drawing.Size(28, 32);
            this.activity.Text = "System Activity";
            this.activity.Click += new System.EventHandler(this.activity_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // contextHelp
            // 
            this.contextHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.contextHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contextHelp.Image = global::STEM.Surge.ControlPanel.Properties.Resources.help;
            this.contextHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contextHelp.Name = "contextHelp";
            this.contextHelp.Size = new System.Drawing.Size(28, 32);
            this.contextHelp.Text = "toolStripButton2";
            this.contextHelp.ToolTipText = "Context Help";
            this.contextHelp.Click += new System.EventHandler(this.contextHelp_Click);
            // 
            // curTime
            // 
            this.curTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.curTime.Name = "curTime";
            this.curTime.Size = new System.Drawing.Size(0, 32);
            this.curTime.ToolTipText = "UTC Time";
            // 
            // pollerStats
            // 
            this.pollerStats.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pollerStats.Image = global::STEM.Surge.ControlPanel.Properties.Resources.health;
            this.pollerStats.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pollerStats.Name = "pollerStats";
            this.pollerStats.Size = new System.Drawing.Size(28, 32);
            this.pollerStats.Text = "Poller Evaluation";
            this.pollerStats.Click += new System.EventHandler(this.pollerStats_Click);
            // 
            // grafana
            // 
            this.grafana.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.grafana.Image = global::STEM.Surge.ControlPanel.Properties.Resources.graph;
            this.grafana.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.grafana.Name = "grafana";
            this.grafana.Size = new System.Drawing.Size(28, 32);
            this.grafana.Text = "Show Graphs";
            this.grafana.ToolTipText = "Show Graphs";
            this.grafana.Click += new System.EventHandler(this.Grafana_Click);
            // 
            // adHocInstructionSet
            // 
            this.adHocInstructionSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.adHocInstructionSet.Image = global::STEM.Surge.ControlPanel.Properties.Resources.AdHoc;
            this.adHocInstructionSet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.adHocInstructionSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.adHocInstructionSet.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.adHocInstructionSet.Name = "adHocInstructionSet";
            this.adHocInstructionSet.Size = new System.Drawing.Size(60, 32);
            this.adHocInstructionSet.Text = "configure and launch an ad hoc InstructionSet";
            this.adHocInstructionSet.Click += new System.EventHandler(this.adHocInstructionSet_Click);
            // 
            // connectedDM
            // 
            this.connectedDM.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.connectedDM.Image = global::STEM.Surge.ControlPanel.Properties.Resources.connect;
            this.connectedDM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.connectedDM.Name = "connectedDM";
            this.connectedDM.Size = new System.Drawing.Size(153, 28);
            this.connectedDM.Text = "No Connection";
            this.connectedDM.ToolTipText = "Click to attach to a different server";
            this.connectedDM.Click += new System.EventHandler(this.connectedDM_Click);
            // 
            // openControllerDialog
            // 
            this.openControllerDialog.DefaultExt = "\"*.dc\"";
            this.openControllerDialog.Title = "Open Controller";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.toolStrip2.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startStopService,
            this.toolStripSeparator4,
            this.connectedDM});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Margin = new System.Windows.Forms.Padding(3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip2.Size = new System.Drawing.Size(643, 31);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // startStopService
            // 
            this.startStopService.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startStopService.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
            this.startStopService.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startStopService.Name = "startStopService";
            this.startStopService.Size = new System.Drawing.Size(28, 28);
            this.startStopService.Text = "Service Control";
            this.startStopService.Click += new System.EventHandler(this.startStopService_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // panelRight
            // 
            this.panelRight.AutoScroll = true;
            this.panelRight.AutoSize = true;
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelRight.Controls.Add(this.panelActive);
            this.panelRight.Controls.Add(this.controlPanelMainTools);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(786, 496);
            this.panelRight.TabIndex = 0;
            // 
            // panelActive
            // 
            this.panelActive.AutoScroll = true;
            this.panelActive.AutoSize = true;
            this.panelActive.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panelActive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelActive.Location = new System.Drawing.Point(0, 35);
            this.panelActive.Name = "panelActive";
            this.panelActive.Size = new System.Drawing.Size(782, 457);
            this.panelActive.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.AutoScroll = true;
            this.panelLeft.AutoSize = true;
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Controls.Add(this.panelBranches);
            this.panelLeft.Controls.Add(this.toolStrip2);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(647, 496);
            this.panelLeft.TabIndex = 0;
            // 
            // panelBranches
            // 
            this.panelBranches.AutoScroll = true;
            this.panelBranches.Controls.Add(this.branchListGrid);
            this.panelBranches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBranches.Location = new System.Drawing.Point(0, 31);
            this.panelBranches.Name = "panelBranches";
            this.panelBranches.Size = new System.Drawing.Size(643, 461);
            this.panelBranches.TabIndex = 0;
            // 
            // openExtensions
            // 
            this.openExtensions.Filter = "Assemblies|*.dll";
            this.openExtensions.Multiselect = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1437, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1422, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelRight);
            this.splitContainer1.Size = new System.Drawing.Size(1437, 496);
            this.splitContainer1.SplitterDistance = 647;
            this.splitContainer1.TabIndex = 2;
            // 
            // sandboxConfigurationUpdateToolStripMenuItem
            // 
            this.sandboxConfigurationUpdateToolStripMenuItem.Name = "sandboxConfigurationUpdateToolStripMenuItem";
            this.sandboxConfigurationUpdateToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.sandboxConfigurationUpdateToolStripMenuItem.Text = "Sandbox Configuration Update";
            this.sandboxConfigurationUpdateToolStripMenuItem.Click += new System.EventHandler(this.sandboxConfigurationUpdateToolStripMenuItem_Click);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1437, 518);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControlPanel";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Surge Control Panel";
            ((System.ComponentModel.ISupportInitialize)(this.branchListGrid)).EndInit();
            this.branchMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.branchDetailsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).EndInit();
            this.controlPanelMainTools.ResumeLayout(false);
            this.controlPanelMainTools.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelBranches.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem takeOffline;
        private System.Windows.Forms.ToolStripMenuItem bringOnline;
        private System.Windows.Forms.ToolStripMenuItem clearErrors;
        private System.Windows.Forms.ContextMenuStrip branchMenu;
        private System.Windows.Forms.DataGridView branchListGrid;
        private System.Windows.Forms.ToolStripMenuItem viewErrors;
        private System.Windows.Forms.ToolStrip controlPanelMainTools;
        private System.Windows.Forms.ToolStripButton connectedDM;
        private System.Windows.Forms.ToolStripButton startStopService;
        private System.Windows.Forms.ToolStripButton licenseAccess;
        private System.Windows.Forms.ToolStripButton about;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton switchboardConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton activity;
        private System.Windows.Forms.ToolStripDropDownButton systemTools;
        private System.Windows.Forms.ToolStripMenuItem deploymentControllerEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionSetEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.OpenFileDialog openControllerDialog;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelActive;
        private System.Windows.Forms.Panel panelBranches;
        private System.Windows.Forms.BindingSource branchDetailsBindingSource;
        private TableDataSources tableDataSources;
        private System.Windows.Forms.ToolStripMenuItem changeVersionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton contextHelp;
        private System.Windows.Forms.ToolStripLabel curTime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem editManagerStaticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton uploadExtensions;
        private System.Windows.Forms.OpenFileDialog openExtensions;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem updateSurge;
        private System.Windows.Forms.ToolStripButton viewLocks;
        private System.Windows.Forms.ToolStripMenuItem manageExtensionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchStateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn OnlineImg;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rAMDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripButton grafana;
        private System.Windows.Forms.ToolStripMenuItem updateConfiguration;
        private System.Windows.Forms.ToolStripMenuItem exploreConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton pollerStats;
        private System.Windows.Forms.ToolStripButton adHocInstructionSet;
        private System.Windows.Forms.ToolStripMenuItem sandboxConfigurationUpdateToolStripMenuItem;
    }
}


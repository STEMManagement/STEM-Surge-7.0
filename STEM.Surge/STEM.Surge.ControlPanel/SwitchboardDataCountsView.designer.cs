namespace STEM.Surge.ControlPanel
{
    partial class SwitchboardDataCountsView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.sourceRowMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.countDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.switchboardDataCountsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.panel3 = new System.Windows.Forms.Panel();
            this.hideZeroCounts = new System.Windows.Forms.CheckBox();
            this.hideDisabled = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.switchboardDataCountsGridView = new System.Windows.Forms.DataGridView();
            this.enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.controllerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backlogDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.perceivedBacklogDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxBranchLoadDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AverageExecutionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Samples = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastAssignmentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pollerCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pollerCountHealthyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMaxHealthyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMinHealthyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMaxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMinDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.pauseSourceCountsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.clearSourceFilter = new System.Windows.Forms.ToolStripButton();
            this.filterTextbox = new System.Windows.Forms.ToolStripTextBox();
            this.pollingInfo = new System.Windows.Forms.ToolStripButton();
            this.totalHealthyPollers = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.pollerCountLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.editController = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceRowMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.switchboardDataCountsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.switchboardDataCountsGridView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceRowMenu
            // 
            this.sourceRowMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sourceRowMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.countDetails,
            this.editController});
            this.sourceRowMenu.Name = "sourceRowMenu";
            this.sourceRowMenu.Size = new System.Drawing.Size(153, 70);
            // 
            // countDetails
            // 
            this.countDetails.Name = "countDetails";
            this.countDetails.Size = new System.Drawing.Size(152, 22);
            this.countDetails.Text = "View Details";
            // 
            // switchboardDataCountsBindingSource
            // 
            this.switchboardDataCountsBindingSource.DataMember = "SwitchboardDataCounts";
            this.switchboardDataCountsBindingSource.DataSource = this.tableDataSources;
            // 
            // tableDataSources
            // 
            this.tableDataSources.DataSetName = "TableDataSources";
            this.tableDataSources.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.hideZeroCounts);
            this.panel3.Controls.Add(this.hideDisabled);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.toolStrip1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1174, 459);
            this.panel3.TabIndex = 0;
            // 
            // hideZeroCounts
            // 
            this.hideZeroCounts.AutoSize = true;
            this.hideZeroCounts.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.hideZeroCounts.Location = new System.Drawing.Point(678, 9);
            this.hideZeroCounts.Name = "hideZeroCounts";
            this.hideZeroCounts.Size = new System.Drawing.Size(134, 17);
            this.hideZeroCounts.TabIndex = 0;
            this.hideZeroCounts.Text = "Hide Zero Count Rows";
            this.hideZeroCounts.UseVisualStyleBackColor = false;
            this.hideZeroCounts.CheckedChanged += new System.EventHandler(this.hideZeroCounts_CheckedChanged);
            // 
            // hideDisabled
            // 
            this.hideDisabled.AutoSize = true;
            this.hideDisabled.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.hideDisabled.Location = new System.Drawing.Point(577, 9);
            this.hideDisabled.Name = "hideDisabled";
            this.hideDisabled.Size = new System.Drawing.Size(89, 17);
            this.hideDisabled.TabIndex = 0;
            this.hideDisabled.Text = "Enabled Only";
            this.hideDisabled.UseVisualStyleBackColor = false;
            this.hideDisabled.CheckedChanged += new System.EventHandler(this.hideDisabled_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.switchboardDataCountsGridView);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 35);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1174, 424);
            this.panel4.TabIndex = 0;
            // 
            // switchboardDataCountsGridView
            // 
            this.switchboardDataCountsGridView.AllowUserToAddRows = false;
            this.switchboardDataCountsGridView.AllowUserToDeleteRows = false;
            this.switchboardDataCountsGridView.AllowUserToOrderColumns = true;
            this.switchboardDataCountsGridView.AutoGenerateColumns = false;
            this.switchboardDataCountsGridView.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.switchboardDataCountsGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.switchboardDataCountsGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.switchboardDataCountsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enabledDataGridViewCheckBoxColumn,
            this.Column1,
            this.controllerDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.backlogDataGridViewTextBoxColumn,
            this.perceivedBacklogDataGridViewTextBoxColumn,
            this.assignedDataGridViewTextBoxColumn,
            this.processingDataGridViewTextBoxColumn,
            this.maxBranchLoadDataGridViewTextBoxColumn,
            this.AverageExecutionTime,
            this.Samples,
            this.lastAssignmentDataGridViewTextBoxColumn,
            this.pollerCountDataGridViewTextBoxColumn,
            this.pollerCountHealthyDataGridViewTextBoxColumn,
            this.activeCountDataGridViewTextBoxColumn,
            this.lastPollMaxHealthyDataGridViewTextBoxColumn,
            this.lastPollMinHealthyDataGridViewTextBoxColumn,
            this.lastPollMaxDataGridViewTextBoxColumn,
            this.lastPollMinDataGridViewTextBoxColumn,
            this.activeDataGridViewCheckBoxColumn,
            this.iDDataGridViewTextBoxColumn});
            this.switchboardDataCountsGridView.ContextMenuStrip = this.sourceRowMenu;
            this.switchboardDataCountsGridView.DataSource = this.switchboardDataCountsBindingSource;
            this.switchboardDataCountsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.switchboardDataCountsGridView.GridColor = System.Drawing.Color.LightGray;
            this.switchboardDataCountsGridView.Location = new System.Drawing.Point(0, 0);
            this.switchboardDataCountsGridView.MultiSelect = false;
            this.switchboardDataCountsGridView.Name = "switchboardDataCountsGridView";
            this.switchboardDataCountsGridView.ReadOnly = true;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.switchboardDataCountsGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.switchboardDataCountsGridView.RowHeadersWidth = 20;
            this.switchboardDataCountsGridView.Size = new System.Drawing.Size(1174, 424);
            this.switchboardDataCountsGridView.TabIndex = 0;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            this.enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            this.enabledDataGridViewCheckBoxColumn.ReadOnly = true;
            this.enabledDataGridViewCheckBoxColumn.Width = 70;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Active";
            this.Column1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 43;
            // 
            // controllerDataGridViewTextBoxColumn
            // 
            this.controllerDataGridViewTextBoxColumn.DataPropertyName = "Controller";
            this.controllerDataGridViewTextBoxColumn.HeaderText = "Controller";
            this.controllerDataGridViewTextBoxColumn.Name = "controllerDataGridViewTextBoxColumn";
            this.controllerDataGridViewTextBoxColumn.ReadOnly = true;
            this.controllerDataGridViewTextBoxColumn.Width = 200;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 140;
            // 
            // backlogDataGridViewTextBoxColumn
            // 
            this.backlogDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.backlogDataGridViewTextBoxColumn.DataPropertyName = "Backlog";
            this.backlogDataGridViewTextBoxColumn.HeaderText = "Backlog";
            this.backlogDataGridViewTextBoxColumn.Name = "backlogDataGridViewTextBoxColumn";
            this.backlogDataGridViewTextBoxColumn.ReadOnly = true;
            this.backlogDataGridViewTextBoxColumn.Width = 69;
            // 
            // perceivedBacklogDataGridViewTextBoxColumn
            // 
            this.perceivedBacklogDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.perceivedBacklogDataGridViewTextBoxColumn.DataPropertyName = "PerceivedBacklog";
            this.perceivedBacklogDataGridViewTextBoxColumn.HeaderText = "Assignable";
            this.perceivedBacklogDataGridViewTextBoxColumn.Name = "perceivedBacklogDataGridViewTextBoxColumn";
            this.perceivedBacklogDataGridViewTextBoxColumn.ReadOnly = true;
            this.perceivedBacklogDataGridViewTextBoxColumn.Width = 81;
            // 
            // assignedDataGridViewTextBoxColumn
            // 
            this.assignedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
            this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
            this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
            this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
            this.assignedDataGridViewTextBoxColumn.Width = 73;
            // 
            // processingDataGridViewTextBoxColumn
            // 
            this.processingDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.processingDataGridViewTextBoxColumn.DataPropertyName = "Processing";
            this.processingDataGridViewTextBoxColumn.HeaderText = "Processing";
            this.processingDataGridViewTextBoxColumn.Name = "processingDataGridViewTextBoxColumn";
            this.processingDataGridViewTextBoxColumn.ReadOnly = true;
            this.processingDataGridViewTextBoxColumn.Width = 82;
            // 
            // maxBranchLoadDataGridViewTextBoxColumn
            // 
            this.maxBranchLoadDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.maxBranchLoadDataGridViewTextBoxColumn.DataPropertyName = "MaxBranchLoad";
            this.maxBranchLoadDataGridViewTextBoxColumn.HeaderText = "Max Branch Load";
            this.maxBranchLoadDataGridViewTextBoxColumn.Name = "maxBranchLoadDataGridViewTextBoxColumn";
            this.maxBranchLoadDataGridViewTextBoxColumn.ReadOnly = true;
            this.maxBranchLoadDataGridViewTextBoxColumn.Width = 114;
            // 
            // AverageExecutionTime
            // 
            this.AverageExecutionTime.DataPropertyName = "AverageExecutionTime";
            dataGridViewCellStyle2.Format = "0.0000";
            this.AverageExecutionTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.AverageExecutionTime.HeaderText = "Avg Execution Time";
            this.AverageExecutionTime.MinimumWidth = 120;
            this.AverageExecutionTime.Name = "AverageExecutionTime";
            this.AverageExecutionTime.ReadOnly = true;
            this.AverageExecutionTime.ToolTipText = "Average execution time over assignments in the last 2 minutes";
            this.AverageExecutionTime.Width = 120;
            // 
            // Samples
            // 
            this.Samples.DataPropertyName = "Samples";
            this.Samples.HeaderText = "Completed (2min)";
            this.Samples.Name = "Samples";
            this.Samples.ReadOnly = true;
            // 
            // lastAssignmentDataGridViewTextBoxColumn
            // 
            this.lastAssignmentDataGridViewTextBoxColumn.DataPropertyName = "LastAssignment";
            dataGridViewCellStyle3.Format = "G";
            this.lastAssignmentDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.lastAssignmentDataGridViewTextBoxColumn.HeaderText = "Last Assignment";
            this.lastAssignmentDataGridViewTextBoxColumn.Name = "lastAssignmentDataGridViewTextBoxColumn";
            this.lastAssignmentDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastAssignmentDataGridViewTextBoxColumn.Width = 150;
            // 
            // pollerCountDataGridViewTextBoxColumn
            // 
            this.pollerCountDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.pollerCountDataGridViewTextBoxColumn.DataPropertyName = "PollerCount";
            this.pollerCountDataGridViewTextBoxColumn.HeaderText = "Pollers";
            this.pollerCountDataGridViewTextBoxColumn.Name = "pollerCountDataGridViewTextBoxColumn";
            this.pollerCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.pollerCountDataGridViewTextBoxColumn.ToolTipText = "Total expanded poller count";
            this.pollerCountDataGridViewTextBoxColumn.Width = 61;
            // 
            // pollerCountHealthyDataGridViewTextBoxColumn
            // 
            this.pollerCountHealthyDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.pollerCountHealthyDataGridViewTextBoxColumn.DataPropertyName = "PollerCountHealthy";
            this.pollerCountHealthyDataGridViewTextBoxColumn.HeaderText = "(Healthy)";
            this.pollerCountHealthyDataGridViewTextBoxColumn.Name = "pollerCountHealthyDataGridViewTextBoxColumn";
            this.pollerCountHealthyDataGridViewTextBoxColumn.ReadOnly = true;
            this.pollerCountHealthyDataGridViewTextBoxColumn.ToolTipText = "Poolable data rows";
            this.pollerCountHealthyDataGridViewTextBoxColumn.Width = 72;
            // 
            // activeCountDataGridViewTextBoxColumn
            // 
            this.activeCountDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.activeCountDataGridViewTextBoxColumn.DataPropertyName = "ActiveCount";
            this.activeCountDataGridViewTextBoxColumn.HeaderText = "(Assigning)";
            this.activeCountDataGridViewTextBoxColumn.Name = "activeCountDataGridViewTextBoxColumn";
            this.activeCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.activeCountDataGridViewTextBoxColumn.Visible = false;
            // 
            // lastPollMaxHealthyDataGridViewTextBoxColumn
            // 
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.DataPropertyName = "LastPollMaxHealthy";
            dataGridViewCellStyle4.Format = "G";
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.HeaderText = "Max List (Healthy)";
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.Name = "lastPollMaxHealthyDataGridViewTextBoxColumn";
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastPollMaxHealthyDataGridViewTextBoxColumn.Width = 150;
            // 
            // lastPollMinHealthyDataGridViewTextBoxColumn
            // 
            this.lastPollMinHealthyDataGridViewTextBoxColumn.DataPropertyName = "LastPollMinHealthy";
            dataGridViewCellStyle5.Format = "G";
            this.lastPollMinHealthyDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.lastPollMinHealthyDataGridViewTextBoxColumn.HeaderText = "Min List (Healthy)";
            this.lastPollMinHealthyDataGridViewTextBoxColumn.Name = "lastPollMinHealthyDataGridViewTextBoxColumn";
            this.lastPollMinHealthyDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastPollMinHealthyDataGridViewTextBoxColumn.Width = 150;
            // 
            // lastPollMaxDataGridViewTextBoxColumn
            // 
            this.lastPollMaxDataGridViewTextBoxColumn.DataPropertyName = "LastPollMax";
            dataGridViewCellStyle6.Format = "G";
            this.lastPollMaxDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.lastPollMaxDataGridViewTextBoxColumn.HeaderText = "Max List";
            this.lastPollMaxDataGridViewTextBoxColumn.Name = "lastPollMaxDataGridViewTextBoxColumn";
            this.lastPollMaxDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastPollMaxDataGridViewTextBoxColumn.Width = 150;
            // 
            // lastPollMinDataGridViewTextBoxColumn
            // 
            this.lastPollMinDataGridViewTextBoxColumn.DataPropertyName = "LastPollMin";
            dataGridViewCellStyle7.Format = "G";
            this.lastPollMinDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.lastPollMinDataGridViewTextBoxColumn.HeaderText = "Min List";
            this.lastPollMinDataGridViewTextBoxColumn.Name = "lastPollMinDataGridViewTextBoxColumn";
            this.lastPollMinDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastPollMinDataGridViewTextBoxColumn.Width = 150;
            // 
            // activeDataGridViewCheckBoxColumn
            // 
            this.activeDataGridViewCheckBoxColumn.DataPropertyName = "Active";
            this.activeDataGridViewCheckBoxColumn.HeaderText = "Active";
            this.activeDataGridViewCheckBoxColumn.Name = "activeDataGridViewCheckBoxColumn";
            this.activeDataGridViewCheckBoxColumn.ReadOnly = true;
            this.activeDataGridViewCheckBoxColumn.Visible = false;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.pauseSourceCountsButton,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.clearSourceFilter,
            this.filterTextbox,
            this.pollingInfo,
            this.totalHealthyPollers,
            this.toolStripSeparator2,
            this.pollerCountLabel,
            this.toolStripLabel3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1174, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(105, 32);
            this.toolStripLabel1.Text = "Source File Counts";
            // 
            // pauseSourceCountsButton
            // 
            this.pauseSourceCountsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseSourceCountsButton.Image = global::STEM.Surge.ControlPanel.Properties.Resources.pause;
            this.pauseSourceCountsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseSourceCountsButton.Name = "pauseSourceCountsButton";
            this.pauseSourceCountsButton.Size = new System.Drawing.Size(24, 32);
            this.pauseSourceCountsButton.ToolTipText = "Stop / Start Table Refresh";
            this.pauseSourceCountsButton.Click += new System.EventHandler(this.pauseSourceCountsButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.AutoSize = false;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(0, 22);
            // 
            // clearSourceFilter
            // 
            this.clearSourceFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearSourceFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearSourceFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearSourceFilter.Name = "clearSourceFilter";
            this.clearSourceFilter.Size = new System.Drawing.Size(24, 32);
            this.clearSourceFilter.Text = "Clear Filter";
            this.clearSourceFilter.Click += new System.EventHandler(this.clearSourceFilter_Click);
            // 
            // filterTextbox
            // 
            this.filterTextbox.AutoSize = false;
            this.filterTextbox.BackColor = System.Drawing.Color.Lavender;
            this.filterTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterTextbox.Name = "filterTextbox";
            this.filterTextbox.Size = new System.Drawing.Size(400, 20);
            this.filterTextbox.ToolTipText = "Filter Rows";
            // 
            // pollingInfo
            // 
            this.pollingInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pollingInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pollingInfo.Image = global::STEM.Surge.ControlPanel.Properties.Resources.info;
            this.pollingInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pollingInfo.Name = "pollingInfo";
            this.pollingInfo.Size = new System.Drawing.Size(24, 32);
            this.pollingInfo.Text = "Polling Info";
            this.pollingInfo.Click += new System.EventHandler(this.pollingInfo_Click);
            // 
            // totalHealthyPollers
            // 
            this.totalHealthyPollers.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.totalHealthyPollers.AutoSize = false;
            this.totalHealthyPollers.Name = "totalHealthyPollers";
            this.totalHealthyPollers.Size = new System.Drawing.Size(80, 32);
            this.totalHealthyPollers.Text = "(0)";
            this.totalHealthyPollers.ToolTipText = "Total Number Of Pollers Enabled And Without Errors";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // pollerCountLabel
            // 
            this.pollerCountLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pollerCountLabel.AutoSize = false;
            this.pollerCountLabel.Name = "pollerCountLabel";
            this.pollerCountLabel.Size = new System.Drawing.Size(80, 32);
            this.pollerCountLabel.Text = "(0)";
            this.pollerCountLabel.ToolTipText = "Total Number Of Pollers";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(42, 32);
            this.toolStripLabel3.Text = "Pollers";
            // 
            // editController
            // 
            this.editController.Name = "editController";
            this.editController.Size = new System.Drawing.Size(152, 22);
            this.editController.Text = "Edit Controller";
            // 
            // SwitchboardDataCountsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.panel3);
            this.Name = "SwitchboardDataCountsView";
            this.Size = new System.Drawing.Size(1174, 459);
            this.sourceRowMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.switchboardDataCountsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.switchboardDataCountsGridView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip sourceRowMenu;
        private System.Windows.Forms.ToolStripMenuItem countDetails;
        private System.Windows.Forms.BindingSource switchboardDataCountsBindingSource;
        private TableDataSources tableDataSources;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox hideZeroCounts;
        private System.Windows.Forms.CheckBox hideDisabled;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView switchboardDataCountsGridView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton pauseSourceCountsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton clearSourceFilter;
        private System.Windows.Forms.ToolStripTextBox filterTextbox;
        private System.Windows.Forms.ToolStripButton pollingInfo;
        private System.Windows.Forms.ToolStripLabel totalHealthyPollers;
        private System.Windows.Forms.ToolStripLabel pollerCountLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn controllerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn backlogDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn perceivedBacklogDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxBranchLoadDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AverageExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Samples;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastAssignmentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pollerCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pollerCountHealthyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn activeCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPollMaxHealthyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPollMinHealthyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPollMaxDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPollMinDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripMenuItem editController;
    }
}

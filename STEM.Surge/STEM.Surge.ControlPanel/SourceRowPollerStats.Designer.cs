namespace STEM.Surge.ControlPanel
{
    partial class SourceRowPollerStats
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.activePollers = new System.Windows.Forms.TabPage();
            this.stuckPollers = new System.Windows.Forms.TabPage();
            this.deadPollers = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.pollerDetailsGridView = new System.Windows.Forms.DataGridView();
            this.AverageExecutionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Samples = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastWalkStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Active = new System.Windows.Forms.DataGridViewImageColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.controllerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backlogDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PerceivedBacklog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMaxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastAssignment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pollerDetailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._TableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._TableDataSources)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.activePollers);
            this.tabControl1.Controls.Add(this.stuckPollers);
            this.tabControl1.Controls.Add(this.deadPollers);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1447, 25);
            this.tabControl1.TabIndex = 0;
            // 
            // activePollers
            // 
            this.activePollers.Location = new System.Drawing.Point(4, 22);
            this.activePollers.Name = "activePollers";
            this.activePollers.Padding = new System.Windows.Forms.Padding(3);
            this.activePollers.Size = new System.Drawing.Size(1439, 0);
            this.activePollers.TabIndex = 0;
            this.activePollers.Text = "Active Pollers";
            this.activePollers.ToolTipText = "Pollers that are currently assigning work";
            this.activePollers.UseVisualStyleBackColor = true;
            // 
            // stuckPollers
            // 
            this.stuckPollers.Location = new System.Drawing.Point(4, 22);
            this.stuckPollers.Name = "stuckPollers";
            this.stuckPollers.Padding = new System.Windows.Forms.Padding(3);
            this.stuckPollers.Size = new System.Drawing.Size(1439, 0);
            this.stuckPollers.TabIndex = 2;
            this.stuckPollers.Text = "Possible Stuck Pollers";
            this.stuckPollers.ToolTipText = "Pollers that have been in an assignment cycle for a long time";
            this.stuckPollers.UseVisualStyleBackColor = true;
            // 
            // deadPollers
            // 
            this.deadPollers.Location = new System.Drawing.Point(4, 22);
            this.deadPollers.Name = "deadPollers";
            this.deadPollers.Padding = new System.Windows.Forms.Padding(3);
            this.deadPollers.Size = new System.Drawing.Size(1439, 0);
            this.deadPollers.TabIndex = 1;
            this.deadPollers.Text = "Possible Dead Pollers";
            this.deadPollers.ToolTipText = "Pollers that have no reachable data source";
            this.deadPollers.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1447, 585);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pollerDetailsGridView);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1447, 554);
            this.panel2.TabIndex = 8;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterMask});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1447, 31);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(28, 28);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // filterMask
            // 
            this.filterMask.AutoSize = false;
            this.filterMask.BackColor = System.Drawing.Color.Lavender;
            this.filterMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterMask.Name = "filterMask";
            this.filterMask.Size = new System.Drawing.Size(400, 20);
            this.filterMask.ToolTipText = "Filter the Rows";
            // 
            // pollerDetailsGridView
            // 
            this.pollerDetailsGridView.AllowUserToAddRows = false;
            this.pollerDetailsGridView.AllowUserToDeleteRows = false;
            this.pollerDetailsGridView.AutoGenerateColumns = false;
            this.pollerDetailsGridView.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.pollerDetailsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pollerDetailsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.descriptionDataGridViewTextBoxColumn,
            this.controllerDataGridViewTextBoxColumn,
            this.backlogDataGridViewTextBoxColumn,
            this.PerceivedBacklog,
            this.assignedDataGridViewTextBoxColumn,
            this.processingDataGridViewTextBoxColumn,
            this.AverageExecutionTime,
            this.Samples,
            this.lastPollMaxDataGridViewTextBoxColumn,
            this.LastAssignment,
            this.LastWalkStart,
            this.Active,
            this.activeDataGridViewCheckBoxColumn,
            this.iDDataGridViewTextBoxColumn});
            this.pollerDetailsGridView.DataSource = this.pollerDetailsBindingSource;
            this.pollerDetailsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollerDetailsGridView.Location = new System.Drawing.Point(0, 25);
            this.pollerDetailsGridView.Name = "pollerDetailsGridView";
            this.pollerDetailsGridView.RowHeadersWidth = 25;
            this.pollerDetailsGridView.Size = new System.Drawing.Size(1447, 529);
            this.pollerDetailsGridView.TabIndex = 1;
            // 
            // AverageExecutionTime
            // 
            this.AverageExecutionTime.DataPropertyName = "AverageExecutionTime";
            dataGridViewCellStyle1.Format = "0.0000";
            this.AverageExecutionTime.DefaultCellStyle = dataGridViewCellStyle1;
            this.AverageExecutionTime.HeaderText = "Avg Execution Time";
            this.AverageExecutionTime.MinimumWidth = 150;
            this.AverageExecutionTime.Name = "AverageExecutionTime";
            this.AverageExecutionTime.ReadOnly = true;
            this.AverageExecutionTime.ToolTipText = "Average execution time over assignments in the last 2 minutes";
            this.AverageExecutionTime.Width = 150;
            // 
            // Samples
            // 
            this.Samples.DataPropertyName = "Samples";
            this.Samples.HeaderText = "Completed (2min)";
            this.Samples.MinimumWidth = 150;
            this.Samples.Name = "Samples";
            this.Samples.ReadOnly = true;
            this.Samples.Width = 150;
            // 
            // LastWalkStart
            // 
            this.LastWalkStart.DataPropertyName = "LastWalkStart";
            dataGridViewCellStyle4.Format = "G";
            this.LastWalkStart.DefaultCellStyle = dataGridViewCellStyle4;
            this.LastWalkStart.FillWeight = 150F;
            this.LastWalkStart.HeaderText = "Assignment Start";
            this.LastWalkStart.Name = "LastWalkStart";
            this.LastWalkStart.Width = 150;
            // 
            // Active
            // 
            this.Active.HeaderText = "Active";
            this.Active.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Active.Name = "Active";
            this.Active.ReadOnly = true;
            this.Active.Width = 43;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // controllerDataGridViewTextBoxColumn
            // 
            this.controllerDataGridViewTextBoxColumn.DataPropertyName = "Controller";
            this.controllerDataGridViewTextBoxColumn.HeaderText = "Controller";
            this.controllerDataGridViewTextBoxColumn.Name = "controllerDataGridViewTextBoxColumn";
            // 
            // backlogDataGridViewTextBoxColumn
            // 
            this.backlogDataGridViewTextBoxColumn.DataPropertyName = "Backlog";
            this.backlogDataGridViewTextBoxColumn.HeaderText = "Backlog";
            this.backlogDataGridViewTextBoxColumn.Name = "backlogDataGridViewTextBoxColumn";
            // 
            // PerceivedBacklog
            // 
            this.PerceivedBacklog.DataPropertyName = "PerceivedBacklog";
            this.PerceivedBacklog.HeaderText = "Perceived Backlog";
            this.PerceivedBacklog.Name = "PerceivedBacklog";
            // 
            // assignedDataGridViewTextBoxColumn
            // 
            this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
            this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
            this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
            // 
            // processingDataGridViewTextBoxColumn
            // 
            this.processingDataGridViewTextBoxColumn.DataPropertyName = "Processing";
            this.processingDataGridViewTextBoxColumn.HeaderText = "Processing";
            this.processingDataGridViewTextBoxColumn.Name = "processingDataGridViewTextBoxColumn";
            // 
            // lastPollMaxDataGridViewTextBoxColumn
            // 
            this.lastPollMaxDataGridViewTextBoxColumn.DataPropertyName = "LastPollMax";
            dataGridViewCellStyle2.Format = "G";
            this.lastPollMaxDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.lastPollMaxDataGridViewTextBoxColumn.HeaderText = "Last Poll";
            this.lastPollMaxDataGridViewTextBoxColumn.Name = "lastPollMaxDataGridViewTextBoxColumn";
            this.lastPollMaxDataGridViewTextBoxColumn.Width = 150;
            // 
            // LastAssignment
            // 
            this.LastAssignment.DataPropertyName = "LastAssignment";
            dataGridViewCellStyle3.Format = "G";
            this.LastAssignment.DefaultCellStyle = dataGridViewCellStyle3;
            this.LastAssignment.FillWeight = 150F;
            this.LastAssignment.HeaderText = "Last Assignment";
            this.LastAssignment.Name = "LastAssignment";
            this.LastAssignment.Width = 150;
            // 
            // activeDataGridViewCheckBoxColumn
            // 
            this.activeDataGridViewCheckBoxColumn.DataPropertyName = "Active";
            this.activeDataGridViewCheckBoxColumn.HeaderText = "Active";
            this.activeDataGridViewCheckBoxColumn.Name = "activeDataGridViewCheckBoxColumn";
            this.activeDataGridViewCheckBoxColumn.Visible = false;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.Visible = false;
            // 
            // pollerDetailsBindingSource
            // 
            this.pollerDetailsBindingSource.DataMember = "SwitchboardDataCounts";
            this.pollerDetailsBindingSource.DataSource = this._TableDataSources;
            // 
            // _TableDataSources
            // 
            this._TableDataSources.DataSetName = "TableDataSources";
            this._TableDataSources.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // SourceRowPollerStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SourceRowPollerStats";
            this.Size = new System.Drawing.Size(1447, 585);
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._TableDataSources)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage activePollers;
        private System.Windows.Forms.TabPage deadPollers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.TabPage stuckPollers;
        private System.Windows.Forms.BindingSource pollerDetailsBindingSource;
        private TableDataSources _TableDataSources;
        private System.Windows.Forms.DataGridView pollerDetailsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn controllerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn backlogDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PerceivedBacklog;
        private System.Windows.Forms.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AverageExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Samples;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPollMaxDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastAssignment;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastWalkStart;
        private System.Windows.Forms.DataGridViewImageColumn Active;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
    }
}

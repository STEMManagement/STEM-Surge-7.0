namespace STEM.Surge.ControlPanel
{
    partial class SourceRowPollerDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceRowPollerDetails));
            this.pollerDetailsGridView = new System.Windows.Forms.DataGridView();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.controllerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backlogDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PerceivedBacklog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AverageExecutionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Samples = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPollMaxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastAssignment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastWalkStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Active = new System.Windows.Forms.DataGridViewImageColumn();
            this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pollerDetailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._TableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.directoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openController = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._TableDataSources)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.pollerDetailsGridView.Location = new System.Drawing.Point(0, 0);
            this.pollerDetailsGridView.Name = "pollerDetailsGridView";
            this.pollerDetailsGridView.RowHeadersWidth = 25;
            this.pollerDetailsGridView.Size = new System.Drawing.Size(1288, 487);
            this.pollerDetailsGridView.TabIndex = 0;
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
            // directoryDataGridViewTextBoxColumn
            // 
            this.directoryDataGridViewTextBoxColumn.DataPropertyName = "Directory";
            this.directoryDataGridViewTextBoxColumn.HeaderText = "Directory";
            this.directoryDataGridViewTextBoxColumn.Name = "directoryDataGridViewTextBoxColumn";
            this.directoryDataGridViewTextBoxColumn.Width = 600;
            // 
            // fileCountDataGridViewTextBoxColumn
            // 
            this.fileCountDataGridViewTextBoxColumn.DataPropertyName = "File Count";
            this.fileCountDataGridViewTextBoxColumn.HeaderText = "File Count";
            this.fileCountDataGridViewTextBoxColumn.Name = "fileCountDataGridViewTextBoxColumn";
            this.fileCountDataGridViewTextBoxColumn.Width = 150;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openController,
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.clearFilter,
            this.filterMask});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1288, 35);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openController
            // 
            this.openController.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openController.Image = global::STEM.Surge.ControlPanel.Properties.Resources.depolyment_controller;
            this.openController.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openController.Name = "openController";
            this.openController.Size = new System.Drawing.Size(28, 32);
            this.openController.Text = "Edit Controller";
            this.openController.Click += new System.EventHandler(this.openController_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(30, 32);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(28, 32);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.pollerDetailsGridView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1288, 487);
            this.panel1.TabIndex = 7;
            // 
            // SourceRowPollerDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1288, 522);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SourceRowPollerDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Source Poller Details";
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollerDetailsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._TableDataSources)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView pollerDetailsGridView;
        private System.Windows.Forms.BindingSource pollerDetailsBindingSource;
        private TableDataSources _TableDataSources;
        private System.Windows.Forms.DataGridViewTextBoxColumn directoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton openController;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
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
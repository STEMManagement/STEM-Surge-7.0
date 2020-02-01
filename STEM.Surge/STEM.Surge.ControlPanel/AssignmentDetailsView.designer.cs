namespace STEM.Surge.ControlPanel
{
    partial class AssignmentDetailsView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.detailsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelJob = new System.Windows.Forms.ToolStripMenuItem();
            this.assignmentDetailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.assignmentDetailsGridView = new System.Windows.Forms.DataGridView();
            this.incomplete = new System.Windows.Forms.CheckBox();
            this.scrollBottom = new System.Windows.Forms.CheckBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.rowCountLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.incompleteLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.pauseDetailsGridButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAssignmentFilter = new System.Windows.Forms.ToolStripButton();
            this.assignmentsFilterTextbox = new System.Windows.Forms.ToolStripTextBox();
            this.advancedFilter = new System.Windows.Forms.ToolStripButton();
            this.avgExecutionTime = new System.Windows.Forms.ToolStripLabel();
            this.managerDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.controllerDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.issuedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.completedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExecutionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Exceptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.instructionSetIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deploymentManagerIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.switchboardRowIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailsMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assignmentDetailsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assignmentDetailsGridView)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // detailsMenuStrip
            // 
            this.detailsMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.detailsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDetails,
            this.cancelJob});
            this.detailsMenuStrip.Name = "detailsMenuStrip";
            this.detailsMenuStrip.Size = new System.Drawing.Size(138, 48);
            // 
            // viewDetails
            // 
            this.viewDetails.Name = "viewDetails";
            this.viewDetails.Size = new System.Drawing.Size(137, 22);
            this.viewDetails.Text = "View Details";
            // 
            // cancelJob
            // 
            this.cancelJob.Name = "cancelJob";
            this.cancelJob.Size = new System.Drawing.Size(137, 22);
            this.cancelJob.Text = "Cancel Job";
            // 
            // assignmentDetailsBindingSource
            // 
            this.assignmentDetailsBindingSource.DataMember = "AssignmentDetails";
            this.assignmentDetailsBindingSource.DataSource = this.tableDataSources;
            // 
            // tableDataSources
            // 
            this.tableDataSources.DataSetName = "TableDataSources";
            this.tableDataSources.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.incomplete);
            this.panel1.Controls.Add(this.scrollBottom);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1300, 459);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.assignmentDetailsGridView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1300, 424);
            this.panel2.TabIndex = 0;
            // 
            // assignmentDetailsGridView
            // 
            this.assignmentDetailsGridView.AllowUserToAddRows = false;
            this.assignmentDetailsGridView.AllowUserToDeleteRows = false;
            this.assignmentDetailsGridView.AutoGenerateColumns = false;
            this.assignmentDetailsGridView.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.assignmentDetailsGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.assignmentDetailsGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.assignmentDetailsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.managerDataGridViewTextBoxColumn1,
            this.BranchIP,
            this.BranchName,
            this.controllerDataGridViewTextBoxColumn1,
            this.sourceDataGridViewTextBoxColumn,
            this.issuedDataGridViewTextBoxColumn,
            this.receivedDataGridViewTextBoxColumn,
            this.completedDataGridViewTextBoxColumn,
            this.ExecutionTime,
            this.Exceptions,
            this.instructionSetIDDataGridViewTextBoxColumn,
            this.deploymentManagerIDDataGridViewTextBoxColumn,
            this.switchboardRowIDDataGridViewTextBoxColumn});
            this.assignmentDetailsGridView.ContextMenuStrip = this.detailsMenuStrip;
            this.assignmentDetailsGridView.DataSource = this.assignmentDetailsBindingSource;
            this.assignmentDetailsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assignmentDetailsGridView.GridColor = System.Drawing.Color.LightGray;
            this.assignmentDetailsGridView.Location = new System.Drawing.Point(0, 0);
            this.assignmentDetailsGridView.Name = "assignmentDetailsGridView";
            this.assignmentDetailsGridView.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.assignmentDetailsGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.assignmentDetailsGridView.RowHeadersWidth = 20;
            this.assignmentDetailsGridView.Size = new System.Drawing.Size(1300, 424);
            this.assignmentDetailsGridView.TabIndex = 0;
            this.assignmentDetailsGridView.VirtualMode = true;
            // 
            // incomplete
            // 
            this.incomplete.AutoSize = true;
            this.incomplete.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.incomplete.Location = new System.Drawing.Point(867, 10);
            this.incomplete.Name = "incomplete";
            this.incomplete.Size = new System.Drawing.Size(78, 17);
            this.incomplete.TabIndex = 0;
            this.incomplete.Text = "Incomplete";
            this.incomplete.UseVisualStyleBackColor = false;
            this.incomplete.CheckedChanged += new System.EventHandler(this.incomplete_CheckedChanged);
            // 
            // scrollBottom
            // 
            this.scrollBottom.AutoSize = true;
            this.scrollBottom.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.scrollBottom.Checked = true;
            this.scrollBottom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scrollBottom.Location = new System.Drawing.Point(768, 10);
            this.scrollBottom.Name = "scrollBottom";
            this.scrollBottom.Size = new System.Drawing.Size(88, 17);
            this.scrollBottom.TabIndex = 0;
            this.scrollBottom.Text = "Bottom Scroll";
            this.scrollBottom.UseVisualStyleBackColor = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rowCountLabel,
            this.toolStripSeparator2,
            this.incompleteLabel,
            this.toolStripLabel5,
            this.pauseDetailsGridButton,
            this.toolStripSeparator3,
            this.clearAssignmentFilter,
            this.assignmentsFilterTextbox,
            this.advancedFilter,
            this.avgExecutionTime});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1300, 35);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // rowCountLabel
            // 
            this.rowCountLabel.AutoSize = false;
            this.rowCountLabel.Name = "rowCountLabel";
            this.rowCountLabel.Size = new System.Drawing.Size(80, 32);
            this.rowCountLabel.Text = "(0)";
            this.rowCountLabel.ToolTipText = "Assignments in the past 2 minutes";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // incompleteLabel
            // 
            this.incompleteLabel.AutoSize = false;
            this.incompleteLabel.Name = "incompleteLabel";
            this.incompleteLabel.Size = new System.Drawing.Size(80, 32);
            this.incompleteLabel.Text = "(0)";
            this.incompleteLabel.ToolTipText = "Incomplete assignments";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.AutoSize = false;
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(100, 22);
            // 
            // pauseDetailsGridButton
            // 
            this.pauseDetailsGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseDetailsGridButton.Image = global::STEM.Surge.ControlPanel.Properties.Resources.pause;
            this.pauseDetailsGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseDetailsGridButton.Name = "pauseDetailsGridButton";
            this.pauseDetailsGridButton.Size = new System.Drawing.Size(24, 32);
            this.pauseDetailsGridButton.ToolTipText = "Stop / Start Table Refresh";
            this.pauseDetailsGridButton.Click += new System.EventHandler(this.pauseDetailsGridButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 35);
            // 
            // clearAssignmentFilter
            // 
            this.clearAssignmentFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearAssignmentFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearAssignmentFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearAssignmentFilter.Name = "clearAssignmentFilter";
            this.clearAssignmentFilter.Size = new System.Drawing.Size(24, 32);
            this.clearAssignmentFilter.Text = "Clear Filter";
            this.clearAssignmentFilter.Click += new System.EventHandler(this.clearAssignmentFilter_Click);
            // 
            // assignmentsFilterTextbox
            // 
            this.assignmentsFilterTextbox.AutoSize = false;
            this.assignmentsFilterTextbox.BackColor = System.Drawing.Color.Lavender;
            this.assignmentsFilterTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.assignmentsFilterTextbox.Name = "assignmentsFilterTextbox";
            this.assignmentsFilterTextbox.Size = new System.Drawing.Size(400, 20);
            this.assignmentsFilterTextbox.ToolTipText = "Filter Rows";
            // 
            // advancedFilter
            // 
            this.advancedFilter.BackgroundImage = global::STEM.Surge.ControlPanel.Properties.Resources.tools;
            this.advancedFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.advancedFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.advancedFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.tools;
            this.advancedFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.advancedFilter.Name = "advancedFilter";
            this.advancedFilter.Size = new System.Drawing.Size(24, 32);
            this.advancedFilter.Text = "Set advanced column filters";
            this.advancedFilter.Click += new System.EventHandler(this.advancedFilter_Click);
            // 
            // avgExecutionTime
            // 
            this.avgExecutionTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.avgExecutionTime.AutoSize = false;
            this.avgExecutionTime.Name = "avgExecutionTime";
            this.avgExecutionTime.Size = new System.Drawing.Size(250, 32);
            this.avgExecutionTime.Text = "Average Execution Time: 0.00s";
            this.avgExecutionTime.ToolTipText = "Average Execution Time Of Displayed Rows";
            // 
            // managerDataGridViewTextBoxColumn1
            // 
            this.managerDataGridViewTextBoxColumn1.DataPropertyName = "Manager";
            this.managerDataGridViewTextBoxColumn1.HeaderText = "Manager";
            this.managerDataGridViewTextBoxColumn1.Name = "managerDataGridViewTextBoxColumn1";
            this.managerDataGridViewTextBoxColumn1.ReadOnly = true;
            this.managerDataGridViewTextBoxColumn1.Width = 150;
            // 
            // BranchIP
            // 
            this.BranchIP.DataPropertyName = "BranchIP";
            this.BranchIP.HeaderText = "BranchIP";
            this.BranchIP.Name = "BranchIP";
            this.BranchIP.ReadOnly = true;
            this.BranchIP.Width = 150;
            // 
            // BranchName
            // 
            this.BranchName.DataPropertyName = "BranchName";
            this.BranchName.HeaderText = "BranchName";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            this.BranchName.Width = 150;
            // 
            // controllerDataGridViewTextBoxColumn1
            // 
            this.controllerDataGridViewTextBoxColumn1.DataPropertyName = "Controller";
            this.controllerDataGridViewTextBoxColumn1.HeaderText = "Controller";
            this.controllerDataGridViewTextBoxColumn1.Name = "controllerDataGridViewTextBoxColumn1";
            this.controllerDataGridViewTextBoxColumn1.ReadOnly = true;
            this.controllerDataGridViewTextBoxColumn1.Width = 200;
            // 
            // sourceDataGridViewTextBoxColumn
            // 
            this.sourceDataGridViewTextBoxColumn.DataPropertyName = "Source";
            this.sourceDataGridViewTextBoxColumn.HeaderText = "Source";
            this.sourceDataGridViewTextBoxColumn.Name = "sourceDataGridViewTextBoxColumn";
            this.sourceDataGridViewTextBoxColumn.ReadOnly = true;
            this.sourceDataGridViewTextBoxColumn.Width = 200;
            // 
            // issuedDataGridViewTextBoxColumn
            // 
            this.issuedDataGridViewTextBoxColumn.DataPropertyName = "Issued";
            dataGridViewCellStyle2.Format = "G";
            this.issuedDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.issuedDataGridViewTextBoxColumn.HeaderText = "Issued";
            this.issuedDataGridViewTextBoxColumn.Name = "issuedDataGridViewTextBoxColumn";
            this.issuedDataGridViewTextBoxColumn.ReadOnly = true;
            this.issuedDataGridViewTextBoxColumn.Width = 150;
            // 
            // receivedDataGridViewTextBoxColumn
            // 
            this.receivedDataGridViewTextBoxColumn.DataPropertyName = "Received";
            dataGridViewCellStyle3.Format = "G";
            this.receivedDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.receivedDataGridViewTextBoxColumn.HeaderText = "Received";
            this.receivedDataGridViewTextBoxColumn.Name = "receivedDataGridViewTextBoxColumn";
            this.receivedDataGridViewTextBoxColumn.ReadOnly = true;
            this.receivedDataGridViewTextBoxColumn.Width = 150;
            // 
            // completedDataGridViewTextBoxColumn
            // 
            this.completedDataGridViewTextBoxColumn.DataPropertyName = "Completed";
            dataGridViewCellStyle4.Format = "G";
            this.completedDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.completedDataGridViewTextBoxColumn.HeaderText = "Completed";
            this.completedDataGridViewTextBoxColumn.Name = "completedDataGridViewTextBoxColumn";
            this.completedDataGridViewTextBoxColumn.ReadOnly = true;
            this.completedDataGridViewTextBoxColumn.Width = 150;
            // 
            // ExecutionTime
            // 
            this.ExecutionTime.DataPropertyName = "ExecutionTime";
            dataGridViewCellStyle5.Format = "0.0000";
            this.ExecutionTime.DefaultCellStyle = dataGridViewCellStyle5;
            this.ExecutionTime.HeaderText = "Execution Time";
            this.ExecutionTime.Name = "ExecutionTime";
            this.ExecutionTime.ReadOnly = true;
            // 
            // Exceptions
            // 
            this.Exceptions.DataPropertyName = "Exceptions";
            this.Exceptions.HeaderText = "Exceptions";
            this.Exceptions.Name = "Exceptions";
            this.Exceptions.ReadOnly = true;
            // 
            // instructionSetIDDataGridViewTextBoxColumn
            // 
            this.instructionSetIDDataGridViewTextBoxColumn.DataPropertyName = "InstructionSetID";
            this.instructionSetIDDataGridViewTextBoxColumn.HeaderText = "InstructionSetID";
            this.instructionSetIDDataGridViewTextBoxColumn.Name = "instructionSetIDDataGridViewTextBoxColumn";
            this.instructionSetIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.instructionSetIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // deploymentManagerIDDataGridViewTextBoxColumn
            // 
            this.deploymentManagerIDDataGridViewTextBoxColumn.DataPropertyName = "DeploymentManagerID";
            this.deploymentManagerIDDataGridViewTextBoxColumn.HeaderText = "DeploymentManagerID";
            this.deploymentManagerIDDataGridViewTextBoxColumn.Name = "deploymentManagerIDDataGridViewTextBoxColumn";
            this.deploymentManagerIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.deploymentManagerIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // switchboardRowIDDataGridViewTextBoxColumn
            // 
            this.switchboardRowIDDataGridViewTextBoxColumn.DataPropertyName = "SwitchboardRowID";
            this.switchboardRowIDDataGridViewTextBoxColumn.HeaderText = "SwitchboardRowID";
            this.switchboardRowIDDataGridViewTextBoxColumn.Name = "switchboardRowIDDataGridViewTextBoxColumn";
            this.switchboardRowIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.switchboardRowIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // AssignmentDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.panel1);
            this.Name = "AssignmentDetailsView";
            this.Size = new System.Drawing.Size(1300, 459);
            this.detailsMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.assignmentDetailsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.assignmentDetailsGridView)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip detailsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewDetails;
        private System.Windows.Forms.ToolStripMenuItem cancelJob;
        private TableDataSources tableDataSources;
        private System.Windows.Forms.BindingSource assignmentDetailsBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView assignmentDetailsGridView;
        private System.Windows.Forms.CheckBox incomplete;
        private System.Windows.Forms.CheckBox scrollBottom;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel rowCountLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel incompleteLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripButton pauseDetailsGridButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton clearAssignmentFilter;
        private System.Windows.Forms.ToolStripTextBox assignmentsFilterTextbox;
        private System.Windows.Forms.ToolStripButton advancedFilter;
        private System.Windows.Forms.ToolStripLabel avgExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn managerDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn controllerDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn issuedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn completedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExecutionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Exceptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn instructionSetIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deploymentManagerIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn switchboardRowIDDataGridViewTextBoxColumn;
    }
}

namespace STEM.Surge.ControlPanel
{
    partial class ErrorViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorViewer));
            this.errorGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableDataSources = new STEM.Surge.ControlPanel.TableDataSources();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.rowCount = new System.Windows.Forms.ToolStripLabel();
            this.branchDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.completeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exceptionSummaryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.instructionSetIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.errorGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorGridView1
            // 
            this.errorGridView1.AllowUserToAddRows = false;
            this.errorGridView1.AutoGenerateColumns = false;
            this.errorGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.errorGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.errorGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.branchDataGridViewTextBoxColumn,
            this.ProcessName,
            this.completeDataGridViewTextBoxColumn,
            this.exceptionSummaryDataGridViewTextBoxColumn,
            this.instructionSetIDDataGridViewTextBoxColumn});
            this.errorGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.errorGridView1.DataSource = this.errorsBindingSource;
            this.errorGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorGridView1.Location = new System.Drawing.Point(0, 0);
            this.errorGridView1.Name = "errorGridView1";
            this.errorGridView1.ReadOnly = true;
            this.errorGridView1.Size = new System.Drawing.Size(1119, 333);
            this.errorGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDetailsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(138, 26);
            // 
            // viewDetailsToolStripMenuItem
            // 
            this.viewDetailsToolStripMenuItem.Name = "viewDetailsToolStripMenuItem";
            this.viewDetailsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.viewDetailsToolStripMenuItem.Text = "View Details";
            // 
            // errorsBindingSource
            // 
            this.errorsBindingSource.DataMember = "Errors";
            this.errorsBindingSource.DataSource = this.tableDataSources;
            // 
            // tableDataSources
            // 
            this.tableDataSources.DataSetName = "TableDataSources";
            this.tableDataSources.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1119, 368);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.errorGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1119, 333);
            this.panel2.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterMask,
            this.rowCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1119, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(23, 32);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // filterMask
            // 
            this.filterMask.AutoSize = false;
            this.filterMask.BackColor = System.Drawing.Color.Lavender;
            this.filterMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterMask.Name = "filterMask";
            this.filterMask.Size = new System.Drawing.Size(400, 35);
            this.filterMask.ToolTipText = "Filter Rows";
            this.filterMask.TextChanged += new System.EventHandler(this.filterMask_TextChanged);
            // 
            // rowCount
            // 
            this.rowCount.Name = "rowCount";
            this.rowCount.Size = new System.Drawing.Size(57, 32);
            this.rowCount.Text = "Count (0)";
            // 
            // branchDataGridViewTextBoxColumn
            // 
            this.branchDataGridViewTextBoxColumn.DataPropertyName = "Branch";
            this.branchDataGridViewTextBoxColumn.HeaderText = "Branch";
            this.branchDataGridViewTextBoxColumn.Name = "branchDataGridViewTextBoxColumn";
            this.branchDataGridViewTextBoxColumn.ReadOnly = true;
            this.branchDataGridViewTextBoxColumn.Width = 150;
            // 
            // ProcessName
            // 
            this.ProcessName.DataPropertyName = "ProcessName";
            this.ProcessName.HeaderText = "Process Name";
            this.ProcessName.Name = "ProcessName";
            this.ProcessName.ReadOnly = true;
            this.ProcessName.Width = 300;
            // 
            // completeDataGridViewTextBoxColumn
            // 
            this.completeDataGridViewTextBoxColumn.DataPropertyName = "Complete";
            dataGridViewCellStyle1.Format = "G";
            this.completeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.completeDataGridViewTextBoxColumn.HeaderText = "Complete";
            this.completeDataGridViewTextBoxColumn.Name = "completeDataGridViewTextBoxColumn";
            this.completeDataGridViewTextBoxColumn.ReadOnly = true;
            this.completeDataGridViewTextBoxColumn.Width = 150;
            // 
            // exceptionSummaryDataGridViewTextBoxColumn
            // 
            this.exceptionSummaryDataGridViewTextBoxColumn.DataPropertyName = "ExceptionSummary";
            this.exceptionSummaryDataGridViewTextBoxColumn.HeaderText = "Exception Summary";
            this.exceptionSummaryDataGridViewTextBoxColumn.Name = "exceptionSummaryDataGridViewTextBoxColumn";
            this.exceptionSummaryDataGridViewTextBoxColumn.ReadOnly = true;
            this.exceptionSummaryDataGridViewTextBoxColumn.Width = 400;
            // 
            // instructionSetIDDataGridViewTextBoxColumn
            // 
            this.instructionSetIDDataGridViewTextBoxColumn.DataPropertyName = "InstructionSetID";
            this.instructionSetIDDataGridViewTextBoxColumn.HeaderText = "InstructionSetID";
            this.instructionSetIDDataGridViewTextBoxColumn.Name = "instructionSetIDDataGridViewTextBoxColumn";
            this.instructionSetIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.instructionSetIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // ErrorViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1119, 368);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.errorGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataSources)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView errorGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewDetailsToolStripMenuItem;
        private System.Windows.Forms.BindingSource errorsBindingSource;
        private TableDataSources tableDataSources;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.ToolStripLabel rowCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProcessName;
        private System.Windows.Forms.DataGridViewTextBoxColumn completeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn exceptionSummaryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn instructionSetIDDataGridViewTextBoxColumn;
    }
}
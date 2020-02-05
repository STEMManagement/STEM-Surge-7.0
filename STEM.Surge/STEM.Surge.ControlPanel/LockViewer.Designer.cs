namespace STEM.Surge.ControlPanel
{
    partial class LockViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LockViewer));
            this.lockGridView1 = new System.Windows.Forms.DataGridView();
            this.managerIPDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.keyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lockTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastLockAttemptDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locksDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.rowCount = new System.Windows.Forms.ToolStripLabel();
            this.lockDetailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.lockGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locksDataTableBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockDetailBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lockGridView1
            // 
            this.lockGridView1.AllowUserToAddRows = false;
            this.lockGridView1.AllowUserToDeleteRows = false;
            this.lockGridView1.AutoGenerateColumns = false;
            this.lockGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.lockGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lockGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.managerIPDataGridViewTextBoxColumn,
            this.keyDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.lockTimeDataGridViewTextBoxColumn,
            this.lastLockAttemptDataGridViewTextBoxColumn});
            this.lockGridView1.DataSource = this.locksDataTableBindingSource;
            this.lockGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockGridView1.Location = new System.Drawing.Point(0, 0);
            this.lockGridView1.Name = "lockGridView1";
            this.lockGridView1.ReadOnly = true;
            this.lockGridView1.RowHeadersVisible = false;
            this.lockGridView1.Size = new System.Drawing.Size(1119, 333);
            this.lockGridView1.TabIndex = 0;
            // 
            // managerIPDataGridViewTextBoxColumn
            // 
            this.managerIPDataGridViewTextBoxColumn.DataPropertyName = "ManagerIP";
            this.managerIPDataGridViewTextBoxColumn.HeaderText = "ManagerIP";
            this.managerIPDataGridViewTextBoxColumn.MinimumWidth = 150;
            this.managerIPDataGridViewTextBoxColumn.Name = "managerIPDataGridViewTextBoxColumn";
            this.managerIPDataGridViewTextBoxColumn.ReadOnly = true;
            this.managerIPDataGridViewTextBoxColumn.Width = 200;
            // 
            // keyDataGridViewTextBoxColumn
            // 
            this.keyDataGridViewTextBoxColumn.DataPropertyName = "Key";
            this.keyDataGridViewTextBoxColumn.HeaderText = "Key";
            this.keyDataGridViewTextBoxColumn.MinimumWidth = 250;
            this.keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            this.keyDataGridViewTextBoxColumn.ReadOnly = true;
            this.keyDataGridViewTextBoxColumn.Width = 250;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.MinimumWidth = 300;
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 300;
            // 
            // lockTimeDataGridViewTextBoxColumn
            // 
            this.lockTimeDataGridViewTextBoxColumn.DataPropertyName = "LockTime";
            dataGridViewCellStyle1.Format = "G";
            this.lockTimeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.lockTimeDataGridViewTextBoxColumn.HeaderText = "Lock Time";
            this.lockTimeDataGridViewTextBoxColumn.MinimumWidth = 150;
            this.lockTimeDataGridViewTextBoxColumn.Name = "lockTimeDataGridViewTextBoxColumn";
            this.lockTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.lockTimeDataGridViewTextBoxColumn.Width = 150;
            // 
            // lastLockAttemptDataGridViewTextBoxColumn
            // 
            this.lastLockAttemptDataGridViewTextBoxColumn.DataPropertyName = "LastLockAttempt";
            dataGridViewCellStyle2.Format = "G";
            this.lastLockAttemptDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.lastLockAttemptDataGridViewTextBoxColumn.HeaderText = "Last Lock Attempt";
            this.lastLockAttemptDataGridViewTextBoxColumn.MinimumWidth = 150;
            this.lastLockAttemptDataGridViewTextBoxColumn.Name = "lastLockAttemptDataGridViewTextBoxColumn";
            this.lastLockAttemptDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastLockAttemptDataGridViewTextBoxColumn.Width = 150;
            // 
            // locksDataTableBindingSource
            // 
            this.locksDataTableBindingSource.DataSource = typeof(STEM.Surge.ControlPanel.TableDataSources.LocksDataTable);
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
            this.panel2.Controls.Add(this.lockGridView1);
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
            // lockDetailBindingSource
            // 
            this.lockDetailBindingSource.DataSource = typeof(STEM.Surge.ControlPanel.LockDetail);
            // 
            // LockViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1119, 368);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LockViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lock Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.lockGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locksDataTableBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockDetailBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView lockGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.ToolStripLabel rowCount;
        private System.Windows.Forms.BindingSource locksDataTableBindingSource;
        private System.Windows.Forms.BindingSource lockDetailBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn managerIPDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lockTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastLockAttemptDataGridViewTextBoxColumn;
    }
}
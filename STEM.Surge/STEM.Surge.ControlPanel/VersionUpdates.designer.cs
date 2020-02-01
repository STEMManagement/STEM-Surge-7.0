namespace STEM.Surge.ControlPanel
{
    partial class VersionUpdates
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.updateVersions = new System.Windows.Forms.ToolStripButton();
            this.selectAll = new System.Windows.Forms.ToolStripButton();
            this.deselectAll = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.updateEntries = new System.Windows.Forms.DataGridView();
            this.UpdateOb = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ObType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AsmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrigAsmVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AsmVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateEntries)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateVersions,
            this.selectAll,
            this.deselectAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(961, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // updateVersions
            // 
            this.updateVersions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updateVersions.Enabled = false;
            this.updateVersions.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.updateVersions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updateVersions.Name = "updateVersions";
            this.updateVersions.Size = new System.Drawing.Size(28, 28);
            this.updateVersions.Text = "Update Selected Files";
            this.updateVersions.ToolTipText = "Update Selected Files";
            this.updateVersions.Click += new System.EventHandler(this.updateVersions_Click);
            // 
            // selectAll
            // 
            this.selectAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_check;
            this.selectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(28, 28);
            this.selectAll.Text = "toolStripButton1";
            this.selectAll.ToolTipText = "Select All";
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // deselectAll
            // 
            this.deselectAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deselectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deselectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_uncheck;
            this.deselectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deselectAll.Name = "deselectAll";
            this.deselectAll.Size = new System.Drawing.Size(28, 28);
            this.deselectAll.Text = "toolStripButton2";
            this.deselectAll.ToolTipText = "Deselect All";
            this.deselectAll.Click += new System.EventHandler(this.deselectAll_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(961, 462);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(961, 431);
            this.panel2.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.updateEntries);
            this.splitContainer1.Size = new System.Drawing.Size(961, 431);
            this.splitContainer1.SplitterDistance = 406;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AsmName,
            this.OrigAsmVersion,
            this.AsmVersion});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(406, 431);
            this.dataGridView1.TabIndex = 0;
            // 
            // updateEntries
            // 
            this.updateEntries.AllowUserToAddRows = false;
            this.updateEntries.AllowUserToDeleteRows = false;
            this.updateEntries.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.updateEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.updateEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UpdateOb,
            this.ObType,
            this.ObFile,
            this.OrigVersion,
            this.NewVersion});
            this.updateEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateEntries.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.updateEntries.Location = new System.Drawing.Point(0, 0);
            this.updateEntries.Name = "updateEntries";
            this.updateEntries.RowHeadersVisible = false;
            this.updateEntries.Size = new System.Drawing.Size(551, 431);
            this.updateEntries.TabIndex = 0;
            // 
            // UpdateOb
            // 
            this.UpdateOb.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.UpdateOb.HeaderText = "Update";
            this.UpdateOb.Name = "UpdateOb";
            this.UpdateOb.Width = 46;
            // 
            // ObType
            // 
            this.ObType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ObType.HeaderText = "Type";
            this.ObType.Name = "ObType";
            this.ObType.ReadOnly = true;
            this.ObType.Width = 54;
            // 
            // ObFile
            // 
            this.ObFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ObFile.HeaderText = "File";
            this.ObFile.Name = "ObFile";
            this.ObFile.ReadOnly = true;
            this.ObFile.Width = 46;
            // 
            // OrigVersion
            // 
            this.OrigVersion.HeaderText = "OrigVersion";
            this.OrigVersion.Name = "OrigVersion";
            this.OrigVersion.ReadOnly = true;
            this.OrigVersion.Visible = false;
            // 
            // NewVersion
            // 
            this.NewVersion.HeaderText = "NewVersion";
            this.NewVersion.Name = "NewVersion";
            this.NewVersion.ReadOnly = true;
            this.NewVersion.Visible = false;
            // 
            // AsmName
            // 
            this.AsmName.HeaderText = "Assembley";
            this.AsmName.Name = "AsmName";
            this.AsmName.ReadOnly = true;
            this.AsmName.Width = 200;
            // 
            // OrigAsmVersion
            // 
            this.OrigAsmVersion.HeaderText = "OrigAsmVersion";
            this.OrigAsmVersion.Name = "OrigAsmVersion";
            this.OrigAsmVersion.ReadOnly = true;
            this.OrigAsmVersion.Visible = false;
            // 
            // AsmVersion
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.AsmVersion.DefaultCellStyle = dataGridViewCellStyle2;
            this.AsmVersion.HeaderText = "Version";
            this.AsmVersion.Name = "AsmVersion";
            this.AsmVersion.Width = 200;
            // 
            // VersionUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.panel1);
            this.Name = "VersionUpdates";
            this.Size = new System.Drawing.Size(961, 462);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateEntries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton updateVersions;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView updateEntries;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UpdateOb;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewVersion;
        private System.Windows.Forms.ToolStripButton selectAll;
        private System.Windows.Forms.ToolStripButton deselectAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn AsmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrigAsmVersion;
        private System.Windows.Forms.DataGridViewComboBoxColumn AsmVersion;
    }
}

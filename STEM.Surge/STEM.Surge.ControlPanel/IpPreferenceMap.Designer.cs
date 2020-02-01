namespace STEM.Surge.ControlPanel
{
    partial class IpPreferenceMap
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IpPreferenceMap));
            this.ipPreferenceGrid = new System.Windows.Forms.DataGridView();
            this.ipPreferenceMapDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sourceOctetsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.destinationOctetsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.strictlyEnforceDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Save = new System.Windows.Forms.ToolStripButton();
            this.Cancel = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.ipPreferenceGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ipPreferenceMapDataTableBindingSource)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipPreferenceGrid
            // 
            this.ipPreferenceGrid.AutoGenerateColumns = false;
            this.ipPreferenceGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.ipPreferenceGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ipPreferenceGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ipPreferenceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ipPreferenceGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.sourceOctetsDataGridViewTextBoxColumn,
            this.destinationOctetsDataGridViewTextBoxColumn,
            this.strictlyEnforceDataGridViewCheckBoxColumn});
            this.ipPreferenceGrid.DataSource = this.ipPreferenceMapDataTableBindingSource;
            this.ipPreferenceGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipPreferenceGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.ipPreferenceGrid.Location = new System.Drawing.Point(0, 0);
            this.ipPreferenceGrid.Name = "ipPreferenceGrid";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ipPreferenceGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.ipPreferenceGrid.Size = new System.Drawing.Size(577, 419);
            this.ipPreferenceGrid.TabIndex = 1;
            // 
            // ipPreferenceMapDataTableBindingSource
            // 
            this.ipPreferenceMapDataTableBindingSource.DataSource = typeof(STEM.Surge.SwitchboardConfig.IpPreferenceMapDataTable);
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.HeaderText = "";
            this.Column2.MinimumWidth = 2;
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.Width = 2;
            // 
            // sourceOctetsDataGridViewTextBoxColumn
            // 
            this.sourceOctetsDataGridViewTextBoxColumn.DataPropertyName = "SourceOctets";
            this.sourceOctetsDataGridViewTextBoxColumn.HeaderText = "SourceOctets";
            this.sourceOctetsDataGridViewTextBoxColumn.MinimumWidth = 200;
            this.sourceOctetsDataGridViewTextBoxColumn.Name = "sourceOctetsDataGridViewTextBoxColumn";
            this.sourceOctetsDataGridViewTextBoxColumn.Width = 200;
            // 
            // destinationOctetsDataGridViewTextBoxColumn
            // 
            this.destinationOctetsDataGridViewTextBoxColumn.DataPropertyName = "DestinationOctets";
            this.destinationOctetsDataGridViewTextBoxColumn.HeaderText = "DestinationOctets";
            this.destinationOctetsDataGridViewTextBoxColumn.MinimumWidth = 200;
            this.destinationOctetsDataGridViewTextBoxColumn.Name = "destinationOctetsDataGridViewTextBoxColumn";
            this.destinationOctetsDataGridViewTextBoxColumn.Width = 200;
            // 
            // strictlyEnforceDataGridViewCheckBoxColumn
            // 
            this.strictlyEnforceDataGridViewCheckBoxColumn.DataPropertyName = "StrictlyEnforce";
            this.strictlyEnforceDataGridViewCheckBoxColumn.HeaderText = "StrictlyEnforce";
            this.strictlyEnforceDataGridViewCheckBoxColumn.Name = "strictlyEnforceDataGridViewCheckBoxColumn";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save,
            this.Cancel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(577, 31);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Save
            // 
            this.Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Save.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(28, 28);
            this.Save.Text = "Save Settings";
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Cancel.Image = global::STEM.Surge.ControlPanel.Properties.Resources.undo;
            this.Cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(28, 28);
            this.Cancel.Text = "Undo and reload last saved configuration";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ipPreferenceGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(577, 419);
            this.panel1.TabIndex = 3;
            // 
            // IpPreferenceMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "IpPreferenceMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IP Preference Map";
            ((System.ComponentModel.ISupportInitialize)(this.ipPreferenceGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ipPreferenceMapDataTableBindingSource)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ipPreferenceGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceOctetsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn destinationOctetsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn strictlyEnforceDataGridViewCheckBoxColumn;
        private System.Windows.Forms.BindingSource ipPreferenceMapDataTableBindingSource;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Save;
        private System.Windows.Forms.ToolStripButton Cancel;
        private System.Windows.Forms.Panel panel1;
    }
}
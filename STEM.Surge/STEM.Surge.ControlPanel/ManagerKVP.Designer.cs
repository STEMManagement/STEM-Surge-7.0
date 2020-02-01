namespace STEM.Surge.ControlPanel
{
    partial class ManagerKVP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerKVP));
            this.macroPlaceholderGrid = new System.Windows.Forms.DataGridView();
            this.configurationMacroMapDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.placeholderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Save = new System.Windows.Forms.ToolStripButton();
            this.Cancel = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterMask = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationMacroMapDataTableBindingSource)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // macroPlaceholderGrid
            // 
            this.macroPlaceholderGrid.AutoGenerateColumns = false;
            this.macroPlaceholderGrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.macroPlaceholderGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.macroPlaceholderGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.macroPlaceholderGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.macroPlaceholderGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.placeholderDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn});
            this.macroPlaceholderGrid.DataSource = this.configurationMacroMapDataTableBindingSource;
            this.macroPlaceholderGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.macroPlaceholderGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.macroPlaceholderGrid.GridColor = System.Drawing.Color.LightGray;
            this.macroPlaceholderGrid.Location = new System.Drawing.Point(0, 0);
            this.macroPlaceholderGrid.Name = "macroPlaceholderGrid";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.macroPlaceholderGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.macroPlaceholderGrid.Size = new System.Drawing.Size(683, 415);
            this.macroPlaceholderGrid.TabIndex = 4;
            // 
            // configurationMacroMapDataTableBindingSource
            // 
            this.configurationMacroMapDataTableBindingSource.DataSource = typeof(STEM.Surge.SwitchboardConfig.ConfigurationMacroMapDataTable);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column1.HeaderText = "";
            this.Column1.MinimumWidth = 2;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.Width = 2;
            // 
            // placeholderDataGridViewTextBoxColumn
            // 
            this.placeholderDataGridViewTextBoxColumn.DataPropertyName = "Placeholder";
            this.placeholderDataGridViewTextBoxColumn.HeaderText = "Placeholder";
            this.placeholderDataGridViewTextBoxColumn.MinimumWidth = 200;
            this.placeholderDataGridViewTextBoxColumn.Name = "placeholderDataGridViewTextBoxColumn";
            this.placeholderDataGridViewTextBoxColumn.Width = 200;
            // 
            // valueDataGridViewTextBoxColumn
            // 
            this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
            this.valueDataGridViewTextBoxColumn.MinimumWidth = 400;
            this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            this.valueDataGridViewTextBoxColumn.Width = 400;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save,
            this.Cancel,
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.clearFilter,
            this.filterMask});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(683, 35);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Save
            // 
            this.Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Save.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(28, 32);
            this.Save.Text = "Save Settings";
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Cancel.Image = global::STEM.Surge.ControlPanel.Properties.Resources.undo;
            this.Cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(28, 32);
            this.Cancel.Text = "Undo and reload last saved configuration";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.macroPlaceholderGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(683, 415);
            this.panel1.TabIndex = 6;
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(28, 32);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.ClearFilter_Click);
            // 
            // filterMask
            // 
            this.filterMask.AutoSize = false;
            this.filterMask.BackColor = System.Drawing.Color.Lavender;
            this.filterMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterMask.Name = "filterMask";
            this.filterMask.Size = new System.Drawing.Size(400, 20);
            this.filterMask.ToolTipText = "Filter the Placeholder Grid Rows";
            this.filterMask.TextChanged += new System.EventHandler(this.filterMask_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(30, 32);
            // 
            // ManagerKVP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ManagerKVP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manager Placeholders";
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationMacroMapDataTableBindingSource)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView macroPlaceholderGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn placeholderDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource configurationMacroMapDataTableBindingSource;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Save;
        private System.Windows.Forms.ToolStripButton Cancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterMask;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}
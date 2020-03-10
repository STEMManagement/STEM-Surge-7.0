namespace STEM.Surge.ControlPanel
{
    partial class ControllerEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.controllerProperties = new System.Windows.Forms.PropertyGrid();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.deploymentControllerName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.save = new System.Windows.Forms.ToolStripButton();
            this.openTemplate = new System.Windows.Forms.ToolStripButton();
            this.detailsLabel = new System.Windows.Forms.ToolStripLabel();
            this.macroPlaceholderGrid = new System.Windows.Forms.DataGridView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.Insert = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Placeholder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).BeginInit();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.controllerProperties);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip2);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.macroPlaceholderGrid);
            this.splitContainer2.Panel2.Controls.Add(this.toolStrip3);
            this.splitContainer2.Size = new System.Drawing.Size(847, 602);
            this.splitContainer2.SplitterDistance = 402;
            this.splitContainer2.TabIndex = 1;
            // 
            // controllerProperties
            // 
            this.controllerProperties.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.controllerProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controllerProperties.HelpBackColor = System.Drawing.Color.Gainsboro;
            this.controllerProperties.LineColor = System.Drawing.Color.Gainsboro;
            this.controllerProperties.Location = new System.Drawing.Point(0, 56);
            this.controllerProperties.Name = "controllerProperties";
            this.controllerProperties.Size = new System.Drawing.Size(847, 346);
            this.controllerProperties.TabIndex = 14;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.deploymentControllerName});
            this.toolStrip2.Location = new System.Drawing.Point(0, 31);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(847, 25);
            this.toolStrip2.TabIndex = 13;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(163, 22);
            this.toolStripLabel1.Text = "Deployment Controller Name";
            // 
            // deploymentControllerName
            // 
            this.deploymentControllerName.AutoSize = false;
            this.deploymentControllerName.BackColor = System.Drawing.Color.Lavender;
            this.deploymentControllerName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.deploymentControllerName.Name = "deploymentControllerName";
            this.deploymentControllerName.Size = new System.Drawing.Size(300, 25);
            // 
            // toolStrip4
            // 
            this.toolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save,
            this.openTemplate,
            this.detailsLabel});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(847, 31);
            this.toolStrip4.TabIndex = 17;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // save
            // 
            this.save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.save.Enabled = false;
            this.save.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(28, 28);
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // openTemplate
            // 
            this.openTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openTemplate.Image = global::STEM.Surge.ControlPanel.Properties.Resources.edit_2;
            this.openTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openTemplate.Name = "openTemplate";
            this.openTemplate.Size = new System.Drawing.Size(28, 28);
            this.openTemplate.Text = "Open InstructionSet Template";
            this.openTemplate.Click += new System.EventHandler(this.openTemplate_Click);
            // 
            // detailsLabel
            // 
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(0, 28);
            // 
            // macroPlaceholderGrid
            // 
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
            this.Insert,
            this.Placeholder,
            this.Value});
            this.macroPlaceholderGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.macroPlaceholderGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.macroPlaceholderGrid.GridColor = System.Drawing.Color.LightGray;
            this.macroPlaceholderGrid.Location = new System.Drawing.Point(0, 25);
            this.macroPlaceholderGrid.Name = "macroPlaceholderGrid";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.macroPlaceholderGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.macroPlaceholderGrid.Size = new System.Drawing.Size(847, 171);
            this.macroPlaceholderGrid.TabIndex = 15;
            // 
            // toolStrip3
            // 
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(847, 25);
            this.toolStrip3.TabIndex = 14;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(105, 22);
            this.toolStripLabel2.Text = "Value Placeholders";
            // 
            // Insert
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "+";
            this.Insert.DefaultCellStyle = dataGridViewCellStyle2;
            this.Insert.HeaderText = "";
            this.Insert.MinimumWidth = 25;
            this.Insert.Name = "Insert";
            this.Insert.Text = "+";
            this.Insert.ToolTipText = "Insert into configuration";
            this.Insert.Width = 25;
            // 
            // Placeholder
            // 
            this.Placeholder.HeaderText = "Placeholder";
            this.Placeholder.MinimumWidth = 200;
            this.Placeholder.Name = "Placeholder";
            this.Placeholder.Width = 200;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 275;
            this.Value.Name = "Value";
            // 
            // ControllerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "ControllerEditor";
            this.Size = new System.Drawing.Size(847, 602);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).EndInit();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView macroPlaceholderGrid;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.PropertyGrid controllerProperties;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton save;
        private System.Windows.Forms.ToolStripButton openTemplate;
        private System.Windows.Forms.ToolStripLabel detailsLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox deploymentControllerName;
        private System.Windows.Forms.DataGridViewButtonColumn Insert;
        private System.Windows.Forms.DataGridViewTextBoxColumn Placeholder;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}

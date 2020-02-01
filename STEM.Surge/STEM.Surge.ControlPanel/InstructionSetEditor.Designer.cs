namespace STEM.Surge.ControlPanel
{
    partial class InstructionSetEditor
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
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.save = new System.Windows.Forms.ToolStripButton();
            this.usedByControllers = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.instructionList = new System.Windows.Forms.ListBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.deleteInstruction = new System.Windows.Forms.ToolStripButton();
            this.addInstruction = new System.Windows.Forms.ToolStripButton();
            this.moveUp = new System.Windows.Forms.ToolStripButton();
            this.moveDown = new System.Windows.Forms.ToolStripButton();
            this.instructionProperties = new System.Windows.Forms.PropertyGrid();
            this.ISetExtras = new System.Windows.Forms.Panel();
            this.runInSandboxes = new System.Windows.Forms.CheckBox();
            this.cachePostMortem = new System.Windows.Forms.CheckBox();
            this.repeatInterval = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.continuousExecution = new System.Windows.Forms.CheckBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.processName = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.macroPlaceholderGrid = new System.Windows.Forms.DataGridView();
            this.Insert = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Placeholder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.ISetExtras.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save,
            this.usedByControllers});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(958, 31);
            this.toolStrip2.TabIndex = 10;
            this.toolStrip2.Text = "toolStrip2";
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
            // usedByControllers
            // 
            this.usedByControllers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.usedByControllers.Image = global::STEM.Surge.ControlPanel.Properties.Resources.connect;
            this.usedByControllers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.usedByControllers.Name = "usedByControllers";
            this.usedByControllers.Size = new System.Drawing.Size(28, 28);
            this.usedByControllers.Text = "Used By...";
            this.usedByControllers.Click += new System.EventHandler(this.usedByControllers_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.instructionList);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.instructionProperties);
            this.splitContainer1.Panel2.Controls.Add(this.ISetExtras);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip4);
            this.splitContainer1.Size = new System.Drawing.Size(958, 268);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 11;
            // 
            // instructionList
            // 
            this.instructionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionList.FormattingEnabled = true;
            this.instructionList.Location = new System.Drawing.Point(0, 25);
            this.instructionList.Name = "instructionList";
            this.instructionList.Size = new System.Drawing.Size(260, 243);
            this.instructionList.TabIndex = 2;
            this.instructionList.SelectedIndexChanged += new System.EventHandler(this.instructionList_SelectedIndexChanged);
            // 
            // toolStrip3
            // 
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteInstruction,
            this.addInstruction,
            this.moveUp,
            this.moveDown});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(260, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // deleteInstruction
            // 
            this.deleteInstruction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deleteInstruction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteInstruction.Image = global::STEM.Surge.ControlPanel.Properties.Resources.garbage;
            this.deleteInstruction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteInstruction.Name = "deleteInstruction";
            this.deleteInstruction.Size = new System.Drawing.Size(23, 22);
            this.deleteInstruction.Text = "Delete Instruction";
            this.deleteInstruction.Click += new System.EventHandler(this.deleteInstruction_Click);
            // 
            // addInstruction
            // 
            this.addInstruction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.addInstruction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addInstruction.Image = global::STEM.Surge.ControlPanel.Properties.Resources.add;
            this.addInstruction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addInstruction.Name = "addInstruction";
            this.addInstruction.Size = new System.Drawing.Size(23, 22);
            this.addInstruction.Text = "Add Instruction";
            this.addInstruction.Click += new System.EventHandler(this.addInstruction_Click);
            // 
            // moveUp
            // 
            this.moveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUp.Image = global::STEM.Surge.ControlPanel.Properties.Resources.up;
            this.moveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUp.Name = "moveUp";
            this.moveUp.Size = new System.Drawing.Size(23, 22);
            this.moveUp.Text = "Move Up";
            this.moveUp.Click += new System.EventHandler(this.moveUp_Click);
            // 
            // moveDown
            // 
            this.moveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDown.Image = global::STEM.Surge.ControlPanel.Properties.Resources.down;
            this.moveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDown.Name = "moveDown";
            this.moveDown.Size = new System.Drawing.Size(23, 22);
            this.moveDown.Text = "Move Down";
            this.moveDown.Click += new System.EventHandler(this.moveDown_Click);
            // 
            // instructionProperties
            // 
            this.instructionProperties.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.instructionProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionProperties.HelpBackColor = System.Drawing.Color.Gainsboro;
            this.instructionProperties.LineColor = System.Drawing.Color.Gainsboro;
            this.instructionProperties.Location = new System.Drawing.Point(0, 76);
            this.instructionProperties.Name = "instructionProperties";
            this.instructionProperties.Size = new System.Drawing.Size(694, 192);
            this.instructionProperties.TabIndex = 12;
            // 
            // ISetExtras
            // 
            this.ISetExtras.Controls.Add(this.runInSandboxes);
            this.ISetExtras.Controls.Add(this.cachePostMortem);
            this.ISetExtras.Controls.Add(this.repeatInterval);
            this.ISetExtras.Controls.Add(this.label1);
            this.ISetExtras.Controls.Add(this.continuousExecution);
            this.ISetExtras.Dock = System.Windows.Forms.DockStyle.Top;
            this.ISetExtras.Location = new System.Drawing.Point(0, 25);
            this.ISetExtras.Name = "ISetExtras";
            this.ISetExtras.Size = new System.Drawing.Size(694, 51);
            this.ISetExtras.TabIndex = 17;
            // 
            // runInSandboxes
            // 
            this.runInSandboxes.AutoSize = true;
            this.runInSandboxes.Location = new System.Drawing.Point(149, 5);
            this.runInSandboxes.Name = "runInSandboxes";
            this.runInSandboxes.Size = new System.Drawing.Size(113, 17);
            this.runInSandboxes.TabIndex = 19;
            this.runInSandboxes.Text = "Run in Sandboxes";
            this.runInSandboxes.UseVisualStyleBackColor = true;
            // 
            // cachePostMortem
            // 
            this.cachePostMortem.AutoSize = true;
            this.cachePostMortem.Location = new System.Drawing.Point(3, 5);
            this.cachePostMortem.Name = "cachePostMortem";
            this.cachePostMortem.Size = new System.Drawing.Size(119, 17);
            this.cachePostMortem.TabIndex = 18;
            this.cachePostMortem.Text = "Cache Post Mortem";
            this.cachePostMortem.UseVisualStyleBackColor = true;
            this.cachePostMortem.CheckedChanged += new System.EventHandler(this.cachePostMortem_CheckedChanged);
            // 
            // repeatInterval
            // 
            this.repeatInterval.Location = new System.Drawing.Point(283, 25);
            this.repeatInterval.Name = "repeatInterval";
            this.repeatInterval.Size = new System.Drawing.Size(55, 20);
            this.repeatInterval.TabIndex = 15;
            this.repeatInterval.Text = "60";
            this.repeatInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.repeatInterval.TextChanged += new System.EventHandler(this.RepeatInterval_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Repeat Interval (Seconds)";
            // 
            // continuousExecution
            // 
            this.continuousExecution.AutoSize = true;
            this.continuousExecution.Location = new System.Drawing.Point(3, 24);
            this.continuousExecution.Name = "continuousExecution";
            this.continuousExecution.Size = new System.Drawing.Size(129, 17);
            this.continuousExecution.TabIndex = 13;
            this.continuousExecution.Text = "Continuous Execution";
            this.continuousExecution.UseVisualStyleBackColor = true;
            this.continuousExecution.CheckedChanged += new System.EventHandler(this.continuousExecution_CheckedChanged);
            // 
            // toolStrip4
            // 
            this.toolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.processName});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(694, 25);
            this.toolStrip4.TabIndex = 11;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(82, 22);
            this.toolStripLabel1.Text = "Process Name";
            // 
            // processName
            // 
            this.processName.BackColor = System.Drawing.Color.Lavender;
            this.processName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.processName.Name = "processName";
            this.processName.Size = new System.Drawing.Size(440, 25);
            this.processName.ToolTipText = "What name would you give the process that these instructions make up?";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 31);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.macroPlaceholderGrid);
            this.splitContainer2.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer2.Size = new System.Drawing.Size(958, 449);
            this.splitContainer2.SplitterDistance = 268;
            this.splitContainer2.TabIndex = 12;
            // 
            // macroPlaceholderGrid
            // 
            this.macroPlaceholderGrid.AllowUserToAddRows = false;
            this.macroPlaceholderGrid.AllowUserToDeleteRows = false;
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
            this.macroPlaceholderGrid.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.macroPlaceholderGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.macroPlaceholderGrid.RowHeadersVisible = false;
            this.macroPlaceholderGrid.Size = new System.Drawing.Size(958, 152);
            this.macroPlaceholderGrid.TabIndex = 16;
            // 
            // Insert
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "+";
            this.Insert.DefaultCellStyle = dataGridViewCellStyle2;
            this.Insert.HeaderText = "";
            this.Insert.MinimumWidth = 25;
            this.Insert.Name = "Insert";
            this.Insert.ReadOnly = true;
            this.Insert.Text = "+";
            this.Insert.ToolTipText = "Insert into configuration";
            this.Insert.Width = 25;
            // 
            // Placeholder
            // 
            this.Placeholder.HeaderText = "Placeholder";
            this.Placeholder.MinimumWidth = 200;
            this.Placeholder.Name = "Placeholder";
            this.Placeholder.ReadOnly = true;
            this.Placeholder.Width = 200;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 275;
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            this.Value.Width = 275;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(958, 25);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(105, 22);
            this.toolStripLabel2.Text = "Value Placeholders";
            // 
            // InstructionSetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.toolStrip2);
            this.Name = "InstructionSetEditor";
            this.Size = new System.Drawing.Size(958, 480);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ISetExtras.ResumeLayout(false);
            this.ISetExtras.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.macroPlaceholderGrid)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton save;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton deleteInstruction;
        private System.Windows.Forms.ToolStripButton addInstruction;
        private System.Windows.Forms.ToolStripButton moveUp;
        private System.Windows.Forms.ToolStripButton moveDown;
        private System.Windows.Forms.ListBox instructionList;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox processName;
        private System.Windows.Forms.PropertyGrid instructionProperties;
        private System.Windows.Forms.CheckBox continuousExecution;
        private System.Windows.Forms.TextBox repeatInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripButton usedByControllers;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.DataGridView macroPlaceholderGrid;
        private System.Windows.Forms.DataGridViewButtonColumn Insert;
        private System.Windows.Forms.DataGridViewTextBoxColumn Placeholder;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Panel ISetExtras;
        private System.Windows.Forms.CheckBox cachePostMortem;
        private System.Windows.Forms.CheckBox runInSandboxes;
    }
}

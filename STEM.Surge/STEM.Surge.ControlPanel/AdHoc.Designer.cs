namespace STEM.Surge.ControlPanel
{
    partial class AdHoc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdHoc));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.instructionList = new System.Windows.Forms.ListBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.deleteInstruction = new System.Windows.Forms.ToolStripButton();
            this.addInstruction = new System.Windows.Forms.ToolStripButton();
            this.moveUp = new System.Windows.Forms.ToolStripButton();
            this.moveDown = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.instructionProperties = new System.Windows.Forms.PropertyGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.branchList = new System.Windows.Forms.CheckedListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.deploy = new System.Windows.Forms.ToolStripButton();
            this.selectAll = new System.Windows.Forms.ToolStripButton();
            this.deselectAll = new System.Windows.Forms.ToolStripButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.panel5 = new System.Windows.Forms.Panel();
            this.savedAdHoc = new System.Windows.Forms.ListBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.deleteFile = new System.Windows.Forms.ToolStripButton();
            this.newFile = new System.Windows.Forms.ToolStripButton();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterBox = new System.Windows.Forms.ToolStripTextBox();
            this.instructionSetEditPanel = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.save = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.processName = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.instructionSetEditPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(642, 480);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 2;
            // 
            // instructionList
            // 
            this.instructionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionList.FormattingEnabled = true;
            this.instructionList.Location = new System.Drawing.Point(0, 25);
            this.instructionList.Name = "instructionList";
            this.instructionList.Size = new System.Drawing.Size(213, 455);
            this.instructionList.TabIndex = 5;
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
            this.toolStrip3.Size = new System.Drawing.Size(213, 25);
            this.toolStrip3.TabIndex = 4;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // deleteInstruction
            // 
            this.deleteInstruction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deleteInstruction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteInstruction.Image = global::STEM.Surge.ControlPanel.Properties.Resources.delete;
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
            // panel1
            // 
            this.panel1.Controls.Add(this.instructionProperties);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 480);
            this.panel1.TabIndex = 16;
            // 
            // instructionProperties
            // 
            this.instructionProperties.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.instructionProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionProperties.HelpBackColor = System.Drawing.Color.Gainsboro;
            this.instructionProperties.LineColor = System.Drawing.Color.Gainsboro;
            this.instructionProperties.Location = new System.Drawing.Point(0, 0);
            this.instructionProperties.Name = "instructionProperties";
            this.instructionProperties.Size = new System.Drawing.Size(421, 480);
            this.instructionProperties.TabIndex = 14;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel3);
            this.splitContainer2.Size = new System.Drawing.Size(1138, 511);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.SplitterWidth = 8;
            this.splitContainer2.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.branchList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(210, 480);
            this.panel2.TabIndex = 17;
            // 
            // branchList
            // 
            this.branchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.branchList.FormattingEnabled = true;
            this.branchList.Location = new System.Drawing.Point(0, 0);
            this.branchList.Name = "branchList";
            this.branchList.Size = new System.Drawing.Size(210, 480);
            this.branchList.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deploy,
            this.selectAll,
            this.deselectAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(210, 31);
            this.toolStrip1.TabIndex = 16;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // deploy
            // 
            this.deploy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deploy.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
            this.deploy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deploy.Name = "deploy";
            this.deploy.Size = new System.Drawing.Size(28, 28);
            this.deploy.Text = "Deploy";
            this.deploy.Click += new System.EventHandler(this.deploy_Click);
            // 
            // selectAll
            // 
            this.selectAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_check;
            this.selectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(28, 28);
            this.selectAll.Text = "Select All";
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
            this.deselectAll.Text = "Deselect All";
            this.deselectAll.Click += new System.EventHandler(this.deselectAll_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splitContainer3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(920, 511);
            this.panel3.TabIndex = 8;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.panel5);
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip4);
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip5);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.instructionSetEditPanel);
            this.splitContainer3.Size = new System.Drawing.Size(920, 511);
            this.splitContainer3.SplitterDistance = 270;
            this.splitContainer3.SplitterWidth = 8;
            this.splitContainer3.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.savedAdHoc);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 58);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(270, 453);
            this.panel5.TabIndex = 9;
            // 
            // savedAdHoc
            // 
            this.savedAdHoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.savedAdHoc.FormattingEnabled = true;
            this.savedAdHoc.Location = new System.Drawing.Point(0, 0);
            this.savedAdHoc.Name = "savedAdHoc";
            this.savedAdHoc.Size = new System.Drawing.Size(270, 453);
            this.savedAdHoc.TabIndex = 6;
            this.savedAdHoc.SelectedIndexChanged += new System.EventHandler(this.savedAdHoc_SelectedIndexChanged);
            // 
            // toolStrip4
            // 
            this.toolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFile,
            this.newFile});
            this.toolStrip4.Location = new System.Drawing.Point(0, 27);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(270, 31);
            this.toolStrip4.TabIndex = 8;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // deleteFile
            // 
            this.deleteFile.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deleteFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteFile.Image = global::STEM.Surge.ControlPanel.Properties.Resources.garbage;
            this.deleteFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteFile.Name = "deleteFile";
            this.deleteFile.Size = new System.Drawing.Size(28, 28);
            this.deleteFile.Text = "Delete InstructionSet";
            this.deleteFile.Click += new System.EventHandler(this.deleteFile_Click);
            // 
            // newFile
            // 
            this.newFile.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.newFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newFile.Image = global::STEM.Surge.ControlPanel.Properties.Resources.add;
            this.newFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newFile.Name = "newFile";
            this.newFile.Size = new System.Drawing.Size(28, 28);
            this.newFile.Text = "New InstructionSet";
            this.newFile.Click += new System.EventHandler(this.newFile_Click);
            // 
            // toolStrip5
            // 
            this.toolStrip5.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip5.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterBox});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(270, 27);
            this.toolStrip5.TabIndex = 7;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.clearFilter.Size = new System.Drawing.Size(32, 24);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // filterBox
            // 
            this.filterBox.AutoSize = false;
            this.filterBox.BackColor = System.Drawing.Color.Lavender;
            this.filterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(211, 20);
            this.filterBox.ToolTipText = "Filter Rows";
            // 
            // instructionSetEditPanel
            // 
            this.instructionSetEditPanel.Controls.Add(this.panel6);
            this.instructionSetEditPanel.Controls.Add(this.toolStrip2);
            this.instructionSetEditPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionSetEditPanel.Location = new System.Drawing.Point(0, 0);
            this.instructionSetEditPanel.Name = "instructionSetEditPanel";
            this.instructionSetEditPanel.Size = new System.Drawing.Size(642, 511);
            this.instructionSetEditPanel.TabIndex = 9;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.splitContainer1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 31);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(642, 480);
            this.panel6.TabIndex = 12;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save,
            this.toolStripLabel1,
            this.processName});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(642, 31);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // save
            // 
            this.save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.save.Enabled = false;
            this.save.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.save.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(28, 28);
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(82, 28);
            this.toolStripLabel1.Text = "Process Name";
            // 
            // processName
            // 
            this.processName.BackColor = System.Drawing.Color.Lavender;
            this.processName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.processName.Name = "processName";
            this.processName.Size = new System.Drawing.Size(440, 31);
            this.processName.ToolTipText = "What name would you give the process that these instructions make up?";
            // 
            // AdHoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 511);
            this.Controls.Add(this.splitContainer2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AdHoc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AdHoc";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.instructionSetEditPanel.ResumeLayout(false);
            this.instructionSetEditPanel.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox instructionList;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton deleteInstruction;
        private System.Windows.Forms.ToolStripButton addInstruction;
        private System.Windows.Forms.ToolStripButton moveUp;
        private System.Windows.Forms.ToolStripButton moveDown;
        private System.Windows.Forms.PropertyGrid instructionProperties;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckedListBox branchList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton deploy;
        private System.Windows.Forms.ToolStripButton selectAll;
        private System.Windows.Forms.ToolStripButton deselectAll;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ListBox savedAdHoc;
        private System.Windows.Forms.Panel instructionSetEditPanel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton deleteFile;
        private System.Windows.Forms.ToolStripButton newFile;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton save;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox processName;
    }
}
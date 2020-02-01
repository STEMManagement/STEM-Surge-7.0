namespace STEM.Surge.ControlPanel
{
    partial class InstructionSetListEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fileList = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.deleteFile = new System.Windows.Forms.ToolStripButton();
            this.newFile = new System.Windows.Forms.ToolStripButton();
            this.archiveUnusedTemplates = new System.Windows.Forms.ToolStripButton();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterBox = new System.Windows.Forms.ToolStripTextBox();
            this.instructionSetEditor1 = new STEM.Surge.ControlPanel.InstructionSetEditor();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileList);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.instructionSetEditor1);
            this.splitContainer1.Size = new System.Drawing.Size(1329, 750);
            this.splitContainer1.SplitterDistance = 381;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // fileList
            // 
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FormattingEnabled = true;
            this.fileList.ItemHeight = 16;
            this.fileList.Location = new System.Drawing.Point(0, 58);
            this.fileList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(381, 692);
            this.fileList.Sorted = true;
            this.fileList.TabIndex = 4;
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFile,
            this.newFile,
            this.archiveUnusedTemplates});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(381, 31);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
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
            // archiveUnusedTemplates
            // 
            this.archiveUnusedTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.archiveUnusedTemplates.Image = global::STEM.Surge.ControlPanel.Properties.Resources.archive;
            this.archiveUnusedTemplates.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.archiveUnusedTemplates.Name = "archiveUnusedTemplates";
            this.archiveUnusedTemplates.Size = new System.Drawing.Size(28, 28);
            this.archiveUnusedTemplates.Text = "Archive Unused Templates";
            this.archiveUnusedTemplates.Click += new System.EventHandler(this.archiveUnusedTemplates_Click);
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
            this.toolStrip5.Size = new System.Drawing.Size(381, 27);
            this.toolStrip5.TabIndex = 2;
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
            this.filterBox.Size = new System.Drawing.Size(280, 23);
            this.filterBox.ToolTipText = "Filter Rows";
            this.filterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            // 
            // instructionSetEditor1
            // 
            this.instructionSetEditor1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.instructionSetEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionSetEditor1.Location = new System.Drawing.Point(0, 0);
            this.instructionSetEditor1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.instructionSetEditor1.Name = "instructionSetEditor1";
            this.instructionSetEditor1.Size = new System.Drawing.Size(943, 750);
            this.instructionSetEditor1.TabIndex = 0;
            // 
            // InstructionSetListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "InstructionSetListEditor";
            this.Size = new System.Drawing.Size(1329, 750);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton deleteFile;
        private System.Windows.Forms.ToolStripButton newFile;
        private System.Windows.Forms.ListBox fileList;
        private InstructionSetEditor instructionSetEditor1;
        private System.Windows.Forms.ToolStripButton archiveUnusedTemplates;
    }
}

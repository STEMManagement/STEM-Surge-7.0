namespace STEM.Surge.ControlPanel
{
    partial class ControllerListEditor
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
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.controllerEditor1 = new STEM.Surge.ControlPanel.ControllerEditor();
            this.filterBox = new System.Windows.Forms.ToolStripTextBox();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.archiveUnusedControllers = new System.Windows.Forms.ToolStripButton();
            this.newFile = new System.Windows.Forms.ToolStripButton();
            this.deleteFile = new System.Windows.Forms.ToolStripButton();
            this.fileList = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.detailsPanel.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // detailsPanel
            // 
            this.detailsPanel.Controls.Add(this.controllerEditor1);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(873, 737);
            this.detailsPanel.TabIndex = 14;
            // 
            // controllerEditor1
            // 
            this.controllerEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controllerEditor1.Location = new System.Drawing.Point(0, 0);
            this.controllerEditor1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.controllerEditor1.Name = "controllerEditor1";
            this.controllerEditor1.Size = new System.Drawing.Size(873, 737);
            this.controllerEditor1.TabIndex = 0;
            // 
            // filterBox
            // 
            this.filterBox.AutoSize = false;
            this.filterBox.BackColor = System.Drawing.Color.Lavender;
            this.filterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(265, 23);
            this.filterBox.ToolTipText = "Filter Rows";
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
            // toolStrip5
            // 
            this.toolStrip5.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip5.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterBox});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(434, 27);
            this.toolStrip5.TabIndex = 4;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // archiveUnusedControllers
            // 
            this.archiveUnusedControllers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.archiveUnusedControllers.Image = global::STEM.Surge.ControlPanel.Properties.Resources.archive;
            this.archiveUnusedControllers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.archiveUnusedControllers.Name = "archiveUnusedControllers";
            this.archiveUnusedControllers.Size = new System.Drawing.Size(28, 28);
            this.archiveUnusedControllers.Text = "Archive Unused Controllers";
            this.archiveUnusedControllers.Click += new System.EventHandler(this.archiveUnusedControllers_Click);
            // 
            // newFile
            // 
            this.newFile.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.newFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newFile.Image = global::STEM.Surge.ControlPanel.Properties.Resources.add;
            this.newFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newFile.Name = "newFile";
            this.newFile.Size = new System.Drawing.Size(28, 28);
            this.newFile.Text = "New Controller";
            this.newFile.Click += new System.EventHandler(this.newFile_Click);
            // 
            // deleteFile
            // 
            this.deleteFile.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deleteFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteFile.Image = global::STEM.Surge.ControlPanel.Properties.Resources.garbage;
            this.deleteFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteFile.Name = "deleteFile";
            this.deleteFile.Size = new System.Drawing.Size(28, 28);
            this.deleteFile.Text = "Delete Controller";
            this.deleteFile.Click += new System.EventHandler(this.deleteFile_Click);
            // 
            // fileList
            // 
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FormattingEnabled = true;
            this.fileList.ItemHeight = 16;
            this.fileList.Location = new System.Drawing.Point(0, 58);
            this.fileList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(434, 679);
            this.fileList.Sorted = true;
            this.fileList.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFile,
            this.newFile,
            this.archiveUnusedControllers});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(434, 31);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
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
            this.splitContainer1.Panel2.Controls.Add(this.detailsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1312, 737);
            this.splitContainer1.SplitterDistance = 434;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // ControllerListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ControllerListEditor";
            this.Size = new System.Drawing.Size(1312, 737);
            this.detailsPanel.ResumeLayout(false);
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel detailsPanel;
        private System.Windows.Forms.ToolStripTextBox filterBox;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton archiveUnusedControllers;
        private System.Windows.Forms.ToolStripButton newFile;
        private System.Windows.Forms.ToolStripButton deleteFile;
        private System.Windows.Forms.ListBox fileList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ControllerEditor controllerEditor1;

    }
}

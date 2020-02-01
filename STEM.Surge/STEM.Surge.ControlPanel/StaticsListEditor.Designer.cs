namespace STEM.Surge.ControlPanel
{
    partial class StaticsListEditor
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
            this.instructionSetListEditor1 = new STEM.Surge.ControlPanel.InstructionSetListEditor(true);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.fileList);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.instructionSetListEditor1);
            this.splitContainer1.Size = new System.Drawing.Size(1040, 586);
            this.splitContainer1.SplitterDistance = 146;
            this.splitContainer1.TabIndex = 0;
            // 
            // fileList
            // 
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FormattingEnabled = true;
            this.fileList.Location = new System.Drawing.Point(0, 31);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(146, 555);
            this.fileList.Sorted = true;
            this.fileList.TabIndex = 6;
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFile,
            this.newFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(146, 31);
            this.toolStrip1.TabIndex = 5;
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
            this.deleteFile.Text = "Delete Group";
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
            this.newFile.Text = "New Group";
            this.newFile.Click += new System.EventHandler(this.newFile_Click);
            // 
            // instructionSetListEditor1
            // 
            this.instructionSetListEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructionSetListEditor1.Location = new System.Drawing.Point(0, 0);
            this.instructionSetListEditor1.Name = "instructionSetListEditor1";
            this.instructionSetListEditor1.Size = new System.Drawing.Size(890, 586);
            this.instructionSetListEditor1.TabIndex = 0;
            // 
            // StaticsListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "StaticsListEditor";
            this.Size = new System.Drawing.Size(1040, 586);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private InstructionSetListEditor instructionSetListEditor1;
        private System.Windows.Forms.ListBox fileList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton deleteFile;
        private System.Windows.Forms.ToolStripButton newFile;
    }
}

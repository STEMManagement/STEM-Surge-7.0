namespace STEM.Surge.ControlPanel
{
    partial class VersionsManagement
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.unusedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.deselectAll = new System.Windows.Forms.ToolStripButton();
            this.selectAll = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.moveToArchive = new System.Windows.Forms.ToolStripButton();
            this.evaluate = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.countLabel = new System.Windows.Forms.ToolStripLabel();
            this.deleteExtensionsFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.extensionsFolders = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(774, 515);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.toolStrip2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(774, 480);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.unusedListBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(774, 455);
            this.panel3.TabIndex = 2;
            // 
            // unusedListBox1
            // 
            this.unusedListBox1.CheckOnClick = true;
            this.unusedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unusedListBox1.FormattingEnabled = true;
            this.unusedListBox1.Location = new System.Drawing.Point(0, 0);
            this.unusedListBox1.Name = "unusedListBox1";
            this.unusedListBox1.Size = new System.Drawing.Size(774, 455);
            this.unusedListBox1.Sorted = true;
            this.unusedListBox1.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deselectAll,
            this.selectAll});
            this.toolStrip2.Location = new System.Drawing.Point(0, 455);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(774, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // deselectAll
            // 
            this.deselectAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.deselectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deselectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_uncheck;
            this.deselectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deselectAll.Name = "deselectAll";
            this.deselectAll.Size = new System.Drawing.Size(23, 22);
            this.deselectAll.Text = "toolStripButton2";
            this.deselectAll.ToolTipText = "Deselect All";
            this.deselectAll.Click += new System.EventHandler(this.deselectAll_Click);
            // 
            // selectAll
            // 
            this.selectAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_check;
            this.selectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(23, 22);
            this.selectAll.Text = "toolStripButton1";
            this.selectAll.ToolTipText = "Select All";
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToArchive,
            this.evaluate,
            this.toolStripLabel1,
            this.countLabel,
            this.toolStripLabel2,
            this.toolStripSeparator1,
            this.deleteExtensionsFolder,
            this.extensionsFolders,
            this.toolStripLabel3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(774, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // moveToArchive
            // 
            this.moveToArchive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveToArchive.Enabled = false;
            this.moveToArchive.Image = global::STEM.Surge.ControlPanel.Properties.Resources.archive;
            this.moveToArchive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveToArchive.Name = "moveToArchive";
            this.moveToArchive.Size = new System.Drawing.Size(28, 32);
            this.moveToArchive.Text = "Archive Selected Assemblies";
            this.moveToArchive.Click += new System.EventHandler(this.moveToArchive_Click);
            // 
            // evaluate
            // 
            this.evaluate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.evaluate.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
            this.evaluate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.evaluate.Name = "evaluate";
            this.evaluate.Size = new System.Drawing.Size(28, 32);
            this.evaluate.Text = "Execute Evaluation";
            this.evaluate.Click += new System.EventHandler(this.evaluate_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(135, 32);
            this.toolStripLabel1.Text = "Find Unused Assemblies";
            // 
            // countLabel
            // 
            this.countLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(0, 32);
            // 
            // deleteExtensionsFolder
            // 
            this.deleteExtensionsFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteExtensionsFolder.Image = global::STEM.Surge.ControlPanel.Properties.Resources.garbage;
            this.deleteExtensionsFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteExtensionsFolder.Name = "deleteExtensionsFolder";
            this.deleteExtensionsFolder.Size = new System.Drawing.Size(28, 32);
            this.deleteExtensionsFolder.Text = "Delete Extensions Folder";
            this.deleteExtensionsFolder.Click += new System.EventHandler(this.deleteExtensionsFolder_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.AutoSize = false;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(30, 32);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // extensionsFolders
            // 
            this.extensionsFolders.Name = "extensionsFolders";
            this.extensionsFolders.Size = new System.Drawing.Size(250, 35);
            this.extensionsFolders.ToolTipText = "Extensions folder to delete...";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(76, 32);
            this.toolStripLabel3.Text = "Delete Folder";
            // 
            // VersionsManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "VersionsManagement";
            this.Size = new System.Drawing.Size(774, 515);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton evaluate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton moveToArchive;
        private System.Windows.Forms.CheckedListBox unusedListBox1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton deselectAll;
        private System.Windows.Forms.ToolStripButton selectAll;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStripLabel countLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton deleteExtensionsFolder;
        private System.Windows.Forms.ToolStripComboBox extensionsFolders;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
    }
}

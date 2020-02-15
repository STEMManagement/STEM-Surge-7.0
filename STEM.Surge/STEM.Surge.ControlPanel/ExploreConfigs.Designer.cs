namespace STEM.Surge.ControlPanel
{
    partial class ExploreConfigs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExploreConfigs));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.notes = new System.Windows.Forms.TabPage();
            this.messageText = new System.Windows.Forms.RichTextBox();
            this.searchResults = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.configsFound = new System.Windows.Forms.DataGridView();
            this.ApplyEdit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.configFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.xmlView = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.xmlText = new System.Windows.Forms.RichTextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.saveXml = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearFind = new System.Windows.Forms.ToolStripButton();
            this.findText = new System.Windows.Forms.ToolStripTextBox();
            this.nextUp = new System.Windows.Forms.ToolStripButton();
            this.nextDown = new System.Windows.Forms.ToolStripButton();
            this.editorControl = new System.Windows.Forms.TabPage();
            this.editorControlPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearFilter = new System.Windows.Forms.ToolStripButton();
            this.filterBox = new System.Windows.Forms.ToolStripTextBox();
            this.applyFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.findForReplace = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.replaceFromFind = new System.Windows.Forms.ToolStripTextBox();
            this.replaceText = new System.Windows.Forms.ToolStripButton();
            this.deselectAll = new System.Windows.Forms.ToolStripButton();
            this.selectAll = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.notes.SuspendLayout();
            this.searchResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configsFound)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.xmlView.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.editorControl.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1284, 631);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.notes);
            this.tabControl1.Controls.Add(this.searchResults);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 35);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1284, 596);
            this.tabControl1.TabIndex = 2;
            // 
            // notes
            // 
            this.notes.Controls.Add(this.messageText);
            this.notes.Location = new System.Drawing.Point(4, 22);
            this.notes.Name = "notes";
            this.notes.Padding = new System.Windows.Forms.Padding(3);
            this.notes.Size = new System.Drawing.Size(1276, 570);
            this.notes.TabIndex = 0;
            this.notes.Text = "Messages";
            this.notes.UseVisualStyleBackColor = true;
            // 
            // messageText
            // 
            this.messageText.BackColor = System.Drawing.Color.White;
            this.messageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageText.Location = new System.Drawing.Point(3, 3);
            this.messageText.Name = "messageText";
            this.messageText.ReadOnly = true;
            this.messageText.Size = new System.Drawing.Size(1270, 564);
            this.messageText.TabIndex = 0;
            this.messageText.Text = "";
            // 
            // searchResults
            // 
            this.searchResults.Controls.Add(this.splitContainer2);
            this.searchResults.Location = new System.Drawing.Point(4, 22);
            this.searchResults.Name = "searchResults";
            this.searchResults.Padding = new System.Windows.Forms.Padding(3);
            this.searchResults.Size = new System.Drawing.Size(1276, 570);
            this.searchResults.TabIndex = 1;
            this.searchResults.Text = "Search Results";
            this.searchResults.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel5);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Size = new System.Drawing.Size(1270, 564);
            this.splitContainer2.SplitterDistance = 423;
            this.splitContainer2.SplitterWidth = 8;
            this.splitContainer2.TabIndex = 3;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.configsFound);
            this.panel5.Controls.Add(this.toolStrip3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(423, 564);
            this.panel5.TabIndex = 2;
            // 
            // toolStrip3
            // 
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deselectAll,
            this.selectAll});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(423, 25);
            this.toolStrip3.TabIndex = 0;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // configsFound
            // 
            this.configsFound.AllowUserToAddRows = false;
            this.configsFound.AllowUserToDeleteRows = false;
            this.configsFound.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configsFound.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ApplyEdit,
            this.configFile});
            this.configsFound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configsFound.Location = new System.Drawing.Point(0, 25);
            this.configsFound.Name = "configsFound";
            this.configsFound.Size = new System.Drawing.Size(423, 539);
            this.configsFound.TabIndex = 0;
            // 
            // ApplyEdit
            // 
            this.ApplyEdit.HeaderText = "Apply Edit";
            this.ApplyEdit.Name = "ApplyEdit";
            this.ApplyEdit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ApplyEdit.Width = 50;
            // 
            // configFile
            // 
            this.configFile.HeaderText = "Config File";
            this.configFile.MinimumWidth = 300;
            this.configFile.Name = "configFile";
            this.configFile.ReadOnly = true;
            this.configFile.Width = 300;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(839, 564);
            this.panel2.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.xmlView);
            this.tabControl2.Controls.Add(this.editorControl);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(839, 564);
            this.tabControl2.TabIndex = 2;
            // 
            // xmlView
            // 
            this.xmlView.Controls.Add(this.panel3);
            this.xmlView.Controls.Add(this.toolStrip2);
            this.xmlView.Location = new System.Drawing.Point(4, 22);
            this.xmlView.Name = "xmlView";
            this.xmlView.Padding = new System.Windows.Forms.Padding(3);
            this.xmlView.Size = new System.Drawing.Size(831, 538);
            this.xmlView.TabIndex = 0;
            this.xmlView.Text = "XML";
            this.xmlView.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.xmlText);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 38);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(825, 497);
            this.panel3.TabIndex = 4;
            // 
            // xmlText
            // 
            this.xmlText.DetectUrls = false;
            this.xmlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlText.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlText.Location = new System.Drawing.Point(0, 0);
            this.xmlText.Name = "xmlText";
            this.xmlText.Size = new System.Drawing.Size(825, 497);
            this.xmlText.TabIndex = 2;
            this.xmlText.Text = "";
            this.xmlText.WordWrap = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveXml,
            this.toolStripLabel3,
            this.toolStripSeparator2,
            this.clearFind,
            this.findText,
            this.nextUp,
            this.nextDown});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(825, 35);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // saveXml
            // 
            this.saveXml.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveXml.Image = global::STEM.Surge.ControlPanel.Properties.Resources.save;
            this.saveXml.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveXml.Name = "saveXml";
            this.saveXml.Size = new System.Drawing.Size(23, 32);
            this.saveXml.Text = "Save";
            this.saveXml.Click += new System.EventHandler(this.saveXml_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.AutoSize = false;
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(30, 32);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // clearFind
            // 
            this.clearFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFind.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFind.Name = "clearFind";
            this.clearFind.Size = new System.Drawing.Size(23, 32);
            this.clearFind.Text = "Clear Find";
            this.clearFind.Click += new System.EventHandler(this.clearFind_Click);
            // 
            // findText
            // 
            this.findText.AutoSize = false;
            this.findText.BackColor = System.Drawing.Color.Lavender;
            this.findText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.findText.Name = "findText";
            this.findText.Size = new System.Drawing.Size(400, 35);
            this.findText.ToolTipText = "Find Text";
            // 
            // nextUp
            // 
            this.nextUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextUp.Image = global::STEM.Surge.ControlPanel.Properties.Resources.up;
            this.nextUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextUp.Name = "nextUp";
            this.nextUp.Size = new System.Drawing.Size(23, 32);
            this.nextUp.Text = "Previous";
            this.nextUp.Click += new System.EventHandler(this.nextUp_Click);
            // 
            // nextDown
            // 
            this.nextDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextDown.Image = global::STEM.Surge.ControlPanel.Properties.Resources.down;
            this.nextDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextDown.Name = "nextDown";
            this.nextDown.Size = new System.Drawing.Size(23, 32);
            this.nextDown.Text = "Next";
            this.nextDown.Click += new System.EventHandler(this.nextDown_Click);
            // 
            // editorControl
            // 
            this.editorControl.Controls.Add(this.editorControlPanel);
            this.editorControl.Location = new System.Drawing.Point(4, 22);
            this.editorControl.Name = "editorControl";
            this.editorControl.Padding = new System.Windows.Forms.Padding(3);
            this.editorControl.Size = new System.Drawing.Size(831, 538);
            this.editorControl.TabIndex = 1;
            this.editorControl.Text = "Editor";
            this.editorControl.UseVisualStyleBackColor = true;
            // 
            // editorControlPanel
            // 
            this.editorControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorControlPanel.Location = new System.Drawing.Point(3, 3);
            this.editorControlPanel.Name = "editorControlPanel";
            this.editorControlPanel.Size = new System.Drawing.Size(825, 532);
            this.editorControlPanel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilter,
            this.filterBox,
            this.applyFilter,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.findForReplace,
            this.toolStripLabel1,
            this.replaceFromFind,
            this.replaceText});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1284, 35);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // clearFilter
            // 
            this.clearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilter.Name = "clearFilter";
            this.clearFilter.Size = new System.Drawing.Size(23, 32);
            this.clearFilter.Text = "Clear Filter";
            this.clearFilter.Click += new System.EventHandler(this.clearFilter_Click);
            // 
            // filterBox
            // 
            this.filterBox.AutoSize = false;
            this.filterBox.BackColor = System.Drawing.Color.Lavender;
            this.filterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(400, 35);
            this.filterBox.ToolTipText = "Filter Configs (prepend with \'!\' to apply a \'not in\' filter)";
            // 
            // applyFilter
            // 
            this.applyFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.applyFilter.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
            this.applyFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.applyFilter.Name = "applyFilter";
            this.applyFilter.Size = new System.Drawing.Size(23, 32);
            this.applyFilter.Text = "Apply Filter";
            this.applyFilter.Click += new System.EventHandler(this.applyFilter_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(70, 32);
            this.toolStripLabel2.Text = "Replace this";
            // 
            // findForReplace
            // 
            this.findForReplace.BackColor = System.Drawing.SystemColors.Info;
            this.findForReplace.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.findForReplace.Name = "findForReplace";
            this.findForReplace.Size = new System.Drawing.Size(200, 35);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(63, 32);
            this.toolStripLabel1.Text = "   With this";
            // 
            // replaceFromFind
            // 
            this.replaceFromFind.BackColor = System.Drawing.SystemColors.Info;
            this.replaceFromFind.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.replaceFromFind.Name = "replaceFromFind";
            this.replaceFromFind.Size = new System.Drawing.Size(200, 35);
            // 
            // replaceText
            // 
            this.replaceText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replaceText.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
            this.replaceText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceText.Name = "replaceText";
            this.replaceText.Size = new System.Drawing.Size(23, 32);
            this.replaceText.Text = "Replace Text";
            this.replaceText.Click += new System.EventHandler(this.replaceText_Click);
            // 
            // deselectAll
            // 
            this.deselectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deselectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_uncheck;
            this.deselectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deselectAll.Name = "deselectAll";
            this.deselectAll.Size = new System.Drawing.Size(23, 22);
            this.deselectAll.Text = "Deselect All";
            this.deselectAll.Click += new System.EventHandler(this.deselectAll_Click);
            // 
            // selectAll
            // 
            this.selectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectAll.Image = global::STEM.Surge.ControlPanel.Properties.Resources.boxes_check;
            this.selectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(23, 22);
            this.selectAll.Text = "Select All";
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // ExploreConfigs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 631);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExploreConfigs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Explore Configuration";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.notes.ResumeLayout(false);
            this.searchResults.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configsFound)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.xmlView.ResumeLayout(false);
            this.xmlView.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.editorControl.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilter;
        private System.Windows.Forms.ToolStripTextBox filterBox;
        private System.Windows.Forms.ToolStripButton applyFilter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage notes;
        private System.Windows.Forms.TabPage searchResults;
        private System.Windows.Forms.RichTextBox messageText;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage xmlView;
        private System.Windows.Forms.RichTextBox xmlText;
        private System.Windows.Forms.TabPage editorControl;
        private System.Windows.Forms.Panel editorControlPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton saveXml;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox findForReplace;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox replaceFromFind;
        private System.Windows.Forms.ToolStripButton replaceText;
        private System.Windows.Forms.DataGridView configsFound;
        private System.Windows.Forms.ToolStripButton clearFind;
        private System.Windows.Forms.ToolStripTextBox findText;
        private System.Windows.Forms.ToolStripButton nextUp;
        private System.Windows.Forms.ToolStripButton nextDown;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ApplyEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn configFile;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton deselectAll;
        private System.Windows.Forms.ToolStripButton selectAll;
    }
}
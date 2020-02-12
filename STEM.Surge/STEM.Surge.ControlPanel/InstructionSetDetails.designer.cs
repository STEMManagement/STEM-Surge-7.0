namespace STEM.Surge.ControlPanel
{
    partial class InstructionSetDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstructionSetDetails));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.summaryText = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.xmlText = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearFind = new System.Windows.Forms.ToolStripButton();
            this.findText = new System.Windows.Forms.ToolStripTextBox();
            this.nextUp = new System.Windows.Forms.ToolStripButton();
            this.nextDown = new System.Windows.Forms.ToolStripButton();
            this.editInstructionSetTemplate = new System.Windows.Forms.ToolStripButton();
            this.editController = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.summaryText);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1528, 582);
            this.splitContainer1.SplitterDistance = 376;
            this.splitContainer1.TabIndex = 0;
            // 
            // summaryText
            // 
            this.summaryText.DetectUrls = false;
            this.summaryText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryText.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.summaryText.Location = new System.Drawing.Point(0, 0);
            this.summaryText.Name = "summaryText";
            this.summaryText.Size = new System.Drawing.Size(376, 582);
            this.summaryText.TabIndex = 0;
            this.summaryText.Text = "";
            this.summaryText.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1148, 582);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.xmlText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1148, 547);
            this.panel2.TabIndex = 4;
            // 
            // xmlText
            // 
            this.xmlText.DetectUrls = false;
            this.xmlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlText.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlText.Location = new System.Drawing.Point(0, 0);
            this.xmlText.Name = "xmlText";
            this.xmlText.Size = new System.Drawing.Size(1148, 547);
            this.xmlText.TabIndex = 0;
            this.xmlText.Text = "";
            this.xmlText.WordWrap = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFind,
            this.findText,
            this.nextUp,
            this.nextDown,
            this.editController,
            this.editInstructionSetTemplate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1148, 35);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
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
            // editInstructionSetTemplate
            // 
            this.editInstructionSetTemplate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.editInstructionSetTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editInstructionSetTemplate.Image = global::STEM.Surge.ControlPanel.Properties.Resources.edit_2;
            this.editInstructionSetTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editInstructionSetTemplate.Name = "editInstructionSetTemplate";
            this.editInstructionSetTemplate.Size = new System.Drawing.Size(23, 32);
            this.editInstructionSetTemplate.Text = "Edit InstructionSet Template";
            this.editInstructionSetTemplate.Click += new System.EventHandler(this.editInstructionSetTemplate_Click);
            // 
            // editController
            // 
            this.editController.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.editController.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editController.Image = global::STEM.Surge.ControlPanel.Properties.Resources.depolyment_controller;
            this.editController.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editController.Name = "editController";
            this.editController.Size = new System.Drawing.Size(23, 32);
            this.editController.Text = "Edit Deployment Controller";
            this.editController.Click += new System.EventHandler(this.editController_Click);
            // 
            // InstructionSetDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1528, 582);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InstructionSetDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InstructionSet Details";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox summaryText;
        private System.Windows.Forms.RichTextBox xmlText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFind;
        private System.Windows.Forms.ToolStripTextBox findText;
        private System.Windows.Forms.ToolStripButton nextUp;
        private System.Windows.Forms.ToolStripButton nextDown;
        private System.Windows.Forms.ToolStripButton editInstructionSetTemplate;
        private System.Windows.Forms.ToolStripButton editController;
    }
}
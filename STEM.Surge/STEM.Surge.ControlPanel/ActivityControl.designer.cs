namespace STEM.Surge.ControlPanel
{
    partial class ActivityControl
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
            this.activityPanelSplitContainer = new System.Windows.Forms.SplitContainer();
            this.switchboardCountsPanel = new System.Windows.Forms.Panel();
            this.assignmentsDetailsPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.activityPanelSplitContainer)).BeginInit();
            this.activityPanelSplitContainer.Panel1.SuspendLayout();
            this.activityPanelSplitContainer.Panel2.SuspendLayout();
            this.activityPanelSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // activityPanelSplitContainer
            // 
            this.activityPanelSplitContainer.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.activityPanelSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.activityPanelSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activityPanelSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.activityPanelSplitContainer.Name = "activityPanelSplitContainer";
            this.activityPanelSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // activityPanelSplitContainer.Panel1
            // 
            this.activityPanelSplitContainer.Panel1.Controls.Add(this.switchboardCountsPanel);
            // 
            // activityPanelSplitContainer.Panel2
            // 
            this.activityPanelSplitContainer.Panel2.Controls.Add(this.assignmentsDetailsPanel);
            this.activityPanelSplitContainer.Size = new System.Drawing.Size(1174, 459);
            this.activityPanelSplitContainer.SplitterDistance = 253;
            this.activityPanelSplitContainer.SplitterWidth = 8;
            this.activityPanelSplitContainer.TabIndex = 0;
            // 
            // switchboardCountsPanel
            // 
            this.switchboardCountsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.switchboardCountsPanel.Location = new System.Drawing.Point(0, 0);
            this.switchboardCountsPanel.Name = "switchboardCountsPanel";
            this.switchboardCountsPanel.Size = new System.Drawing.Size(1170, 249);
            this.switchboardCountsPanel.TabIndex = 0;
            // 
            // assignmentsDetailsPanel
            // 
            this.assignmentsDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assignmentsDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this.assignmentsDetailsPanel.Name = "assignmentsDetailsPanel";
            this.assignmentsDetailsPanel.Size = new System.Drawing.Size(1170, 194);
            this.assignmentsDetailsPanel.TabIndex = 0;
            // 
            // ActivityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.activityPanelSplitContainer);
            this.Name = "ActivityControl";
            this.Size = new System.Drawing.Size(1174, 459);
            this.activityPanelSplitContainer.Panel1.ResumeLayout(false);
            this.activityPanelSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.activityPanelSplitContainer)).EndInit();
            this.activityPanelSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer activityPanelSplitContainer;
        private System.Windows.Forms.Panel switchboardCountsPanel;
        private System.Windows.Forms.Panel assignmentsDetailsPanel;
    }
}

namespace STEM.Surge.ControlPanel
{
    partial class ColumnFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnFilter));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.managerIP = new System.Windows.Forms.TextBox();
            this.branchIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deploymentController = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.source = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.clearManagerIP = new System.Windows.Forms.Button();
            this.clearBranchIP = new System.Windows.Forms.Button();
            this.clearController = new System.Windows.Forms.Button();
            this.clearSource = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(350, 112);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(269, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ManagerIP like";
            // 
            // managerIP
            // 
            this.managerIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.managerIP.Location = new System.Drawing.Point(161, 6);
            this.managerIP.Name = "managerIP";
            this.managerIP.Size = new System.Drawing.Size(238, 20);
            this.managerIP.TabIndex = 0;
            // 
            // branchIP
            // 
            this.branchIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.branchIP.Location = new System.Drawing.Point(161, 32);
            this.branchIP.Name = "branchIP";
            this.branchIP.Size = new System.Drawing.Size(238, 20);
            this.branchIP.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "BranchIP like";
            // 
            // deploymentController
            // 
            this.deploymentController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deploymentController.Location = new System.Drawing.Point(161, 58);
            this.deploymentController.Name = "deploymentController";
            this.deploymentController.Size = new System.Drawing.Size(238, 20);
            this.deploymentController.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Deployment Controller like";
            // 
            // source
            // 
            this.source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.source.Location = new System.Drawing.Point(161, 84);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(238, 20);
            this.source.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Source like";
            // 
            // clearManagerIP
            // 
            this.clearManagerIP.BackgroundImage = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearManagerIP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearManagerIP.Location = new System.Drawing.Point(400, 4);
            this.clearManagerIP.Name = "clearManagerIP";
            this.clearManagerIP.Size = new System.Drawing.Size(25, 23);
            this.clearManagerIP.TabIndex = 0;
            this.clearManagerIP.UseVisualStyleBackColor = true;
            this.clearManagerIP.Click += new System.EventHandler(this.clearManagerIP_Click);
            // 
            // clearBranchIP
            // 
            this.clearBranchIP.BackgroundImage = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearBranchIP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearBranchIP.Location = new System.Drawing.Point(400, 30);
            this.clearBranchIP.Name = "clearBranchIP";
            this.clearBranchIP.Size = new System.Drawing.Size(25, 23);
            this.clearBranchIP.TabIndex = 0;
            this.clearBranchIP.UseVisualStyleBackColor = true;
            this.clearBranchIP.Click += new System.EventHandler(this.clearBranchIP_Click);
            // 
            // clearController
            // 
            this.clearController.BackgroundImage = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearController.Location = new System.Drawing.Point(400, 56);
            this.clearController.Name = "clearController";
            this.clearController.Size = new System.Drawing.Size(25, 23);
            this.clearController.TabIndex = 0;
            this.clearController.UseVisualStyleBackColor = true;
            this.clearController.Click += new System.EventHandler(this.clearController_Click);
            // 
            // clearSource
            // 
            this.clearSource.BackgroundImage = global::STEM.Surge.ControlPanel.Properties.Resources.erase;
            this.clearSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearSource.Location = new System.Drawing.Point(400, 82);
            this.clearSource.Name = "clearSource";
            this.clearSource.Size = new System.Drawing.Size(25, 23);
            this.clearSource.TabIndex = 0;
            this.clearSource.UseVisualStyleBackColor = true;
            this.clearSource.Click += new System.EventHandler(this.clearSource_Click);
            // 
            // ColumnFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(437, 138);
            this.Controls.Add(this.clearSource);
            this.Controls.Add(this.clearController);
            this.Controls.Add(this.clearBranchIP);
            this.Controls.Add(this.clearManagerIP);
            this.Controls.Add(this.source);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.deploymentController);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.branchIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.managerIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ColumnFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Column Filters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox managerIP;
        private System.Windows.Forms.TextBox branchIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox deploymentController;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox source;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button clearManagerIP;
        private System.Windows.Forms.Button clearBranchIP;
        private System.Windows.Forms.Button clearController;
        private System.Windows.Forms.Button clearSource;
    }
}
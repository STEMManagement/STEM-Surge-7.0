namespace STEM.Surge.ControlPanel
{
    partial class ServiceConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceConfiguration));
            this.remoteConfigurationDir = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.postmortemOutputDir = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.processorOverload = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.envManagers = new System.Windows.Forms.TextBox();
            this.envPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.useSSL = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // remoteConfigurationDir
            // 
            this.remoteConfigurationDir.Location = new System.Drawing.Point(253, 141);
            this.remoteConfigurationDir.Name = "remoteConfigurationDir";
            this.remoteConfigurationDir.Size = new System.Drawing.Size(345, 20);
            this.remoteConfigurationDir.TabIndex = 5;
            this.remoteConfigurationDir.Text = "\\\\[MANAGERS]\\STEM.Workforce\\Configuration";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(221, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Surge Remote Configuration Directory";
            // 
            // postmortemOutputDir
            // 
            this.postmortemOutputDir.Location = new System.Drawing.Point(253, 108);
            this.postmortemOutputDir.Name = "postmortemOutputDir";
            this.postmortemOutputDir.Size = new System.Drawing.Size(345, 20);
            this.postmortemOutputDir.TabIndex = 4;
            this.postmortemOutputDir.Text = "D:\\STEM.Workforce\\PostMortem";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "PostMortem Output Directory";
            // 
            // processorOverload
            // 
            this.processorOverload.Location = new System.Drawing.Point(253, 75);
            this.processorOverload.Name = "processorOverload";
            this.processorOverload.Size = new System.Drawing.Size(79, 20);
            this.processorOverload.TabIndex = 3;
            this.processorOverload.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Processor Overload Factor";
            // 
            // envManagers
            // 
            this.envManagers.Location = new System.Drawing.Point(253, 42);
            this.envManagers.Name = "envManagers";
            this.envManagers.Size = new System.Drawing.Size(264, 20);
            this.envManagers.TabIndex = 1;
            this.envManagers.Text = "172.19.12.50#172.19.24.50";
            // 
            // envPort
            // 
            this.envPort.Location = new System.Drawing.Point(253, 9);
            this.envPort.Name = "envPort";
            this.envPort.Size = new System.Drawing.Size(79, 20);
            this.envPort.TabIndex = 0;
            this.envPort.Text = "7847";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Surge Deployment Manager Addresses";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(234, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Surge Environment Communication Port ";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(523, 176);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(442, 176);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // useSSL
            // 
            this.useSSL.AutoSize = true;
            this.useSSL.Location = new System.Drawing.Point(524, 44);
            this.useSSL.Name = "useSSL";
            this.useSSL.Size = new System.Drawing.Size(46, 17);
            this.useSSL.TabIndex = 2;
            this.useSSL.Text = "SSL";
            this.useSSL.UseVisualStyleBackColor = true;
            // 
            // ServiceConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 211);
            this.Controls.Add(this.useSSL);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.remoteConfigurationDir);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.postmortemOutputDir);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.processorOverload);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.envManagers);
            this.Controls.Add(this.envPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Service Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox remoteConfigurationDir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox postmortemOutputDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox processorOverload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox envManagers;
        private System.Windows.Forms.TextBox envPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox useSSL;
    }
}